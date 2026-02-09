using UnityEngine;
namespace ShanHaiKing.Hero {
    public class Hero_LvBu : HeroBase {
        protected override void Awake() {
            base.Awake();
            heroName = "吕布";
            heroTitle = "无双之魔";
            heroType = HeroType.Warrior;
            stats.maxHealth = 4000f;
            stats.maxMana = 550f;
            stats.attackDamage = 185f;
            stats.Initialize();
        }
        protected override void InitializeSkills() {
            skills[0] = gameObject.AddComponent<Skill_LvBu_Q>();
            skills[1] = gameObject.AddComponent<Skill_LvBu_W>();
            skills[2] = gameObject.AddComponent<Skill_LvBu_E>();
            skills[3] = gameObject.AddComponent<Skill_LvBu_R>();
        }
    }
    public class Skill_LvBu_Q : Skill.SkillBase { void Awake() { skillName = "方天画斩"; hotkey = KeyCode.Q; manaCost = 50f; cooldown = 6f; baseDamage = 170f; } protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) { /* 实现 */ } }
    public class Skill_LvBu_W : Skill.SkillBase { void Awake() { skillName = "贪狼之握"; hotkey = KeyCode.W; manaCost = 70f; cooldown = 10f; } protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) { /* 实现 */ } }
    public class Skill_LvBu_E : Skill.SkillBase { void Awake() { skillName = "魔神降世"; hotkey = KeyCode.E; manaCost = 80f; cooldown = 12f; } protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) { /* 实现 */ } }
    public class Skill_LvBu_R : Skill.SkillBase { void Awake() { skillName = "无双之魔"; hotkey = KeyCode.R; manaCost = 120f; cooldown = 80f; baseDamage = 380f; } protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) { /* 实现 */ } }
}
