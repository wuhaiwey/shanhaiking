using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Battle
{
    /// <summary>
    /// 战斗统计系统
    /// </summary>
    public class BattleStatistics : MonoBehaviour
    {
        public static BattleStatistics Instance { get; private set; }
        
        [Header("玩家统计")]
        public PlayerBattleStats localPlayerStats;
        public List<PlayerBattleStats> allPlayerStats = new List<PlayerBattleStats>();
        
        [Header("团队统计")]
        public TeamBattleStats blueTeamStats;
        public TeamBattleStats redTeamStats;
        
        [Header("全局统计")]
        public float gameDuration = 0f;
        public int totalKills = 0;
        public int totalDeaths = 0;
        public int totalAssists = 0;
        public int totalGoldEarned = 0;
        public int totalDamageDealt = 0;
        public int totalDamageTaken = 0;
        public int totalHealingDone = 0;
        
        void Awake()
        {
            Instance = this;
        }
        
        void Update()
        {
            gameDuration += Time.deltaTime;
        }
        
        #region 统计记录
        
        public void RecordKill(int playerId, int victimId, bool isFirstBlood = false)
        {
            PlayerBattleStats stats = GetPlayerStats(playerId);
            if (stats != null)
            {
                stats.kills++;
                if (isFirstBlood)
                    stats.firstBlood = true;
            }
            
            totalKills++;
            
            // 记录连杀
            CheckKillStreak(playerId);
        }
        
        public void RecordDeath(int playerId)
        {
            PlayerBattleStats stats = GetPlayerStats(playerId);
            if (stats != null)
            {
                stats.deaths++;
                stats.killStreak = 0; // 重置连杀
            }
            
            totalDeaths++;
        }
        
        public void RecordAssist(int playerId)
        {
            PlayerBattleStats stats = GetPlayerStats(playerId);
            if (stats != null)
            {
                stats.assists++;
            }
            
            totalAssists++;
        }
        
        public void RecordDamageDealt(int playerId, float damage)
        {
            PlayerBattleStats stats = GetPlayerStats(playerId);
            if (stats != null)
            {
                stats.damageDealt += (int)damage;
            }
            
            totalDamageDealt += (int)damage;
        }
        
        public void RecordDamageTaken(int playerId, float damage)
        {
            PlayerBattleStats stats = GetPlayerStats(playerId);
            if (stats != null)
            {
                stats.damageTaken += (int)damage;
            }
            
            totalDamageTaken += (int)damage;
        }
        
        public void RecordHealing(int playerId, float healAmount)
        {
            PlayerBattleStats stats = GetPlayerStats(playerId);
            if (stats != null)
            {
                stats.healingDone += (int)healAmount;
            }
            
            totalHealingDone += (int)healAmount;
        }
        
        public void RecordGoldEarned(int playerId, int gold)
        {
            PlayerBattleStats stats = GetPlayerStats(playerId);
            if (stats != null)
            {
                stats.goldEarned += gold;
            }
            
            totalGoldEarned += gold;
        }
        
        public void RecordMinionKill(int playerId)
        {
            PlayerBattleStats stats = GetPlayerStats(playerId);
            if (stats != null)
            {
                stats.minionsKilled++;
            }
        }
        
        public void RecordTowerDestroyed(int playerId)
        {
            PlayerBattleStats stats = GetPlayerStats(playerId);
            if (stats != null)
            {
                stats.towersDestroyed++;
            }
        }
        
        void CheckKillStreak(int playerId)
        {
            PlayerBattleStats stats = GetPlayerStats(playerId);
            if (stats != null)
            {
                stats.killStreak++;
                stats.maxKillStreak = Mathf.Max(stats.maxKillStreak, stats.killStreak);
                
                // 通知连杀
                if (stats.killStreak >= 3)
                {
                    // 触发连杀事件
                }
            }
        }
        
        #endregion
        
        #region 查询方法
        
        PlayerBattleStats GetPlayerStats(int playerId)
        {
            return allPlayerStats.Find(s => s.playerId == playerId);
        }
        
        public float GetKDA(int playerId)
        {
            PlayerBattleStats stats = GetPlayerStats(playerId);
            if (stats == null) return 0f;
            return stats.GetKDA();
        }
        
        public int GetTotalScore(int playerId)
        {
            PlayerBattleStats stats = GetPlayerStats(playerId);
            if (stats == null) return 0;
            return stats.GetTotalScore();
        }
        
        public PlayerBattleStats GetMVP()
        {
            PlayerBattleStats mvp = null;
            int maxScore = 0;
            
            foreach (var stats in allPlayerStats)
            {
                int score = stats.GetTotalScore();
                if (score > maxScore)
                {
                    maxScore = score;
                    mvp = stats;
                }
            }
            
            return mvp;
        }
        
        public List<PlayerBattleStats> GetRanking()
        {
            List<PlayerBattleStats> ranking = new List<PlayerBattleStats>(allPlayerStats);
            ranking.Sort((a, b) => b.GetTotalScore().CompareTo(a.GetTotalScore()));
            return ranking;
        }
        
        #endregion
        
        #region 报告生成
        
        public BattleReport GenerateReport()
        {
            BattleReport report = new BattleReport
            {
                gameDuration = gameDuration,
                totalKills = totalKills,
                totalDeaths = totalDeaths,
                totalAssists = totalAssists,
                totalGold = totalGoldEarned,
                totalDamage = totalDamageDealt,
                playerStats = allPlayerStats,
                mvp = GetMVP()
            };
            
            return report;
        }
        
        public void SaveMatchHistory()
        {
            BattleReport report = GenerateReport();
            // 保存到本地数据库
            string json = JsonUtility.ToJson(report);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/last_match.json", json);
        }
        
        #endregion
    }
    
    [System.Serializable]
    public class PlayerBattleStats
    {
        public int playerId;
        public string playerName;
        public string heroName;
        public int team;
        
        public int kills = 0;
        public int deaths = 0;
        public int assists = 0;
        public int killStreak = 0;
        public int maxKillStreak = 0;
        public bool firstBlood = false;
        public bool doubleKill = false;
        public bool tripleKill = false;
        public bool quadraKill = false;
        public bool pentaKill = false;
        
        public int damageDealt = 0;
        public int damageTaken = 0;
        public int healingDone = 0;
        public int goldEarned = 0;
        public int minionsKilled = 0;
        public int towersDestroyed = 0;
        
        public float GetKDA()
        {
            if (deaths == 0)
                return kills + assists;
            return (kills + (float)assists) / deaths;
        }
        
        public int GetTotalScore()
        {
            int score = kills * 3;
            score += assists;
            score -= deaths;
            score += damageDealt / 1000;
            score += goldEarned / 1000;
            score += towersDestroyed * 5;
            
            if (firstBlood) score += 10;
            if (doubleKill) score += 5;
            if (tripleKill) score += 10;
            if (quadraKill) score += 20;
            if (pentaKill) score += 50;
            
            return score;
        }
    }
    
    [System.Serializable]
    public class TeamBattleStats
    {
        public int teamId;
        public int kills = 0;
        public int towersDestroyed = 0;
        public int dragonsSlain = 0;
        public int baronsSlain = 0;
        public int totalGold = 0;
    }
    
    [System.Serializable]
    public class BattleReport
    {
        public float gameDuration;
        public int totalKills;
        public int totalDeaths;
        public int totalAssists;
        public int totalGold;
        public int totalDamage;
        public List<PlayerBattleStats> playerStats;
        public PlayerBattleStats mvp;
    }
}
