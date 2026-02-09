using UnityEngine;
namespace ShanHaiKing.Hero {
    /// <summary>
    /// 刑天 - 战神（山海经）
    /// 以乳为目，以脐为口，操干戚以舞
    /// </summary>
    public class Hero_XingTian_Full : HeroBase {
        [Header("刑天特性")]
        public bool isHeadless = false; // 断头状态
        public float rageMeter = 0f;
        public float maxRage = 100f;
        
        protected override void Awake() {
            base.Awake();
            heroName = "刑天";
            heroTitle = "战神";
            heroType = HeroType.Warrior;
            heroOrigin = "《山海经·海外西经》";
            heroLore = "刑天与帝争神，帝断其首，葬之常羊之山。乃以乳为目，以脐为口，操干戚以舞。";
            
            stats.maxHealth = 4500f;
            stats.maxMana = 400f;
            stats.attackDamage = 165f;
            stats.armor = 95f;
            stats.magicResist = 55f;
            stats.moveSpeed = 355f;
            stats.Initialize();
        }
        
        protected override void InitializeSkills() {
            skills[0] = gameObject.AddComponent<Skill_XingTian_Q>();
            skills[1] = gameObject.AddComponent<Skill_XingTian_W>();
            skills[2] = gameObject.AddComponent<Skill_XingTian_E>();
            skills[3] = gameObject.AddComponent<Skill_XingTian_R>();
        }
        
        public void AddRage(float amount) {
            rageMeter = Mathf.Min(rageMeter + amount, maxRage);
            if (rageMeter >= maxRage && !isHeadless) {
                EnterHeadlessMode();
            }
        }
        
        void EnterHeadlessMode() {
            isHeadless = true;
            stats.attackDamage *= 1.5f;
            stats.attackSpeed *= 1.3f;
            // 视觉特效：移除头部模型
        }
    }
    
    // Q技能：干戚猛击
    public class Skill_XingTian_Q : Skill.SkillBase {
        void Awake() {
            skillName = "干戚猛击";
            description = "刑天挥舞干戚（盾和斧）对前方敌人造成伤害";
            hotkey = KeyCode.Q;
            manaCost = 45f;
            cooldown = 5f;
            baseDamage = 180f;
            range = 4f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            // 盾击特效
            GameObject shield = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shield.transform.position = caster.transform.position + caster.transform.forward * 2f;
            shield.transform.localScale = new Vector3(3f, 0.2f, 3f);
            shield.GetComponent<Renderer>().material.color = new Color(0.4f, 0.3f, 0.2f);
            Destroy(shield.GetComponent<Collider>());
            
            Collider[] hits = Physics.OverlapSphere(shield.transform.position, 2f);
            foreach (Collider hit in hits) {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster) {
                    enemy.TakeDamage(CurrentDamage, Core.DamageType.Physical, caster);
                    // 击退效果
                }
            }
            
            Destroy(shield, 0.5f);
            
            // 增加怒气
            Hero_XingTian_Full xingtian = caster as Hero_XingTian_Full;
            xingtian?.AddRage(15f);
        }
    }
    
    // W技能：不屈意志
    public class Skill_XingTian_W : Skill.SkillBase {
        void Awake() {
            skillName = "不屈意志";
            description = "刑天获得护盾并减少受到的伤害";
            hotkey = KeyCode.W;
            manaCost = 60f;
            cooldown = 12f;
            baseDamage = 0f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            // 护盾效果
            caster.stats.currentHealth += 300f;
            caster.stats.armor += 50f;
            
            // 视觉特效
            GameObject shieldEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shieldEffect.transform.position = caster.transform.position;
            shieldEffect.transform.localScale = Vector3.one * 2.5f;
            shieldEffect.GetComponent<Renderer>().material.color = new Color(1f, 0.8f, 0.2f, 0.3f);
            Destroy(shieldEffect.GetComponent<Collider>());
            Destroy(shieldEffect, 4f);
            
            Hero_XingTian_Full xingtian = caster as Hero_XingTian_Full;
            xingtian?.AddRage(20f);
        }
    }
    
    // E技能：战魂冲锋
    public class Skill_XingTian_E : Skill.SkillBase {
        void Awake() {
            skillName = "战魂冲锋";
            description = "刑天向目标方向冲锋，撞到的敌人会受到伤害和眩晕";
            hotkey = KeyCode.E;
            manaCost = 55f;
            cooldown = 10f;
            baseDamage = 150f;
            range = 8f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            Vector3 dashDirection = (targetPosition - caster.transform.position).normalized;
            float dashDistance = 8f;
            
            // 冲刺效果
            caster.StartCoroutine(DashCoroutine(caster, dashDirection, dashDistance));
        }
        
        System.Collections.IEnumerator DashCoroutine(HeroBase caster, Vector3 direction, float distance) {
            float traveled = 0f;
            while (traveled < distance) {
                caster.transform.position += direction * 20f * Time.deltaTime;
                traveled += 20f * Time.deltaTime;
                
                // 碰撞检测
                Collider[] hits = Physics.OverlapSphere(caster.transform.position, 1.5f);
                foreach (Collider hit in hits) {
                    HeroBase enemy = hit.GetComponent<HeroBase>();
                    if (enemy != null && enemy != caster) {
                        enemy.TakeDamage(CurrentDamage, Core.DamageType.Physical, caster);
                    }
                }
                
                yield return null;
            }
        }
    }
    
    // R技能：断头不死
    public class Skill_XingTian_R : Skill.SkillBase {
        void Awake() {
            skillName = "断头不死";
            description = "刑天进入断头状态，攻击力大幅提升，免疫控制";
            hotkey = KeyCode.R;
            manaCost = 100f;
            cooldown = 90f;
            baseDamage = 0f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            Hero_XingTian_Full xingtian = caster as Hero_XingTian_Full;
            if (xingtian != null) {
                xingtian.AddRage(100f); // 直接进入断头状态
                
                // 全屏特效
                GameObject ultimateEffect = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                ultimateEffect.transform.position = caster.transform.position;
                ultimateEffect.transform.localScale = new Vector3(15f, 0.1f, 15f);
                ultimateEffect.GetComponent<Renderer>().material.color = new Color(0.8f, 0.2f, 0.1f, 0.4f);
                Destroy(ultimateEffect.GetComponent<Collider>());
                Destroy(ultimateEffect, 8f);
            }
        }
    }
}
