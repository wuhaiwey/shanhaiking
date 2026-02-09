using UnityEngine;
using UnityEngine.UI;

namespace ShanHaiKing.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        
        [Header("HUD元素")]
        public Slider healthBar;
        public Slider manaBar;
        public Text levelText;
        public Image skillIcon_Q;
        public Image skillIcon_W;
        public Image skillIcon_E;
        public Image skillIcon_R;
        
        [Header("小地图")]
        public RectTransform minimap;
        public RectTransform playerIcon;
        
        void Awake()
        {
            Instance = this;
        }
        
        public void UpdateHealth(float current, float max)
        {
            if (healthBar != null)
                healthBar.value = current / max;
        }
        
        public void UpdateMana(float current, float max)
        {
            if (manaBar != null)
                manaBar.value = current / max;
        }
        
        public void UpdateLevel(int level)
        {
            if (levelText != null)
                levelText.text = "Lv." + level;
        }
    }
}
