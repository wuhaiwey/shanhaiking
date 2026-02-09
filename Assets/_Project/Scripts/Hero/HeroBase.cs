using UnityEngine;
using ShanHaiKing.Core;
using System.Collections.Generic;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 英雄基类 - 所有英雄的父类
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class HeroBase : MonoBehaviour
    {
        [Header("英雄信息")]
        public string heroName = "无名英雄";
        public string heroTitle = "";
        public HeroType heroType = HeroType.Warrior;
        public Sprite heroIcon;
        
        [Header("属性")]
        public HeroStats stats = new HeroStats();
        
        [Header("等级")]
        public int level = 1;
        public int maxLevel = 15;
        public int currentExp = 0;
        public int[] expToLevel = { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1100, 1200, 1300, 1400 };
        
        [Header("组件引用")]
        public CharacterController characterController;
        public Animator animator;
        
        [Header("技能")]
        public Skill.SkillBase[] skills = new Skill.SkillBase[4]; // Q, W, E, R
        
        // 状态
        public bool isDead = false;
        public bool isStunned = false;
        public bool isSilenced = false;
        public bool isRooted = false;
        
        // 事件
        public System.Action OnDeath;
        public System.Action OnRespawn;
        public System.Action<int> OnLevelUp;
        public System.Action<float> OnHealthChanged;
        public System.Action<float> OnManaChanged;
        
        protected virtual void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();
            stats.Initialize();
        }
        
        protected virtual void Start()
        {
            // 初始化技能
            InitializeSkills();
        }
        
        protected virtual void Update()
        {
            if (isDead) return;
            
            // 生命恢复
            RegenerateHealth();
            
            // 法力恢复
            RegenerateMana();
        }
        
        #region 移动控制
        
        /// <summary>
        /// 移动到目标位置
        /// </summary>
        public virtual void Move(Vector3 direction)
        {
            if (isRooted || isStunned || isDead) return;
            
            if (direction.magnitude > 0.1f)
            {
                // 旋转朝向
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    Time.deltaTime * 10f
                );
                
                // 移动
                Vector3 move = direction * stats.moveSpeed * Time.deltaTime;
                characterController.Move(move);
                
                // 动画
                if (animator != null)
                {
                    animator.SetFloat("Speed", direction.magnitude);
                }
            }
            else
            {
                if (animator != null)
                {
                    animator.SetFloat("Speed", 0f);
                }
            }
        }
        
        /// <summary>
        /// 瞬间移动（闪现等）
        
        /// </summary>
        public virtual void Blink(Vector3 targetPosition)
        {
            if (isStunned || isDead) return;
            
            characterController.enabled = false;
            transform.position = targetPosition;
            characterController.enabled = true;
            
            // 特效
            // TODO: 播放闪现特效
        }
        
        #endregion
        
        #region 战斗
        
        /// <summary>
        /// 受到伤害
        /// </summary>
        public virtual void TakeDamage(float damage, DamageType damageType, HeroBase attacker)
        {
            if (isDead) return;
            
            float actualDamage = damage;
            
            // 根据伤害类型计算
            switch (damageType)
            {
                case DamageType.Physical:
                    actualDamage = damage * (100f / (100f + stats.armor));
                    break;
                case DamageType.Magic:
                    actualDamage = damage * (100f / (100f + stats.magicResist));
                    break;
                case DamageType.True:
                    // 真实伤害无视防御
                    break;
            }
            
            // 暴击判定
            if (attacker != null && Random.value < attacker.stats.criticalChance)
            {
                actualDamage *= attacker.stats.criticalDamage;
                // TODO: 显示暴击特效
            }
            
            stats.currentHealth -= actualDamage;
            OnHealthChanged?.Invoke(stats.currentHealth);
            
            // 受伤动画
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
            
            // 检查死亡
            if (stats.currentHealth <= 0)
            {
                Die(attacker);
            }
        }
        
        /// <summary>
        /// 普通攻击
        /// </summary>
        public virtual void BasicAttack(HeroBase target)
        {
            if (isDead || isStunned) return;
            
            if (target != null && Vector3.Distance(transform.position, target.transform.position) <= stats.attackRange)
            {
                // 攻击动画
                if (animator != null)
                {
                    animator.SetTrigger("Attack");
                }
                
                // 造成伤害
                target.TakeDamage(stats.attackDamage, DamageType.Physical, this);
            }
        }
        
        /// <summary>
        /// 治疗
        /// </summary>
        public virtual void Heal(float amount)
        {
            if (isDead) return;
            
            stats.currentHealth = Mathf.Min(stats.currentHealth + amount, stats.maxHealth);
            OnHealthChanged?.Invoke(stats.currentHealth);
        }
        
        /// <summary>
        /// 恢复法力
        /// </summary>
        public virtual void RestoreMana(float amount)
        {
            if (isDead) return;
            
            stats.currentMana = Mathf.Min(stats.currentMana + amount, stats.maxMana);
            OnManaChanged?.Invoke(stats.currentMana);
        }
        
        /// <summary>
        /// 死亡
        /// </summary>
        protected virtual void Die(HeroBase killer)
        {
            isDead = true;
            
            // 死亡动画
            if (animator != null)
            {
                animator.SetTrigger("Death");
            }
            
            OnDeath?.Invoke();
            
            // TODO: 击杀奖励
            if (killer != null)
            {
                killer.GainExp(100); // 击杀获得经验
            }
            
            // 延迟复活
            Invoke(nameof(Respawn), 5f);
        }
        
        /// <summary>
        /// 复活
        /// </summary>
        protected virtual void Respawn()
        {
            isDead = false;
            stats.currentHealth = stats.maxHealth;
            stats.currentMana = stats.maxMana;
            
            // 回到出生点
            // TODO: 传送回复活点
            
            OnRespawn?.Invoke();
            
            if (animator != null)
            {
                animator.SetTrigger("Respawn");
            }
        }
        
        #endregion
        
        #region 经验与等级
        
        /// <summary>
        /// 获得经验
        /// </summary>
        public virtual void GainExp(int amount)
        {
            if (level >= maxLevel) return;
            
            currentExp += amount;
            
            // 检查升级
            while (currentExp >= expToLevel[level - 1] && level < maxLevel)
            {
                currentExp -= expToLevel[level - 1];
                LevelUp();
            }
        }
        
        /// <summary>
        /// 升级
        /// </summary>
        protected virtual void LevelUp()
        {
            level++;
            stats.LevelUp(level);
            
            OnLevelUp?.Invoke(level);
            
            // 升级特效
            // TODO: 播放升级特效
        }
        
        #endregion
        
        #region 技能
        
        /// <summary>
        /// 初始化技能 - 子类重写
        /// </summary>
        protected virtual void InitializeSkills()
        {
            // 子类实现
        }
        
        /// <summary>
        /// 释放技能
        /// </summary>
        public virtual void CastSkill(int skillIndex, Vector3 targetPosition, HeroBase target)
        {
            if (isDead || isStunned || isSilenced) return;
            
            if (skillIndex >= 0 && skillIndex < skills.Length && skills[skillIndex] != null)
            {
                skills[skillIndex].Cast(this, targetPosition, target);
            }
        }
        
        #endregion
        
        #region 状态恢复
        
        protected virtual void RegenerateHealth()
        {
            if (!isDead && stats.currentHealth < stats.maxHealth)
            {
                float regen = stats.maxHealth * 0.01f * Time.deltaTime; // 1%每秒
                stats.currentHealth = Mathf.Min(stats.currentHealth + regen, stats.maxHealth);
            }
        }
        
        protected virtual void RegenerateMana()
        {
            if (!isDead && stats.currentMana < stats.maxMana)
            {
                float regen = stats.maxMana * 0.02f * Time.deltaTime; // 2%每秒
                stats.currentMana = Mathf.Min(stats.currentMana + regen, stats.maxMana);
            }
        }
        
        #endregion
        
        #region 控制效果
        
        public virtual void ApplyStun(float duration)
        {
            if (isDead) return;
            StartCoroutine(StunCoroutine(duration));
        }
        
        private System.Collections.IEnumerator StunCoroutine(float duration)
        {
            isStunned = true;
            yield return new WaitForSeconds(duration);
            isStunned = false;
        }
        
        public virtual void ApplySilence(float duration)
        {
            if (isDead) return;
            StartCoroutine(SilenceCoroutine(duration));
        }
        
        private System.Collections.IEnumerator SilenceCoroutine(float duration)
        {
            isSilenced = true;
            yield return new WaitForSeconds(duration);
            isSilenced = false;
        }
        
        public virtual void ApplyRoot(float duration)
        {
            if (isDead) return;
            StartCoroutine(RootCoroutine(duration));
        }
        
        private System.Collections.IEnumerator RootCoroutine(float duration)
        {
            isRooted = true;
            yield return new WaitForSeconds(duration);
            isRooted = false;
        }
        
        #endregion
    }
}
