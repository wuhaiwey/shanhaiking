using UnityEngine;

namespace ShanHaiKing.Skill
{
    /// <summary>
    /// 技能基类
    /// </summary>
    public abstract class SkillBase : MonoBehaviour
    {
        [Header("技能信息")]
        public string skillName = "技能";
        public string description = "";
        public Sprite skillIcon;
        public KeyCode hotkey = KeyCode.Q;
        
        [Header("技能类型")]
        public SkillType skillType = SkillType.Active;
        public TargetType targetType = TargetType.Unit;
        
        [Header("消耗与冷却")]
        public float manaCost = 50f;
        public float cooldown = 5f;
        protected float currentCooldown = 0f;
        
        [Header("技能数值")]
        public float baseDamage = 100f;
        public float damagePerLevel = 20f;
        public float range = 10f;
        public float effectRadius = 0f; // 0表示单体
        
        [Header("特效")]
        public GameObject castEffect;
        public GameObject hitEffect;
        public AudioClip castSound;
        public AudioClip hitSound;
        
        // 技能等级
        public int skillLevel = 1;
        public int maxLevel = 5;
        
        // 是否可升级
        public bool CanLevelUp => skillLevel < maxLevel;
        
        /// <summary>
        /// 当前伤害值
        /// </summary>
        public virtual float CurrentDamage => baseDamage + (damagePerLevel * (skillLevel - 1));
        
        /// <summary>
        /// 释放技能
        /// </summary>
        public virtual bool Cast(Hero.HeroBase caster, Vector3 targetPosition, Hero.HeroBase target)
        {
            // 检查冷却
            if (currentCooldown > 0)
            {
                Debug.Log($"{skillName} 冷却中: {currentCooldown:F1}秒");
                return false;
            }
            
            // 检查法力
            if (caster.stats.currentMana < manaCost)
            {
                Debug.Log("法力不足!");
                return false;
            }
            
            // 检查距离
            if (Vector3.Distance(caster.transform.position, targetPosition) > range)
            {
                Debug.Log("目标超出范围!");
                return false;
            }
            
            // 消耗法力
            caster.stats.currentMana -= manaCost;
            
            // 执行技能效果
            ExecuteSkill(caster, targetPosition, target);
            
            // 开始冷却
            StartCooldown();
            
            // 播放特效
            PlayEffects(caster, targetPosition);
            
            return true;
        }
        
        /// <summary>
        /// 执行技能效果 - 子类实现
        /// </summary>
        protected abstract void ExecuteSkill(Hero.HeroBase caster, Vector3 targetPosition, Hero.HeroBase target);
        
        /// <summary>
        /// 开始冷却
        /// </summary>
        protected virtual void StartCooldown()
        {
            currentCooldown = cooldown;
            StartCoroutine(CooldownCoroutine());
        }
        
        private System.Collections.IEnumerator CooldownCoroutine()
        {
            while (currentCooldown > 0)
            {
                currentCooldown -= Time.deltaTime;
                yield return null;
            }
            currentCooldown = 0;
        }
        
        /// <summary>
        /// 播放特效
        /// </summary>
        protected virtual void PlayEffects(Hero.HeroBase caster, Vector3 targetPosition)
        {
            if (castEffect != null)
            {
                Instantiate(castEffect, caster.transform.position, Quaternion.identity);
            }
            
            if (castSound != null)
            {
                AudioSource.PlayClipAtPoint(castSound, caster.transform.position);
            }
        }
        
        /// <summary>
        /// 升级技能
        /// </summary>
        public virtual void LevelUp()
        {
            if (CanLevelUp)
            {
                skillLevel++;
                OnLevelUp();
            }
        }
        
        protected virtual void OnLevelUp()
        {
            // 子类可重写
        }
        
        /// <summary>
        /// 获取冷却进度 (0-1)
        /// </summary>
        public float GetCooldownProgress()
        {
            if (cooldown <= 0) return 0;
            return 1f - (currentCooldown / cooldown);
        }
    }
    
    /// <summary>
    /// 技能类型
    /// </summary>
    public enum SkillType
    {
        Active,     // 主动技能
        Passive,    // 被动技能
        Toggle      // 开关技能
    }
    
    /// <summary>
    /// 目标类型
    /// </summary>
    public enum TargetType
    {
        None,       // 无目标
        Self,       // 自身
        Unit,       // 单位
        Point,      // 地点
        Direction   // 方向
    }
}
