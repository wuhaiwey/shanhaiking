using UnityEngine;
namespace ShanHaiKing.Hero {
    /// <summary>
    /// 九尾狐 - 妖狐（山海经）
    /// 青丘之山有兽焉，其状如狐而九尾
    /// </summary>
    public class Hero_JiuWeiHu_Full : HeroBase {
        [Header("九尾特性")]
        public int tailCount = 9;
        public float charmMeter = 0f;
        public bool isFoxForm = true;
        
        protected override void Awake() {
            base.Awake();
            heroName = "九尾狐";
            heroTitle = "青丘妖狐";
            heroType = HeroType.Mage;
            heroOrigin = "《山海经·南山经》";
            heroLore = "青丘之山有兽焉，其状如狐而九尾，其音如婴儿，能食人，食者不蛊。";
            
            stats.maxHealth = 2350f;
            stats.maxMana = 950f;
            stats.attackDamage = 52f;
            stats.abilityPower = 205f;
            stats.moveSpeed = 365f;
            stats.Initialize();
        }
        
        protected override void InitializeSkills() {
            skills[0] = gameObject.AddComponent<Skill_JiuWeiHu_Q>();
            skills[1] = gameObject.AddComponent<Skill_JiuWeiHu_W>();
            skills[2] = gameObject.AddComponent<Skill_JiuWeiHu_E>();
            skills[3] = gameObject.AddComponent<Skill_JiuWeiHu_R>();
        }
        
        public void TransformToHuman() {
            isFoxForm = false;
            stats.abilityPower += 50f;
        }
        
        public void TransformToFox() {
            isFoxForm = true;
            stats.moveSpeed += 30f;
        }
    }
    
    public class Skill_JiuWeiHu_Q : Skill.SkillBase {
        void Awake() {
            skillName = "妖狐之焰";
            description = "九尾释放狐火攻击敌人";
            hotkey = KeyCode.Q;
            manaCost = 50f;
            cooldown = 5f;
            baseDamage = 145f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            for (int i = 0; i < 3; i++) {
                GameObject fireball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                fireball.transform.position = caster.transform.position + Vector3.up + Random.insideUnitSphere * 0.5f;
                fireball.transform.localScale = Vector3.one * 0.4f;
                fireball.GetComponent<Renderer>().material.color = new Color(1f, 0.4f, 0f);
                
                Rigidbody rb = fireball.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.velocity = (targetPosition - caster.transform.position).normalized * 12f + Random.insideUnitSphere * 3f;
                
                Destroy(fireball.GetComponent<Collider>());
                Destroy(fireball, 2f);
            }
        }
    }
    
    public class Skill_JiuWeiHu_W : Skill.SkillBase {
        void Awake() {
            skillName = "魅惑之术";
            description = "魅惑敌人使其走向九尾狐";
            hotkey = KeyCode.W;
            manaCost = 70f;
            cooldown = 14f;
            range = 6f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            if (target != null) {
                GameObject charmEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                charmEffect.transform.position = target.transform.position;
                charmEffect.transform.localScale = Vector3.one * 2f;
                charmEffect.GetComponent<Renderer>().material.color = new Color(1f, 0.3f, 0.6f, 0.5f);
                Destroy(charmEffect.GetComponent<Collider>());
                Destroy(charmEffect, 3f);
            }
        }
    }
    
    public class Skill_JiuWeiHu_E : Skill.SkillBase {
        void Awake() {
            skillName = "九尾真身";
            description = "切换狐形/人形";
            hotkey = KeyCode.E;
            manaCost = 40f;
            cooldown = 8f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            Hero_JiuWeiHu_Full jiwei = caster as Hero_JiuWeiHu_Full;
            if (jiwei.isFoxForm) {
                jiwei.TransformToHuman();
            } else {
                jiwei.TransformToFox();
            }
        }
    }
    
    public class Skill_JiuWeiHu_R : Skill.SkillBase {
        void Awake() {
            skillName = "九尾天罚";
            description = "召唤九条尾巴的力量毁灭敌人";
            hotkey = KeyCode.R;
            manaCost = 150f;
            cooldown = 85f;
            baseDamage = 350f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            // 九尾特效
            for (int i = 0; i < 9; i++) {
                float angle = i * 40f * Mathf.Deg2Rad;
                Vector3 pos = caster.transform.position + new Vector3(Mathf.Cos(angle) * 3f, 0.5f, Mathf.Sin(angle) * 3f);
                
                GameObject tail = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                tail.transform.position = pos;
                tail.transform.localScale = new Vector3(0.8f, 5f, 0.8f);
                tail.transform.rotation = Quaternion.Euler(0, i * 40f, 30f);
                tail.GetComponent<Renderer>().material.color = new Color(1f, 0.2f, 0.4f);
                Destroy(tail.GetComponent<Collider>());
                Destroy(tail, 6f);
            }
            
            Collider[] hits = Physics.OverlapSphere(targetPosition, 6f);
            foreach (Collider hit in hits) {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster) {
                    enemy.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
                }
            }
        }
    }
}
