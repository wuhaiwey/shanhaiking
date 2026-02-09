using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ShanHaiKing.UI
{
    /// <summary>
    /// 英雄选择界面UI
    /// </summary>
    public class HeroSelectUI : MonoBehaviour
    {
        public static HeroSelectUI Instance { get; private set;; }
        
        [Header("英雄列表")]
        public Transform heroGrid;
        public GameObject heroSlotPrefab;
        public int heroesPerRow = 5;
        
        [Header("英雄信息")]
        public Image heroPortrait;
        public Text heroNameText;
        public Text heroTitleText;
        public Text heroDescriptionText;
        public Image heroTypeIcon;
        public Text heroStatsText;
        
        [Header("技能预览")]
        public Image[] skillIcons;
        public Text[] skillNames;
        public Text[] skillDescriptions;
        
        [Header("属性条")]
        public Slider attackSlider;
        public Slider defenseSlider;
        public Slider magicSlider;
        public Slider difficultySlider;
        
        [Header("按钮")]
        public Button selectButton;
        public Button backButton;
        public Button skinButton;
        
        [Header("皮肤选择")]
        public Transform skinGrid;
        public GameObject skinSlotPrefab;
        
        private List<GameObject> heroSlots = new List<GameObject>();
        private List<GameObject> skinSlots = new List<GameObject>();
        private int selectedHeroIndex = -1;
        private int selectedSkinIndex = 0;
        
        void Awake()
        {
            Instance = this;
        }
        
        void Start()
        {
            InitializeHeroList();
            InitializeButtons();
        }
        
        void InitializeHeroList()
        {
            // 英雄数据
            var heroes = new (string name, string title, string type, string desc, int attack, int defense, int magic, int difficulty)[]
            {
                ("后羿", "射日英雄", "射手", "远程物理输出，擅长持续输出", 9, 3, 2, 4),
                ("九尾狐", "魅惑妖狐", "刺客", "高爆发刺客，擅长切入后排", 8, 2, 6, 7),
                ("刑天", "不屈战神", "坦克", "前排坦克，越战越勇", 5, 10, 2, 3),
                ("女娲", "创世女神", "法师", "控制型法师，团队辅助", 3, 4, 9, 6),
                ("精卫", "填海志鸟", "射手", "灵活射手，持续输出", 8, 3, 3, 5),
                ("穷奇", "四凶之翼", "刺客", "高机动刺客，连招输出", 9, 3, 4, 8),
                ("共工", "水神", "法师", "控制型法师，水元素", 4, 5, 9, 6),
                ("伏羲", "人皇", "法师", "阴阳法师，变化多端", 4, 4, 10, 9),
                ("祝融", "火神", "法师", "持续输出法师，火焰掌控", 6, 5, 9, 7),
                ("蚩尤", "兵主", "战士", "近战战士，高输出", 8, 6, 2, 4),
                ("饕餮", "贪婪之兽", "坦克", "肉盾坦克，吞噬敌人", 4, 10, 3, 3),
                ("白泽", "祥瑞之兽", "辅助", "辅助型，治疗增益", 3, 5, 6, 5)
            };
            
            for (int i = 0; i < heroes.Length; i++)
            {
                CreateHeroSlot(i, heroes[i]);
            }
        }
        
        void CreateHeroSlot(int index, (string name, string title, string type, string desc, int attack, int defense, int magic, int difficulty) hero)
        {
            GameObject slot = Instantiate(heroSlotPrefab, heroGrid);
            
            Text nameText = slot.GetComponentInChildren<Text>();
            if (nameText != null)
                nameText.text = hero.name;
            
            Button button = slot.GetComponent<Button>();
            if (button != null)
            {
                int heroIndex = index;
                button.onClick.AddListener(() => SelectHero(heroIndex));
            }
            
            // 设置类型颜色
            Image slotImage = slot.GetComponent<Image>();
            if (slotImage != null)
            {
                switch (hero.type)
                {
                    case "坦克": slotImage.color = new Color(0.2f, 0.6f, 0.2f); break;
                    case "战士": slotImage.color = new Color(0.8f, 0.4f, 0.2f); break;
                    case "刺客": slotImage.color = new Color(0.8f, 0.2f, 0.8f); break;
                    case "法师": slotImage.color = new Color(0.2f, 0.4f, 0.9f); break;
                    case "射手": slotImage.color = new Color(0.9f, 0.8f, 0.2f); break;
                    case "辅助": slotImage.color = new Color(0.2f, 0.8f, 0.8f); break;
                }
            }
            
            heroSlots.Add(slot);
        }
        
        void SelectHero(int index)
        {
            selectedHeroIndex = index;
            
            // 高亮选中
            for (int i = 0; i < heroSlots.Count; i++)
            {
                Image slotImage = heroSlots[i].GetComponent<Image>();
                if (slotImage != null)
                {
                    Color color = slotImage.color;
                    color.a = (i == index) ? 1f : 0.5f;
                    slotImage.color = color;
                }
            }
            
            UpdateHeroInfo(index);
            LoadHeroSkins(index);
        }
        
        void UpdateHeroInfo(int index)
        {
            var heroes = new (string name, string title, string type, string desc, int attack, int defense, int magic, int difficulty)[]
            {
                ("后羿", "射日英雄", "射手", "后羿是神话中的射日英雄，擅长使用弓箭进行远程攻击。他的箭矢可以穿透敌人，对多个目标造成伤害。", 9, 3, 2, 4),
                ("九尾狐", "魅惑妖狐", "刺客", "九尾狐是传说中的妖狐，拥有魅惑人心的能力。她擅长快速切入战场，对单个目标造成致命打击。", 8, 2, 6, 7),
                ("刑天", "不屈战神", "坦克", "刑天是被砍头仍战斗的战神，拥有极强的生存能力。他在战斗中越伤越强，是可靠的前排。", 5, 10, 2, 3),
                ("女娲", "创世女神", "法师", "女娲是创造人类的古神，掌握强大的法术力量。她可以召唤土灵协助战斗，使用补天之力治愈盟友。", 3, 4, 9, 6),
                ("精卫", "填海志鸟", "射手", "精卫是炎帝之女，化为神鸟后不停衔石填海。她的石子可以储存并连续发射，爆发极高。", 8, 3, 3, 5),
                ("穷奇", "四凶之翼", "刺客", "穷奇是上古四凶之一，拥有锋利的爪子。他可以进入隐身状态，从阴影中给予敌人致命一击。", 9, 3, 4, 8),
                ("共工", "水神", "法师", "共工是掌管水的神祇，可以操控水元素。他的技能可以创造水域，在水域中获得强大的增益。", 4, 5, 9, 6),
                ("伏羲", "人皇", "法师", "伏羲是人类的始祖，创造了八卦。他可以在阴阳之间切换，根据战况灵活应对。", 4, 4, 10, 9),
                ("祝融", "火神", "法师", "祝融是火的神祇，掌控烈焰。他可以通过叠加火焰层数进入过热状态，释放出毁天灭地的力量。", 6, 5, 9, 7),
                ("蚩尤", "兵主", "战士", "蚩尤是上古战神，铜头铁额。他是近战输出的代表，可以在战场上横扫千军。", 8, 6, 2, 4),
                ("饕餮", "贪婪之兽", "坦克", "饕餮是贪婪的怪兽，永远吃不饱。他拥有极强的生存能力，可以吞噬敌人恢复生命。", 4, 10, 3, 3),
                ("白泽", "祥瑞之兽", "辅助", "白泽是祥瑞之兽，通晓万物。他可以为队友提供治疗和增益，是团队中不可或缺的辅助。", 3, 5, 6, 5)
            };
            
            if (index >= 0 && index < heroes.Length)
            {
                var hero = heroes[index];
                
                if (heroNameText != null) heroNameText.text = hero.name;
                if (heroTitleText != null) heroTitleText.text = hero.title;
                if (heroDescriptionText != null) heroDescriptionText.text = hero.desc;
                
                if (attackSlider != null) attackSlider.value = hero.attack / 10f;
                if (defenseSlider != null) defenseSlider.value = hero.defense / 10f;
                if (magicSlider != null) magicSlider.value = hero.magic / 10f;
                if (difficultySlider != null) difficultySlider.value = hero.difficulty / 10f;
            }
        }
        
        void LoadHeroSkins(int heroIndex)
        {
            // 清除旧皮肤
            foreach (GameObject skin in skinSlots)
            {
                Destroy(skin);
            }
            skinSlots.Clear();
            
            // 创建默认皮肤槽
            CreateSkinSlot(0, "经典", true);
            CreateSkinSlot(1, "史诗", false);
            CreateSkinSlot(2, "传说", false);
        }
        
        void CreateSkinSlot(int index, string skinName, bool isUnlocked)
        {
            GameObject slot = Instantiate(skinSlotPrefab, skinGrid);
            
            Text nameText = slot.GetComponentInChildren<Text>();
            if (nameText != null)
                nameText.text = skinName;
            
            Button button = slot.GetComponent<Button>();
            if (button != null)
            {
                int skinIndex = index;
                button.onClick.AddListener(() => SelectSkin(skinIndex));
                button.interactable = isUnlocked;
            }
            
            Image slotImage = slot.GetComponent<Image>();
            if (slotImage != null)
            {
                slotImage.color = isUnlocked ? Color.white : Color.gray;
            }
            
            skinSlots.Add(slot);
        }
        
        void SelectSkin(int index)
        {
            selectedSkinIndex = index;
            
            for (int i = 0; i < skinSlots.Count; i++)
            {
                Image slotImage = skinSlots[i].GetComponent<Image>();
                if (slotImage != null)
                {
                    slotImage.color = (i == index) ? Color.yellow : Color.white;
                }
            }
        }
        
        void InitializeButtons()
        {
            selectButton?.onClick.AddListener(OnSelectButtonClicked);
            backButton?.onClick.AddListener(OnBackButtonClicked);
            skinButton?.onClick.AddListener(OnSkinButtonClicked);
        }
        
        void OnSelectButtonClicked()
        {
            if (selectedHeroIndex >= 0)
            {
                Debug.Log($"选择英雄: {selectedHeroIndex}");
                // 进入游戏
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
            }
        }
        
        void OnBackButtonClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
        
        void OnSkinButtonClicked()
        {
            // 显示皮肤选择
        }
    }
}
