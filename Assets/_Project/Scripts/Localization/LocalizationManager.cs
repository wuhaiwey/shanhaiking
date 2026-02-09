using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Localization
{
    /// <summary>
    /// 本地化系统 - 多语言支持
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }
        
        [Header("当前语言")]
        public SystemLanguage currentLanguage = SystemLanguage.ChineseSimplified;
        
        [Header("语言数据")]
        private Dictionary<SystemLanguage, Dictionary<string, string>> localizationData = 
            new Dictionary<SystemLanguage, Dictionary<string, string>>();
        
        [Header("支持的语言")]
        public List<SystemLanguage> supportedLanguages = new List<SystemLanguage>
        {
            SystemLanguage.ChineseSimplified,
            SystemLanguage.English,
            SystemLanguage.Japanese,
            SystemLanguage.Korean
        };
        
        void Awake()
        {
            Instance = this;
            LoadLocalizationData();
        }
        
        void LoadLocalizationData()
        {
            // 中文
            var zhData = new Dictionary<string, string>
            {
                // 游戏标题
                { "GAME_TITLE", "山海经王者荣耀" },
                { "GAME_SUBTITLE", "Ancient MOBA" },
                
                // 主菜单
                { "MENU_PLAY", "开始游戏" },
                { "MENU_HERO", "英雄" },
                { "MENU_SHOP", "商店" },
                { "MENU_SETTINGS", "设置" },
                { "MENU_PROFILE", "个人资料" },
                { "MENU_EXIT", "退出" },
                
                // 游戏模式
                { "MODE_RANKED", "排位赛" },
                { "MODE_CASUAL", "匹配赛" },
                { "MODE_TUTORIAL", "教程" },
                { "MODE_CUSTOM", "自定义" },
                
                // 英雄类型
                { "TYPE_TANK", "坦克" },
                { "TYPE_WARRIOR", "战士" },
                { "TYPE_ASSASSIN", "刺客" },
                { "TYPE_MAGE", "法师" },
                { "TYPE_MARKSMAN", "射手" },
                { "TYPE_SUPPORT", "辅助" },
                
                // 技能
                { "SKILL_Q", "技能Q" },
                { "SKILL_W", "技能W" },
                { "SKILL_E", "技能E" },
                { "SKILL_R", "大招R" },
                { "SKILL_PASSIVE", "被动" },
                
                // 游戏界面
                { "UI_HEALTH", "生命值" },
                { "UI_MANA", "法力值" },
                { "UI_EXP", "经验值" },
                { "UI_GOLD", "金币" },
                { "UI_LEVEL", "等级" },
                { "UI_KILL", "击杀" },
                { "UI_DEATH", "死亡" },
                { "UI_ASSIST", "助攻" },
                
                // 商店
                { "SHOP_WEAPON", "武器" },
                { "SHOP_ARMOR", "防具" },
                { "SHOP_ACCESSORY", "饰品" },
                { "SHOP_CONSUMABLE", "消耗品" },
                { "SHOP_BUY", "购买" },
                { "SHOP_SELL", "出售" },
                
                // 提示信息
                { "TIP_FIRSTBLOOD", "第一滴血！" },
                { "TIP_DOUBLEKILL", "双杀！" },
                { "TIP_TRIPLEKILL", "三杀！" },
                { "TIP_QUADRAKILL", "四杀！" },
                { "TIP_PENTAKILL", "五杀！" },
                { "TIP_SHUTDOWN", "终结！" },
                { "TIP_VICTORY", "胜利！" },
                { "TIP_DEFEAT", "失败！" },
                
                // 英雄名称
                { "HERO_HOUYI", "后羿" },
                { "HERO_JIUWEIHU", "九尾狐" },
                { "HERO_XINGTIAN", "刑天" },
                { "HERO_NVWA", "女娲" },
                { "HERO_JINGWEI", "精卫" },
                { "HERO_QIONGQI", "穷奇" },
                { "HERO_GONGGONG", "共工" },
                { "HERO_FUXI", "伏羲" },
                { "HERO_ZHURONG", "祝融" },
                { "HERO_CHIYOU", "蚩尤" },
                { "HERO_TAOTIE", "饕餮" },
                { "HERO_BAIZE", "白泽" }
            };
            
            // 英文
            var enData = new Dictionary<string, string>
            {
                { "GAME_TITLE", "Shan Hai King" },
                { "GAME_SUBTITLE", "Ancient MOBA" },
                { "MENU_PLAY", "Play" },
                { "MENU_HERO", "Heroes" },
                { "MENU_SHOP", "Shop" },
                { "MENU_SETTINGS", "Settings" },
                { "MENU_PROFILE", "Profile" },
                { "MENU_EXIT", "Exit" },
                { "MODE_RANKED", "Ranked" },
                { "MODE_CASUAL", "Casual" },
                { "MODE_TUTORIAL", "Tutorial" },
                { "MODE_CUSTOM", "Custom" },
                { "TYPE_TANK", "Tank" },
                { "TYPE_WARRIOR", "Warrior" },
                { "TYPE_ASSASSIN", "Assassin" },
                { "TYPE_MAGE", "Mage" },
                { "TYPE_MARKSMAN", "Marksman" },
                { "TYPE_SUPPORT", "Support" },
                { "SKILL_Q", "Skill Q" },
                { "SKILL_W", "Skill W" },
                { "SKILL_E", "Skill E" },
                { "SKILL_R", "Ultimate R" },
                { "SKILL_PASSIVE", "Passive" },
                { "UI_HEALTH", "Health" },
                { "UI_MANA", "Mana" },
                { "UI_EXP", "Experience" },
                { "UI_GOLD", "Gold" },
                { "UI_LEVEL", "Level" },
                { "UI_KILL", "Kill" },
                { "UI_DEATH", "Death" },
                { "UI_ASSIST", "Assist" },
                { "SHOP_WEAPON", "Weapon" },
                { "SHOP_ARMOR", "Armor" },
                { "SHOP_ACCESSORY", "Accessory" },
                { "SHOP_CONSUMABLE", "Consumable" },
                { "SHOP_BUY", "Buy" },
                { "SHOP_SELL", "Sell" },
                { "TIP_FIRSTBLOOD", "First Blood!" },
                { "TIP_DOUBLEKILL", "Double Kill!" },
                { "TIP_TRIPLEKILL", "Triple Kill!" },
                { "TIP_QUADRAKILL", "Quadra Kill!" },
                { "TIP_PENTAKILL", "PENTAKILL!" },
                { "TIP_SHUTDOWN", "Shutdown!" },
                { "TIP_VICTORY", "Victory!" },
                { "TIP_DEFEAT", "Defeat!" },
                { "HERO_HOUYI", "Hou Yi" },
                { "HERO_JIUWEIHU", "Nine-Tailed Fox" },
                { "HERO_XINGTIAN", "Xing Tian" },
                { "HERO_NVWA", "Nuwa" },
                { "HERO_JINGWEI", "Jingwei" },
                { "HERO_QIONGQI", "Qiong Qi" },
                { "HERO_GONGGONG", "Gong Gong" },
                { "HERO_FUXI", "Fu Xi" },
                { "HERO_ZHURONG", "Zhu Rong" },
                { "HERO_CHIYOU", "Chi You" },
                { "HERO_TAOTIE", "Tao Tie" },
                { "HERO_BAIZE", "Bai Ze" }
            };
            
            localizationData[SystemLanguage.ChineseSimplified] = zhData;
            localizationData[SystemLanguage.English] = enData;
        }
        
        public string GetLocalizedString(string key)
        {
            if (localizationData.ContainsKey(currentLanguage))
            {
                var langData = localizationData[currentLanguage];
                if (langData.ContainsKey(key))
                {
                    return langData[key];
                }
            }
            
            // 回退到英文
            if (localizationData.ContainsKey(SystemLanguage.English))
            {
                var enData = localizationData[SystemLanguage.English];
                if (enData.ContainsKey(key))
                {
                    return enData[key];
                }
            }
            
            return key;
        }
        
        public void SetLanguage(SystemLanguage language)
        {
            if (supportedLanguages.Contains(language))
            {
                currentLanguage = language;
                OnLanguageChanged();
            }
        }
        
        void OnLanguageChanged()
        {
            // 通知所有UI更新语言
            var localizedTexts = FindObjectsOfType<LocalizedText>();
            foreach (var text in localizedTexts)
            {
                text.UpdateText();
            }
        }
        
        public SystemLanguage GetSystemLanguage()
        {
            return Application.systemLanguage;
        }
        
        public void AutoDetectLanguage()
        {
            SystemLanguage sysLang = GetSystemLanguage();
            if (supportedLanguages.Contains(sysLang))
            {
                SetLanguage(sysLang);
            }
            else
            {
                SetLanguage(SystemLanguage.English);
            }
        }
    }
    
    /// <summary>
    /// 本地化文本组件
    /// </summary>
    public class LocalizedText : MonoBehaviour
    {
        public string localizationKey;
        private UnityEngine.UI.Text textComponent;
        
        void Start()
        {
            textComponent = GetComponent<UnityEngine.UI.Text>();
            UpdateText();
        }
        
        public void UpdateText()
        {
            if (textComponent != null && LocalizationManager.Instance != null)
            {
                textComponent.text = LocalizationManager.Instance.GetLocalizedString(localizationKey);
            }
        }
    }
}
