using System.Collections.Generic;

namespace ShanHaiKing.Shop
{
    [System.Serializable]
    public class ShopItem
    {
        public int id;
        public string name;
        public string description;
        public int price;
        public ItemType type;
        public int attackBonus;
        public int defenseBonus;
        public int healthBonus;
        public int abilityPowerBonus;
    }
    
    public enum ItemType
    {
        Weapon,
        Armor,
        Accessory,
        Consumable
    }
    
    public class ShopDatabase
    {
        public static List<ShopItem> items = new List<ShopItem>
        {
            new ShopItem {
                id = 1,
                name = "破军",
                description = "增加100点攻击力",
                price = 3000,
                type = ItemType.Weapon,
                attackBonus = 100
            },
            new ShopItem {
                id = 2,
                name = "魔女斗篷",
                description = "增加80点魔抗和300生命值",
                price = 2080,
                type = ItemType.Armor,
                defenseBonus = 80,
                healthBonus = 300
            },
            new ShopItem {
                id = 3,
                name = "贤者的庇护",
                description = "复活效果，增加双抗",
                price = 2150,
                type = ItemType.Accessory,
                defenseBonus = 50,
                healthBonus = 200
            },
            new ShopItem {
                id = 4,
                name = "红药瓶",
                description = "恢复150点生命值",
                price = 50,
                type = ItemType.Consumable,
                healthBonus = 150
            }
        };
        
        public static ShopItem GetItem(int id)
        {
            return items.Find(item => item.id == id);
        }
    }
}
