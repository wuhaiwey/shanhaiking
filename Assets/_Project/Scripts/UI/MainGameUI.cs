using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ShanHaiKing.UI
{
    /// <summary>
    /// 游戏主UI系统
    /// </summary>
    public class MainGameUI : MonoBehaviour
    {
        public static MainGameUI Instance { get; private set; }
        
        [Header("顶部信息栏")]
        public Text gameTimeText;
        public Text teamScoreText;
        public Text fpsText;
        public Text pingText;
        
        [Header("玩家状态")]
        public Slider healthBar;
        public Slider manaBar;
        public Slider expBar;
        public Text levelText;
        public Text healthText;
        public Text manaText;
        public Text goldText;
        
        [Header("技能栏")]
        public Image[] skillIcons;
        public Image[] skillCooldownMasks;
        public Text[] skillCooldownTexts;
        public Text[] skillKeyTexts;
        
        [Header("装备栏")]
        public Image[] itemSlots;
        public Image[] itemBorders;
        
        [Header("小地图")]
        public RectTransform minimapRect;
        public Image minimapImage;
        public RectTransform playerIcon;
        public RectTransform[] allyIcons;
        public RectTransform[] enemyIcons;
        
        [Header("击杀信息")]
        public Text killFeedText;
        public float killFeedDuration = 3f;
        
        [Header("商店")]
        public GameObject shopPanel;
        public Transform shopItemContainer;
        public Text shopGoldText;
        
        [Header("设置")]
        public GameObject settingsPanel;
        public Slider masterVolumeSlider;
        public Slider bgmVolumeSlider;
        public Slider sfxVolumeSlider;
        
        [Header("其他")]
        public GameObject deathPanel;
        public Text respawnTimerText;
        public GameObject victoryPanel;
        public GameObject defeatPanel;
        
        private Hero.HeroBase currentHero;
        private float updateTimer = 0f;
        
        void Awake()
        {
            Instance = this;
        }
        
        void Start()
        {
            InitializeUI();
        }
        
        void InitializeUI()
        {
            // 初始化技能键位显示
            if (skillKeyTexts.Length >= 4)
            {
                skillKeyTexts[0].text = "Q";
                skillKeyTexts[1].text = "W";
                skillKeyTexts[2].text = "E";
                skillKeyTexts[3].text = "R";
            }
            
            // 隐藏面板
            shopPanel.SetActive(false);
            settingsPanel.SetActive(false);
            deathPanel.SetActive(false);
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);
        }
        
        void Update()
        {
            updateTimer += Time.deltaTime;
            
            if (updateTimer >= 0.1f) // 每0.1秒更新一次
            {
                UpdatePlayerStats();
                UpdateSkillCooldowns();
                UpdateGameInfo();
                updateTimer = 0f;
            }
            
            // 按键检测
            if (Input.GetKeyDown(KeyCode.B))
            {
                ToggleShop();
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleSettings();
            }
        }
        
        public void SetHero(Hero.HeroBase hero)
        {
            currentHero = hero;
            UpdateSkillIcons();
        }
        
        void UpdatePlayerStats()
        {
            if (currentHero == null) return;
            
            // 血条
            healthBar.value = currentHero.CurrentHealth / currentHero.MaxHealth;
            healthText.text = $"{currentHero.CurrentHealth:0}/{currentHero.MaxHealth:0}";
            
            // 法力条
            manaBar.value = currentHero.CurrentMana / currentHero.MaxMana;
            manaText.text = $"{currentHero.CurrentMana:0}/{currentHero.MaxMana:0}";
            
            // 经验条
            expBar.value = currentHero.Experience / currentHero.ExpToNextLevel;
            levelText.text = $"Lv.{currentHero.Level}";
            
            // 金币
            if (Core.ShopManager.Instance != null)
            {
                goldText.text = $"{Core.ShopManager.Instance.playerGold}G";
            }
        }
        
        void UpdateSkillCooldowns()
        {
            if (currentHero == null) return;
            
            for (int i = 0; i < 4 && i < currentHero.skills.Length; i++)
            {
                if (currentHero.skills[i] != null)
                {
                    float cooldownPercent = currentHero.skills[i].GetCooldownPercent();
                    skillCooldownMasks[i].fillAmount = cooldownPercent;
                    
                    if (cooldownPercent > 0)
                    {
                        float remaining = currentHero.skills[i].GetRemainingCooldown();
                        skillCooldownTexts[i].text = remaining > 1f ? $"{remaining:0}" : $"{remaining:0.0}";
                    }
                    else
                    {
                        skillCooldownTexts[i].text = "";
                    }
                }
            }
        }
        
        void UpdateSkillIcons()
        {
            if (currentHero == null) return;
            
            for (int i = 0; i < 4 && i < currentHero.skills.Length; i++)
            {
                if (currentHero.skills[i] != null && currentHero.skills[i].skillIcon != null)
                {
                    skillIcons[i].sprite = currentHero.skills[i].skillIcon;
                }
            }
        }
        
        void UpdateGameInfo()
        {
            // 游戏时间
            if (Core.GameManager.Instance != null)
            {
                float gameTime = Core.GameManager.Instance.gameTime;
                int minutes = Mathf.FloorToInt(gameTime / 60);
                int seconds = Mathf.FloorToInt(gameTime % 60);
                gameTimeText.text = $"{minutes:D2}:{seconds:D2}";
                
                // 比分
                teamScoreText.text = $"{Core.GameManager.Instance.blueKills} - {Core.GameManager.Instance.redKills}";
            }
            
            // FPS
            fpsText.text = $"FPS: {Mathf.RoundToInt(1f / Time.deltaTime)}";
            
            // Ping
            if (Network.NetworkManager.Instance != null)
            {
                pingText.text = $"{Network.NetworkManager.Instance.ping}ms";
            }
        }
        
        public void ShowKillFeed(string message)
        {
            killFeedText.text = message;
            killFeedText.gameObject.SetActive(true);
            CancelInvoke(nameof(HideKillFeed));
            Invoke(nameof(HideKillFeed), killFeedDuration);
        }
        
        void HideKillFeed()
        {
            killFeedText.gameObject.SetActive(false);
        }
        
        public void ToggleShop()
        {
            shopPanel.SetActive(!shopPanel.activeSelf);
            if (shopPanel.activeSelf)
            {
                RefreshShopItems();
            }
        }
        
        void RefreshShopItems()
        {
            // 刷新商店商品显示
            if (Core.ShopManager.Instance != null)
            {
                shopGoldText.text = $"{Core.ShopManager.Instance.playerGold}G";
            }
        }
        
        public void ToggleSettings()
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
        
        public void ShowDeathScreen(float respawnTime)
        {
            deathPanel.SetActive(true);
            StartCoroutine(RespawnCountdown(respawnTime));
        }
        
        System.Collections.IEnumerator RespawnCountdown(float time)
        {
            float remaining = time;
            while (remaining > 0)
            {
                respawnTimerText.text = $"复活倒计时: {remaining:0}秒";
                remaining -= Time.deltaTime;
                yield return null;
            }
            deathPanel.SetActive(false);
        }
        
        public void ShowVictory()
        {
            victoryPanel.SetActive(true);
        }
        
        public void ShowDefeat()
        {
            defeatPanel.SetActive(true);
        }
        
        public void OnVolumeChanged()
        {
            if (Core.AudioManager.Instance != null)
            {
                Core.AudioManager.Instance.SetMasterVolume(masterVolumeSlider.value);
                Core.AudioManager.Instance.SetBGMVolume(bgmVolumeSlider.value);
                Core.AudioManager.Instance.SetSFXVolume(sfxVolumeSlider.value);
            }
        }
    }
}
