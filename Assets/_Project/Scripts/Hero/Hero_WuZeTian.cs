using UnityEngine;
using System.Collections;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 武则天 - 法师型英雄（女帝）
    /// </summary>
    public class Hero_WuZeTian : HeroBase
    {
        [Header("武则天特性")]
        public int imperialPower = 0;
        public int maxImperialPower = 100;
        public bool isEmpressForm = false;
        
        protected override void Awake()
        {
            base.Awake();
            heroName = "武则天";
            heroTitle = "女帝";
            heroType = HeroType.Mage;
            
            stats.maxHealth = 2550f;
            stats.maxMana = 950f;
            stats.attackDamage = 50f;
            stats.abilityPower = 225f;
            stats.armor = 48f;
            stats.magicResist = 72f;
            stats.moveSpeed = 345f;
            
            stats.Initialize();
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_WuZeTian_Q>();
            skills[1] = gameObject.AddComponent<Skill_WuZeTian_W>();
            skills[2] = gameObject.AddComponent<Skill_WuZeTian_E>();
            skills[3] = gameObject.AddComponent<Skill_WuZeTian_R>();
        }
    }
    
    public class Skill_WuZeTian_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "女帝辉光";
            description = "释放皇家光辉对敌人造成伤害";
            hotkey = KeyCode.Q;
            manaCost = 55f;
            cooldown = 5f;
            baseDamage = 130f;
            damagePerLevel = 32f;
            range = 8f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            GameObject lightOrb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            lightOrb.transform.position = caster.transform.position + Vector3.up;
            lightOrb.transform.localScale = Vector3.one * 0.5f;
            
            Renderer renderer = lightOrb.GetComponent<Renderer>();
            renderer.material.color = new Color(1f, 0.9f, 0.4f);
            
            Rigidbody rb = lightOrb.AddComponent<Rigidbody>();
            rb.useGravity = false;
            
            Vector3 direction = (targetPosition - caster.transform.position).normalized;
            rb.velocity = direction * 12f;
            
            LightOrbProjectile proj = lightOrb.AddComponent<LightOrbProjectile>();
            proj.Initialize(CurrentDamage, caster);
            
            Destroy(lightOrb, 3f);
        }
    }
    
    public class LightOrbProjectile : MonoBehaviour
    {
        private float damage;
        private HeroBase owner;
        
        public void Initialize(float dmg, HeroBase caster)
        {
            damage = dmg;
            owner = caster;
        }
        
        void OnCollisionEnter(Collision collision)
        {
            HeroBase enemy = collision.gameObject.GetComponent<HeroBase>();
            if (enemy != null && enemy != owner)
            {
                enemy.TakeDamage(damage, Core.DamageType.Magic, owner);
                Destroy(gameObject);
            }
        }
    }
    
    public class Skill_WuZeTian_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "威严震慑";
            description = "释放威压眩晕周围敌人";
            hotkey = KeyCode.W;
            manaCost = 70f;
            cooldown = 12f;
            baseDamage = 110f;
            range = 6f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            GameObject shockwave = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shockwave.transform.position = caster.transform.position;
            shockwave.transform.localScale = new Vector3(range * 2f, 0.1f, range * 2f);
            
            Renderer renderer = shockwave.GetComponent<Renderer>();
            Material mat = new Material(renderer.material);
            mat.color = new Color(0.9f, 0.8f, 0.3f, 0.4f);
            renderer.material = mat;
            
            Destroy(shockwave.GetComponent<Collider>());
            
            Collider[] hits = Physics.OverlapSphere(caster.transform.position, range);
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    enemy.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
                    // enemy.ApplyBuff(BuffType.Stun, 0, 1.5f);
                }
            }
            
            Destroy(shockwave, 0.5f);
        }
    }
    
    public class Skill_WuZeTian_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "帝王之威";
            description = "强化自身，提升法强和移速";
            hotkey = KeyCode.E;
            manaCost = 60f;
            cooldown = 14f;
            duration = 5f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(ImperialMight(caster));
        }
        
        IEnumerator ImperialMight(HeroBase caster)
        {
            float originalAP = caster.stats.abilityPower;
            float originalSpeed = caster.stats.moveSpeed;
            
            caster.stats.abilityPower *= 1.4f;
            caster.stats.moveSpeed *= 1.3f;
            
            GameObject aura = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            aura.transform.SetParent(caster.transform);
            aura.transform.localPosition = Vector3.zero;
            aura.transform.localScale = Vector3.one * 2f;
            
            Renderer renderer = aura.GetComponent<Renderer>();
            Material mat = new Material(renderer.material);
            mat.color = new Color(1f, 0.85f, 0.2f, 0.3f);
            renderer.material = mat;
            
            Destroy(aura.GetComponent<Collider>());
            
            yield return new WaitForSeconds(duration);
            
            caster.stats.abilityPower = originalAP;
            caster.stats.moveSpeed = originalSpeed;
            Destroy(aura);
        }
    }
    
    public class Skill_WuZeTian_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "君临天下";
            description = "召唤皇权威压全场";
            hotkey = KeyCode.R;
            manaCost = 150f;
            cooldown = 80f;
            baseDamage = 350f;
            damagePerLevel = 150f;
            range = 15f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(EmpressDominion(caster, targetPosition));
        }
        
        IEnumerator EmpressDominion(HeroBase caster, Vector3 center)
        {
            GameObject throne = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            throne.transform.position = center + Vector3.up * 0.1f;
            throne.transform.localScale = new Vector3(8f, 0.2f, 8f);
            
            Renderer renderer = throne.GetComponent<Renderer>();
            Material mat = new Material(renderer.material);
            mat.color = new Color(0.9f, 0.75f, 0.25f, 0.5f);
            renderer.material = mat;
            
            Destroy(throne.GetComponent<Collider>());
            
            GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pillar.transform.position = center + Vector3.up * 5f;
            pillar.transform.localScale = new Vector3(2f, 10f, 2f);
            
            Renderer pillarRenderer = pillar.GetComponent<Renderer>();
            pillarRenderer.material.color = new Color(1f, 0.9f, 0.5f, 0.6f);
            
            Destroy(pillar.GetComponent<Collider>());
            
            float elapsed = 0f;
            float duration = 6f;
            
            while (elapsed < duration)
            {
                if (elapsed % 0.8f < Time.deltaTime)
                {
                    Collider[] hits = Physics.OverlapSphere(center, 6f);
                    foreach (Collider hit in hits)
                    {
                        HeroBase enemy = hit.GetComponent<HeroBase>();
                        if (enemy != null && enemy != caster)
                        {
                            enemy.TakeDamage(CurrentDamage / (duration / 0.8f), Core.DamageType.Magic, caster);
                        }
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            Destroy(throne);
            Destroy(pillar);
        }
    }
}
