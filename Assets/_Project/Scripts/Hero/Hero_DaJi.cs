using UnityEngine;
using System.Collections;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 妲己 - 法师型英雄（妖狐）
    /// </summary>
    public class Hero_DaJi : HeroBase
    {
        [Header("妲己特性")]
        public int charmStacks = 0;
        public int maxCharmStacks = 5;
        public bool isFoxForm = false;
        public float seductionPower = 0f;
        
        protected override void Awake()
        {
            base.Awake();
            heroName = "妲己";
            heroTitle = "妖狐";
            heroType = HeroType.Mage;
            
            stats.maxHealth = 2400f;
            stats.maxMana = 900f;
            stats.attackDamage = 50f;
            stats.abilityPower = 210f;
            stats.armor = 45f;
            stats.magicResist = 65f;
            stats.moveSpeed = 360f;
            stats.attackRange = 7f;
            
            stats.Initialize();
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_DaJi_Q>();
            skills[1] = gameObject.AddComponent<Skill_DaJi_W>();
            skills[2] = gameObject.AddComponent<Skill_DaJi_E>();
            skills[3] = gameObject.AddComponent<Skill_DaJi_R>();
        }
        
        public void AddCharmStack()
        {
            if (charmStacks < maxCharmStacks)
            {
                charmStacks++;
                stats.abilityPower += 12f;
            }
        }
    }
    
    public class Skill_DaJi_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "魅惑之吻";
            description = "发射爱心对敌人造成伤害并魅惑";
            hotkey = KeyCode.Q;
            manaCost = 55f;
            cooldown = 6f;
            baseDamage = 130f;
            damagePerLevel = 30f;
            range = 8f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            GameObject heart = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            heart.transform.position = caster.transform.position + Vector3.up;
            heart.transform.localScale = new Vector3(0.4f, 0.35f, 0.1f);
            
            Renderer renderer = heart.GetComponent<Renderer>();
            renderer.material.color = new Color(1f, 0.4f, 0.6f);
            
            Rigidbody rb = heart.AddComponent<Rigidbody>();
            rb.useGravity = false;
            
            Vector3 direction = (targetPosition - caster.transform.position).normalized;
            rb.velocity = direction * 10f;
            
            HeartProjectile proj = heart.AddComponent<HeartProjectile>();
            proj.Initialize(CurrentDamage, caster);
            
            Destroy(heart, 3f);
            
            Hero_DaJi daji = caster as Hero_DaJi;
            daji?.AddCharmStack();
        }
    }
    
    public class HeartProjectile : MonoBehaviour
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
                // 魅惑效果
                // enemy.ApplyBuff(BuffType.Charm, 0, 1.5f);
                Destroy(gameObject);
            }
        }
    }
    
    public class Skill_DaJi_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "狐火乱舞";
            description = "召唤狐火围绕自身攻击周围敌人";
            hotkey = KeyCode.W;
            manaCost = 70f;
            cooldown = 10f;
            baseDamage = 100f;
            damagePerLevel = 25f;
            duration = 5f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(FoxFireDance(caster));
        }
        
        IEnumerator FoxFireDance(HeroBase caster)
        {
            GameObject[] fires = new GameObject[3];
            
            // 创建3团狐火
            for (int i = 0; i < 3; i++)
            {
                fires[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                fires[i].transform.localScale = Vector3.one * 0.4f;
                
                Renderer renderer = fires[i].GetComponent<Renderer>();
                renderer.material.color = new Color(1f, 0.4f, 0.1f);
                
                Destroy(fires[i].GetComponent<Collider>());
            }
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                // 狐火围绕旋转
                for (int i = 0; i < 3; i++)
                {
                    float angle = (elapsed * 3f + i * 120f * Mathf.Deg2Rad);
                    Vector3 offset = new Vector3(Mathf.Cos(angle) * 3f, 1f, Mathf.Sin(angle) * 3f);
                    fires[i].transform.position = caster.transform.position + offset;
                }
                
                // 每秒伤害
                if (elapsed % 1f < Time.deltaTime)
                {
                    Collider[] hits = Physics.OverlapSphere(caster.transform.position, 3.5f);
                    foreach (Collider hit in hits)
                    {
                        HeroBase enemy = hit.GetComponent<HeroBase>();
                        if (enemy != null && enemy != caster)
                        {
                            enemy.TakeDamage(CurrentDamage / duration, Core.DamageType.Magic, caster);
                        }
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            foreach (GameObject fire in fires)
            {
                Destroy(fire);
            }
        }
    }
    
    public class Skill_DaJi_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "妖狐变身";
            description = "变身为狐狸形态，大幅提升移速和法强";
            hotkey = KeyCode.E;
            manaCost = 60f;
            cooldown = 15f;
            duration = 6f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_DaJi daji = caster as Hero_DaJi;
            if (daji != null)
            {
                daji.isFoxForm = true;
            }
            
            caster.StartCoroutine(FoxForm(caster, daji));
        }
        
        IEnumerator FoxForm(HeroBase caster, Hero_DaJi daji)
        {
            // 保存原始属性
            float originalSpeed = caster.stats.moveSpeed;
            float originalAP = caster.stats.abilityPower;
            Vector3 originalScale = caster.transform.localScale;
            
            // 变身效果
            caster.stats.moveSpeed *= 1.5f;
            caster.stats.abilityPower *= 1.3f;
            caster.transform.localScale = originalScale * 0.7f;
            
            // 尾巴效果
            GameObject tail = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tail.transform.SetParent(caster.transform);
            tail.transform.localPosition = new Vector3(0, 0.5f, -0.5f);
            tail.transform.localScale = new Vector3(0.15f, 0.8f, 0.15f);
            tail.transform.localRotation = Quaternion.Euler(-30, 0, 0);
            
            Renderer tailRenderer = tail.GetComponent<Renderer>();
            tailRenderer.material.color = new Color(1f, 0.6f, 0.8f);
            
            Destroy(tail.GetComponent<Collider>());
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                // 尾巴摆动
                tail.transform.localRotation = Quaternion.Euler(-30 + Mathf.Sin(elapsed * 10f) * 10f, 0, 0);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // 恢复
            caster.stats.moveSpeed = originalSpeed;
            caster.stats.abilityPower = originalAP;
            caster.transform.localScale = originalScale;
            
            if (daji != null)
            {
                daji.isFoxForm = false;
            }
            
            Destroy(tail);
        }
    }
    
    public class Skill_DaJi_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "倾国倾城";
            description = "释放全部魅力，魅惑范围内所有敌人";
            hotkey = KeyCode.R;
            manaCost = 140f;
            cooldown = 75f;
            baseDamage = 300f;
            damagePerLevel = 120f;
            range = 12f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_DaJi daji = caster as Hero_DaJi;
            float damageMultiplier = 1f + (daji?.charmStacks ?? 0) * 0.1f;
            
            caster.StartCoroutine(DevastatingBeauty(caster, targetPosition, damageMultiplier));
        }
        
        IEnumerator DevastatingBeauty(HeroBase caster, Vector3 center, float multiplier)
        {
            // 魅惑领域
            GameObject charmField = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            charmField.transform.position = center;
            charmField.transform.localScale = new Vector3(15f, 0.1f, 15f);
            
            Renderer fieldRenderer = charmField.GetComponent<Renderer>();
            Material fieldMat = new Material(fieldRenderer.material);
            fieldMat.color = new Color(1f, 0.5f, 0.7f, 0.3f);
            fieldRenderer.material = fieldMat;
            
            Destroy(charmField.GetComponent<Collider>());
            
            // 妲己形象放大
            Vector3 originalScale = caster.transform.localScale;
            caster.transform.localScale = originalScale * 1.5f;
            
            // 爱心特效
            for (int i = 0; i < 10; i++)
            {
                float angle = i * 36f * Mathf.Deg2Rad;
                Vector3 pos = center + new Vector3(Mathf.Cos(angle) * 6f, 1f, Mathf.Sin(angle) * 6f);
                
                GameObject heart = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                heart.transform.position = pos;
                heart.transform.localScale = new Vector3(0.5f, 0.4f, 0.1f);
                
                Renderer heartRenderer = heart.GetComponent<Renderer>();
                heartRenderer.material.color = new Color(1f, 0.3f, 0.5f);
                
                Destroy(heart.GetComponent<Collider>());
                Destroy(heart, 2f);
            }
            
            // 魅惑效果
            Collider[] hits = Physics.OverlapSphere(center, 7f);
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    float damage = CurrentDamage * multiplier;
                    enemy.TakeDamage(damage, Core.DamageType.Magic, caster);
                    // 魅惑
                    // enemy.ApplyBuff(BuffType.Charm, 0, 2f);
                }
            }
            
            yield return new WaitForSeconds(3f);
            
            Destroy(charmField);
            caster.transform.localScale = originalScale;
        }
    }
}
