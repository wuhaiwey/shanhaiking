using UnityEngine;
using System.Collections;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 牛魔王 - 坦克型英雄（平天大圣）
    /// </summary>
    public class Hero_NiuMoWang : HeroBase
    {
        [Header("牛魔王特性")]
        public int rageMeter = 0;
        public int maxRage = 100;
        public bool isBerserk = false;
        public float ironBullDuration = 0f;
        
        protected override void Awake()
        {
            base.Awake();
            heroName = "牛魔王";
            heroTitle = "平天大圣";
            heroType = HeroType.Tank;
            
            stats.maxHealth = 4800f;
            stats.maxMana = 550f;
            stats.attackDamage = 145f;
            stats.attackSpeed = 1.0f;
            stats.armor = 140f;
            stats.magicResist = 100f;
            stats.moveSpeed = 345f;
            
            stats.Initialize();
        }
        
        protected override void Update()
        {
            base.Update();
            
            // 狂暴状态管理
            if (isBerserk)
            {
                ironBullDuration -= Time.deltaTime;
                if (ironBullDuration <= 0)
                {
                    ExitBerserk();
                }
            }
        }
        
        public void AddRage(int amount)
        {
            rageMeter = Mathf.Min(rageMeter + amount, maxRage);
            
            if (rageMeter >= maxRage && !isBerserk)
            {
                EnterBerserk();
            }
        }
        
        void EnterBerserk()
        {
            isBerserk = true;
            ironBullDuration = 8f;
            
            // 狂暴加成
            stats.attackDamage *= 1.4f;
            stats.armor *= 1.2f;
            
            Debug.Log("牛魔王进入狂暴状态！");
        }
        
        void ExitBerserk()
        {
            isBerserk = false;
            rageMeter = 0;
            
            // 恢复
            stats.attackDamage /= 1.4f;
            stats.armor /= 1.2f;
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_NiuMoWang_Q>();
            skills[1] = gameObject.AddComponent<Skill_NiuMoWang_W>();
            skills[2] = gameObject.AddComponent<Skill_NiuMoWang_E>();
            skills[3] = gameObject.AddComponent<Skill_NiuMoWang_R>();
        }
    }
    
    public class Skill_NiuMoWang_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "蛮牛冲撞";
            description = "向前冲锋，击退敌人并积累怒气";
            hotkey = KeyCode.Q;
            manaCost = 50f;
            cooldown = 8f;
            baseDamage = 160f;
            damagePerLevel = 35f;
            range = 10f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_NiuMoWang niu = caster as Hero_NiuMoWang;
            caster.StartCoroutine(BullCharge(caster, targetPosition, niu));
        }
        
        IEnumerator BullCharge(HeroBase caster, Vector3 targetPos, Hero_NiuMoWang niu)
        {
            Vector3 startPos = caster.transform.position;
            Vector3 direction = (targetPos - startPos).normalized;
            float chargeTime = 0.5f;
            float elapsed = 0f;
            
            // 牛头效果
            GameObject bullHead = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bullHead.transform.position = caster.transform.position + direction * 1.5f + Vector3.up;
            bullHead.transform.localScale = new Vector3(0.8f, 0.6f, 0.8f);
            bullHead.transform.rotation = Quaternion.LookRotation(direction);
            
            Renderer renderer = bullHead.GetComponent<Renderer>();
            renderer.material.color = new Color(0.3f, 0.2f, 0.15f);
            
            Destroy(bullHead.GetComponent<Collider>());
            
            // 无视碰撞
            Collider col = caster.GetComponent<Collider>();
            bool wasTrigger = col.isTrigger;
            col.isTrigger = true;
            
            while (elapsed < chargeTime)
            {
                caster.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / chargeTime);
                bullHead.transform.position = caster.transform.position + direction * 1.5f + Vector3.up;
                
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
                        enemy.transform.position += knockback * Time.deltaTime * 8f;
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            caster.transform.position = targetPos;
            col.isTrigger = wasTrigger;
            Destroy(bullHead);
            
            // 积累怒气
            niu?.AddRage(20);
        }
    }
    
    public class Skill_NiuMoWang_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "巨斧横扫";
            description = "挥舞巨斧造成大范围伤害";
            hotkey = KeyCode.W;
            manaCost = 60f;
            cooldown = 10f;
            baseDamage = 180f;
            damagePerLevel = 45f;
            range = 6f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_NiuMoWang niu = caster as Hero_NiuMoWang;
            
            // 巨斧效果
            GameObject axe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            axe.transform.position = caster.transform.position;
            axe.transform.localScale = new Vector3(range * 2f, 0.2f, range * 2f);
            
            Renderer renderer = axe.GetComponent<Renderer>();
            Material mat = new Material(renderer.material);
            mat.color = new Color(0.4f, 0.3f, 0.2f, 0.4f);
            renderer.material = mat;
            
            Destroy(axe.GetComponent<Collider>());
            
            // 范围伤害
            Collider[] hits = Physics.OverlapSphere(caster.transform.position, range);
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    float damage = CurrentDamage;
                    if (niu != null && niu.isBerserk)
                    {
                        damage *= 1.5f;
                    }
                    enemy.TakeDamage(damage, Core.DamageType.Physical, caster);
                }
            }
            
            // 旋转消失
            caster.StartCoroutine(SpinAxe(axe));
            
            niu?.AddRage(15);
        }
        
        IEnumerator SpinAxe(GameObject axe)
        {
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                axe.transform.Rotate(Vector3.up, 360f * Time.deltaTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
            Destroy(axe);
        }
    }
    
    public class Skill_NiuMoWang_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "铁壁铜墙";
            description = "大幅提升防御，反弹部分伤害";
            hotkey = KeyCode.E;
            manaCost = 70f;
            cooldown = 14f;
            duration = 5f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_NiuMoWang niu = caster as Hero_NiuMoWang;
            caster.StartCoroutine(IronWall(caster, niu));
        }
        
        IEnumerator IronWall(HeroBase caster, Hero_NiuMoWang niu)
        {
            // 防御提升
            float originalArmor = caster.stats.armor;
            caster.stats.armor *= 2f;
            
            // 护盾效果
            GameObject shield = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shield.transform.SetParent(caster.transform);
            shield.transform.localPosition = Vector3.zero;
            shield.transform.localScale = Vector3.one * 2f;
            
            Renderer renderer = shield.GetComponent<Renderer>();
            Material mat = new Material(renderer.material);
            mat.color = new Color(0.5f, 0.4f, 0.3f, 0.3f);
            renderer.material = mat;
            
            Destroy(shield.GetComponent<Collider>());
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            caster.stats.armor = originalArmor;
            Destroy(shield);
        }
    }
    
    public class Skill_NiuMoWang_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "牛魔真身";
            description = "现出牛魔真身，成为不可阻挡的坦克";
            hotkey = KeyCode.R;
            manaCost = 120f;
            cooldown = 85f;
            duration = 10f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_NiuMoWang niu = caster as Hero_NiuMoWang;
            caster.StartCoroutine(TrueBullForm(caster, niu));
        }
        
        IEnumerator TrueBullForm(HeroBase caster, Hero_NiuMoWang niu)
        {
            niu.EnterBerserk();
            
            // 体型变大
            Vector3 originalScale = caster.transform.localScale;
            caster.transform.localScale = originalScale * 1.8f;
            
            // 牛角
            GameObject leftHorn = CreateHorn(caster, true);
            GameObject rightHorn = CreateHorn(caster, false);
            
            // 火焰效果
            GameObject fire = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            fire.transform.SetParent(caster.transform);
            fire.transform.localPosition = Vector3.up * 2f;
            fire.transform.localScale = Vector3.one * 1.5f;
            
            Renderer fireRenderer = fire.GetComponent<Renderer>();
            fireRenderer.material.color = new Color(1f, 0.3f, 0.1f, 0.4f);
            
            Destroy(fire.GetComponent<Collider>());
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                // 持续伤害
                if (elapsed % 0.5f < Time.deltaTime)
                {
                    Collider[] hits = Physics.OverlapSphere(caster.transform.position, 4f);
                    foreach (Collider hit in hits)
                    {
                        HeroBase enemy = hit.GetComponent<HeroBase>();
                        if (enemy != null && enemy != caster)
                        {
                            enemy.TakeDamage(40f, Core.DamageType.Magic, caster);
                        }
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            niu.ExitBerserk();
            caster.transform.localScale = originalScale;
            
            Destroy(leftHorn);
            Destroy(rightHorn);
            Destroy(fire);
        }
        
        GameObject CreateHorn(HeroBase caster, bool isLeft)
        {
            GameObject horn = GameObject.CreatePrimitive(PrimitiveType.Cone);
            horn.transform.SetParent(caster.transform);
            
            float xOffset = isLeft ? -0.4f : 0.4f;
            horn.transform.localPosition = new Vector3(xOffset, 2.5f, 0.3f);
            horn.transform.localScale = new Vector3(0.2f, 0.8f, 0.2f);
            horn.transform.localRotation = Quaternion.Euler(-30, 0, isLeft ? 15f : -15f);
            
            Renderer renderer = horn.GetComponent<Renderer>();
            renderer.material.color = new Color(0.9f, 0.85f, 0.7f);
            
            Destroy(horn.GetComponent<Collider>());
            
            return horn;
        }
    }
}
