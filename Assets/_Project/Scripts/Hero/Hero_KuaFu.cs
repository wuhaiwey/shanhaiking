using UnityEngine;
using System.Collections;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 夸父 - 战士型英雄（逐日者）
    /// </summary>
    public class Hero_KuaFu : HeroBase
    {
        [Header("夸父特性")]
        public float chaseSpeed = 0f;
        public float maxChaseSpeed = 100f;
        public float chaseDecay = 10f;
        public bool isChasingSun = false;
        public float thirstLevel = 0f;
        public float maxThirst = 100f;
        
        protected override void Awake()
        {
            base.Awake();
            heroName = "夸父";
            heroTitle = "逐日者";
            heroType = HeroType.Warrior;
            
            stats.maxHealth = 4200f;
            stats.maxMana = 500f;
            stats.attackDamage = 165f;
            stats.attackSpeed = 1.15f;
            stats.armor = 95f;
            stats.magicResist = 55f;
            stats.moveSpeed = 365f;
            
            stats.Initialize();
        }
        
        protected override void Update()
        {
            base.Update();
            
            // 追逐状态衰减
            if (chaseSpeed > 0)
            {
                chaseSpeed -= chaseDecay * Time.deltaTime;
                if (chaseSpeed < 0) chaseSpeed = 0;
                
                // 应用加速到移动速度
                stats.moveSpeed = 365f + chaseSpeed;
            }
            
            // 饥渴值管理
            if (isChasingSun)
            {
                thirstLevel += 5f * Time.deltaTime;
                if (thirstLevel >= maxThirst)
                {
                    OnThirstMax();
                }
            }
        }
        
        void OnThirstMax()
        {
            isChasingSun = false;
            // 饥渴达到最大时的惩罚
            TakeDamage(maxHealth * 0.1f, Core.DamageType.True, this);
            thirstLevel = 0;
        }
        
        public void AddChaseSpeed(float amount)
        {
            chaseSpeed = Mathf.Min(chaseSpeed + amount, maxChaseSpeed);
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_KuaFu_Q>();
            skills[1] = gameObject.AddComponent<Skill_KuaFu_W>();
            skills[2] = gameObject.AddComponent<Skill_KuaFu_E>();
            skills[3] = gameObject.AddComponent<Skill_KuaFu_R>();
        }
    }
    
    public class Skill_KuaFu_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "巨人之踏";
            description = "践踏地面造成伤害并减速敌人";
            hotkey = KeyCode.Q;
            manaCost = 50f;
            cooldown = 6f;
            baseDamage = 130f;
            damagePerLevel = 35f;
            range = 5f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            // 践踏效果
            Collider[] hits = Physics.OverlapSphere(caster.transform.position, range);
            
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    enemy.TakeDamage(CurrentDamage, Core.DamageType.Physical, caster);
                    // 减速效果
                    // enemy.ApplyBuff(BuffType.SpeedDown, 0.4f, 2f);
                }
            }
            
            // 视觉效果：地面震动
            Effects.ScreenEffectManager.Instance?.ShakeCamera(0.3f, 0.2f);
            
            // 增加追逐速度
            Hero_KuaFu kuafu = caster as Hero_KuaFu;
            kuafu?.AddChaseSpeed(10f);
        }
    }
    
    public class Skill_KuaFu_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "逐日";
            description = "向目标方向冲刺，对路径上敌人造成伤害";
            hotkey = KeyCode.W;
            manaCost = 60f;
            cooldown = 10f;
            baseDamage = 150f;
            damagePerLevel = 40f;
            range = 12f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_KuaFu kuafu = caster as Hero_KuaFu;
            if (kuafu != null)
            {
                kuafu.isChasingSun = true;
                kuafu.AddChaseSpeed(20f);
            }
            
            caster.StartCoroutine(ChaseDash(caster, targetPosition));
        }
        
        IEnumerator ChaseDash(HeroBase caster, Vector3 targetPos)
        {
            Vector3 startPos = caster.transform.position;
            Vector3 direction = (targetPos - startPos).normalized;
            float distance = Vector3.Distance(startPos, targetPos);
            float dashTime = 0.4f;
            float elapsed = 0f;
            
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
                        enemy.TakeDamage(CurrentDamage * Time.deltaTime / dashTime, Core.DamageType.Physical, caster);
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            caster.transform.position = targetPos;
            col.isTrigger = wasTrigger;
            
            // 结束追逐状态
            Hero_KuaFu kuafu = caster as Hero_KuaFu;
            if (kuafu != null)
            {
                kuafu.isChasingSun = false;
                kuafu.thirstLevel = 0;
            }
        }
    }
    
    public class Skill_KuaFu_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "渴饮";
            description = "吸取周围敌人的生命恢复自身";
            hotkey = KeyCode.E;
            manaCost = 70f;
            cooldown = 12f;
            range = 6f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            Hero_KuaFu kuafu = caster as Hero_KuaFu;
            
            Collider[] hits = Physics.OverlapSphere(caster.transform.position, range);
            float totalHealing = 0f;
            
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    float damage = 80f + caster.stats.attackDamage * 0.3f;
                    enemy.TakeDamage(damage, Core.DamageType.Magic, caster);
                    totalHealing += damage * 0.5f; // 50%吸血
                }
            }
            
            // 恢复生命
            if (totalHealing > 0)
            {
                caster.RestoreHealth(totalHealing);
                
                // 降低饥渴值
                if (kuafu != null)
                {
                    kuafu.thirstLevel = Mathf.Max(0, kuafu.thirstLevel - totalHealing);
                }
            }
        }
    }
    
    public class Skill_KuaFu_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "夸父逐日";
            description = "进入狂暴状态，大幅提升攻击和移速，但持续损失生命";
            hotkey = KeyCode.R;
            manaCost = 100f;
            cooldown = 70f;
            duration = 10f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(UltimateChase(caster));
        }
        
        IEnumerator UltimateChase(HeroBase caster)
        {
            Hero_KuaFu kuafu = caster as Hero_KuaFu;
            
            // 保存原始属性
            float originalAttack = caster.stats.attackDamage;
            float originalSpeed = caster.stats.moveSpeed;
            
            // 狂暴加成
            caster.stats.attackDamage *= 1.5f;
            caster.stats.moveSpeed *= 1.4f;
            
            if (kuafu != null)
            {
                kuafu.isChasingSun = true;
                kuafu.thirstLevel = 0;
            }
            
            // 视觉效果
            GameObject aura = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            aura.transform.position = caster.transform.position;
            aura.transform.localScale = Vector3.one * 3f;
            aura.transform.SetParent(caster.transform);
            
            Renderer renderer = aura.GetComponent<Renderer>();
            renderer.material.color = new Color(1f, 0.5f, 0f, 0.5f);
            Destroy(aura.GetComponent<Collider>());
            
            float elapsed = 0f;
            while (elapsed < duration)
            {
                // 持续损失生命（燃烧生命追逐太阳）
                float healthCost = caster.MaxHealth * 0.02f * Time.deltaTime;
                caster.TakeDamage(healthCost, Core.DamageType.True, caster);
                
                // 增加饥渴
                if (kuafu != null)
                {
                    kuafu.thirstLevel += 10f * Time.deltaTime;
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // 恢复原始属性
            caster.stats.attackDamage = originalAttack;
            caster.stats.moveSpeed = originalSpeed;
            
            if (kuafu != null)
            {
                kuafu.isChasingSun = false;
            }
            
            Destroy(aura);
        }
    }
}
