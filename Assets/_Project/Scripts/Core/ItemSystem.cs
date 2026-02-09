using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Core
{
    /// <summary>
    /// 装备/道具系统
    /// </summary>
    [CreateAssetMenu(fileName = "NewItem", menuName = "ShanHaiKing/Item")]
    public class ItemData : ScriptableObject
    {
        public string itemName;
        public string description;
        public Sprite icon;
        public int cost;
        public ItemType itemType;
        public ItemRarity rarity;
        
        [Header("属性加成")]
        public float healthBonus;
        public float manaBonus;
        public float attackBonus;
        public float defenseBonus;
        public float attackSpeedBonus;
        public float moveSpeedBonus;
        public float criticalChanceBonus;
        
        [Header("特殊效果")]
        public bool hasActiveSkill;
        public string activeSkillDescription;
        
        public enum ItemType
        {
            Weapon,     // 武器
            Armor,      // 防具
            Accessory,  // 饰品
            Consumable  // 消耗品
        }
        
        public enum ItemRarity
        {
            Common,     // 普通
            Rare,       // 稀有
            Epic,       // 史诗
            Legendary   // 传说
        }
    }
    
    /// <summary>
    /// 英雄装备管理器
    /// </summary>
    public class HeroInventory : MonoBehaviour
    {
        [Header("装备槽")]
        public ItemData[] equippedItems = new ItemData[6]; // 6个装备槽
        
        [Header("属性加成")]
        public float totalHealthBonus;
        public float totalAttackBonus;
        public float totalDefenseBonus;
        
        private Hero.HeroBase hero;
        
        void Start()
        {
            hero = GetComponent<Hero.HeroBase>();
        }
        
        public void EquipItem(ItemData item, int slot)
        {
            if (slot >= 0 && slot < equippedItems.Length)
            {
                equippedItems[slot] = item;
                CalculateStats();
            }
        }
        
        public void UnequipItem(int slot)
        {
            if (slot >= 0 && slot < equippedItems.Length)
            {
                equippedItems[slot] = null;
                CalculateStats();
            }
        }
        
        void CalculateStats()
        {
            totalHealthBonus = 0;
            totalAttackBonus = 0;
            totalDefenseBonus = 0;
            
            foreach (var item in equippedItems)
            {
                if (item != null)
                {
                    totalHealthBonus += item.healthBonus;
                    totalAttackBonus += item.attackBonus;
                    totalDefenseBonus += item.defenseBonus;
                }
            }
            
            // 应用到英雄
            if (hero != null)
            {
                hero.stats.maxHealth += totalHealthBonus;
                hero.stats.attackDamage += totalAttackBonus;
                hero.stats.armor += totalDefenseBonus;
            }
        }
    }
}
