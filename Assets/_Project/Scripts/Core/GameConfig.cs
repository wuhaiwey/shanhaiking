using UnityEngine;

namespace ShanHaiKing.Core
{
    /// <summary>
    /// 游戏配置系统
    /// </summary>
    public class GameConfig : ScriptableObject
    {
        public static GameConfig Instance { get; private set; }
        
        [Header("游戏设置")]
        public float gameDuration = 1800f; // 30分钟
        public int maxPlayers = 10;
        public int maxLevel = 18;
        
        [Header("经验设置")]
        public AnimationCurve expCurve = AnimationCurve.Linear(1, 100, 18, 2000);
        public float sharedExpPercent = 0.5f;
        
        [Header("经济设置")]
        public int startingGold = 500;
        public int passiveGoldRate = 3; // 每秒获得金币
        public int killGoldBase = 300;
        public int assistGoldBase = 150;
        public int minionGoldBase = 20;
        public int towerGoldBase = 250;
        
        [Header("战斗设置")]
        public float globalCooldown = 0.5f;
        public float attackRangeBuffer = 0.5f;
        public float abilityRangeBuffer = 1f;
        
        [Header("复活设置")]
        public float baseRespawnTime = 10f;
        public float respawnTimePerLevel = 2f;
        public float maxRespawnTime = 60f;
        
        [Header("小兵设置")]
        public float minionSpawnInterval = 30f;
        public int minionsPerWave = 6;
        public float minionEnhanceInterval = 90f;
        
        [Header("防御塔设置")]
        public float towerAttackRange = 8f;
        public float towerAttackSpeed = 1f;
        public int towerHealth = 5000;
        public int towerDamage = 150;
        
        [Header("野区设置")]
        public float jungleRespawnTime = 120f;
        public int jungleCampCount = 6;
        
        void OnEnable()
        {
            Instance = this;
        }
        
        /// <summary>
        /// 获取指定等级所需经验
        /// </summary>
        public int GetExpForLevel(int level)
        {
            return Mathf.RoundToInt(expCurve.Evaluate(level));
        }
        
        /// <summary>
        /// 获取击杀金币
        /// </summary>
        public int GetKillGold(int killerLevel, int victimLevel)
        {
            int levelDiff = victimLevel - killerLevel;
            float multiplier = 1f + (levelDiff * 0.1f);
            return Mathf.RoundToInt(killGoldBase * Mathf.Max(0.5f, multiplier));
        }
        
        /// <summary>
        /// 获取复活时间
        /// </summary>
        public float GetRespawnTime(int level)
        {
            float time = baseRespawnTime + (level * respawnTimePerLevel);
            return Mathf.Min(time, maxRespawnTime);
        }
        
        /// <summary>
        /// 获取小兵增强倍率
        /// </summary>
        public float GetMinionEnhanceMultiplier(float gameTime)
        {
            int enhancements = Mathf.FloorToInt(gameTime / minionEnhanceInterval);
            return 1f + (enhancements * 0.1f);
        }
    }
}
