using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ShanHaiKing.Core
{
    /// <summary>
    /// Buff/Debuff 管理器
    /// </summary>
    public class BuffManager : MonoBehaviour
    {
        private List<Buff> activeBuffs = new List<Buff>();
        
        void Update()
        {
            // 更新所有Buff
            for (int i = activeBuffs.Count - 1; i >= 0; i--)
            {
                activeBuffs[i].duration -= Time.deltaTime;
                
                if (activeBuffs[i].duration <= 0)
                {
                    RemoveBuff(activeBuffs[i]);
                    activeBuffs.RemoveAt(i);
                }
            }
        }
        
        public void AddBuff(BuffType type, float value, float duration)
        {
            Buff buff = new Buff
            {
                type = type,
                value = value,
                duration = duration
            };
            
            activeBuffs.Add(buff);
            ApplyBuffEffect(buff);
        }
        
        public void RemoveBuff(Buff buff)
        {
            RemoveBuffEffect(buff);
        }
        
        void ApplyBuffEffect(Buff buff)
        {
            Hero.HeroBase hero = GetComponent<Hero.HeroBase>();
            if (hero == null) return;
            
            switch (buff.type)
            {
                case BuffType.SpeedUp:
                    hero.stats.moveSpeed *= (1 + buff.value);
                    break;
                case BuffType.AttackUp:
                    hero.stats.attackDamage *= (1 + buff.value);
                    break;
                case BuffType.DefenseUp:
                    hero.stats.armor *= (1 + buff.value);
                    break;
                case BuffType.Invincible:
                    // 实现无敌逻辑
                    break;
            }
        }
        
        void RemoveBuffEffect(Buff buff)
        {
            Hero.HeroBase hero = GetComponent<Hero.HeroBase>();
            if (hero == null) return;
            
            switch (buff.type)
            {
                case BuffType.SpeedUp:
                    hero.stats.moveSpeed /= (1 + buff.value);
                    break;
                case BuffType.AttackUp:
                    hero.stats.attackDamage /= (1 + buff.value);
                    break;
                case BuffType.DefenseUp:
                    hero.stats.armor /= (1 + buff.value);
                    break;
            }
        }
        
        public class Buff
        {
            public BuffType type;
            public float value;
            public float duration;
        }
        
        public enum BuffType
        {
            SpeedUp,    // 加速
            SpeedDown,  // 减速
            AttackUp,   // 攻击提升
            AttackDown, // 攻击降低
            DefenseUp,  // 防御提升
            DefenseDown,// 防御降低
            HealOverTime, // 持续治疗
            Poison,     // 中毒
            Stun,       // 眩晕
            Silence,    // 沉默
            Invincible  // 无敌
        }
    }
}
