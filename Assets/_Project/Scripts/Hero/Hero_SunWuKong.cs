using UnityEngine;
using System.Collections;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 孙悟空 - 刺客型英雄（齐天大圣）
    /// </summary>
    public class Hero_SunWuKong : HeroBase
    {
        [Header("孙悟空特性")]
        public int cloneCount = 0;
        public int maxClones = 3;
        public bool isUndercover = false;
        public float staffSize = 1f;
        public int immortalityCharges = 0;
        
        protected override void Awake()
        {
            base.Awake();
            heroName = "孙悟空";
            heroTitle = "齐天大圣";
            heroType = HeroType.Assassin;
            
            stats.maxHealth = 2800f;
            stats.maxMana = 550f;
            stats.attackDamage = 195f;
            stats.attackSpeed = 1.45f;
            stats.criticalChance = 0.35f;
            stats.armor = 60f;
            stats.magicResist = 50f;
            stats.moveSpeed = 385f;
            
            stats.Initialize();
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_SunWuKong_Q>();
            skills[1] = gameObject.AddComponent<Skill_SunWuKong_W>();
            skills[2] = gameObject.AddComponent<Skill_SunWuKong_E>();
            skills[3] = gameObject.AddComponent<Skill_SunWuKong_R>();
        }
        
        public void CreateClone()
        {
            if (cloneCount < maxClones)
            {
                cloneCount++;
                // 创建分身逻辑
            }
        }
    }
    
    public class Skill_SunWuKong_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "金箍棒";
            description = "挥舞金箍棒造成范围伤害，可以变大";
            hotkey = KeyCode.Q;
            manaCost = 45f;
            cooldown = 5f;
            baseDamage = 160f;
            damagePerLevel = 40f;
            range = 5f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_SunWuKong wukong = caster as Hero_SunWuKong;
            
            // 金箍棒变大
            float staffScale = 1f + (wukong?.staffSize ?? 1f) * 0.5f;
            
            GameObject staff = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            staff.transform.position = caster.transform.position + caster.transform.forward * 2f;
            staff.transform.localScale = new Vector3(0.15f * staffScale, 3f * staffScale, 0.15f * staffScale);
            staff.transform.rotation = caster.transform.rotation * Quaternion.Euler(90, 0, 0);
            
            Renderer renderer = staff.GetComponent<Renderer>();
            renderer.material.color = new Color(0.9f, 0.7f, 0.2f);
            
            Destroy(staff.GetComponent<Collider>());
            
            // 旋转攻击
            caster.StartCoroutine(SpinStaff(caster, staff));
        }
        
        IEnumerator SpinStaff(HeroBase caster, GameObject staff)
        {
            float elapsed = 0f;
            float duration = 0.5f;
            
            while (elapsed < duration)
            {
                staff.transform.RotateAround(caster.transform.position, Vector3.up, 720f * Time.deltaTime);
                
                // 伤害
                Collider[] hits = Physics.OverlapSphere(staff.transform.position, 1.5f);
                foreach (Collider hit in hits)
                {
                    HeroBase enemy = hit.GetComponent<HeroBase>();
                    if (enemy != null && enemy != caster)
                    {
                        enemy.TakeDamage(CurrentDamage * Time.deltaTime / duration, Core.DamageType.Physical, caster);
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            Destroy(staff);
        }
    }
    
    public class Skill_SunWuKong_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "分身术";
            description = "创造一个分身协助战斗";
            hotkey = KeyCode.W;
            manaCost = 70f;
            cooldown = 12f;
            duration = 6f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_SunWuKong wukong = caster as Hero_SunWuKong;
            if (wukong != null)
            {
                wukong.CreateClone();
            }
            
            caster.StartCoroutine(CloneTechnique(caster));
        }
        
        IEnumerator CloneTechnique(HeroBase caster)
        {
            // 创建分身
            GameObject clone = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            clone.transform.position = caster.transform.position + caster.transform.right * 1.5f;
            clone.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
            
            Renderer renderer = clone.GetComponent<Renderer>();
            renderer.material.color = new Color(1f, 0.8f, 0.6f);
            
            Destroy(clone.GetComponent<Collider>());
            
            float elapsed = 0f;
            while (elapsed < 6f)
            {
                // 分身自动攻击附近敌人
                Collider[] hits = Physics.OverlapSphere(clone.transform.position, 4f);
                foreach (Collider hit in hits)
                {
                    HeroBase enemy = hit.GetComponent<HeroBase>();
                    if (enemy != null && enemy != caster)
                    {
                        enemy.TakeDamage(50f * Time.deltaTime, Core.DamageType.Physical, caster);
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // 分身消失效果
            Destroy(clone);
        }
    }
    
    public class Skill_SunWuKong_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "筋斗云";
            description = "骑上筋斗云快速移动到目标位置";
            hotkey = KeyCode.E;
            manaCost = 60f;
            cooldown = 10f;
            range = 15f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(SomersaultCloud(caster, targetPosition));
        }
        
        IEnumerator SomersaultCloud(HeroBase caster, Vector3 targetPos)
        {
            // 筋斗云
            GameObject cloud = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cloud.transform.position = caster.transform.position;
            cloud.transform.localScale = new Vector3(1.5f, 0.2f, 1f);
            
            Renderer renderer = cloud.GetComponent<Renderer>();
            renderer.material.color = new Color(0.95f, 0.95f, 1f);
            
            Destroy(cloud.GetComponent<Collider>());
            
            Vector3 startPos = caster.transform.position;
            float jumpTime = 0.6f;
            float elapsed = 0f;
            
            while (elapsed < jumpTime)
            {
                // 抛物线运动
                float t = elapsed / jumpTime;
                Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
                pos.y += Mathf.Sin(t * Mathf.PI) * 8f;
                
                caster.transform.position = pos;
                cloud.transform.position = pos - Vector3.up * 0.5f;
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            caster.transform.position = targetPos;
            Destroy(cloud);
            
            // 落地冲击
            Collider[] hits = Physics.OverlapSphere(targetPos, 3f);
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    enemy.TakeDamage(120f, Core.DamageType.Physical, caster);
                }
            }
        }
    }
    
    public class Skill_SunWuKong_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "七十二变";
            description = "化身为无敌状态并创造多个分身";
            hotkey = KeyCode.R;
            manaCost = 120f;
            cooldown = 75f;
            duration = 8f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_SunWuKong wukong = caster as Hero_SunWuKong;
            if (wukong != null)
            {
                wukong.immortalityCharges = 3; // 3次无敌
            }
            
            caster.StartCoroutine(SeventyTwoTransformations(caster, wukong));
        }
        
        IEnumerator SeventyTwoTransformations(HeroBase caster, Hero_SunWuKong wukong)
        {
            // 无敌状态
            // caster.isInvulnerable = true;
            
            // 创建多个分身
            GameObject[] clones = new GameObject[5];
            for (int i = 0; i < 5; i++)
            {
                float angle = i * 72f * Mathf.Deg2Rad;
                Vector3 pos = caster.transform.position + new Vector3(Mathf.Cos(angle) * 3f, 0, Mathf.Sin(angle) * 3f);
                
                clones[i] = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                clones[i].transform.position = pos;
                clones[i].transform.localScale = new Vector3(0.5f, 1f, 0.5f);
                
                Renderer renderer = clones[i].GetComponent<Renderer>();
                renderer.material.color = new Color(1f, 0.8f, 0.6f);
                
                Destroy(clones[i].GetComponent<Collider>());
            }
            
            // 属性提升
            float originalAttack = caster.stats.attackDamage;
            caster.stats.attackDamage *= 1.5f;
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                // 分身围绕旋转
                for (int i = 0; i < 5; i++)
                {
                    float angle = (elapsed * 2f + i * 72f) * Mathf.Deg2Rad;
                    Vector3 pos = caster.transform.position + new Vector3(Mathf.Cos(angle) * 3f, 0, Mathf.Sin(angle) * 3f);
                    clones[i].transform.position = pos;
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // 恢复
            caster.stats.attackDamage = originalAttack;
            // caster.isInvulnerable = false;
            
            foreach (GameObject clone in clones)
            {
                Destroy(clone);
            }
        }
    }
}
