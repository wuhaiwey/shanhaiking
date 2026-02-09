using UnityEngine;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 女娲 - 法师型英雄
    /// </summary>
    public class Hero_NvWa : HeroBase
    {
        protected override void Awake()
        {
            base.Awake();
            heroName = "女娲";
            heroTitle = "创世女神";
            heroType = HeroType.Mage;
            
            stats.maxHealth = 2600f;
            stats.maxMana = 800f;
            stats.attackDamage = 60f;
            stats.abilityPower = 180f;
            stats.armor = 50f;
            stats.magicResist = 60f;
            stats.moveSpeed = 350f;
            
            stats.Initialize();
        }
    }
    
    public class Skill_NvWa_Q : Skill.SkillBase
    {
        void Awake() { skillName = "五彩神石"; hotkey = KeyCode.Q; manaCost = 60f; cooldown = 5f; baseDamage = 120f; }
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            // 发射五彩神石
            Collider[] hits = Physics.OverlapSphere(targetPosition, 3f);
            foreach (var hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                    enemy.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
            }
        }
    }
    
    public class Skill_NvWa_W : Skill.SkillBase
    {
        void Awake() { skillName = "抟土造人"; hotkey = KeyCode.W; manaCost = 80f; cooldown = 12f; }
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            // 召唤土灵协助战斗
            // 创建一个临时小兵协助
        }
    }
    
    public class Skill_NvWa_E : Skill.SkillBase
    {
        void Awake() { skillName = "瞬移"; hotkey = KeyCode.E; manaCost = 50f; cooldown = 8f; range = 8f; }
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.Blink(targetPosition);
        }
    }
    
    public class Skill_NvWa_R : Skill.SkillBase
    {
        void Awake() { skillName = "补天"; hotkey = KeyCode.R; manaCost = 150f; cooldown = 45f; range = 15f; }
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            // 大范围治疗队友 + 伤害敌人
            Collider[] hits = Physics.OverlapSphere(targetPosition, 8f);
            foreach (var hit in hits)
            {
                HeroBase unit = hit.GetComponent<HeroBase>();
                if (unit != null)
                {
                    if (unit == caster) // 治疗队友逻辑简化
                        unit.Heal(300f);
                    else
                        unit.TakeDamage(400f, Core.DamageType.Magic, caster);
                }
            }
        }
    }
}
