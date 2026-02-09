using UnityEngine;
using ShanHaiKing.Core;

namespace ShanHaiKing.Hero
{
    /// <summary>
    /// 后羿 - 射日英雄
    /// </summary>
    public class Hero_HouYi : HeroBase
    {
        protected override void Awake()
        {
            base.Awake();
            
            // 设置英雄信息
            heroName = "后羿";
            heroTitle = "射日英雄";
            heroType = HeroType.Marksman;
            
            // 设置属性
            stats.maxHealth = 3200f;
            stats.maxMana = 500f;
            stats.attackDamage = 180f;
            stats.attackSpeed = 1.2f;
            stats.criticalChance = 0.25f;
            stats.criticalDamage = 2.0f;
            stats.armor = 80f;
            stats.magicResist = 40f;
            stats.moveSpeed = 360f;
            stats.attackRange = 8f; // 射手射程远
            
            // 成长属性
            stats.healthGrowth = 180f;
            stats.manaGrowth = 30f;
            stats.attackGrowth = 12f;
            stats.armorGrowth = 5f;
            
            stats.Initialize();
        }
        
        protected override void InitializeSkills()
        {
            // Q技能: 连珠箭
            skills[0] = gameObject.AddComponent<Skill_HouYi_Q>();
            
            // W技能: 穿云箭
            skills[1] = gameObject.AddComponent<Skill_HouYi_W>();
            
            // E技能: 逐日步
            skills[2] = gameObject.AddComponent<Skill_HouYi_E>();
            
            // R技能: 射日
            skills[3] = gameObject.AddComponent<Skill_HouYi_R>();
        }
    }
    
    /// <summary>
    /// 后羿 Q技能: 连珠箭
    /// </summary>
    public class Skill_HouYi_Q : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "连珠箭";
            description = "快速射出3支箭，每支箭造成物理伤害";
            hotkey = KeyCode.Q;
            manaCost = 60f;
            cooldown = 6f;
            baseDamage = 80f;
            damagePerLevel = 25f;
            range = 8f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            // 连续射出3箭
            StartCoroutine(FireArrows(caster, target));
        }
        
        private System.Collections.IEnumerator FireArrows(HeroBase caster, HeroBase target)
        {
            for (int i = 0; i < 3; i++)
            {
                if (target != null && !target.isDead)
                {
                    // 造成伤害
                    target.TakeDamage(CurrentDamage, DamageType.Physical, caster);
                    
                    // 播放特效
                    if (hitEffect != null)
                    {
                        Instantiate(hitEffect, target.transform.position, Quaternion.identity);
                    }
                    
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
    }
    
    /// <summary>
    /// 后羿 W技能: 穿云箭
    /// </summary>
    public class Skill_HouYi_W : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "穿云箭";
            description = "射出穿透箭，对路径上所有敌人造成伤害";
            hotkey = KeyCode.W;
            manaCost = 80f;
            cooldown = 8f;
            baseDamage = 150f;
            damagePerLevel = 40f;
            range = 12f;
            effectRadius = 2f;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            // 计算方向
            Vector3 direction = (targetPosition - caster.transform.position).normalized;
            
            // 射线检测
            RaycastHit[] hits = Physics.SphereCastAll(
                caster.transform.position, 
                effectRadius, 
                direction, 
                range,
                LayerMask.GetMask("Hero")
            );
            
            foreach (var hit in hits)
            {
                HeroBase enemy = hit.collider.GetComponent<HeroBase>();
                if (enemy != null && enemy != caster)
                {
                    enemy.TakeDamage(CurrentDamage, DamageType.Physical, caster);
                    
                    if (hitEffect != null)
                    {
                        Instantiate(hitEffect, enemy.transform.position, Quaternion.identity);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 后羿 E技能: 逐日步
    /// </summary>
    public class Skill_HouYi_E : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "逐日步";
            description = "向指定方向翻滚，下次攻击必定暴击";
            hotkey = KeyCode.E;
            manaCost = 40f;
            cooldown = 10f;
            range = 5f;
            targetType = TargetType.Point;
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            // 计算翻滚方向
            Vector3 direction = (targetPosition - caster.transform.position).normalized;
            Vector3 blinkPosition = caster.transform.position + direction * range;
            
            // 瞬间移动
            caster.Blink(blinkPosition);
            
            // 下次攻击必定暴击（需要实现一个Buff系统）
            // TODO: 添加暴击Buff
        }
    }
    
    /// <summary>
    /// 后羿 R技能: 射日
    /// </summary>
    public class Skill_HouYi_R : Skill.SkillBase
    {
        void Awake()
        {
            skillName = "射日";
            description = "蓄力射出太阳神箭，对全图一个敌人造成巨额伤害";
            hotkey = KeyCode.R;
            manaCost = 150f;
            cooldown = 60f;
            baseDamage = 400f;
            damagePerLevel = 150f;
            range = 1000f; // 全图
        }
        
        protected override void ExecuteSkill(HeroBase caster, Vector3 targetPosition, HeroBase target)
        {
            if (target == null) return;
            
            // 蓄力时间
            StartCoroutine(ChargeAndFire(caster, target));
        }
        
        private System.Collections.IEnumerator ChargeAndFire(HeroBase caster, HeroBase target)
        {
            // 蓄力1秒
            yield return new WaitForSeconds(1f);
            
            // 发射神箭（无视距离）
            if (!target.isDead)
            {
                // 造成巨额伤害
                target.TakeDamage(CurrentDamage * 1.5f, DamageType.Physical, caster);
                
                // 播放全屏特效
                // TODO: 播放大招特效
            }
        }
    }
}
