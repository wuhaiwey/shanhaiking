using UnityEngine;
using System.Collections;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 穷奇 - 刺客型英雄（四凶之一）
    /// </summary>
    public class Hero_QiongQi : HeroBase
    {
        [Header("穷奇特性")]
        public int comboCount = 0;
        public float comboTimer = 0f;
        public bool isInShadow = false;
        
        protected override void Awake()
        {
            base.Awake();
            heroName = "穷奇";
            heroTitle = "四凶之翼";
            heroType = HeroType.Assassin;
            
            stats.maxHealth = 2700f;
            stats.maxMana = 550f;
            stats.attackDamage = 210f;
            stats.attackSpeed = 1.4f;
            stats.criticalChance = 0.35f;
            stats.armor = 55f;
            stats.magicResist = 40f;
            stats.moveSpeed = 390f;
            
            stats.Initialize();
        }
        
        protected override void Update()
        {
            base.Update();
            
            // 连击计时器
            if (comboCount > 0)
            {
                comboTimer -= Time.deltaTime;
                if (comboTimer <= 0)
                {
                    comboCount = 0;
                }
            }
        }
        
        public void AddCombo()
        {
            comboCount++;
            comboTimer = 3f; // 3秒内保持连击
            
            // 连击加成
            if (comboCount >= 3)
            {
                stats.attackDamage *= 1.1f;
            }
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_QiongQi_Q>();
            skills[1] = gameObject.AddComponent<Skill_QiongQi_W>();
            skills[2] = gameObject.AddComponent<Skill_QiongQi_E>();
            skills[3] = gameObject.AddComponent<Skill_QiongQi_R>();
        }
    }
    
    public class Skill_QiongQi_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "凶爪撕裂";
            description = "快速爪击目标3次，每次伤害递增";
            hotkey = KeyCode.Q;
            manaCost = 45f;
            cooldown = 5f;
            baseDamage = 60f;
            damagePerLevel = 15f;
            range = 3f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            if (target == null) return;
            
            caster.StartCoroutine(ClawAttack(caster, target));
        }
        
        IEnumerator ClawAttack(HeroBase caster, HeroBase target)
        {
            Hero_QiongQi qiongqi = caster as Hero_QiongQi;
            
            for (int i = 1; i <= 3; i++)
            {
                if (target == null || target.isDead) yield break;
                
                float multiplier = 1f + (i * 0.3f); // 每次增加30%伤害
                target.TakeDamage(CurrentDamage * multiplier, Core.DamageType.Physical, caster);
                
                if (qiongqi != null) qiongqi.AddCombo();
                
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
    
    public class Skill_QiongQi_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "影遁";
            description = "进入隐身状态，下次攻击必定暴击";
            hotkey = KeyCode.W;
            manaCost = 60f;
            cooldown = 10f;
            duration = 4f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(ShadowForm(caster));
        }
        
        IEnumerator ShadowForm(HeroBase caster)
        {
            Hero_QiongQi qiongqi = caster as Hero_QiongQi;
            if (qiongqi != null) qiongqi.isInShadow = true;
            
            // 隐身效果：半透明
            Renderer renderer = caster.GetComponent<Renderer>();
            Color originalColor = renderer.material.color;
            renderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.3f);
            
            // 记录原始暴击率并提升
            float originalCrit = caster.stats.criticalChance;
            caster.stats.criticalChance = 1f; // 100%暴击
            
            yield return new WaitForSeconds(duration);
            
            renderer.material.color = originalColor;
            caster.stats.criticalChance = originalCrit;
            if (qiongqi != null) qiongqi.isInShadow = false;
        }
    }
    
    public class Skill_QiongQi_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "翼击";
            description = "展开双翼冲向目标，可穿越地形";
            hotkey = KeyCode.E;
            manaCost = 50f;
            cooldown = 8f;
            range = 10f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(WingDash(caster, targetPosition));
        }
        
        IEnumerator WingDash(HeroBase caster, Vector3 targetPos)
        {
            Vector3 startPos = caster.transform.position;
            float distance = Vector3.Distance(startPos, targetPos);
            float duration = 0.3f;
            float elapsed = 0f;
            
            // 冲刺期间无视碰撞
            Collider col = caster.GetComponent<Collider>();
            bool wasEnabled = col.enabled;
            col.enabled = false;
            
            while (elapsed < duration)
            {
                caster.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            caster.transform.position = targetPos;
            col.enabled = wasEnabled;
            
            // 落地伤害
            Collider[] hits = Physics.OverlapSphere(caster.transform.position, 3f);
            foreach (Collider hit in hits)
            {
                HeroBase enemy = hit.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    enemy.TakeDamage(100f, Core.DamageType.Physical, caster);
                }
            }
        }
    }
    
    public class Skill_QiongQi_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "凶性爆发";
            description = "进入狂暴状态，攻速移速大幅提升，击杀刷新技能";
            hotkey = KeyCode.R;
            manaCost = 120f;
            cooldown = 60f;
            duration = 8f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            caster.StartCoroutine(BerserkMode(caster));
        }
        
        IEnumerator BerserkMode(HeroBase caster)
        {
            Hero_QiongQi qiongqi = caster as Hero_QiongQi;
            
            // 狂暴加成
            float originalAttackSpeed = caster.stats.attackSpeed;
            float originalMoveSpeed = caster.stats.moveSpeed;
            
            caster.stats.attackSpeed *= 2f; // 双倍攻速
            caster.stats.moveSpeed *= 1.5f; // 50%移速
            
            // 视觉变化：变红
            Renderer renderer = caster.GetComponent<Renderer>();
            Color originalColor = renderer.material.color;
            renderer.material.color = Color.red;
            
            float timer = 0f;
            while (timer < duration)
            {
                // 检测击杀
                // 简化版：这里可以通过事件系统监听击杀
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            // 恢复
            caster.stats.attackSpeed = originalAttackSpeed;
            caster.stats.moveSpeed = originalMoveSpeed;
            renderer.material.color = originalColor;
        }
    }
}
