using UnityEngine;
namespace ShanHaiKing.Hero {
    public class Hero_XiaoQiao : HeroBase {
        protected override void Awake() {
            base.Awake();
            heroName = "小乔";
            heroTitle = "恋之微风";
            heroType = HeroType.Mage;
            stats.maxHealth = 2350f;
            stats.maxMana = 880f;
            stats.abilityPower = 195f;
            stats.Initialize();
        }
        protected override void InitializeSkills() {
            skills[0] = gameObject.AddComponent<Skill_XiaoQiao_Q>();
            skills[1] = gameObject.AddComponent<Skill_XiaoQiao_W>();
            skills[2] = gameObject.AddComponent<Skill_XiaoQiao_E>();
            skills[3] = gameObject.AddComponent<Skill_XiaoQiao_R>();
        }
    }
    public class Skill_XiaoQiao_Q : Skill.SkillBase {
        void Awake() { skillName = "绽放之舞"; hotkey = KeyCode.Q; manaCost = 55f; cooldown = 6f; baseDamage = 140f; }
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            GameObject fan = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            fan.transform.position = caster.transform.position;
            fan.transform.localScale = new Vector3(5f, 0.1f, 5f);
            fan.GetComponent<Renderer>().material.color = new Color(0.9f, 0.6f, 0.8f, 0.4f);
            Destroy(fan.GetComponent<Collider>());
            Collider[] hits = Physics.OverlapSphere(caster.transform.position, 2.5f);
            foreach (Collider hit in hits) {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster) enemy.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
            }
            Destroy(fan, 2f);
        }
    }
    public class Skill_XiaoQiao_W : Skill.SkillBase { void Awake() { skillName = "甜蜜恋风"; hotkey = KeyCode.W; manaCost = 70f; cooldown = 12f; } protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) { /* 实现 */ } }
    public class Skill_XiaoQiao_E : Skill.SkillBase { void Awake() { skillName = "星华缭乱"; hotkey = KeyCode.E; manaCost = 60f; cooldown = 10f; } protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) { /* 实现 */ } }
    public class Skill_XiaoQiao_R : Skill.SkillBase { void Awake() { skillName = "星华绽放"; hotkey = KeyCode.R; manaCost = 140f; cooldown = 75f; baseDamage = 300f; } protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) { /* 实现 */ } }
}
