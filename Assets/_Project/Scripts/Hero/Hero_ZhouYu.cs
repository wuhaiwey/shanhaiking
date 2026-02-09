using UnityEngine;
using System.Collections;
namespace ShanHaiKing.Hero {
    public class Hero_ZhouYu : HeroBase {
        protected override void Awake() {
            base.Awake();
            heroName = "周瑜";
            heroTitle = "铁血都督";
            heroType = HeroType.Mage;
            stats.maxHealth = 2600f;
            stats.maxMana = 900f;
            stats.abilityPower = 215f;
            stats.Initialize();
        }
        protected override void InitializeSkills() {
            skills[0] = gameObject.AddComponent<Skill_ZhouYu_Q>();
            skills[1] = gameObject.AddComponent<Skill_ZhouYu_W>();
            skills[2] = gameObject.AddComponent<Skill_ZhouYu_E>();
            skills[3] = gameObject.AddComponent<Skill_ZhouYu_R>();
        }
    }
    public class Skill_ZhouYu_Q : Skill.SkillBase {
        void Awake() { skillName = "烽火赤壁"; hotkey = KeyCode.Q; manaCost = 60f; cooldown = 6f; baseDamage = 150f; }
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            GameObject fire = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            fire.transform.position = targetPosition;
            fire.transform.localScale = new Vector3(4f, 0.1f, 4f);
            fire.GetComponent<Renderer>().material.color = new Color(1f, 0.4f, 0.1f, 0.5f);
            Destroy(fire.GetComponent<Collider>());
            Collider[] hits = Physics.OverlapSphere(targetPosition, 2f);
            foreach (Collider hit in hits) {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster) enemy.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
            }
            Destroy(fire, 3f);
        }
    }
    public class Skill_ZhouYu_W : Skill.SkillBase {
        void Awake() { skillName = "流火之矢"; hotkey = KeyCode.W; manaCost = 70f; cooldown = 10f; baseDamage = 180f; }
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            caster.StartCoroutine(FireArrows(caster, targetPosition));
        }
        System.Collections.IEnumerator FireArrows(HeroBase caster, Vector3 center) {
            for (int i = 0; i < 3; i++) {
                Vector3 pos = center + Random.insideUnitSphere * 3f;
                pos.y = center.y;
                GameObject arrow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                arrow.transform.position = pos + Vector3.up * 10f;
                arrow.transform.localScale = new Vector3(0.1f, 2f, 0.1f);
                arrow.GetComponent<Renderer>().material.color = new Color(1f, 0.3f, 0.1f);
                Destroy(arrow.GetComponent<Collider>());
                Rigidbody rb = arrow.AddComponent<Rigidbody>();
                rb.velocity = Vector3.down * 20f;
                Destroy(arrow, 2f);
                yield return new WaitForSeconds(0.3f);
            }
        }
    }
    public class Skill_ZhouYu_E : Skill.SkillBase {
        void Awake() { skillName = "东风破袭"; hotkey = KeyCode.E; manaCost = 65f; cooldown = 12f; baseDamage = 160f; }
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            GameObject wind = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wind.transform.position = caster.transform.position;
            wind.transform.localScale = new Vector3(6f, 0.1f, 6f);
            wind.GetComponent<Renderer>().material.color = new Color(0.8f, 0.9f, 1f, 0.3f);
            Destroy(wind.GetComponent<Collider>());
            Collider[] hits = Physics.OverlapSphere(caster.transform.position, 3f);
            foreach (Collider hit in hits) {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster) {
                    enemy.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
                    Vector3 push = (enemy.transform.position - caster.transform.position).normalized;
                    enemy.transform.position += push * 4f;
                }
            }
            Destroy(wind, 0.5f);
        }
    }
    public class Skill_ZhouYu_R : Skill.SkillBase {
        void Awake() { skillName = "火烧连营"; hotkey = KeyCode.R; manaCost = 150f; cooldown = 80f; baseDamage = 350f; }
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            caster.StartCoroutine(FireStorm(caster, targetPosition));
        }
        System.Collections.IEnumerator FireStorm(HeroBase caster, Vector3 center) {
            GameObject storm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            storm.transform.position = center;
            storm.transform.localScale = new Vector3(12f, 0.1f, 12f);
            storm.GetComponent<Renderer>().material.color = new Color(1f, 0.3f, 0.1f, 0.4f);
            Destroy(storm.GetComponent<Collider>());
            float elapsed = 0f;
            while (elapsed < 6f) {
                if (elapsed % 0.5f < Time.deltaTime) {
                    Collider[] hits = Physics.OverlapSphere(center, 6f);
                    foreach (Collider hit in hits) {
                        HeroBase enemy = hit.GetComponent<HeroBase>();
                        if (enemy != null && enemy != caster) enemy.TakeDamage(60f, Core.DamageType.Magic, caster);
                    }
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
            Destroy(storm);
        }
    }
}
