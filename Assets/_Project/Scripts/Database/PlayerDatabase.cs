using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ShanHaiKing.Database
{
    /// <summary>
    /// 玩家数据库系统 - 管理所有玩家数据
    /// </summary>
    public class PlayerDatabase : MonoBehaviour
    {
        public static PlayerDatabase Instance { get; private set; }
        
        [Header("玩家数据")]
        public PlayerData currentPlayer;
        public List<PlayerData> allPlayers = new List<PlayerData>();
        
        [Header("文件路径")]
        private string dataPath;
        
        void Awake()
        {
            Instance = this;
            dataPath = Application.persistentDataPath + "/playerdata.dat";
            LoadData();
        }
        
        void LoadData()
        {
            if (File.Exists(dataPath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(dataPath, FileMode.Open);
                
                allPlayers = formatter.Deserialize(stream) as List<PlayerData>;
                stream.Close();
                
                if (allPlayers.Count > 0)
                {
                    currentPlayer = allPlayers[0];
                }
            }
            else
            {
                CreateDefaultPlayer();
            }
        }
        
        void CreateDefaultPlayer()
        {
            currentPlayer = new PlayerData
            {
                playerId = System.Guid.NewGuid().ToString(),
                playerName = "Player",
                level = 1,
                experience = 0,
                gold = 1000,
                gems = 0,
                totalMatches = 0,
                wins = 0,
                losses = 0,
                kills = 0,
                deaths = 0,
                assists = 0,
                unlockedHeroes = new List<string> { "后羿" },
                ownedSkins = new List<string>(),
                achievements = new List<string>(),
                friends = new List<string>()
            };
            
            allPlayers.Add(currentPlayer);
            SaveData();
        }
        
        public void SaveData()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(dataPath, FileMode.Create);
            
            formatter.Serialize(stream, allPlayers);
            stream.Close();
        }
        
        public void AddExperience(int amount)
        {
            if (currentPlayer == null) return;
            
            currentPlayer.experience += amount;
            
            // 检查升级
            int expNeeded = GetExpForLevel(currentPlayer.level + 1);
            while (currentPlayer.experience >= expNeeded)
            {
                currentPlayer.experience -= expNeeded;
                currentPlayer.level++;
                OnLevelUp();
                expNeeded = GetExpForLevel(currentPlayer.level + 1);
            }
            
            SaveData();
        }
        
        void OnLevelUp()
        {
            Debug.Log($"升级！当前等级: {currentPlayer.level}");
            // 解锁新内容
            if (currentPlayer.level == 5)
            {
                UnlockHero("九尾狐");
            }
            else if (currentPlayer.level == 10)
            {
                UnlockHero("刑天");
            }
        }
        
        int GetExpForLevel(int level)
        {
            return level * 100 + (level - 1) * 50;
        }
        
        public void AddGold(int amount)
        {
            if (currentPlayer == null) return;
            currentPlayer.gold += amount;
            SaveData();
        }
        
        public void AddGems(int amount)
        {
            if (currentPlayer == null) return;
            currentPlayer.gems += amount;
            SaveData();
        }
        
        public void UnlockHero(string heroName)
        {
            if (currentPlayer == null) return;
            
            if (!currentPlayer.unlockedHeroes.Contains(heroName))
            {
                currentPlayer.unlockedHeroes.Add(heroName);
                SaveData();
                Debug.Log($"解锁英雄: {heroName}");
            }
        }
        
        public void RecordMatch(bool isWin, int kills, int deaths, int assists)
        {
            if (currentPlayer == null) return;
            
            currentPlayer.totalMatches++;
            if (isWin)
                currentPlayer.wins++;
            else
                currentPlayer.losses++;
            
            currentPlayer.kills += kills;
            currentPlayer.deaths += deaths;
            currentPlayer.assists += assists;
            
            // 计算评分
            int score = kills * 3 + assists - deaths;
            AddExperience(score * 10);
            
            SaveData();
        }
        
        public float GetWinRate()
        {
            if (currentPlayer == null || currentPlayer.totalMatches == 0)
                return 0f;
            return (float)currentPlayer.wins / currentPlayer.totalMatches * 100f;
        }
        
        public float GetKDA()
        {
            if (currentPlayer == null || currentPlayer.deaths == 0)
                return currentPlayer.kills + currentPlayer.assists;
            return (float)(currentPlayer.kills + currentPlayer.assists) / currentPlayer.deaths;
        }
    }
    
    [System.Serializable]
    public class PlayerData
    {
        public string playerId;
        public string playerName;
        public int level;
        public int experience;
        public int gold;
        public int gems;
        
        public int totalMatches;
        public int wins;
        public int losses;
        public int kills;
        public int deaths;
        public int assists;
        
        public List<string> unlockedHeroes;
        public List<string> ownedSkins;
        public List<string> achievements;
        public List<string> friends;
        
        public System.DateTime lastLogin;
        public System.DateTime accountCreated;
    }
}
