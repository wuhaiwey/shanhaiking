using UnityEngine;
using System.Collections;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 雷震子 - 刺客/战士型英雄（雷神）
    /// </summary>
    public class Hero_LeiZhenZi : HeroBase
    {
        [Header("雷震子特性")]
        public int thunderCharges = 0;
        public int maxThunderCharges = 5;
        public bool isThunderForm = false;
        public float windRiderDuration = 0f;
        
        protected override void Awake()
        {
            base.Awake();
            heroName = "雷震子";
            heroTitle = "雷神";
            heroType = HeroType.Assassin;
            
            stats.maxHealth = 2900f;
            stats.maxMana = 550f;
            stats.attackDamage = 180f;
            stats.attackSpeed = 1.3f;
            stats.criticalChance = 0.3f;
            stats.armor = 65f;
            stats.magicResist = 55f;
            stats.moveSpeed = 380f;
            
            stats.Initialize();
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_LeiZhenZi_Q>();
            skills[1] = gameObject.AddComponent<Skill_LeiZhenZi_W>();
            skills[2] = gameObject.AddComponent<Skill_LeiZhenZi_E>();
            skills[3] = gameObject.AddComponent<Skill_LeiZhenZi_R>();
        }
        
        public void AddThunderCharge()
        {
            if (thunderCharges < maxThunderCharges)
            {
                thunderCharges++;
            }
        }
    }
    
    public class Skill_LeiZhenZi_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "雷霆一击";
            description = "召唤雷电攻击目标区域";
            hotkey = KeyCode.Q;
            manaCost = 50f;
            cooldown = 6f;
            baseDamage = 160f;
            damagePerLevel = 40f;
            range = 9f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            // 雷电效果
            GameObject lightning = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            lightning.transform.position = targetPosition + Vector3.up * 10f;
            lightning.transform.localScale = new Vector3(0.8f, 20f, 0.8f);
            
            Renderer renderer = lightning.GetComponent<Renderer>();
            renderer.material.color = new Color(0.8f, 0.9f, 1f);
            
            Destroy(lightning.GetComponent<Collider>());
            Destroy(lightning, 0.2f);
            
            // 范围伤害
            Collider[] hits = Physics.OverlapSphere(targetPosition, 3f);
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    enemy.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
                    // 麻痹效果
                    // enemy.ApplyBuff(BuffType.Stun, 0, 0.5f);
                }
            }
            
            // 积累雷电
            Hero_LeiZhenZi lei = caster as Hero_LeiZhenZi;
            lei?.AddThunderCharge();
        }
    }
    
    public class Skill_LeiZhenZi_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "风雷双翼";
            description = "展开双翼飞行，提升移速并可以穿越地形";
            hotkey = KeyCode.W;
            manaCost = 70f;
            cooldown = 12f;
            duration = 5f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(WindRider(caster));
        }
        
        IEnumerator WindRider(HeroBase caster)
        {
            Hero_LeiZhenZi lei = caster as Hero_LeiZhenZi;
            
            // 翅膀效果
            GameObject leftWing = CreateWing(caster, true);
            GameObject rightWing = CreateWing(caster, false);
            
            // 提升移速
            float originalSpeed = caster.stats.moveSpeed;
            caster.stats.moveSpeed *= 1.6f;
            
            // 无视地形
            Collider col = caster.GetComponent<Collider>();
            bool wasTrigger = col.isTrigger;
            col.isTrigger = true;
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                // 翅膀扇动
                if (leftWing != null)
                    leftWing.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(elapsed * 15f) * 30f);
                if (rightWing != null)
                    rightWing.transform.localRotation = Quaternion.Euler(0, 0, -Mathf.Sin(elapsed * 15f) * 30f);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // 恢复
            caster.stats.moveSpeed = originalSpeed;
            col.isTrigger = wasTrigger;
            
            Destroy(leftWing);
            Destroy(rightWing);
        }
        
        GameObject CreateWing(HeroBase caster, bool isLeft)
        {
            GameObject wing = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wing.name = isLeft ? "LeftWing" : "RightWing";
            wing.transform.SetParent(caster.transform);
            
            float xOffset = isLeft ? -0.7f : 0.7f;
            wing.transform.localPosition = new Vector3(xOffset, 1.2f, -0.3f);
            wing.transform.localScale = new Vector3(0.1f, 1.5f, 0.8f);
            
            Renderer renderer = wing.GetComponent<Renderer>();
            renderer.material.color = new Color(0.9f, 0.9f, 0.7f);
            
            Destroy(wing.GetComponent<Collider>());
            
            return wing;
        }
    }
    
    public class Skill_LeiZhenZi_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "雷鸣冲刺";
            description = "化为雷电冲向目标，对路径上敌人造成伤害";
            hotkey = KeyCode.E;
            manaCost = 60f;
            cooldown = 10f;
            range = 12f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(ThunderDash(caster, targetPosition));
        }
        
        IEnumerator ThunderDash(HeroBase caster, Vector3 targetPos)
        {
            Vector3 startPos = caster.transform.position;
            float dashTime = 0.3f;
            float elapsed = 0f;
            
            // 雷电轨迹
            GameObject trail = new GameObject("ThunderTrail");
            
            // 无视碰撞
            Collider col = caster.GetComponent<Collider>();
            bool wasTrigger = col.isTrigger;
            col.isTrigger = true;
            
            while (elapsed < dashTime)
            {
                caster.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / dashTime);
                
                // 路径伤害
                Collider[] hits = Physics.OverlapSphere(caster.transform.position, 2f);
                foreach (Collider hit in hits)
                {
                    HeroBase enemy = hit.GetComponent<HeroBase>();
                    if (enemy != null && enemy != caster)
                    {
                        enemy.TakeDamage(120f, Core.DamageType.Magic, caster);
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            caster.transform.position = targetPos;
            col.isTrigger = wasTrigger;
            
            // 落地雷击
            GameObject lightning = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            lightning.transform.position = targetPos + Vector3.up * 5f;
            lightning.transform.localScale = new Vector3(1f, 10f, 1f);
            
            Renderer renderer = lightning.GetComponent<Renderer>();
            renderer.material.color = new Color(0.7f, 0.9f, 1f);
            
            Destroy(lightning.GetComponent<Collider>());
            Destroy(lightning, 0.2f);
        }
    }
    
    public class Skill_LeiZhenZi_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "九天神雷";
            description = "召唤九天神雷轰击大范围区域";
            hotkey = KeyCode.R;
            manaCost = 130f;
            cooldown = 70f;
            baseDamage = 350f;
            damagePerLevel = 150f;
            range = 15f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_LeiZhenZi lei = caster as Hero_LeiZhenZi;
            float damageMultiplier = 1f + (lei?.thunderCharges ?? 0) * 0.15f;
            
            caster.StartCoroutine(DivineThunder(caster, targetPosition, damageMultiplier));
        }
        
        IEnumerator DivineThunder(HeroBase caster, Vector3 center, float multiplier)
        {
            // 乌云效果
            GameObject cloud = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cloud.transform.position = center + Vector3.up * 15f;
            cloud.transform.localScale = new Vector3(12f, 4f, 12f);
            
            Renderer cloudRenderer = cloud.GetComponent<Renderer>();
            cloudRenderer.material.color = new Color(0.3f, 0.3f, 0.4f, 0.8f);
            
            Destroy(cloud.GetComponent<Collider>());
            
            yield return new WaitForSeconds(1f);
            
            // 连续雷击
            for (int i = 0; i < 8; i++)
            {
                Vector3 strikePos = center + Random.insideUnitSphere * 6f;
                strikePos.y = center.y;
                
                // 雷电
                GameObject lightning = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                lightning.transform.position = strikePos + Vector3.up * 15f;
                lightning.transform.localScale = new Vector3(0.6f, 30f, 0.6f);
                
                Renderer lightningRenderer = lightning.GetComponent<Renderer>();
                lightningRenderer.material.color = new Color(0.8f, 0.95f, 1f);
                
                Destroy(lightning.GetComponent<Collider>());
                Destroy(lightning, 0.15f);
                
                // 伤害
                Collider[] hits = Physics.OverlapSphere(strikePos, 2.5f);
                foreach (Collider hit in hits)
                {
                    HeroBase enemy = hit.GetComponent<HeroBase>();
                    if (enemy != null && enemy != caster)
                    {
                        float damage = CurrentDamage * multiplier / 8f;
                        enemy.TakeDamage(damage, Core.DamageType.Magic, caster);
                    }
                }
                
                yield return new WaitForSeconds(0.1f);
            }
            
            Destroy(cloud);
            
            // 消耗雷电层数
            Hero_LeiZhenZi lei = caster as Hero_LeiZhenZi;
            if (lei != null)
            {
                lei.thunderCharges = 0;
            }
        }
    }
}
