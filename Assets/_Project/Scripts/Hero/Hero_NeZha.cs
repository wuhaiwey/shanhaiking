using UnityEngine;
using System.Collections;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 哪吒 - 战士型英雄（三太子）
    /// </summary>
    public class Hero_NeZha : HeroBase
    {
        [Header("哪吒特性")]
        public bool hasWindFireWheels = true;
        public bool hasUniverseRing = true;
        public bool hasRedSash = true;
        public int wheelStacks = 0;
        public int maxWheelStacks = 3;
        
        protected override void Awake()
        {
            base.Awake();
            heroName = "哪吒";
            heroTitle = "三太子";
            heroType = HeroType.Warrior;
            
            stats.maxHealth = 3200f;
            stats.maxMana = 600f;
            stats.attackDamage = 155f;
            stats.attackSpeed = 1.35f;
            stats.armor = 75f;
            stats.magicResist = 60f;
            stats.moveSpeed = 375f; // 风火轮加速
            
            stats.Initialize();
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_NeZha_Q>();
            skills[1] = gameObject.AddComponent<Skill_NeZha_W>();
            skills[2] = gameObject.AddComponent<Skill_NeZha_E>();
            skills[3] = gameObject.AddComponent<Skill_NeZha_R>();
        }
        
        public void AddWheelStack()
        {
            if (wheelStacks < maxWheelStacks)
            {
                wheelStacks++;
                stats.moveSpeed += 20f; // 每层增加移速
            }
        }
    }
    
    public class Skill_NeZha_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "火尖枪";
            description = "刺出火尖枪，对直线敌人造成伤害";
            hotkey = KeyCode.Q;
            manaCost = 45f;
            cooldown = 5f;
            baseDamage = 150f;
            damagePerLevel = 35f;
            range = 7f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_NeZha nezha = caster as Hero_NeZha;
            
            // 火尖枪突刺
            GameObject spear = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            spear.transform.position = caster.transform.position + Vector3.up + caster.transform.forward * 3f;
            spear.transform.localScale = new Vector3(0.1f, 3f, 0.1f);
            spear.transform.rotation = caster.transform.rotation * Quaternion.Euler(90, 0, 0);
            
            Renderer renderer = spear.GetComponent<Renderer>();
            renderer.material.color = new Color(1f, 0.3f, 0.1f);
            
            Destroy(spear.GetComponent<Collider>());
            
            // 直线伤害
            RaycastHit[] hits = Physics.RaycastAll(caster.transform.position + Vector3.up, 
                (targetPosition - caster.transform.position).normalized, range);
            
            foreach (RaycastHit hit in hits)
            {
                HeroBase enemy = hit.collider.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    enemy.TakeDamage(CurrentDamage, Core.DamageType.Physical, caster);
                    // 火焰效果
                    // enemy.ApplyBuff(BuffType.Burn, 0.1f, 3f);
                }
            }
            
            Destroy(spear, 0.3f);
            nezha?.AddWheelStack();
        }
    }
    
    public class Skill_NeZha_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "乾坤圈";
            description = "投掷乾坤圈，可弹射多个目标";
            hotkey = KeyCode.W;
            manaCost = 60f;
            cooldown = 10f;
            baseDamage = 130f;
            damagePerLevel = 30f;
            range = 10f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Torus);
            ring.transform.position = caster.transform.position + Vector3.up;
            ring.transform.localScale = new Vector3(0.5f, 0.5f, 0.1f);
            
            Renderer renderer = ring.GetComponent<Renderer>();
            renderer.material.color = new Color(1f, 0.8f, 0.2f);
            
            Rigidbody rb = ring.AddComponent<Rigidbody>();
            rb.useGravity = false;
            
            Vector3 direction = (targetPosition - caster.transform.position).normalized;
            rb.velocity = direction * 15f;
            rb.angularVelocity = new Vector3(0, 0, 360);
            
            UniverseRingProjectile proj = ring.AddComponent<UniverseRingProjectile>();
            proj.Initialize(CurrentDamage, caster, 4); // 弹射4次
            
            Destroy(ring, 5f);
        }
    }
    
    public class UniverseRingProjectile : MonoBehaviour
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
        
        void OnTriggerEnter(Collider other)
        {
            HeroBase enemy = other.GetComponent<HeroBase>();
            if (enemy != null && enemy != owner)
            {
                enemy.TakeDamage(damage, Core.DamageType.Physical, owner);
                
                if (bouncesRemaining > 0)
                {
                    BounceToNextTarget(other.transform.position);
                }
                else
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
                damage *= 0.85f; // 每次弹射伤害降低
                
                // 改变方向
                Rigidbody rb = GetComponent<Rigidbody>();
                Vector3 newDirection = (nextTarget.transform.position - fromPosition).normalized;
                rb.velocity = newDirection * 15f;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    
    public class Skill_NeZha_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "混天绫";
            description = "挥舞混天绫束缚周围敌人";
            hotkey = KeyCode.E;
            manaCost = 70f;
            cooldown = 12f;
            range = 6f;
            duration = 2f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(RedSashBinding(caster));
        }
        
        IEnumerator RedSashBinding(HeroBase caster)
        {
            // 混天绫效果
            GameObject sash = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            sash.transform.position = caster.transform.position;
            sash.transform.localScale = new Vector3(range * 2f, 0.1f, range * 2f);
            
            Renderer renderer = sash.GetComponent<Renderer>();
            Material mat = new Material(renderer.material);
            mat.color = new Color(1f, 0.2f, 0.2f, 0.3f);
            renderer.material = mat;
            
            Destroy(sash.GetComponent<Collider>());
            
            // 束缚敌人
            Collider[] hits = Physics.OverlapSphere(caster.transform.position, range);
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    // 束缚效果
                    // enemy.ApplyBuff(BuffType.Stun, 0, duration);
                    // enemy.ApplyBuff(BuffType.SpeedDown, 0.5f, duration);
                }
            }
            
            // 旋转效果
            float elapsed = 0f;
            while (elapsed < duration)
            {
                sash.transform.Rotate(Vector3.up, 180f * Time.deltaTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            Destroy(sash);
        }
    }
    
    public class Skill_NeZha_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "三头六臂";
            description = "化身为三头六臂，大幅提升战斗力";
            hotkey = KeyCode.R;
            manaCost = 120f;
            cooldown = 75f;
            duration = 10f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(ThreeHeadSixArms(caster));
        }
        
        IEnumerator ThreeHeadSixArms(HeroBase caster)
        {
            Hero_NeZha nezha = caster as Hero_NeZha;
            
            // 保存原始属性
            float originalAttack = caster.stats.attackDamage;
            float originalAttackSpeed = caster.stats.attackSpeed;
            
            // 三头六臂加成
            caster.stats.attackDamage *= 1.8f;
            caster.stats.attackSpeed *= 1.5f;
            
            // 额外手臂视觉效果
            GameObject[] extraArms = new GameObject[4];
            for (int i = 0; i < 4; i++)
            {
                bpy.ops.mesh.primitive_cylinder_add(radius=0.08, depth=0.6);
                extraArms[i] = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                extraArms[i].name = $"ExtraArm_{i}";
                extraArms[i].transform.SetParent(caster.transform);
                
                float angle = i * 90f * Mathf.Deg2Rad;
                extraArms[i].transform.localPosition = new Vector3(Mathf.Cos(angle) * 0.6f, 1.3f, Mathf.Sin(angle) * 0.6f);
                extraArms[i].transform.localRotation = Quaternion.Euler(0, i * 90f, 0);
                extraArms[i].transform.localScale = new Vector3(0.08f, 0.6f, 0.08f);
                
                Renderer renderer = extraArms[i].GetComponent<Renderer>();
                renderer.material.color = new Color(0.96f, 0.80f, 0.69f);
                
                Destroy(extraArms[i].GetComponent<Collider>());
            }
            
            // 火焰光环
            GameObject fireAura = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            fireAura.transform.SetParent(caster.transform);
            fireAura.transform.localPosition = Vector3.zero;
            fireAura.transform.localScale = Vector3.one * 2f;
            
            Renderer auraRenderer = fireAura.GetComponent<Renderer>();
            Material auraMat = new Material(auraRenderer.material);
            auraMat.color = new Color(1f, 0.3f, 0.1f, 0.3f);
            auraRenderer.material = auraMat;
            
            Destroy(fireAura.GetComponent<Collider>());
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                // 周围火焰伤害
                if (elapsed % 0.5f < Time.deltaTime)
                {
                    Collider[] hits = Physics.OverlapSphere(caster.transform.position, 3f);
                    foreach (Collider hit in hits)
                    {
                        HeroBase enemy = hit.GetComponent<HeroBase>();
                        if (enemy != null && enemy != caster)
                        {
                            enemy.TakeDamage(30f, Core.DamageType.Magic, caster);
                        }
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // 恢复
            caster.stats.attackDamage = originalAttack;
            caster.stats.attackSpeed = originalAttackSpeed;
            
            foreach (GameObject arm in extraArms)
            {
                Destroy(arm);
            }
            Destroy(fireAura);
        }
    }
}
