using UnityEngine;
using System.Collections;
namespace ShanHaiKing.Hero {
    public class Hero_DaQiao : HeroBase {
        protected override void Awake() {
            base.Awake();
            heroName = "大乔";
            heroTitle = "沧海之曜";
            heroType = HeroType.Support;
            stats.maxHealth = 2400f;
            stats.maxMana = 850f;
            stats.abilityPower = 180f;
            stats.Initialize();
        }
        protected override void InitializeSkills() {
            skills[0] = gameObject.AddComponent<Skill_DaQiao_Q>();
            skills[1] = gameObject.AddComponent<Skill_DaQiao_W>();
            skills[2] = gameObject.AddComponent<Skill_DaQiao_E>();
            skills[3] = gameObject.AddComponent<Skill_DaQiao_R>();
        }
    }
    public class Skill_DaQiao_Q : Skill.SkillBase {
        void Awake() { skillName = "鲤跃之潮"; hotkey = KeyCode.Q; manaCost = 55f; cooldown = 7f; baseDamage = 120f; }
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            GameObject wave = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wave.transform.position = caster.transform.position;
            wave.transform.localScale = new Vector3(5f, 0.1f, 5f);
            wave.GetComponent<Renderer>().material.color = new Color(0.3f, 0.7f, 0.9f, 0.4f);
            Destroy(wave.GetComponent<Collider>());
            Collider[] hits = Physics.OverlapSphere(caster.transform.position, 2.5f);
            foreach (Collider hit in hits) {
                HeroBase unit = hit.GetComponent<HeroBase>();
                if (unit != null) {
                    if (unit == caster) unit.RestoreHealth(150f);
                    else if (unit.team != caster.team) unit.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
                }
            }
            Destroy(wave, 2f);
        }
    }
    public class Skill_DaQiao_W : Skill.SkillBase {
        void Awake() { skillName = "宿命之海"; hotkey = KeyCode.W; manaCost = 70f; cooldown = 14f; }
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            GameObject portal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            portal.transform.position = targetPosition;
            portal.transform.localScale = new Vector3(3f, 0.1f, 3f);
            portal.GetComponent<Renderer>().material.color = new Color(0.4f, 0.8f, 1f, 0.5f);
            Destroy(portal.GetComponent<Collider>());
            if (target != null && target.team == caster.team) {
                target.RestoreHealth(200f);
                target.RestoreMana(100f);
            }
            Destroy(portal, 3f);
        }
    }
    public class Skill_DaQiao_E : Skill.SkillBase {
        void Awake() { skillName = "决断之桥"; hotkey = KeyCode.E; manaCost = 60f; cooldown = 12f; baseDamage = 140f; }
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            GameObject bridge = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bridge.transform.position = targetPosition;
            bridge.transform.localScale = new Vector3(6f, 0.1f, 1f);
            bridge.GetComponent<Renderer>().material.color = new Color(0.5f, 0.9f, 1f, 0.4f);
            Destroy(bridge.GetComponent<Collider>());
            Collider[] hits = Physics.OverlapSphere(targetPosition, 3f);
            foreach (Collider hit in hits) {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster) {
                    enemy.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
                    // enemy.ApplyBuff(BuffType.Silence, 0, 1.5f);
                }
            }
            Destroy(bridge, 3f);
        }
    }
    public class Skill_DaQiao_R : Skill.SkillBase {
        void Awake() { skillName = "漩涡之门"; hotkey = KeyCode.R; manaCost = 140f; cooldown = 75f; baseDamage = 250f; }
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            GameObject vortex = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            vortex.transform.position = targetPosition;
            vortex.transform.localScale = new Vector3(10f, 0.1f, 10f);
            vortex.GetComponent<Renderer>().material.color = new Color(0.3f, 0.7f, 1f, 0.5f);
            Destroy(vortex.GetComponent<Collider>());
            GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pillar.transform.position = targetPosition + Vector3.up * 5f;
            pillar.transform.localScale = new Vector3(2f, 10f, 2f);
            pillar.GetComponent<Renderer>().material.color = new Color(0.5f, 0.85f, 1f, 0.6f);
            Destroy(pillar.GetComponent<Collider>());
            Collider[] hits = Physics.OverlapSphere(targetPosition, 5f);
            foreach (Collider hit in hits) {
                HeroBase unit = hit.GetComponent<HeroBase>();
                if (unit != null) {
                    if (unit.team == caster.team) unit.RestoreHealth(300f);
                    else unit.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
                }
            }
            Destroy(vortex, 5f);
            Destroy(pillar, 5f);
        }
    }
}
