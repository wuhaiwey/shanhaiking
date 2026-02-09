using UnityEngine;
using System.Collections;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 杨戬 - 战士型英雄（二郎神）
    /// </summary>
    public class Hero_YangJian : HeroBase
    {
        [Header("杨戬特性")]
        public bool hasThirdEye = true;
        public bool isErlangForm = false;
        public GameObject heavenlyDog;
        public int divinePower = 0;
        public int maxDivinePower = 100;
        
        protected override void Awake()
        {
            base.Awake();
            heroName = "杨戬";
            heroTitle = "二郎神";
            heroType = HeroType.Warrior;
            
            stats.maxHealth = 3600f;
            stats.maxMana = 600f;
            stats.attackDamage = 170f;
            stats.attackSpeed = 1.25f;
            stats.armor = 90f;
            stats.magicResist = 70f;
            stats.moveSpeed = 365f;
            
            stats.Initialize();
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_YangJian_Q>();
            skills[1] = gameObject.AddComponent<Skill_YangJian_W>();
            skills[2] = gameObject.AddComponent<Skill_YangJian_E>();
            skills[3] = gameObject.AddComponent<Skill_YangJian_R>();
        }
        
        public void AddDivinePower(int amount)
        {
            divinePower = Mathf.Min(divinePower + amount, maxDivinePower);
        }
    }
    
    public class Skill_YangJian_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "三尖两刃刀";
            description = "挥舞三尖两刃刀造成范围伤害";
            hotkey = KeyCode.Q;
            manaCost = 50f;
            cooldown = 6f;
            baseDamage = 150f;
            damagePerLevel = 35f;
            range = 5f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            // 武器横扫效果
            GameObject weaponArc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            weaponArc.transform.position = caster.transform.position;
            weaponArc.transform.localScale = new Vector3(range * 2f, 0.1f, range * 2f);
            
            Renderer renderer = weaponArc.GetComponent<Renderer>();
            Material mat = new Material(renderer.material);
            mat.color = new Color(0.9f, 0.8f, 0.5f, 0.3f);
            renderer.material = mat;
            
            Destroy(weaponArc.GetComponent<Collider>());
            
            // 范围伤害
            Collider[] hits = Physics.OverlapSphere(caster.transform.position, range);
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    enemy.TakeDamage(CurrentDamage, Core.DamageType.Physical, caster);
                }
            }
            
            // 旋转消失
            caster.StartCoroutine(RotateAndFade(weaponArc));
            
            // 积累神力
            Hero_YangJian yang = caster as Hero_YangJian;
            yang?.AddDivinePower(10);
        }
        
        System.Collections.IEnumerator RotateAndFade(GameObject obj)
        {
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                obj.transform.Rotate(Vector3.up, 360f * Time.deltaTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
            Destroy(obj);
        }
    }
    
    public class Skill_YangJian_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "天眼";
            description = "开启天眼识破隐身并造成真实伤害";
            hotkey = KeyCode.W;
            manaCost = 70f;
            cooldown = 12f;
            range = 10f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_YangJian yang = caster as Hero_YangJian;
            
            // 天眼效果
            GameObject thirdEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            thirdEye.transform.position = caster.transform.position + Vector3.up * 2f + caster.transform.forward * 0.5f;
            thirdEye.transform.localScale = Vector3.one * 0.3f;
            
            Renderer renderer = thirdEye.GetComponent<Renderer>();
            renderer.material.color = new Color(1f, 0.9f, 0.5f);
            
            Destroy(thirdEye.GetComponent<Collider>());
            
            // 光束
            GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            beam.transform.position = caster.transform.position + Vector3.up * 2f + caster.transform.forward * range / 2f;
            beam.transform.rotation = caster.transform.rotation * Quaternion.Euler(90, 0, 0);
            beam.transform.localScale = new Vector3(0.5f, range / 2f, 0.5f);
            
            Renderer beamRenderer = beam.GetComponent<Renderer>();
            beamRenderer.material.color = new Color(1f, 1f, 0.8f, 0.5f);
            
            Destroy(beam.GetComponent<Collider>());
            
            // 真实伤害
            Collider[] hits = Physics.OverlapSphere(targetPosition, 3f);
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    // 真实伤害
                    enemy.TakeDamage(200f, Core.DamageType.True, caster);
                    // 显形
                    // if (enemy.isInvisible) enemy.Reveal();
                }
            }
            
            Destroy(thirdEye, 1f);
            Destroy(beam, 0.5f);
            
            yang?.AddDivinePower(15);
        }
    }
    
    public class Skill_YangJian_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "哮天犬";
            description = "召唤哮天犬追击敌人";
            hotkey = KeyCode.E;
            manaCost = 60f;
            cooldown = 15f;
            range = 12f;
            duration = 6f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            if (target != null)
            {
                caster.StartCoroutine(HeavenlyDog(caster, target));
            }
        }
        
        IEnumerator HeavenlyDog(HeroBase caster, HeroBase target)
        {
            // 创建哮天犬
            GameObject dog = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            dog.transform.position = caster.transform.position + caster.transform.right * 2f;
            dog.transform.localScale = new Vector3(0.4f, 0.8f, 0.4f);
            
            Renderer renderer = dog.GetComponent<Renderer>();
            renderer.material.color = new Color(0.9f, 0.9f, 0.95f);
            
            Destroy(dog.GetComponent<Collider>());
            
            float elapsed = 0f;
            int biteCount = 0;
            
            while (elapsed < duration && target != null && !target.isDead)
            {
                // 追击目标
                Vector3 direction = (target.transform.position - dog.transform.position).normalized;
                dog.transform.position += direction * 8f * Time.deltaTime;
                dog.transform.LookAt(target.transform);
                
                // 攻击
                float dist = Vector3.Distance(dog.transform.position, target.transform.position);
                if (dist < 1.5f && elapsed > biteCount * 1.5f)
                {
                    target.TakeDamage(100f, Core.DamageType.Physical, caster);
                    biteCount++;
                    
                    // 吸血
                    caster.RestoreHealth(50f);
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            Destroy(dog);
        }
    }
    
    public class Skill_YangJian_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "法天象地";
            description = "化身为巨大战神，全属性大幅提升";
            hotkey = KeyCode.R;
            manaCost = 130f;
            cooldown = 80f;
            duration = 10f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_YangJian yang = caster as Hero_YangJian;
            if (yang != null)
            {
                yang.isErlangForm = true;
            }
            
            caster.StartCoroutine(DivineForm(caster, yang));
        }
        
        IEnumerator DivineForm(HeroBase caster, Hero_YangJian yang)
        {
            // 保存原始属性
            float originalScale = caster.transform.localScale.x;
            float originalHealth = caster.MaxHealth;
            float originalAttack = caster.stats.attackDamage;
            float originalArmor = caster.stats.armor;
            
            // 巨大化
            caster.transform.localScale = Vector3.one * 2.5f;
            
            // 属性加成
            caster.stats.maxHealth *= 1.5f;
            caster.RestoreHealth(caster.MaxHealth * 0.5f);
            caster.stats.attackDamage *= 1.4f;
            caster.stats.armor *= 1.3f;
            
            // 神光效果
            GameObject divineLight = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            divineLight.transform.SetParent(caster.transform);
            divineLight.transform.localPosition = Vector3.zero;
            divineLight.transform.localScale = Vector3.one * 3f;
            
            Renderer lightRenderer = divineLight.GetComponent<Renderer>();
            Material lightMat = new Material(lightRenderer.material);
            lightMat.color = new Color(1f, 0.9f, 0.5f, 0.3f);
            lightRenderer.material = lightMat;
            
            Destroy(divineLight.GetComponent<Collider>());
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                // 持续回复
                if (elapsed % 1f < Time.deltaTime)
                {
                    caster.RestoreHealth(caster.MaxHealth * 0.02f);
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // 恢复
            caster.transform.localScale = Vector3.one * originalScale;
            caster.stats.maxHealth = originalHealth;
            caster.stats.attackDamage = originalAttack;
            caster.stats.armor = originalArmor;
            
            if (yang != null)
            {
                yang.isErlangForm = false;
            }
            
            Destroy(divineLight);
        }
    }
}
