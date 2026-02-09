using UnityEngine;
using System.Collections;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 铁扇公主 - 法师型英雄（罗刹女）
    /// </summary>
    public class Hero_TieShanGongZhu : HeroBase
    {
        [Header("铁扇公主特性")]
        public float windPower = 0f;
        public float maxWindPower = 100f;
        public bool isFanning = false;
        public int flameStacks = 0;
        
        protected override void Awake()
        {
            base.Awake();
            heroName = "铁扇公主";
            heroTitle = "罗刹女";
            heroType = HeroType.Mage;
            
            stats.maxHealth = 2500f;
            stats.maxMana = 850f;
            stats.attackDamage = 55f;
            stats.abilityPower = 200f;
            stats.armor = 50f;
            stats.magicResist = 70f;
            stats.moveSpeed = 350f;
            
            stats.Initialize();
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_TieShan_Q>();
            skills[1] = gameObject.AddComponent<Skill_TieShan_W>();
            skills[2] = gameObject.AddComponent<Skill_TieShan_E>();
            skills[3] = gameObject.AddComponent<Skill_TieShan_R>();
        }
    }
    
    public class Skill_TieShan_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "芭蕉扇";
            description = "挥动芭蕉扇击退敌人并造成伤害";
            hotkey = KeyCode.Q;
            manaCost = 55f;
            cooldown = 6f;
            baseDamage = 140f;
            range = 8f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            // 扇形风效果
            GameObject wind = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wind.transform.position = caster.transform.position + caster.transform.forward * 4f;
            wind.transform.localScale = new Vector3(8f, 0.1f, 6f);
            wind.transform.rotation = caster.transform.rotation;
            
            Renderer renderer = wind.GetComponent<Renderer>();
            Material mat = new Material(renderer.material);
            mat.color = new Color(0.8f, 0.9f, 0.95f, 0.3f);
            renderer.material = mat;
            
            Destroy(wind.GetComponent<Collider>());
            
            // 击退伤害
            Collider[] hits = Physics.OverlapSphere(caster.transform.position + caster.transform.forward * 4f, 4f);
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    enemy.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
                    
                    // 击退
                    Vector3 pushDir = (enemy.transform.position - caster.transform.position).normalized;
                    enemy.transform.position += pushDir * 5f;
                }
            }
            
            Destroy(wind, 0.5f);
        }
    }
    
    public class Skill_TieShan_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "三昧真火";
            description = "召唤三昧真火燃烧敌人";
            hotkey = KeyCode.W;
            manaCost = 70f;
            cooldown = 10f;
            baseDamage = 180f;
            range = 7f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            GameObject fire = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            fire.transform.position = targetPosition;
            fire.transform.localScale = new Vector3(4f, 0.1f, 4f);
            
            Renderer renderer = fire.GetComponent<Renderer>();
            renderer.material.color = new Color(1f, 0.3f, 0.1f, 0.5f);
            
            Destroy(fire.GetComponent<Collider>());
            
            caster.StartCoroutine(FireBurn(caster, targetPosition, fire));
        }
        
        IEnumerator FireBurn(HeroBase caster, Vector3 center, GameObject fireObj)
        {
            float elapsed = 0f;
            float duration = 4f;
            
            while (elapsed < duration)
            {
                // 持续伤害
                if (elapsed % 0.5f < Time.deltaTime)
                {
                    Collider[] hits = Physics.OverlapSphere(center, 2.5f);
                    foreach (Collider hit in hits)
                    {
                        HeroBase enemy = hit.GetComponent<HeroBase>();
                        if (enemy != null && enemy != caster)
                        {
                            enemy.TakeDamage(CurrentDamage / 8f, Core.DamageType.Magic, caster);
                        }
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            Destroy(fireObj);
        }
    }
    
    public class Skill_TieShan_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "风之护盾";
            description = "用风力保护自己，反弹远程攻击";
            hotkey = KeyCode.E;
            manaCost = 60f;
            cooldown = 12f;
            duration = 4f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(WindShield(caster));
        }
        
        IEnumerator WindShield(HeroBase caster)
        {
            GameObject shield = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shield.transform.SetParent(caster.transform);
            shield.transform.localPosition = Vector3.zero;
            shield.transform.localScale = Vector3.one * 2.5f;
            
            Renderer renderer = shield.GetComponent<Renderer>();
            Material mat = new Material(renderer.material);
            mat.color = new Color(0.7f, 0.9f, 1f, 0.2f);
            renderer.material = mat;
            
            Destroy(shield.GetComponent<Collider>());
            
            yield return new WaitForSeconds(duration);
            
            Destroy(shield);
        }
    }
    
    public class Skill_TieShan_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "火焰风暴";
            description = "结合风火之力创造毁灭性风暴";
            hotkey = KeyCode.R;
            manaCost = 140f;
            cooldown = 80f;
            range = 12f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(FireStorm(caster, targetPosition));
        }
        
        IEnumerator FireStorm(HeroBase caster, Vector3 center)
        {
            // 风暴核心
            GameObject storm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            storm.transform.position = center;
            storm.transform.localScale = new Vector3(10f, 0.1f, 10f);
            
            Renderer renderer = storm.GetComponent<Renderer>();
            renderer.material.color = new Color(1f, 0.5f, 0.2f, 0.4f);
            
            Destroy(storm.GetComponent<Collider>());
            
            float elapsed = 0f;
            float duration = 6f;
            
            while (elapsed < duration)
            {
                storm.transform.Rotate(Vector3.up, 180f * Time.deltaTime);
                
                // 持续伤害
                if (elapsed % 0.3f < Time.deltaTime)
                {
                    Collider[] hits = Physics.OverlapSphere(center, 5f);
                    foreach (Collider hit in hits)
                    {
                        HeroBase enemy = hit.GetComponent<HeroBase>();
                        if (enemy != null && enemy != caster)
                        {
                            enemy.TakeDamage(80f, Core.DamageType.Magic, caster);
                            
                            // 向中心吸引
                            Vector3 pullDir = (center - enemy.transform.position).normalized;
                            enemy.transform.position += pullDir * Time.deltaTime * 3f;
                        }
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            Destroy(storm);
        }
    }
}
