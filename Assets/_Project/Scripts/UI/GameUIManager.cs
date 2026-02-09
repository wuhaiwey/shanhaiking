using UnityEngine;
using UnityEngine.UI;

namespace ShanHaiKing.UI
{
    /// <summary>
    /// 游戏主UI管理器
    /// </summary>
    public class GameUIManager : MonoBehaviour
    {
        public static GameUIManager Instance { get; private set; }
        
        [Header("血条UI")]
        public Slider playerHealthSlider;
        public Slider playerManaSlider;
        public Text healthText;
        public Text manaText;
        
        [Header("技能UI")]
        public Image[] skillIcons = new Image[4];
        public Image[] skillCooldownMasks = new Image[4];
        public Text[] skillCooldownTexts = new Text[4];
        
        [Header("信息UI")]
        public Text gameTimeText;
        public Text scoreText;
        public Text killDeathAssistText;
        
        [Header("小地图")]
        public RectTransform minimap;
        public RectTransform minimapPlayerIcon;
        
        [Header("击杀提示")]
        public GameObject killFeedPanel;
        public GameObject killFeedItemPrefab;
        
        void Awake()
        {
            Instance = this;
        }
        
        void Update()
        {
            UpdateGameTime();
        }
        
        public void UpdateHealth(float current, float max)
        {
            if (playerHealthSlider != null)
            {
                playerHealthSlider.value = current / max;
            }
            if (healthText != null)
            {
                healthText.text = $"{current:F0}/{max:F0}";
            }
        }
        
        public void UpdateMana(float current, float max)
        {
            if (playerManaSlider != null)
            {
                playerManaSlider.value = current / max;
            }
            if (manaText != null)
            {
                manaText.text = $"{current:F0}/{max:F0}";
            }
        }
        
        public void UpdateSkillCooldown(int skillIndex, float cooldownProgress, float remainingTime)
        {
            if (skillIndex < 0 || skillIndex >= 4) return;
            
            if (skillCooldownMasks[skillIndex] != null)
            {
                skillCooldownMasks[skillIndex].fillAmount = cooldownProgress;
            }
            
            if (skillCooldownTexts[skillIndex] != null)
            {
                skillCooldownTexts[skillIndex].text = remainingTime > 0 ? $"{remainingTime:F1}" : "";
            }
        }
        
        void UpdateGameTime()
        {
            if (Core.GameManager.Instance != null && gameTimeText != null)
            {
                gameTimeText.text = Core.GameManager.Instance.GetFormattedTime();
            }
        }
        
        public void ShowKillFeed(string killer, string victim, string weapon)
        {
            // TODO: 实现击杀提示
        }
    }
}
