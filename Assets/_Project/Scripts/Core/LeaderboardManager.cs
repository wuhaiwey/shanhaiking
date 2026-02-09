using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Core
{
    /// <summary>
    /// 排行榜系统
    /// </summary>
    public class LeaderboardManager : MonoBehaviour
    {
        public static LeaderboardManager Instance { get; private set; }
        
        [Header("排行榜数据")]
        public List<PlayerRank> globalRanks = new List<PlayerRank>();
        public List<PlayerRank> friendRanks = new List<PlayerRank>();
        
        void Awake()
        {
            Instance = this;
            InitializeMockData();
        }
        
        void InitializeMockData()
        {
            // 模拟全球排行榜数据
            globalRanks.Add(new PlayerRank { playerName = "王者归来", rank = 1, score = 9999, tier = "王者" });
            globalRanks.Add(new PlayerRank { playerName = "九天揽月", rank = 2, score = 9500, tier = "王者" });
            globalRanks.Add(new PlayerRank { playerName = "一剑霜寒", rank = 3, score = 9200, tier = "王者" });
            globalRanks.Add(new PlayerRank { playerName = "风清扬", rank = 4, score = 8800, tier = "星耀" });
            globalRanks.Add(new PlayerRank { playerName = "独孤求败", rank = 5, score = 8500, tier = "星耀" });
            globalRanks.Add(new PlayerRank { playerName = "你", rank = 999, score = 100, tier = "青铜" });
        }
        
        public void UpdatePlayerRank(string playerName, int newScore)
        {
            PlayerRank player = globalRanks.Find(p => p.playerName == playerName);
            if (player != null)
            {
                player.score = newScore;
                player.lastUpdated = System.DateTime.Now;
            }
            else
            {
                globalRanks.Add(new PlayerRank
                {
                    playerName = playerName,
                    score = newScore,
                    rank = globalRanks.Count + 1,
                    tier = CalculateTier(newScore),
                    lastUpdated = System.DateTime.Now
                });
            }
            
            SortAndUpdateRanks();
        }
        
        void SortAndUpdateRanks()
        {
            globalRanks.Sort((a, b) => b.score.CompareTo(a.score));
            
            for (int i = 0; i < globalRanks.Count; i++)
            {
                globalRanks[i].rank = i + 1;
                globalRanks[i].tier = CalculateTier(globalRanks[i].score);
            }
        }
        
        string CalculateTier(int score)
        {
            if (score >= 9000) return "王者";
            if (score >= 8000) return "星耀";
            if (score >= 7000) return "钻石";
            if (score >= 6000) return "铂金";
            if (score >= 5000) return "黄金";
            if (score >= 4000) return "白银";
            return "青铜";
        }
        
        public List<PlayerRank> GetTopPlayers(int count)
        {
            return globalRanks.GetRange(0, Mathf.Min(count, globalRanks.Count));
        }
    }
    
    [System.Serializable]
    public class PlayerRank
    {
        public string playerName;
        public int rank;
        public int score;
        public string tier;
        public System.DateTime lastUpdated;
        public Sprite avatar;
    }
}
