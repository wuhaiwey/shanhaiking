using UnityEngine;
using System.Collections;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 嫦娥 - 法师型英雄（月宫仙子）
    /// </summary>
    public class Hero_ChangE : HeroBase
    {
        [Header("嫦娥特性")]
        public int moonlightStacks = 0;
        public int maxMoonlightStacks = 5;
        public bool isInMoonRealm = false;
        public GameObject moonRabbit;
        
        protected override void Awake()
        {
            base.Awake();
            heroName = "嫦娥";
            heroTitle = "月宫仙子";
            heroType = HeroType.Mage;
            
            stats.maxHealth = 2450f;
            stats.maxMana = 950f;
            stats.attackDamage = 50f;
            stats.abilityPower = 220f;
            stats.armor = 45f;
            stats.magicResist = 70f;
            stats.moveSpeed = 355f;
            stats.attackRange = 7f;
            
            stats.Initialize();
        }
        
        protected override void Update()
        {
            base.Update();
            
            // 月光被动：夜间增强（简化版：每5秒增加一层）
            if (!isInMoonRealm && moonlightStacks < maxMoonlightStacks)
            {
                // 实际游戏中应该基于游戏时间
            }
        }
        
        public void AddMoonlightStack()
        {
            if (moonlightStacks < maxMoonlightStacks)
            {
                moonlightStacks++;
                // 每层增加法术强度
                stats.abilityPower += 10f;
            }
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_ChangE_Q>();
            skills[1] = gameObject.AddComponent<Skill_ChangE_W>();
            skills[2] = gameObject.AddComponent<Skill_ChangE_E>();
            skills[3] = gameObject.AddComponent<Skill_ChangE_R>();
        }
    }
    
    public class Skill_ChangE_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "月华";
            description = "发射月华光束，可弹射多个目标";
            hotkey = KeyCode.Q;
            manaCost = 55f;
            cooldown = 5f;
            baseDamage = 100f;
            damagePerLevel = 30f;
            range = 9f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            GameObject moonBeam = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            moonBeam.transform.position = caster.transform.position + Vector3.up;
            moonBeam.transform.localScale = new Vector3(0.2f, 1f, 0.2f);
            
            Renderer renderer = moonBeam.GetComponent<Renderer>();
            renderer.material.color = new Color(0.8f, 0.9f, 1f);
            
            Rigidbody rb = moonBeam.AddComponent<Rigidbody>();
            rb.useGravity = false;
            
            Vector3 direction = (targetPosition - caster.transform.position).normalized;
            moonBeam.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);
            rb.velocity = direction * 12f;
            
            MoonBeamProjectile proj = moonBeam.AddComponent<MoonBeamProjectile>();
            proj.Initialize(CurrentDamage, caster, 3); // 可弹射3次
            
            Destroy(moonBeam, 4f);
            
            // 增加月光层数
            Hero_ChangE change = caster as Hero_ChangE;
            change?.AddMoonlightStack();
        }
    }
    
    public class MoonBeamProjectile : MonoBehaviour
    {
        private float damage;
        private HeroBase owner;
        private int bouncesRemaining;
        private float bounceRange = 8f;
        
        public void Initialize(float dmg, HeroBase caster, int bounces)
        {
            damage = dmg;
            owner = caster;
            bouncesRemaining = bounces;
        }
        
        void OnCollisionEnter(Collision collision)
        {
            HeroBase enemy = collision.gameObject.GetComponent<HeroBase>();
            if (enemy != null && enemy != owner)
            {
                enemy.TakeDamage(damage, Core.DamageType.Magic, owner);
                
                if (bouncesRemaining > 0)
                {
                    // 寻找下一个目标进行弹射
                    BounceToNextTarget(collision.contacts[0].point);
                }
                
                if (bouncesRemaining <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
        
        void BounceToNextTarget(Vector3 fromPosition)
        {
            Collider[] nearbyEnemies = Physics.OverlapSphere(fromPosition, bounceRange);
            HeroBase nextTarget = null;
            float closestDist = float.MaxValue;
            
            foreach (Collider col in nearbyEnemies)
            {
                HeroBase potentialTarget = col.GetComponent<HeroBase>();
                if (potentialTarget != null && potentialTarget != owner)
                {
                    float dist = Vector3.Distance(fromPosition, potentialTarget.transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        nextTarget = potentialTarget;
                    }
                }
            }
            
            if (nextTarget != null)
            {
                bouncesRemaining--;
                damage *= 0.8f; // 每次弹射伤害降低
                
                // 改变方向
                Vector3 newDirection = (nextTarget.transform.position - fromPosition).normalized;
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.velocity = newDirection * 12f;
                transform.rotation = Quaternion.LookRotation(newDirection) * Quaternion.Euler(90, 0, 0);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    
    public class Skill_ChangE_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "玉兔捣药";
            description = "召唤玉兔治疗友军并造成伤害";
            hotkey = KeyCode.W;
            manaCost = 75f;
            cooldown = 12f;
            baseDamage = 120f;
            healAmount = 150f;
            range = 8f;
        }
        
        public float healAmount = 150f;
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            // 创建玉兔
            GameObject jadeRabbit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            jadeRabbit.name = "JadeRabbit";
            jadeRabbit.transform.position = targetPosition + Vector3.up * 2f;
            jadeRabbit.transform.localScale = Vector3.one * 0.8f;
            
            Renderer renderer = jadeRabbit.GetComponent<Renderer>();
            renderer.material.color = Color.white;
            
            Destroy(jadeRabbit.GetComponent<Collider>());
            
            // 范围效果
            Collider[] hits = Physics.OverlapSphere(targetPosition, range);
            
            foreach (Collider hit in hits)
            {
                HeroBase unit = hit.GetComponent<HeroBase>();
                if (unit != null)
                {
                    if (unit == caster) // 友军治疗
                    {
                        unit.RestoreHealth(healAmount);
                    }
                    else // 敌人伤害
                    {
                        unit.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
                    }
                }
            }
            
            // 动画效果
            caster.StartCoroutine(RabbitAnimation(jadeRabbit));
        }
        
        IEnumerator RabbitAnimation(GameObject rabbit)
        {
            float elapsed = 0f;
            Vector3 startPos = rabbit.transform.position;
            
            while (elapsed < 2f)
            {
                rabbit.transform.position = startPos + Vector3.up * Mathf.Sin(elapsed * 3f) * 0.5f;
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            Destroy(rabbit);
        }
    }
    
    public class Skill_ChangE_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "月影步";
            description = "瞬移到目标位置并进入隐身状态";
            hotkey = KeyCode.E;
            manaCost = 60f;
            cooldown = 14f;
            range = 10f;
            duration = 3f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            // 瞬移
            caster.transform.position = targetPosition;
            
            // 进入隐身
            caster.StartCoroutine(MoonShadow(caster));
        }
        
        IEnumerator MoonShadow(HeroBase caster)
        {
            // 隐身效果
            Renderer[] renderers = caster.GetComponentsInChildren<Renderer>();
            Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();
            
            foreach (Renderer rend in renderers)
            {
                originalColors[rend] = rend.material.color;
                Color transparentColor = rend.material.color;
                transparentColor.a = 0.3f;
                rend.material.color = transparentColor;
            }
            
            // 增加移速
            float originalSpeed = caster.stats.moveSpeed;
            caster.stats.moveSpeed *= 1.3f;
            
            yield return new WaitForSeconds(duration);
            
            // 恢复
            foreach (var kvp in originalColors)
            {
                if (kvp.Key != null)
                {
                    kvp.Key.material.color = kvp.Value;
                }
            }
            
            caster.stats.moveSpeed = originalSpeed;
        }
        
        private System.Collections.Generic.Dictionary<Renderer, Color> originalColors;
    }
    
    public class Skill_ChangE_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "广寒月宫";
            description = "创造月宫领域，领域内友军持续恢复，敌人持续伤害";
            hotkey = KeyCode.R;
            manaCost = 150f;
            cooldown = 75f;
            baseDamage = 80f;
            damagePerLevel = 40f;
            range = 12f;
            duration = 8f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(MoonPalaceUltimate(caster, targetPosition));
        }
        
        IEnumerator MoonPalaceUltimate(HeroBase caster, Vector3 center)
        {
            // 创建月宫领域
            GameObject moonRealm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            moonRealm.name = "MoonPalace";
            moonRealm.transform.position = center;
            moonRealm.transform.localScale = new Vector3(15f, 0.1f, 15f);
            
            Renderer renderer = moonRealm.GetComponent<Renderer>();
            Material mat = new Material(renderer.material);
            mat.color = new Color(0.8f, 0.9f, 1f, 0.4f);
            renderer.material = mat;
            
            Destroy(moonRealm.GetComponent<Collider>());
            
            // 中央月亮
            GameObject moon = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            moon.transform.position = center + Vector3.up * 5f;
            moon.transform.localScale = Vector3.one * 3f;
            
            Renderer moonRenderer = moon.GetComponent<Renderer>();
            moonRenderer.material.color = new Color(1f, 1f, 0.9f);
            
            Destroy(moon.GetComponent<Collider>());
            
            // 嫦娥进入月宫状态
            Hero_ChangE change = caster as Hero_ChangE;
            if (change != null)
            {
                change.isInMoonRealm = true;
            }
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                // 每秒效果
                if (elapsed % 1f < Time.deltaTime)
                {
                    Collider[] hits = Physics.OverlapSphere(center, 7.5f);
                    
                    foreach (Collider hit in hits)
                    {
                        HeroBase unit = hit.GetComponent<HeroBase>();
                        if (unit != null)
                        {
                            if (unit == caster) // 友军恢复
                            {
                                unit.RestoreHealth(CurrentDamage * 0.5f);
                                unit.RestoreMana(20f);
                            }
                            else // 敌人伤害
                            {
                                unit.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
                                // 减速
                                // unit.ApplyBuff(BuffType.SpeedDown, 0.3f, 1f);
                            }
                        }
                    }
                }
                
                // 旋转月亮
                moon.transform.RotateAround(center, Vector3.up, 20f * Time.deltaTime);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            if (change != null)
            {
                change.isInMoonRealm = false;
            }
            
            Destroy(moonRealm);
            Destroy(moon);
        }
    }
}
