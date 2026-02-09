using UnityEngine;
namespace ShanHaiKing.Hero {
    /// <summary>
    /// 后羿 - 射日英雄（山海经）
    /// 帝俊赐羿彤弓素矰，以扶下国
    /// </summary>
    public class Hero_HouYi_Full : HeroBase {
        [Header("后羿特性")]
        public int sunsShot = 0; // 射落的太阳数
        public bool hasDivineBow = true;
        public float arrowPower = 1f;
        
        protected override void Awake() {
            base.Awake();
            heroName = "后羿";
            heroTitle = "射日英雄";
            heroType = HeroType.Marksman;
            heroOrigin = "《山海经·海内经》";
            heroLore = "帝俊赐羿彤弓素矰，以扶下国，羿是始去恤下地之百艰。";
            
            stats.maxHealth = 2150f;
            stats.maxMana = 480f;
            stats.attackDamage = 175f;
            stats.attackSpeed = 0.75f;
            stats.moveSpeed = 350f;
            stats.Initialize();
        }
        
        protected override void InitializeSkills() {
            skills[0] = gameObject.AddComponent<Skill_HouYi_Q>();
            skills[1] = gameObject.AddComponent<Skill_HouYi_W>();
            skills[2] = gameObject.AddComponent<Skill_HouYi_E>();
            skills[3] = gameObject.AddComponent<Skill_HouYi_R>();
        }
        
        public void ShootSun() {
            sunsShot++;
            arrowPower += 0.1f;
        }
    }
    
    public class Skill_HouYi_Q : Skill.SkillBase {
        void Awake() {
            skillName = "射日箭";
            description = "射出强力一箭，穿透敌人";
            hotkey = KeyCode.Q;
            manaCost = 45f;
            cooldown = 5f;
            baseDamage = 160f;
            range = 12f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            GameObject arrow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            arrow.transform.position = caster.transform.position + Vector3.up + caster.transform.forward;
            arrow.transform.localScale = new Vector3(0.15f, 3f, 0.15f);
            arrow.transform.rotation = Quaternion.LookRotation(targetPosition - caster.transform.position) * Quaternion.Euler(90, 0, 0);
            arrow.GetComponent<Renderer>().material.color = new Color(1f, 0.8f, 0.2f);
            
            Rigidbody rb = arrow.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.velocity = (targetPosition - caster.transform.position).normalized * 25f;
            
            Destroy(arrow.GetComponent<Collider>());
            Destroy(arrow, 3f);
        }
    }
    
    public class Skill_HouYi_W : Skill.SkillBase {
        void Awake() {
            skillName = "多重箭";
            description = "同时射出多支箭";
            hotkey = KeyCode.W;
            manaCost = 60f;
            cooldown = 8f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            for (int i = -2; i <= 2; i++) {
                GameObject arrow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                arrow.transform.position = caster.transform.position + Vector3.up;
                arrow.transform.localScale = new Vector3(0.1f, 2.5f, 0.1f);
                
                Quaternion spread = Quaternion.Euler(0, i * 10f, 0);
                Vector3 dir = spread * (targetPosition - caster.transform.position).normalized;
                arrow.transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(90, 0, 0);
                arrow.GetComponent<Renderer>().material.color = Color.red;
                
                Rigidbody rb = arrow.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.velocity = dir * 20f;
                
                Destroy(arrow.GetComponent<Collider>());
                Destroy(arrow, 2.5f);
            }
        }
    }
    
    public class Skill_HouYi_E : Skill.SkillBase {
        void Awake() {
            skillName = "逐日步";
            description = "后羿向指定方向快速移动";
            hotkey = KeyCode.E;
            manaCost = 40f;
            cooldown = 10f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            Vector3 dashDir = (targetPosition - caster.transform.position).normalized;
            caster.transform.position += dashDir * 6f;
            
            // 残影特效
            GameObject afterimage = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            afterimage.transform.position = caster.transform.position - dashDir * 3f;
            afterimage.transform.localScale = Vector3.one;
            afterimage.GetComponent<Renderer>().material.color = new Color(1f, 0.9f, 0.7f, 0.3f);
            Destroy(afterimage.GetComponent<Collider>());
            Destroy(afterimage, 1f);
        }
    }
    
    public class Skill_HouYi_R : Skill.SkillBase {
        void Awake() {
            skillName = "九日连射";
            description = "连续射出九支太阳之箭";
            hotkey = KeyCode.R;
            manaCost = 120f;
            cooldown = 80f;
            baseDamage = 280f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target) {
            caster.StartCoroutine(SunShotCoroutine(caster, targetPosition));
        }
        
        System.Collections.IEnumerator SunShotCoroutine(HeroBase caster, Vector3 targetPos) {
            for (int i = 0; i < 9; i++) {
                GameObject sunArrow = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sunArrow.transform.position = caster.transform.position + Vector3.up * 2f + caster.transform.forward * 2f;
                sunArrow.transform.localScale = Vector3.one * 0.5f;
                sunArrow.GetComponent<Renderer>().material.color = new Color(1f, 0.6f, 0f);
                
                Rigidbody rb = sunArrow.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.velocity = (targetPos - caster.transform.position).normalized * 30f;
                
                Destroy(sunArrow.GetComponent<Collider>());
                Destroy(sunArrow, 3f);
                
                yield return new WaitForSeconds(0.3f);
            }
            
            Hero_HouYi_Full houyi = caster as Hero_HouYi_Full;
            houyi?.ShootSun();
        }
    }
}
