using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Core
{
    /// <summary>
    /// 成就系统
    /// </summary>
    public class AchievementSystem : MonoBehaviour
    {
        public static AchievementSystem Instance { get; private set; }
        
        [Header("成就列表")]
        public List<Achievement> achievements = new List<Achievement>();
        
        [Header("玩家成就进度")]
        public Dictionary<string, int> achievementProgress = new Dictionary<string, int>();
        
        void Awake()
        {
            Instance = this;
            InitializeAchievements();
        }
        
        void InitializeAchievements()
        {
            // 战斗成就
            achievements.Add(new Achievement
            {
                id = "first_blood",
                name = "第一滴血",
                description = "获得首次击杀",
                targetValue = 1,
                reward = 100
            });
            
            achievements.Add(new Achievement
            {
                id = "killing_spree",
                name = "杀人如麻",
                description = "单局获得10次击杀",
                targetValue = 10,
                reward = 500
            });
            
            achievements.Add(new Achievement
            {
                id = "legendary",
                name = "超神",
                description = "单局获得20次击杀且不死",
                targetValue = 20,
                reward = 1000
            });
            
            // 英雄成就
            achievements.Add(new Achievement
            {
                id = "hero_collector",
                name = "英雄收藏家",
                description = "解锁10个英雄",
                targetValue = 10,
                reward = 2000
            });
            
            achievements.Add(new Achievement
            {
                id = "master_hero",
                name = "英雄大师",
                description = "将一个英雄练至满级",
                targetValue = 1,
                reward = 1500
            });
            
            // 对战成就
            achievements.Add(new Achievement
            {
                id = "veteran",
                name = "老兵",
                description = "完成100场对战",
                targetValue = 100,
                reward = 3000
            });
            
            achievements.Add(new Achievement
            {
                id = "champion",
                name = "冠军",
                description = "赢得50场排位赛",
                targetValue = 50,
                reward = 5000
            });
            
            // 经济成就
            achievements.Add(new Achievement
            {
                id = "wealthy",
                name = "富可敌国",
                description = "累计获得100000金币",
                targetValue = 100000,
                reward = 2000
            });
        }
        
        public void UpdateProgress(string achievementId, int amount)
        {
            if (!achievementProgress.ContainsKey(achievementId))
            {
                achievementProgress[achievementId] = 0;
            }
            
            achievementProgress[achievementId] += amount;
            
            // 检查是否完成
            CheckCompletion(achievementId);
        }
        
        void CheckCompletion(string achievementId)
        {
            Achievement achievement = achievements.Find(a => a.id == achievementId);
            if (achievement == null || achievement.isCompleted) return;
            
            int progress = achievementProgress.GetValueOrDefault(achievementId, 0);
            if (progress >= achievement.targetValue)
            {
                CompleteAchievement(achievement);
            }
        }
        
        void CompleteAchievement(Achievement achievement)
        {
            achievement.isCompleted = true;
            
            // 发放奖励
            Database.PlayerDatabase.Instance?.AddGold(achievement.reward);
            
            // 显示通知
            Debug.Log($"成就完成: {achievement.name}！获得 {achievement.reward} 金币");
            
            // 触发事件
            OnAchievementCompleted?.Invoke(achievement);
        }
        
        public delegate void AchievementHandler(Achievement achievement);
        public event AchievementHandler OnAchievementCompleted;
        
        public float GetProgressPercent(string achievementId)
        {
            Achievement achievement = achievements.Find(a => a.id == achievementId);
            if (achievement == null) return 0f;
            
            int progress = achievementProgress.GetValueOrDefault(achievementId, 0);
            return Mathf.Clamp01((float)progress / achievement.targetValue);
        }
    }
    
    [System.Serializable]
    public class Achievement
    {
        public string id;
        public string name;
        public string description;
        public int targetValue;
        public int reward;
        public bool isCompleted;
        public Sprite icon;
    }
}
