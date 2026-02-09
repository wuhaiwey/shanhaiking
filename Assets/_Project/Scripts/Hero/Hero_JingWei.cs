using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 精卫 - 射手型英雄（填海志鸟）
    /// </summary>
    public class Hero_JingWei : HeroBase
    {
        [Header("精卫特性")]
        public int stoneCount = 0;
        public int maxStones = 5;
        public bool isFlying = false;
        
        protected override void Awake()
        {
            base.Awake();
            heroName = "精卫";
            heroTitle = "填海志鸟";
            heroType = HeroType.Marksman;
            
            stats.maxHealth = 2900f;
            stats.maxMana = 550f;
            stats.attackDamage = 175f;
            stats.attackSpeed = 1.3f;
            stats.criticalChance = 0.2f;
            stats.armor = 70f;
            stats.magicResist = 45f;
            stats.moveSpeed = 370f;
            stats.attackRange = 7.5f;
            
            stats.Initialize();
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_JingWei_Q>();
            skills[1] = gameObject.AddComponent<Skill_JingWei_W>();
            skills[2] = gameObject.AddComponent<Skill_JingWei_E>();
            skills[3] = gameObject.AddComponent<Skill_JingWei_R>();
        }
        
        void Update()
        {
            base.Update();
            
            // 自动回复石子
            if (stoneCount < maxStones)
            {
                // 每5秒回复一个石子
            }
        }
        
        public void AddStone()
        {
            if (stoneCount < maxStones)
            {
                stoneCount++;
            }
        }
    }
    
    public class Skill_JingWei_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "衔石";
            description = "投掷石子对敌人造成伤害，可储存5发";
            hotkey = KeyCode.Q;
            manaCost = 25f;
            cooldown = 0.5f;
            baseDamage = 90f;
            damagePerLevel = 20f;
            range = 8f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_JingWei jingwei = caster as Hero_JingWei;
            if (jingwei == null || jingwei.stoneCount <= 0) return;
            
            jingwei.stoneCount--;
            
            // 创建石子 projectile
            GameObject stone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            stone.transform.position = caster.transform.position + Vector3.up;
            stone.transform.localScale = Vector3.one * 0.3f;
            
            Rigidbody rb = stone.AddComponent<Rigidbody>();
            Vector3 direction = (targetPosition - caster.transform.position).normalized;
            rb.velocity = direction * 15f;
            
            StoneProjectile proj = stone.AddComponent<StoneProjectile>();
            proj.Initialize(CurrentDamage, caster);
            
            Destroy(stone, 3f);
        }
    }
    
    public class StoneProjectile : MonoBehaviour
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
                enemy.TakeDamage(damage, Core.DamageType.Physical, owner);
                Destroy(gameObject);
            }
        }
    }
    
    public class Skill_JingWei_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "羽翼旋风";
            description = "挥动翅膀击退周围敌人并造成伤害";
            hotkey = KeyCode.W;
            manaCost = 60f;
            cooldown = 8f;
            baseDamage = 140f;
            damagePerLevel = 35f;
            range = 5f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Collider[] hits = Physics.OverlapSphere(caster.transform.position, range);
            
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    enemy.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
                    
                    // 击退效果
                    Vector3 knockbackDir = (enemy.transform.position - caster.transform.position).normalized;
                    enemy.transform.position += knockbackDir * 3f;
                }
            }
        }
    }
    
    public class Skill_JingWei_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "化鸟";
            description = "化身鸟形态获得飞行能力，攻击附带额外伤害";
            hotkey = KeyCode.E;
            manaCost = 70f;
            cooldown = 15f;
            duration = 6f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_JingWei jingwei = caster as Hero_JingWei;
            if (jingwei == null) return;
            
            jingwei.StartCoroutine(BirdForm(jingwei));
        }
        
        System.Collections.IEnumerator BirdForm(Hero_JingWei jingwei)
        {
            jingwei.isFlying = true;
            jingwei.stats.attackDamage *= 1.3f;
            jingwei.stats.moveSpeed *= 1.2f;
            
            // 视觉变化：变小、悬浮
            Vector3 originalScale = jingwei.transform.localScale;
            jingwei.transform.localScale = originalScale * 0.7f;
            
            yield return new WaitForSeconds(duration);
            
            jingwei.isFlying = false;
            jingwei.stats.attackDamage /= 1.3f;
            jingwei.stats.moveSpeed /= 1.2f;
            jingwei.transform.localScale = originalScale;
        }
    }
    
    public class Skill_JingWei_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "填海之志";
            description = "召唤石雨轰击大范围区域";
            hotkey = KeyCode.R;
            manaCost = 130f;
            cooldown = 50f;
            baseDamage = 300f;
            damagePerLevel = 100f;
            range = 12f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(StoneRain(caster, targetPosition));
        }
        
        System.Collections.IEnumerator StoneRain(HeroBase caster, Vector3 targetPos)
        {
            // 预警圈
            for (int wave = 0; wave < 3; wave++)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector3 randomPos = targetPos + Random.insideUnitSphere * 5f;
                    randomPos.y = targetPos.y;
                    
                    GameObject stone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    stone.transform.position = randomPos + Vector3.up * 10f;
                    stone.transform.localScale = Vector3.one * 0.5f;
                    
                    Rigidbody rb = stone.AddComponent<Rigidbody>();
                    
                    // 检测落地伤害
                    StoneFall fall = stone.AddComponent<StoneFall>();
                    fall.Initialize(CurrentDamage, caster);
                    
                    Destroy(stone, 3f);
                }
                
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
    
    public class StoneFall : MonoBehaviour
    {
        private float damage;
        private HeroBase owner;
        private bool hasDamaged = false;
        
        public void Initialize(float dmg, HeroBase caster)
        {
            damage = dmg;
            owner = caster;
        }
        
        void OnCollisionEnter(Collision collision)
        {
            if (hasDamaged) return;
            
            HeroBase enemy = collision.gameObject.GetComponent<HeroBase>();
            if (enemy != null && enemy != owner)
            {
                enemy.TakeDamage(damage, Core.DamageType.Physical, owner);
                hasDamaged = true;
            }
            
            // 地面撞击效果
            Destroy(gameObject, 0.5f);
        }
    }
}
