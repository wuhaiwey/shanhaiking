using UnityEngine;
using System;

namespace ShanHaiKing.Core
{
    /// <summary>
    /// 英雄基础属性
    /// </summary>
    [Serializable]
    public class HeroStats
    {
        [Header("基础属性")]
        public float maxHealth = 3000f;
        public float currentHealth;
        public float maxMana = 500f;
        public float currentMana;
        
        [Header("攻击属性")]
        public float attackDamage = 100f;
        public float abilityPower = 0f;
        public float attackSpeed = 1.0f;
        public float criticalChance = 0.1f;
        public float criticalDamage = 1.5f;
        
        [Header("防御属性")]
        public float armor = 50f;
        public float magicResist = 30f;
        
        [Header("移动属性")]
        public float moveSpeed = 360f;
        public float attackRange = 5f;
        
        [Header("成长属性")]
        public float healthGrowth = 180f;
        public float manaGrowth = 30f;
        public float attackGrowth = 8f;
        public float armorGrowth = 4f;
        
        public void Initialize()
        {
            currentHealth = maxHealth;
            currentMana = maxMana;
        }
        
        public void LevelUp(int level)
        {
            maxHealth += healthGrowth;
            maxMana += manaGrowth;
            attackDamage += attackGrowth;
            armor += armorGrowth;
            
            // 升级回满状态
            currentHealth = maxHealth;
            currentMana = maxMana;
        }
    }
    
    /// <summary>
    /// 英雄类型枚举
    /// </summary>
    public enum HeroType
    {
        Tank,       // 坦克
        Warrior,    // 战士
        Assassin,   // 刺客
        Mage,       // 法师
        Marksman,   // 射手
        Support     // 辅助
    }
    
    /// <summary>
    /// 伤害类型
    /// </summary>
    public enum DamageType
    {
        Physical,   // 物理伤害
        Magic,      // 魔法伤害
        True        // 真实伤害
    }
}
