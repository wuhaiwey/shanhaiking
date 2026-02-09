using UnityEngine;
using System.Collections;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 麒麟 - 坦克型英雄（瑞兽）
    /// </summary>
    public class Hero_QiLin : HeroBase
    {
        [Header("麒麟特性")]
        public float blessingCharge = 0f;
        public float maxBlessing = 100f;
        public bool isBlessingActive = false;
        public int shieldStacks = 0;
        public int maxShieldStacks = 5;
        
        protected override void Awake()
        {
            base.Awake();
            heroName = "麒麟";
            heroTitle = "瑞兽";
            heroType = HeroType.Tank;
            
            stats.maxHealth = 4500f;
            stats.maxMana = 650f;
            stats.attackDamage = 120f;
            stats.attackSpeed = 1.0f;
            stats.armor = 130f;
            stats.magicResist = 110f;
            stats.moveSpeed = 350f;
            
            stats.Initialize();
        }
        
        protected override void Update()
        {
            base.Update();
            
            // 祥瑞之气自然回复
            if (blessingCharge < maxBlessing)
            {
                blessingCharge += 2f * Time.deltaTime;
            }
            
            // 护盾堆叠衰减
            if (shieldStacks > 0 && !isBlessingActive)
            {
                // 缓慢衰减护盾层数
            }
        }
        
        public void AddBlessing(float amount)
        {
            blessingCharge = Mathf.Min(blessingCharge + amount, maxBlessing);
        }
        
        public void AddShieldStack()
        {
            if (shieldStacks < maxShieldStacks)
            {
                shieldStacks++;
                // 每层护盾提供额外护甲
                stats.armor += 15f;
            }
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_QiLin_Q>();
            skills[1] = gameObject.AddComponent<Skill_QiLin_W>();
            skills[2] = gameObject.AddComponent<Skill_QiLin_E>();
            skills[3] = gameObject.AddComponent<Skill_QiLin_R>();
        }
    }
    
    public class Skill_QiLin_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "祥瑞冲击";
            description = "向前冲锋，击退敌人并获得护盾";
            hotkey = KeyCode.Q;
            manaCost = 50f;
            cooldown = 8f;
            baseDamage = 140f;
            damagePerLevel = 30f;
            range = 8f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_QiLin qilin = caster as Hero_QiLin;
            caster.StartCoroutine(BlessingCharge(caster, targetPosition, qilin));
        }
        
        IEnumerator BlessingCharge(HeroBase caster, Vector3 targetPos, Hero_QiLin qilin)
        {
            Vector3 startPos = caster.transform.position;
            Vector3 direction = (targetPos - startPos).normalized;
            float chargeTime = 0.4f;
            float elapsed = 0f;
            
            // 无视碰撞
            Collider col = caster.GetComponent<Collider>();
            bool wasTrigger = col.isTrigger;
            col.isTrigger = true;
            
            // 祥瑞光环
            GameObject aura = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            aura.transform.position = caster.transform.position;
            aura.transform.localScale = Vector3.one * 2f;
            aura.transform.SetParent(caster.transform);
            
            Renderer renderer = aura.GetComponent<Renderer>();
            renderer.material.color = new Color(1f, 0.8f, 0.2f, 0.4f);
            Destroy(aura.GetComponent<Collider>());
            
            while (elapsed < chargeTime)
            {
                caster.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / chargeTime);
                
                // 路径伤害和击退
                Collider[] hits = Physics.OverlapSphere(caster.transform.position, 2f);
                foreach (Collider hit in hits)
                {
                    HeroBase enemy = hit.GetComponent<HeroBase>();
                    if (enemy != null && enemy != caster)
                    {
                        enemy.TakeDamage(CurrentDamage * Time.deltaTime / chargeTime, Core.DamageType.Physical, caster);
                        
                        // 击退
                        Vector3 knockback = (enemy.transform.position - caster.transform.position).normalized;
                        enemy.transform.position += knockback * Time.deltaTime * 10f;
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            caster.transform.position = targetPos;
            col.isTrigger = wasTrigger;
            Destroy(aura);
            
            // 获得护盾
            qilin?.AddShieldStack();
        }
    }
    
    public class Skill_QiLin_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "瑞光普照";
            description = "释放祥瑞之光，治疗友军并伤害敌人";
            hotkey = KeyCode.W;
            manaCost = 70f;
            cooldown = 12f;
            baseDamage = 100f;
            healAmount = 120f;
            range = 10f;
        }
        
        public float healAmount = 120f;
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_QiLin qilin = caster as Hero_QiLin;
            
            // 创建光球
            GameObject lightOrb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            lightOrb.transform.position = caster.transform.position + Vector3.up * 2f;
            lightOrb.transform.localScale = Vector3.one * 3f;
            
            Renderer renderer = lightOrb.GetComponent<Renderer>();
            Material mat = new Material(renderer.material);
            mat.color = new Color(1f, 1f, 0.6f, 0.6f);
            renderer.material = mat;
            
            Destroy(lightOrb.GetComponent<Collider>());
            
            // 范围效果
            Collider[] hits = Physics.OverlapSphere(caster.transform.position, range);
            
            foreach (Collider hit in hits)
            {
                HeroBase unit = hit.GetComponent<HeroBase>();
                if (unit != null)
                {
                    if (unit == caster) // 自己获得双倍治疗
                    {
                        unit.RestoreHealth(healAmount * 2f);
                        qilin?.AddBlessing(20f);
                    }
                    else if (unit.team == caster.team) // 友军治疗
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
            caster.StartCoroutine(ExpandAndFade(lightOrb));
        }
        
        IEnumerator ExpandAndFade(GameObject orb)
        {
            float elapsed = 0f;
            Vector3 startScale = orb.transform.localScale;
            
            while (elapsed < 2f)
            {
                orb.transform.localScale = startScale * (1f + elapsed * 0.5f);
                
                Renderer renderer = orb.GetComponent<Renderer>();
                Color color = renderer.material.color;
                color.a = 0.6f - (elapsed / 2f) * 0.6f;
                renderer.material.color = color;
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            Destroy(orb);
        }
    }
    
    public class Skill_QiLin_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "御兽之盾";
            description = "为队友施加护盾，吸收伤害";
            hotkey = KeyCode.E;
            manaCost = 60f;
            cooldown = 10f;
            range = 8f;
            duration = 5f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            if (target != null && target != caster)
            {
                target.StartCoroutine(ProtectiveShield(target, caster));
            }
            else
            {
                // 对自己施放
                caster.StartCoroutine(ProtectiveShield(caster, caster));
            }
        }
        
        IEnumerator ProtectiveShield(HeroBase protectedUnit, HeroBase caster)
        {
            // 护盾视觉效果
            GameObject shield = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shield.transform.position = protectedUnit.transform.position;
            shield.transform.localScale = Vector3.one * 1.5f;
            shield.transform.SetParent(protectedUnit.transform);
            
            Renderer renderer = shield.GetComponent<Renderer>();
            Material mat = new Material(renderer.material);
            mat.color = new Color(1f, 0.9f, 0.4f, 0.3f);
            renderer.material = mat;
            
            Destroy(shield.GetComponent<Collider>());
            
            // 临时增加护盾值
            float shieldHealth = 200f + caster.stats.abilityPower * 0.5f;
            float remainingShield = shieldHealth;
            
            float elapsed = 0f;
            while (elapsed < duration && remainingShield > 0)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // 护盾破裂效果
            Destroy(shield);
        }
    }
    
    public class Skill_QiLin_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "天降祥瑞";
            description = "召唤祥瑞领域，全队获得大量增益";
            hotkey = KeyCode.R;
            manaCost = 130f;
            cooldown = 90f;
            duration = 10f;
            range = 15f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_QiLin qilin = caster as Hero_QiLin;
            if (qilin != null)
            {
                qilin.isBlessingActive = true;
            }
            
            caster.StartCoroutine(DivineBlessing(caster, qilin));
        }
        
        IEnumerator DivineBlessing(HeroBase caster, Hero_QiLin qilin)
        {
            Vector3 center = caster.transform.position;
            
            // 创建祥瑞领域
            GameObject blessingZone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            blessingZone.transform.position = center;
            blessingZone.transform.localScale = new Vector3(20f, 0.1f, 20f);
            
            Renderer zoneRenderer = blessingZone.GetComponent<Renderer>();
            Material zoneMat = new Material(zoneRenderer.material);
            zoneMat.color = new Color(1f, 0.9f, 0.3f, 0.2f);
            zoneRenderer.material = zoneMat;
            
            Destroy(blessingZone.GetComponent<Collider>());
            
            // 创建光柱
            GameObject[] pillars = new GameObject[4];
            for (int i = 0; i < 4; i++)
            {
                float angle = i * 90f * Mathf.Deg2Rad;
                Vector3 pos = center + new Vector3(Mathf.Cos(angle) * 8f, 0, Mathf.Sin(angle) * 8f);
                
                pillars[i] = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pillars[i].transform.position = pos + Vector3.up * 5f;
                pillars[i].transform.localScale = new Vector3(1f, 10f, 1f);
                
                Renderer pillarRenderer = pillars[i].GetComponent<Renderer>();
                pillarRenderer.material.color = new Color(1f, 1f, 0.5f, 0.8f);
                
                Destroy(pillars[i].GetComponent<Collider>());
            }
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                // 每秒效果
                if (elapsed % 1f < Time.deltaTime)
                {
                    Collider[] hits = Physics.OverlapSphere(center, 10f);
                    
                    foreach (Collider hit in hits)
                    {
                        HeroBase ally = hit.GetComponent<HeroBase>();
                        if (ally != null && ally.team == caster.team)
                        {
                            // 治疗
                            ally.RestoreHealth(ally.MaxHealth * 0.05f);
                            // 增加护甲
                            // ally.ApplyBuff(BuffType.DefenseUp, 0.2f, 2f);
                        }
                    }
                    
                    // 增加祥瑞值
                    qilin?.AddBlessing(10f);
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            if (qilin != null)
            {
                qilin.isBlessingActive = false;
            }
            
            Destroy(blessingZone);
            foreach (GameObject pillar in pillars)
            {
                Destroy(pillar);
            }
        }
    }
}
