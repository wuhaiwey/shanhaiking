using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Core
{
    /// <summary>
    /// 游戏商店系统
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        public static ShopManager Instance { get; private set; }
        
        [Header("商品列表")]
        public List<ShopItem> availableItems = new List<ShopItem>();
        
        [Header("玩家金币")]
        public int playerGold = 500;
        
        [Header("商店界面")]
        public bool isShopOpen = false;
        
        void Awake()
        {
            Instance = this;
            InitializeShopItems();
        }
        
        void InitializeShopItems()
        {
            // 攻击类装备
            availableItems.Add(new ShopItem
            {
                itemName = "轩辕剑",
                description = "攻击力 +50",
                cost = 1500,
                icon = null,
                attackBonus = 50f,
                itemType = ShopItem.ItemType.Weapon,
                rarity = ShopItem.ItemRarity.Epic
            });
            
            availableItems.Add(new ShopItem
            {
                itemName = "盘古斧",
                description = "攻击力 +80, 暴击率 +15%",
                cost = 2800,
                icon = null,
                attackBonus = 80f,
                criticalChanceBonus = 0.15f,
                itemType = ShopItem.ItemType.Weapon,
                rarity = ShopItem.ItemRarity.Legendary
            });
            
            availableItems.Add(new ShopItem
            {
                itemName = "射日弓",
                description = "攻击力 +40, 攻击速度 +20%",
                cost = 1200,
                icon = null,
                attackBonus = 40f,
                attackSpeedBonus = 0.2f,
                itemType = ShopItem.ItemType.Weapon,
                rarity = ShopItem.ItemRarity.Rare
            });
            
            // 防御类装备
            availableItems.Add(new ShopItem
            {
                itemName = "玄龟盾",
                description = "护甲 +60, 生命值 +200",
                cost = 1000,
                icon = null,
                defenseBonus = 60f,
                healthBonus = 200f,
                itemType = ShopItem.ItemType.Armor,
                rarity = ShopItem.ItemRarity.Rare
            });
            
            availableItems.Add(new ShopItem
            {
                itemName = "八卦仙衣",
                description = "护甲 +40, 魔抗 +40",
                cost = 1200,
                icon = null,
                defenseBonus = 40f,
                magicResistBonus = 40f,
                itemType = ShopItem.ItemType.Armor,
                rarity = ShopItem.ItemRarity.Epic
            });
            
            availableItems.Add(new ShopItem
            {
                itemName = "金刚不坏体",
                description = "护甲 +100, 生命值 +500",
                cost = 2500,
                icon = null,
                defenseBonus = 100f,
                healthBonus = 500f,
                itemType = ShopItem.ItemType.Armor,
                rarity = ShopItem.ItemRarity.Legendary
            });
            
            // 移动类装备
            availableItems.Add(new ShopItem
            {
                itemName = "风火轮",
                description = "移动速度 +15%",
                cost = 800,
                icon = null,
                moveSpeedBonus = 0.15f,
                itemType = ShopItem.ItemType.Boots,
                rarity = ShopItem.ItemRarity.Common
            });
            
            availableItems.Add(new ShopItem
            {
                itemName = "筋斗云",
                description = "移动速度 +25%, 无视碰撞",
                cost = 1500,
                icon = null,
                moveSpeedBonus = 0.25f,
                itemType = ShopItem.ItemType.Boots,
                rarity = ShopItem.ItemRarity.Epic
            });
            
            // 法术类装备
            availableItems.Add(new ShopItem
            {
                itemName = "昆仑镜",
                description = "法术强度 +60",
                cost = 1300,
                icon = null,
                abilityPowerBonus = 60f,
                itemType = ShopItem.ItemType.Accessory,
                rarity = ShopItem.ItemRarity.Rare
            });
            
            availableItems.Add(new ShopItem
            {
                itemName = "东皇钟",
                description = "法术强度 +100, 冷却缩减 +10%",
                cost = 2600,
                icon = null,
                abilityPowerBonus = 100f,
                cooldownReduction = 0.1f,
                itemType = ShopItem.ItemType.Accessory,
                rarity = ShopItem.ItemRarity.Legendary
            });
        }
        
        public bool BuyItem(ShopItem item, Hero.HeroBase hero)
        {
            if (playerGold >= item.cost)
            {
                playerGold -= item.cost;
                ApplyItemToHero(item, hero);
                return true;
            }
            return false;
        }
        
        void ApplyItemToHero(ShopItem item, Hero.HeroBase hero)
        {
            hero.stats.attackDamage += item.attackBonus;
            hero.stats.maxHealth += item.healthBonus;
            hero.stats.armor += item.defenseBonus;
            hero.stats.magicResist += item.magicResistBonus;
            hero.stats.moveSpeed *= (1 + item.moveSpeedBonus);
            hero.stats.attackSpeed *= (1 + item.attackSpeedBonus);
            hero.stats.criticalChance += item.criticalChanceBonus;
        }
        
        public void SellItem(int slotIndex, Hero.HeroBase hero)
        {
            // 实现出售逻辑
        }
        
        public void AddGold(int amount)
        {
            playerGold += amount;
        }
        
        public void ToggleShop()
        {
            isShopOpen = !isShopOpen;
        }
    }
    
    [System.Serializable]
    public class ShopItem
    {
        public string itemName;
        public string description;
        public int cost;
        public Sprite icon;
        public ItemType itemType;
        public ItemRarity rarity;
        
        [Header("属性加成")]
        public float attackBonus;
        public float healthBonus;
        public float defenseBonus;
        public float magicResistBonus;
        public float moveSpeedBonus;
        public float attackSpeedBonus;
        public float criticalChanceBonus;
        public float abilityPowerBonus;
        public float cooldownReduction;
        
        public enum ItemType
        {
            Weapon,
            Armor,
            Boots,
            Accessory
        }
        
        public enum ItemRarity
        {
            Common,
            Rare,
            Epic,
            Legendary
        }
        
        public Color GetRarityColor()
        {
            switch (rarity)
            {
                case ItemRarity.Common: return Color.white;
                case ItemRarity.Rare: return Color.cyan;
                case ItemRarity.Epic: return Color.magenta;
                case ItemRarity.Legendary: return Color.yellow;
                default: return Color.white;
            }
        }
    }
}
