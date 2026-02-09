using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 祝融 - 法师/战士型英雄（火神）
    /// </summary>
    public class Hero_ZhuRong : HeroBase
    {
        [Header("祝融特性")]
        public int flameStacks = 0;
        public int maxFlameStacks = 10;
        public float flameDamageBonus = 0.05f;
        public bool isOverheated = false;
        public float overheatThreshold = 0.8f;
        
        protected override void Awake()
        {
            base.Awake();
            heroName = "祝融";
            heroTitle = "火神";
            heroType = HeroType.Mage;
            
            stats.maxHealth = 3000f;
            stats.maxMana = 700f;
            stats.attackDamage = 110f;
            stats.abilityPower = 185f;
            stats.armor = 80f;
            stats.magicResist = 50f;
            stats.moveSpeed = 360f;
            stats.attackRange = 5f;
            
            stats.Initialize();
        }
        
        protected override void Update()
        {
            base.Update();
            
            // 过热检测
            if (!isOverheated && flameStacks >= maxFlameStacks * overheatThreshold)
            {
                EnterOverheat();
            }
            else if (isOverheated && flameStacks < maxFlameStacks * 0.3f)
            {
                ExitOverheat();
            }
        }
        
        void EnterOverheat()
        {
            isOverheated = true;
            stats.abilityPower *= 1.3f;
            stats.moveSpeed *= 0.8f;
            
            // 过热视觉效果
            Debug.Log("祝融进入过热状态！");
        }
        
        void ExitOverheat()
        {
            isOverheated = false;
            stats.abilityPower /= 1.3f;
            stats.moveSpeed /= 0.8f;
        }
        
        public void AddFlameStack(int count = 1)
        {
            flameStacks = Mathf.Min(flameStacks + count, maxFlameStacks);
        }
        
        public void ConsumeFlameStacks(int count)
        {
            flameStacks = Mathf.Max(0, flameStacks - count);
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_ZhuRong_Q>();
            skills[1] = gameObject.AddComponent<Skill_ZhuRong_W>();
            skills[2] = gameObject.AddComponent<Skill_ZhuRong_E>();
            skills[3] = gameObject.AddComponent<Skill_ZhuRong_R>();
        }
    }
    
    public class Skill_ZhuRong_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "火焰喷射";
            description = "持续喷射火焰，叠加燃烧层数";
            hotkey = KeyCode.Q;
            manaCost = 15f;
            cooldown = 0.5f;
            baseDamage = 40f;
            damagePerLevel = 10f;
            range = 7f;
            duration = 3f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(FlameSpray(caster, targetPosition));
        }
        
        IEnumerator FlameSpray(HeroBase caster, Vector3 targetPos)
        {
            Hero_ZhuRong zhurong = caster as Hero_ZhuRong;
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                Vector3 direction = (targetPos - caster.transform.position).normalized;
                
                // 创建火焰投射物
                GameObject flame = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                flame.transform.position = caster.transform.position + Vector3.up + direction * 2f;
                flame.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
                flame.transform.rotation = Quaternion.LookRotation(direction);
                
                Renderer renderer = flame.GetComponent<Renderer>();
                renderer.material.color = Color.red;
                
                Rigidbody rb = flame.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.velocity = direction * 15f;
                
                FlameProjectile proj = flame.AddComponent<FlameProjectile>();
                proj.Initialize(CurrentDamage / duration, caster, zhurong);
                
                Destroy(flame, 0.5f);
                
                elapsed += 0.1f;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    
    public class FlameProjectile : MonoBehaviour
    {
        private float damage;
        private HeroBase owner;
        private Hero_ZhuRong zhurong;
        
        public void Initialize(float dmg, HeroBase caster, Hero_ZhuRong zr)
        {
            damage = dmg;
            owner = caster;
            zhurong = zr;
        }
        
        void OnCollisionEnter(Collision collision)
        {
            HeroBase enemy = collision.gameObject.GetComponent<HeroBase>();
            if (enemy != null && enemy != owner)
            {
                enemy.TakeDamage(damage, Core.DamageType.Magic, owner);
                
                // 叠加火焰层数
                zhurong?.AddFlameStack();
                
                Destroy(gameObject);
            }
        }
    }
    
    public class Skill_ZhuRong_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "火墙";
            description = "创造一道火墙阻挡敌人并造成伤害";
            hotkey = KeyCode.W;
            manaCost = 80f;
            cooldown = 12f;
            baseDamage = 80f;
            damagePerLevel = 20f;
            range = 10f;
            duration = 5f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_ZhuRong zhurong = caster as Hero_ZhuRong;
            caster.StartCoroutine(CreateFireWall(caster, targetPosition, zhurong));
        }
        
        IEnumerator CreateFireWall(HeroBase caster, Vector3 center, Hero_ZhuRong zhurong)
        {
            Vector3 direction = (center - caster.transform.position).normalized;
            Vector3 wallCenter = caster.transform.position + direction * 5f;
            
            GameObject[] fireSegments = new GameObject[5];
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;
            
            for (int i = 0; i < 5; i++)
            {
                float offset = (i - 2) * 1.5f;
                Vector3 pos = wallCenter + perpendicular * offset;
                
                fireSegments[i] = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                fireSegments[i].transform.position = pos;
                fireSegments[i].transform.localScale = new Vector3(1f, 3f, 1f);
                
                Renderer renderer = fireSegments[i].GetComponent<Renderer>();
                renderer.material.color = new Color(1f, 0.3f, 0f);
                
                Destroy(fireSegments[i].GetComponent<Collider>());
            }
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                // 每秒造成伤害
                if (elapsed % 0.5f < Time.deltaTime)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Collider[] hits = Physics.OverlapSphere(fireSegments[i].transform.position, 1f);
                        foreach (Collider hit in hits)
                        {
                            HeroBase enemy = hit.GetComponent<HeroBase>();
                            if (enemy != null && enemy != caster)
                            {
                                enemy.TakeDamage(CurrentDamage / (duration / 0.5f), Core.DamageType.Magic, caster);
                                zhurong?.AddFlameStack();
                            }
                        }
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            foreach (GameObject segment in fireSegments)
            {
                Destroy(segment);
            }
        }
    }
    
    public class Skill_ZhuRong_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "火遁";
            description = "化为火焰向前冲刺，对路径上敌人造成伤害";
            hotkey = KeyCode.E;
            manaCost = 60f;
            cooldown = 10f;
            baseDamage = 120f;
            damagePerLevel = 30f;
            range = 8f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_ZhuRong zhurong = caster as Hero_ZhuRong;
            caster.StartCoroutine(FireDash(caster, targetPosition, zhurong));
        }
        
        IEnumerator FireDash(HeroBase caster, Vector3 targetPos, Hero_ZhuRong zhurong)
        {
            Vector3 startPos = caster.transform.position;
            Vector3 direction = (targetPos - startPos).normalized;
            float distance = Vector3.Distance(startPos, targetPos);
            float dashTime = 0.3f;
            float elapsed = 0f;
            
            // 火焰轨迹
            GameObject fireTrail = new GameObject("FireTrail");
            
            // 无视碰撞
            Collider col = caster.GetComponent<Collider>();
            bool wasTrigger = col.isTrigger;
            col.isTrigger = true;
            
            while (elapsed < dashTime)
            {
                float t = elapsed / dashTime;
                Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
                caster.transform.position = currentPos;
                
                // 创建火焰轨迹
                GameObject trail = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                trail.transform.position = currentPos;
                trail.transform.localScale = Vector3.one * 1.5f;
                
                Renderer renderer = trail.GetComponent<Renderer>();
                renderer.material.color = new Color(1f, 0.5f, 0f);
                
                Destroy(trail.GetComponent<Collider>());
                Destroy(trail, 1f);
                
                // 路径伤害
                Collider[] hits = Physics.OverlapSphere(currentPos, 2f);
                foreach (Collider hit in hits)
                {
                    HeroBase enemy = hit.GetComponent<HeroBase>();
                    if (enemy != null && enemy != caster)
                    {
                        enemy.TakeDamage(CurrentDamage * Time.deltaTime / dashTime, Core.DamageType.Magic, caster);
                        zhurong?.AddFlameStack();
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            caster.transform.position = targetPos;
            col.isTrigger = wasTrigger;
        }
    }
    
    public class Skill_ZhuRong_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "炼狱火海";
            description = "召唤炼狱火海，大范围持续伤害并降低敌人魔抗";
            hotkey = KeyCode.R;
            manaCost = 150f;
            cooldown = 65f;
            baseDamage = 350f;
            damagePerLevel = 150f;
            range = 12f;
            duration = 8f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_ZhuRong zhurong = caster as Hero_ZhuRong;
            caster.StartCoroutine(InfernoUltimate(caster, targetPosition, zhurong));
        }
        
        IEnumerator InfernoUltimate(HeroBase caster, Vector3 center, Hero_ZhuRong zhurong)
        {
            // 创建炼狱区域
            GameObject inferno = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            inferno.transform.position = center;
            inferno.transform.localScale = new Vector3(12f, 0.1f, 12f);
            
            Renderer renderer = inferno.GetComponent<Renderer>();
            Material mat = new Material(renderer.material);
            mat.color = new Color(1f, 0.2f, 0f, 0.6f);
            renderer.material = mat;
            
            Destroy(inferno.GetComponent<Collider>());
            
            // 中心火柱
            GameObject firePillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            firePillar.transform.position = center + Vector3.up * 3f;
            firePillar.transform.localScale = new Vector3(3f, 6f, 3f);
            
            Renderer pillarRenderer = firePillar.GetComponent<Renderer>();
            pillarRenderer.material.color = new Color(1f, 0.5f, 0f);
            
            Destroy(firePillar.GetComponent<Collider>());
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                // 每秒多次伤害
                if (elapsed % 0.5f < Time.deltaTime)
                {
                    Collider[] hits = Physics.OverlapSphere(center, 6f);
                    foreach (Collider hit in hits)
                    {
                        HeroBase enemy = hit.GetComponent<HeroBase>();
                        if (enemy != null && enemy != caster)
                        {
                            float damage = CurrentDamage / (duration / 0.5f);
                            
                            // 过热状态伤害加成
                            if (zhurong != null && zhurong.isOverheated)
                            {
                                damage *= 1.5f;
                            }
                            
                            enemy.TakeDamage(damage, Core.DamageType.Magic, caster);
                            zhurong?.AddFlameStack();
                        }
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            Destroy(inferno);
            Destroy(firePillar);
        }
    }
}
