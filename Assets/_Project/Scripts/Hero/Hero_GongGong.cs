using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 共工 - 法师/辅助型英雄（水神）
    /// </summary>
    public class Hero_GongGong : HeroBase
    {
        [Header("共工特性")]
        public bool isInWaterForm = false;
        public float waterManaRegen = 5f;
        public List<GameObject> waterZones = new List<GameObject>();
        
        protected override void Awake()
        {
            base.Awake();
            heroName = "共工";
            heroTitle = "水神";
            heroType = HeroType.Mage;
            
            stats.maxHealth = 2800f;
            stats.maxMana = 850f;
            stats.attackDamage = 55f;
            stats.abilityPower = 195f;
            stats.armor = 60f;
            stats.magicResist = 65f;
            stats.moveSpeed = 345f;
            stats.attackRange = 6f;
            
            stats.Initialize();
        }
        
        protected override void Update()
        {
            base.Update();
            
            // 水形态被动回复
            if (isInWaterForm)
            {
                RestoreMana(waterManaRegen * Time.deltaTime);
            }
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_GongGong_Q>();
            skills[1] = gameObject.AddComponent<Skill_GongGong_W>();
            skills[2] = gameObject.AddComponent<Skill_GongGong_E>();
            skills[3] = gameObject.AddComponent<Skill_GongGong_R>();
        }
        
        public void CreateWaterZone(Vector3 position, float duration)
        {
            StartCoroutine(SpawnWaterZone(position, duration));
        }
        
        IEnumerator SpawnWaterZone(Vector3 position, float duration)
        {
            GameObject zone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            zone.transform.position = position;
            zone.transform.localScale = new Vector3(6f, 0.1f, 6f);
            
            Renderer renderer = zone.GetComponent<Renderer>();
            Material mat = new Material(renderer.material);
            mat.color = new Color(0.2f, 0.5f, 1f, 0.5f);
            renderer.material = mat;
            
            Collider col = zone.GetComponent<Collider>();
            col.isTrigger = true;
            
            WaterZone zoneComponent = zone.AddComponent<WaterZone>();
            zoneComponent.owner = this;
            
            waterZones.Add(zone);
            
            yield return new WaitForSeconds(duration);
            
            waterZones.Remove(zone);
            Destroy(zone);
        }
    }
    
    public class WaterZone : MonoBehaviour
    {
        public Hero_GongGong owner;
        
        void OnTriggerStay(Collider other)
        {
            HeroBase hero = other.GetComponent<HeroBase>();
            if (hero != null && hero == owner)
            {
                // 共工在水域中获得加成
                owner.isInWaterForm = true;
            }
        }
        
        void OnTriggerExit(Collider other)
        {
            HeroBase hero = other.GetComponent<HeroBase>();
            if (hero != null && hero == owner)
            {
                owner.isInWaterForm = false;
            }
        }
    }
    
    public class Skill_GongGong_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "水弹";
            description = "发射水弹，命中后溅射并减速敌人";
            hotkey = KeyCode.Q;
            manaCost = 50f;
            cooldown = 4f;
            baseDamage = 110f;
            damagePerLevel = 25f;
            range = 8f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            GameObject waterBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            waterBall.transform.position = caster.transform.position + Vector3.up;
            waterBall.transform.localScale = Vector3.one * 0.5f;
            
            Renderer renderer = waterBall.GetComponent<Renderer>();
            renderer.material.color = Color.blue;
            
            Rigidbody rb = waterBall.AddComponent<Rigidbody>();
            rb.useGravity = false;
            
            Vector3 direction = (targetPosition - caster.transform.position).normalized;
            rb.velocity = direction * 12f;
            
            WaterBallProjectile proj = waterBall.AddComponent<WaterBallProjectile>();
            proj.Initialize(CurrentDamage, caster);
            
            Destroy(waterBall, 3f);
        }
    }
    
    public class WaterBallProjectile : MonoBehaviour
    {
        private float damage;
        private HeroBase owner;
        private bool hasHit = false;
        
        public void Initialize(float dmg, HeroBase caster)
        {
            damage = dmg;
            owner = caster;
        }
        
        void OnCollisionEnter(Collision collision)
        {
            if (hasHit) return;
            
            HeroBase enemy = collision.gameObject.GetComponent<HeroBase>();
            if (enemy != null && enemy != owner)
            {
                hasHit = true;
                enemy.TakeDamage(damage, Core.DamageType.Magic, owner);
                
                // 减速效果
                // enemy.ApplyBuff(BuffType.SpeedDown, 0.3f, 2f);
                
                // 溅射
                Collider[] splashHits = Physics.OverlapSphere(transform.position, 2f);
                foreach (Collider hit in splashHits)
                {
                    HeroBase splashEnemy = hit.GetComponent<HeroBase>();
                    if (splashEnemy != null && splashEnemy != owner && splashEnemy != enemy)
                    {
                        splashEnemy.TakeDamage(damage * 0.5f, Core.DamageType.Magic, owner);
                    }
                }
                
                Destroy(gameObject);
            }
        }
    }
    
    public class Skill_GongGong_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "水幕";
            description = "创造水幕阻挡敌人并治疗友军";
            hotkey = KeyCode.W;
            manaCost = 80f;
            cooldown = 12f;
            baseDamage = 0f;
            range = 10f;
            duration = 5f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_GongGong gonggong = caster as Hero_GongGong;
            if (gonggong != null)
            {
                gonggong.CreateWaterZone(targetPosition, duration);
            }
        }
    }
    
    public class Skill_GongGong_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "激流";
            description = "化为水流向前冲刺，不可阻挡";
            hotkey = KeyCode.E;
            manaCost = 70f;
            cooldown = 10f;
            range = 12f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(WaterDash(caster, targetPosition));
        }
        
        IEnumerator WaterDash(HeroBase caster, Vector3 targetPos)
        {
            Vector3 startPos = caster.transform.position;
            float dashTime = 0.4f;
            float elapsed = 0f;
            
            // 无视碰撞
            Collider col = caster.GetComponent<Collider>();
            bool originalTrigger = col.isTrigger;
            col.isTrigger = true;
            
            // 水形态特效
            GameObject waterEffect = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            waterEffect.transform.position = caster.transform.position;
            waterEffect.transform.localScale = Vector3.one * 1.5f;
            waterEffect.transform.SetParent(caster.transform);
            
            Renderer renderer = waterEffect.GetComponent<Renderer>();
            renderer.material.color = new Color(0.2f, 0.6f, 1f, 0.5f);
            
            Destroy(waterEffect.GetComponent<Collider>());
            
            while (elapsed < dashTime)
            {
                caster.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / dashTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            caster.transform.position = targetPos;
            col.isTrigger = originalTrigger;
            Destroy(waterEffect);
            
            // 落地伤害
            Collider[] hits = Physics.OverlapSphere(caster.transform.position, 3f);
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    enemy.TakeDamage(100f, Core.DamageType.Magic, caster);
                    // 击退
                    Vector3 knockback = (enemy.transform.position - caster.transform.position).normalized;
                    enemy.transform.position += knockback * 2f;
                }
            }
        }
    }
    
    public class Skill_GongGong_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "怒触不周山";
            description = "召唤洪水淹没战场，大范围伤害和减速";
            hotkey = KeyCode.R;
            manaCost = 150f;
            cooldown = 60f;
            baseDamage = 400f;
            damagePerLevel = 150f;
            range = 15f;
            duration = 6f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(FloodUltimate(caster, targetPosition));
        }
        
        IEnumerator FloodUltimate(HeroBase caster, Vector3 center)
        {
            // 创建洪水区域
            GameObject floodZone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            floodZone.transform.position = center;
            floodZone.transform.localScale = new Vector3(15f, 0.2f, 15f);
            
            Renderer renderer = floodZone.GetComponent<Renderer>();
            Material mat = new Material(renderer.material);
            mat.color = new Color(0.1f, 0.3f, 0.8f, 0.6f);
            renderer.material = mat;
            
            Destroy(floodZone.GetComponent<Collider>());
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                // 每秒造成伤害
                if (elapsed % 1f < Time.deltaTime)
                {
                    Collider[] hits = Physics.OverlapSphere(center, 7.5f);
                    foreach (Collider hit in hits)
                    {
                        HeroBase enemy = hit.GetComponent<HeroBase>();
                        if (enemy != null && enemy != caster)
                        {
                            enemy.TakeDamage(CurrentDamage / duration, Core.DamageType.Magic, caster);
                            // 减速
                            // enemy.ApplyBuff(BuffType.SpeedDown, 0.5f, 1f);
                        }
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            Destroy(floodZone);
        }
    }
}
