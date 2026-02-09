using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ShanHaiKing.UI
{
    /// <summary>
    /// 主菜单UI系统
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("主面板")]
        public GameObject mainPanel;
        public GameObject playPanel;
        public GameObject heroPanel;
        public GameObject shopPanel;
        public GameObject settingsPanel;
        public GameObject profilePanel;
        
        [Header("主菜单按钮")]
        public Button playButton;
        public Button heroButton;
        public Button shopButton;
        public Button settingsButton;
        public Button profileButton;
        public Button exitButton;
        
        [Header("游戏模式选择")]
        public Button rankedButton;
        public Button casualButton;
        public Button tutorialButton;
        public Button customButton;
        public Button backFromPlayButton;
        
        [Header("英雄选择")]
        public Transform heroGrid;
        public GameObject heroCardPrefab;
        public Image selectedHeroImage;
        public Text selectedHeroName;
        public Text selectedHeroDescription;
        public Button backFromHeroButton;
        
        [Header("玩家信息")]
        public Text playerNameText;
        public Text playerLevelText;
        public Text playerGoldText;
        public Image playerAvatar;
        public Slider expSlider;
        
        [Header("设置")]
        public Slider masterVolumeSlider;
        public Slider bgmVolumeSlider;
        public Slider sfxVolumeSlider;
        public Toggle fullscreenToggle;
        public Dropdown resolutionDropdown;
        public Dropdown qualityDropdown;
        public Button backFromSettingsButton;
        
        private List<GameObject> heroCards = new List<GameObject>();
        private int selectedHeroIndex = 0;
        
        void Start()
        {
            InitializeButtons();
            LoadPlayerData();
            ShowMainPanel();
        }
        
        void InitializeButtons()
        {
            // 主菜单
            playButton?.onClick.AddListener(ShowPlayPanel);
            heroButton?.onClick.AddListener(ShowHeroPanel);
            shopButton?.onClick.AddListener(ShowShopPanel);
            settingsButton?.onClick.AddListener(ShowSettingsPanel);
            profileButton?.onClick.AddListener(ShowProfilePanel);
            exitButton?.onClick.AddListener(ExitGame);
            
            // 游戏模式
            rankedButton?.onClick.AddListener(() => StartMatch("ranked"));
            casualButton?.onClick.AddListener(() => StartMatch("casual"));
            tutorialButton?.onClick.AddListener(() => StartMatch("tutorial"));
            customButton?.onClick.AddListener(() => StartMatch("custom"));
            backFromPlayButton?.onClick.AddListener(ShowMainPanel);
            
            // 英雄选择
            backFromHeroButton?.onClick.AddListener(ShowMainPanel);
            
            // 设置
            backFromSettingsButton?.onClick.AddListener(ShowMainPanel);
            
            // 音量设置
            masterVolumeSlider?.onValueChanged.AddListener(OnMasterVolumeChanged);
            bgmVolumeSlider?.onValueChanged.AddListener(OnBGMVolumeChanged);
            sfxVolumeSlider?.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        
        void LoadPlayerData()
        {
            // 加载玩家数据
            if (playerNameText != null)
                playerNameText.text = "Player";
            if (playerLevelText != null)
                playerLevelText.text = "Lv.1";
            if (playerGoldText != null)
                playerGoldText.text = "1000G";
        }
        
        void ShowMainPanel()
        {
            HideAllPanels();
            mainPanel.SetActive(true);
        }
        
        void ShowPlayPanel()
        {
            HideAllPanels();
            playPanel.SetActive(true);
        }
        
        void ShowHeroPanel()
        {
            HideAllPanels();
            heroPanel.SetActive(true);
            LoadHeroList();
        }
        
        void ShowShopPanel()
        {
            HideAllPanels();
            shopPanel.SetActive(true);
        }
        
        void ShowSettingsPanel()
        {
            HideAllPanels();
            settingsPanel.SetActive(true);
        }
        
        void ShowProfilePanel()
        {
            HideAllPanels();
            profilePanel.SetActive(true);
        }
        
        void HideAllPanels()
        {
            mainPanel?.SetActive(false);
            playPanel?.SetActive(false);
            heroPanel?.SetActive(false);
            shopPanel?.SetActive(false);
            settingsPanel?.SetActive(false);
            profilePanel?.SetActive(false);
        }
        
        void LoadHeroList()
        {
            // 清除旧列表
            foreach (GameObject card in heroCards)
            {
                Destroy(card);
            }
            heroCards.Clear();
            
            // 创建英雄卡片
            string[] heroNames = { "后羿", "九尾狐", "刑天", "女娲", "精卫", "穷奇", "蚩尤", "饕餮", "白泽" };
            string[] heroTitles = { "射日英雄", "魅惑妖狐", "不屈战神", "创世女神", "填海志鸟", "四凶之翼", "兵主", "贪婪之兽", "祥瑞之兽" };
            
            for (int i = 0; i < heroNames.Length; i++)
            {
                GameObject card = Instantiate(heroCardPrefab, heroGrid);
                
                Text nameText = card.GetComponentInChildren<Text>();
                if (nameText != null)
                    nameText.text = heroNames[i];
                
                Button button = card.GetComponent<Button>();
                int index = i;
                button?.onClick.AddListener(() => SelectHero(index, heroNames[index], heroTitles[index]));
                
                heroCards.Add(card);
            }
        }
        
        void SelectHero(int index, string name, string title)
        {
            selectedHeroIndex = index;
            if (selectedHeroName != null)
                selectedHeroName.text = $"{name} - {title}";
        }
        
        void StartMatch(string mode)
        {
            Debug.Log($"开始 {mode} 模式");
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        }
        
        void OnMasterVolumeChanged(float value)
        {
            AudioListener.volume = value;
        }
        
        void OnBGMVolumeChanged(float value)
        {
            // 调整BGM音量
        }
        
        void OnSFXVolumeChanged(float value)
        {
            // 调整SFX音量
        }
        
        void ExitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
