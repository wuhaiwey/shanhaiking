using UnityEngine;
using System.Collections;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 九尾狐 - 刺客型英雄
    /// </summary>
    public class Hero_JiuWeiHu : HeroBase
    {
        [Header("九尾狐特性")]
        public int maxFoxFires = 3; // 最大狐火数量
        public int currentFoxFires = 0;
        
        protected override void Awake()
        {
            base.Awake();
            
            heroName = "九尾狐";
            heroTitle = "青丘之灵";
            heroType = HeroType.Assassin;
            
            // 刺客属性
            stats.maxHealth = 2800f;
            stats.maxMana = 600f;
            stats.attackDamage = 200f;
            stats.attackSpeed = 1.5f;
            stats.criticalChance = 0.3f;
            stats.criticalDamage = 2.0f;
            stats.armor = 60f;
            stats.magicResist = 40f;
            stats.moveSpeed = 380f; // 刺客移速快
            
            // 成长
            stats.healthGrowth = 150f;
            stats.manaGrowth = 40f;
            stats.attackGrowth = 15f;
            
            stats.Initialize();
        }
        
        protected override void InitializeSkills()
        {
            skills[0] = gameObject.AddComponent<Skill_JiuWeiHu_Q>();
            skills[1] = gameObject.AddComponent<Skill_JiuWeiHu_W>();
            skills[2] = gameObject.AddComponent<Skill_JiuWeiHu_E>();
            skills[3] = gameObject.AddComponent<Skill_JiuWeiHu_R>();
        }
    }
    
    /// <summary>
    /// Q技能: 狐火
    /// </summary>
    public class Skill_JiuWeiHu_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "狐火";
            description = "释放狐火攻击附近敌人，可叠加3层";
            hotkey = KeyCode.Q;
            manaCost = 40f;
            cooldown = 4f;
            baseDamage = 80f;
            damagePerLevel = 25f;
            range = 6f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            // 获取九尾狐组件
            Hero_JiuWeiHu fox = caster as Hero_JiuWeiHu;
            if (fox == null) return;
            
            // 搜索附近敌人
            Collider[] colliders = Physics.OverlapSphere(caster.transform.position, range);
            
            foreach (Collider col in colliders)
            {
                HeroBase enemy = col.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    // 造成伤害
                    enemy.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
                    
                    // 添加狐火标记（可叠加）
                    FoxFireMark mark = enemy.gameObject.AddComponent<FoxFireMark>();
                    mark.stackCount++;
                    
                    // 播放特效
                    if (hitEffect != null)
                    {
                        Instantiate(hitEffect, enemy.transform.position, Quaternion.identity);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// W技能: 魅惑
    /// </summary>
    public class Skill_JiuWeiHu_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "魅惑";
            description = "魅惑目标，使其走向你并无法攻击";
            hotkey = KeyCode.W;
            manaCost = 70f;
            cooldown = 12f;
            baseDamage = 50f;
            damagePerLevel = 20f;
            range = 8f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            if (target == null) return;
            
            // 造成伤害
            target.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
            
            // 魅惑效果：向九尾狐移动并无法攻击
            StartCoroutine(CharmTarget(caster, target));
        }
        
        IEnumerator CharmTarget(HeroBase caster, HeroBase target)
        {
            float charmDuration = 1.5f + (skillLevel * 0.3f); // 随等级增加
            float timer = 0f;
            
            while (timer < charmDuration && target != null && !target.isDead)
            {
                // 强制目标向施法者移动
                Vector3 direction = (caster.transform.position - target.transform.position).normalized;
                target.transform.position += direction * 3f * Time.deltaTime;
                
                // 沉默目标
                if (target is Hero_JiuWeiHu == false)
                {
                    target.ApplySilence(0.1f);
                }
                
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
    
    /// <summary>
    /// E技能: 瞬影
    /// </summary>
    public class Skill_JiuWeiHu_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "瞬影";
            description = "瞬间移动到指定位置，并在原地留下幻影";
            hotkey = KeyCode.E;
            manaCost = 50f;
            cooldown = 8f;
            range = 6f;
            targetType = TargetType.Point;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            // 保存原位置
            Vector3 originalPosition = caster.transform.position;
            
            // 创建幻影
            StartCoroutine(CreatePhantom(caster, originalPosition));
            
            // 瞬间移动
            caster.Blink(targetPosition);
            
            // 移动后获得短暂加速
            // TODO: 添加加速Buff
        }
        
        IEnumerator CreatePhantom(HeroBase caster, Vector3 position)
        {
            // 创建幻影GameObject
            GameObject phantom = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            phantom.transform.position = position;
            phantom.transform.localScale = caster.transform.localScale;
            
            // 复制外观
            Renderer phantomRenderer = phantom.GetComponent<Renderer>();
            Renderer casterRenderer = caster.GetComponent<Renderer>();
            if (phantomRenderer != null && casterRenderer != null)
            {
                phantomRenderer.material = casterRenderer.material;
            }
            
            // 幻影渐隐
            float duration = 1f;
            float timer = 0f;
            
            while (timer < duration)
            {
                if (phantomRenderer != null)
                {
                    Color color = phantomRenderer.material.color;
                    color.a = Mathf.Lerp(1f, 0f, timer / duration);
                    phantomRenderer.material.color = color;
                }
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            Destroy(phantom);
        }
    }
    
    /// <summary>
    /// R技能: 九尾分身
    /// </summary>
    public class Skill_JiuWeiHu_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "九尾分身";
            description = "召唤9个分身同时攻击周围敌人";
            hotkey = KeyCode.R;
            manaCost = 120f;
            cooldown = 45f;
            baseDamage = 200f;
            damagePerLevel = 80f;
            range = 10f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            // 创建9个分身
            for (int i = 0; i < 9; i++)
            {
                float angle = i * (360f / 9f);
                Vector3 offset = Quaternion.Euler(0, angle, 0) * Vector3.forward * 3f;
                Vector3 spawnPos = caster.transform.position + offset;
                
                StartCoroutine(SpawnFoxSpirit(caster, spawnPos, i * 0.1f));
            }
        }
        
        IEnumerator SpawnFoxSpirit(HeroBase caster, Vector3 position, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            // 创建狐灵
            GameObject spirit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            spirit.transform.position = position;
            spirit.transform.localScale = Vector3.one * 0.5f;
            
            // 设置颜色（粉色）
            Renderer renderer = spirit.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.magenta;
            }
            
            // 搜索并攻击敌人
            Collider[] enemies = Physics.OverlapSphere(position, 5f);
            foreach (Collider col in enemies)
            {
                HeroBase enemy = col.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    enemy.TakeDamage(CurrentDamage, Core.DamageType.Magic, caster);
                    break; // 每个狐灵只攻击一个敌人
                }
            }
            
            // 延迟销毁
            yield return new WaitForSeconds(0.5f);
            Destroy(spirit);
        }
    }
    
    /// <summary>
    /// 狐火标记组件
    /// </summary>
    public class FoxFireMark : MonoBehaviour
    {
        public int stackCount = 0;
        public float maxStacks = 3f;
        public float damagePerStack = 30f;
        public float duration = 5f;
        
        void Start()
        {
            StartCoroutine(ExplodeAfterDuration());
        }
        
        IEnumerator ExplodeAfterDuration()
        {
            yield return new WaitForSeconds(duration);
            
            // 引爆造成额外伤害
            if (stackCount > 0)
            {
                HeroBase hero = GetComponent<HeroBase>();
                if (hero != null)
                {
                    float explodeDamage = stackCount * damagePerStack;
                    hero.TakeDamage(explodeDamage, Core.DamageType.Magic, null);
                }
            }
            
            Destroy(this);
        }
    }
}
