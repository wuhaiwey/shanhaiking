using UnityEngine;
using System.Collections;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 王昭君 - 法师型英雄（冰雪之华）
    /// </summary>
    public class Hero_WangZhaoJun : HeroBase
    {
        [Header("王昭君特性")]
        public int iceStacks = 0;
        public int maxIceStacks = 5;
        public bool isFrozenForm = false;
        
        protected override void Awake()
        {
            base.Awake();
            heroName = "王昭君";
            heroTitle = "冰雪之华";
            heroType = HeroType.Mage;
            
            stats.maxHealth = 2480f;
            stats.maxMana = 920f;
            stats.attackDamage = 48f;
            stats.abilityPower = 210f;
            stats.armor = 46f;
            stats.magicResist = 68f;
            stats.moveSpeed = 340f;
            
            stats.Initialize();
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_WangZhaoJun_Q>();
            skills[1] = gameObject.AddComponent<Skill_WangZhaoJun_W>();
            skills[2] = gameObject.AddComponent<Skill_WangZhaoJun_E>();
            skills[3] = gameObject.AddComponent<Skill_WangZhaoJun_R>();
        }
        
        public void AddIceStack()
        {
            if (iceStacks < maxIceStacks)
            {
                iceStacks++;
                stats.abilityPower += 12f;
            }
        }
    }
    
    public class Skill_WangZhaoJun_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "凋零冰晶";
            description = "释放冰晶对敌人造成伤害并减速";
            hotkey = KeyCode.Q;
            manaCost = 55f;
            cooldown = 6f;
            baseDamage = 135f;
            damagePerLevel = 30f;
            range = 8f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            GameObject iceCrystal = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            iceCrystal.transform.position = targetPosition;
            iceCrystal.transform.localScale = Vector3.one * 0.6f;
            
            Renderer renderer = iceCrystal.GetComponent<Renderer>();
            renderer.material.color = new Color(0.7f, 0.9f, 1f);
            
            Destroy(iceCrystal.GetComponent<Collider>());
            
            Collider[] hits = Physics.OverlapSphere(targetPosition, 2.5f);
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    enemy.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
                    // 减速效果
                }
            }
            
            Destroy(iceCrystal, 2f);
            
            Hero_WangZhaoJun wang = caster as Hero_WangZhaoJun;
            wang?.AddIceStack();
        }
    }
    
    public class Skill_WangZhaoJun_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "禁锢寒霜";
            description = "冰冻区域内的敌人";
            hotkey = KeyCode.W;
            manaCost = 70f;
            cooldown = 12f;
            baseDamage = 160f;
            range = 7f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            GameObject frostRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            frostRing.transform.position = targetPosition;
            frostRing.transform.localScale = new Vector3(4f, 0.1f, 4f);
            
            Renderer renderer = frostRing.GetComponent<Renderer>();
            Material mat = new Material(renderer.material);
            mat.color = new Color(0.6f, 0.85f, 1f, 0.5f);
            renderer.material = mat;
            
            Destroy(frostRing.GetComponent<Collider>());
            
            Collider[] hits = Physics.OverlapSphere(targetPosition, 2f);
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    enemy.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
                    // 冰冻效果
                }
            }
            
            Destroy(frostRing, 3f);
        }
    }
    
    public class Skill_WangZhaoJun_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "霜寒之袭";
            description = "向目标方向释放冰霜冲击";
            hotkey = KeyCode.E;
            manaCost = 60f;
            cooldown = 10f;
            range = 10f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            GameObject iceShard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            iceShard.transform.position = caster.transform.position + Vector3.up;
            iceShard.transform.localScale = new Vector3(0.3f, 0.3f, 2f);
            iceShard.transform.rotation = Quaternion.LookRotation(targetPosition - caster.transform.position);
            
            Renderer renderer = iceShard.GetComponent<Renderer>();
            renderer.material.color = new Color(0.8f, 0.95f, 1f);
            
            Rigidbody rb = iceShard.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.velocity = (targetPosition - caster.transform.position).normalized * 15f;
            
            Destroy(iceShard.GetComponent<Collider>());
            Destroy(iceShard, 3f);
        }
    }
    
    public class Skill_WangZhaoJun_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "凛冬已至";
            description = "召唤暴风雪大范围攻击敌人";
            hotkey = KeyCode.R;
            manaCost = 150f;
            cooldown = 80f;
            baseDamage = 320f;
            damagePerLevel = 120f;
            range = 15f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(BlizzardUltimate(caster, targetPosition));
        }
        
        System.Collections.IEnumerator BlizzardUltimate(HeroBase caster, Vector3 center)
        {
            GameObject blizzard = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            blizzard.transform.position = center;
            blizzard.transform.localScale = new Vector3(12f, 0.1f, 12f);
            
            Renderer renderer = blizzard.GetComponent<Renderer>();
            renderer.material.color = new Color(0.7f, 0.9f, 1f, 0.3f);
            
            Destroy(blizzard.GetComponent<Collider>());
            
            float elapsed = 0f;
            float duration = 5f;
            
            while (elapsed < duration)
            {
                if (elapsed % 0.5f < Time.deltaTime)
                {
                    Collider[] hits = Physics.OverlapSphere(center, 6f);
                    foreach (Collider hit in hits)
                    {
                        HeroBase enemy = hit.GetComponent<HeroBase>();
                        if (enemy != null && enemy != caster)
                        {
                            enemy.TakeDamage(CurrentDamage / (duration / 0.5f), Core.DamageType.Magic, caster);
                        }
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            Destroy(blizzard);
        }
    }
}
