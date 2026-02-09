using UnityEngine;
using System.Collections;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 刑天 - 坦克型英雄（无头战神）
    /// </summary>
    public class Hero_XingTian : HeroBase
    {
        [Header("刑天特性")]
        public float rageValue = 0f; // 怒气值
        public float maxRage = 100f;
        public bool isBerserk = false; // 狂暴状态
        
        protected override void Awake()
        {
            base.Awake();
            
            heroName = "刑天";
            heroTitle = "无头战神";
            heroType = HeroType.Tank;
            
            // 坦克属性（高生命高防御）
            stats.maxHealth = 4500f;
            stats.maxMana = 400f;
            stats.attackDamage = 120f;
            stats.attackSpeed = 0.9f;
            stats.criticalChance = 0.1f;
            stats.armor = 150f; // 高护甲
            stats.magicResist = 100f; // 高魔抗
            stats.moveSpeed = 340f; // 坦克移速慢
            
            // 成长
            stats.healthGrowth = 250f;
            stats.attackGrowth = 8f;
            stats.armorGrowth = 10f;
            
            stats.Initialize();
        }
        
        protected override void Update()
        {
            base.Update();
            
            // 怒气自然衰减
            if (rageValue > 0 && !isBerserk)
            {
                rageValue = Mathf.Max(0, rageValue - Time.deltaTime * 5f);
            }
        }
        
        public override void TakeDamage(float damage, Core.DamageType damageType, HeroBase attacker)
        {
            base.TakeDamage(damage, damageType, attacker);
            
            // 受伤增加怒气
            AddRage(damage * 0.5f);
        }
        
        void AddRage(float amount)
        {
            rageValue = Mathf.Min(rageValue + amount, maxRage);
            
            // 怒气满进入狂暴
            if (rageValue >= maxRage && !isBerserk)
            {
                EnterBerserkMode();
            }
        }
        
        void EnterBerserkMode()
        {
            isBerserk = true;
            
            // 狂暴状态加成
            stats.attackDamage *= 1.5f;
            stats.attackSpeed *= 1.3f;
            
            // 播放狂暴特效
            // TODO: 粒子特效
            
            StartCoroutine(BerserkDuration());
        }
        
        IEnumerator BerserkDuration()
        {
            yield return new WaitForSeconds(5f); // 狂暴持续5秒
            
            // 退出狂暴
            isBerserk = false;
            rageValue = 0;
            
            // 恢复属性
            stats.attackDamage /= 1.5f;
            stats.attackSpeed /= 1.3f;
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_XingTian_Q>();
            skills[1] = gameObject.AddComponent<Skill_XingTian_W>();
            skills[2] = gameObject.AddComponent<Skill_XingTian_E>();
            skills[3] = gameObject.AddComponent<Skill_XingTian_R>();
        }
    }
    
    /// <summary>
    /// Q技能: 干戚猛击
    /// </summary>
    public class Skill_XingTian_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "干戚猛击";
            description = "挥舞巨斧对前方扇形区域造成伤害并减速";
            hotkey = KeyCode.Q;
            manaCost = 50f;
            cooldown = 6f;
            baseDamage = 150f;
            damagePerLevel = 35f;
            range = 4f;
            effectRadius = 120f; // 扇形角度
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            // 扇形区域检测
            Collider[] colliders = Physics.OverlapSphere(caster.transform.position, range);
            
            foreach (Collider col in colliders)
            {
                HeroBase enemy = col.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    // 检查是否在扇形区域内
                    Vector3 directionToEnemy = (enemy.transform.position - caster.transform.position).normalized;
                    float angle = Vector3.Angle(caster.transform.forward, directionToEnemy);
                    
                    if (angle < effectRadius / 2f)
                    {
                        // 造成伤害
                        enemy.TakeDamage(CurrentDamage, Core.DamageType.Physical, caster);
                        
                        // 减速效果
                        // TODO: 添加减速Debuff
                        
                        if (hitEffect != null)
                        {
                            Instantiate(hitEffect, enemy.transform.position, Quaternion.identity);
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// W技能: 不屈战吼
    /// </summary>
    public class Skill_XingTian_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "不屈战吼";
            description = "嘲讽周围敌人攻击你，并获得护盾";
            hotkey = KeyCode.W;
            manaCost = 60f;
            cooldown = 12f;
            range = 6f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            // 嘲讽范围
            Collider[] colliders = Physics.OverlapSphere(caster.transform.position, range);
            
            foreach (Collider col in colliders)
            {
                HeroBase enemy = col.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    // 强制敌人攻击刑天（嘲讽效果）
                    // TODO: 实现嘲讽AI
                    
                    // 眩晕短暂时间
                    enemy.ApplyStun(0.5f);
                }
            }
            
            // 获得护盾
            Hero_XingTian xingtian = caster as Hero_XingTian;
            if (xingtian != null)
            {
                float shieldAmount = 200f + (skillLevel * 100f);
                // TODO: 添加护盾Buff
            }
        }
    }
    
    /// <summary>
    /// E技能: 铜头铁额
    /// </summary>
    public class Skill_XingTian_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "铜头铁额";
            description = "被动: 受到伤害减少。主动: 反弹下一次伤害";
            hotkey = KeyCode.E;
            manaCost = 40f;
            cooldown = 10f;
            duration = 3f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            // 激活反弹护盾
            DamageReflect reflect = caster.gameObject.AddComponent<DamageReflect>();
            reflect.reflectPercentage = 0.5f + (skillLevel * 0.1f); // 50%-90%反弹
            reflect.duration = duration;
        }
    }
    
    /// <summary>
    /// R技能: 战神降临
    /// </summary>
    public class Skill_XingTian_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "战神降临";
            description = "变身巨大形态，免疫控制并大幅提升属性";
            hotkey = KeyCode.R;
            manaCost = 150f;
            cooldown = 60f;
            duration = 8f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_XingTian xingtian = caster as Hero_XingTian;
            if (xingtian == null) return;
            
            StartCoroutine(UltimateTransform(xingtian));
        }
        
        IEnumerator UltimateTransform(Hero_XingTian xingtian)
        {
            // 变大
            Vector3 originalScale = xingtian.transform.localScale;
            xingtian.transform.localScale = originalScale * 1.5f;
            
            // 属性加成
            float originalArmor = xingtian.stats.armor;
            float originalHealth = xingtian.stats.maxHealth;
            
            xingtian.stats.armor *= 2f; // 双倍护甲
            xingtian.stats.maxHealth *= 1.5f; // 50%额外生命
            xingtian.stats.currentHealth = xingtian.stats.maxHealth; // 回满血
            
            // 免疫控制
            // TODO: 添加控制免疫Buff
            
            // 持续伤害光环
            float timer = 0f;
            while (timer < duration)
            {
                // 对周围敌人造成伤害
                Collider[] enemies = Physics.OverlapSphere(xingtian.transform.position, 5f);
                foreach (Collider col in enemies)
                {
                    HeroBase enemy = col.GetComponent<HeroBase>();
                    if (enemy != null && enemy != xingtian)
                    {
                        enemy.TakeDamage(50f, Core.DamageType.Magic, xingtian);
                    }
                }
                
                timer += 0.5f;
                yield return new WaitForSeconds(0.5f);
            }
            
            // 恢复原状
            xingtian.transform.localScale = originalScale;
            xingtian.stats.armor = originalArmor;
            xingtian.stats.maxHealth = originalHealth;
        }
    }
    
    /// <summary>
    /// 伤害反弹组件
    /// </summary>
    public class DamageReflect : MonoBehaviour
    {
        public float reflectPercentage = 0.5f;
        public float duration = 3f;
        
        void Start()
        {
            Invoke(nameof(DestroyReflect), duration);
        }
        
        void DestroyReflect()
        {
            Destroy(this);
        }
    }
}
