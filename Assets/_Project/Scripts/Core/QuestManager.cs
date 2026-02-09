using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Core
{
    /// <summary>
    /// 任务和成就系统
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }
        
        [Header("活跃任务")]
        public List<Quest> activeQuests = new List<Quest>();
        
        [Header("已完成任务")]
        public List<Quest> completedQuests = new List<Quest>();
        
        [Header("成就")]
        public List<Achievement> achievements = new List<Achievement>();
        
        void Awake()
        {
            Instance = this;
            InitializeQuests();
            InitializeAchievements();
        }
        
        void InitializeQuests()
        {
            // 新手任务
            activeQuests.Add(new Quest
            {
                questName = "初出茅庐",
                description = "完成第一场对战",
                questType = QuestType.Tutorial,
                targetValue = 1,
                currentValue = 0,
                rewardGold = 100,
                rewardExp = 50
            });
            
            activeQuests.Add(new Quest
            {
                questName = "首杀",
                description = "获得第一次击杀",
                questType = QuestType.Combat,
                targetValue = 1,
                currentValue = 0,
                rewardGold = 50,
                rewardExp = 30
            });
            
            activeQuests.Add(new Quest
            {
                questName = "推塔先锋",
                description = "摧毁3座防御塔",
                questType = QuestType.Objective,
                targetValue = 3,
                currentValue = 0,
                rewardGold = 200,
                rewardExp = 100
            });
            
            activeQuests.Add(new Quest
            {
                questName = "补刀大师",
                description = "补刀100个小兵",
                questType = QuestType.Combat,
                targetValue = 100,
                currentValue = 0,
                rewardGold = 150,
                rewardExp = 80
            });
            
            activeQuests.Add(new Quest
            {
                questName = "团战英雄",
                description = "参与10次团战",
                questType = QuestType.Combat,
                targetValue = 10,
                currentValue = 0,
                rewardGold = 300,
                rewardExp = 150
            });
        }
        
        void InitializeAchievements()
        {
            achievements.Add(new Achievement
            {
                achievementName = "百战百胜",
                description = "赢得100场对战",
                targetValue = 100,
                currentValue = 0,
                rewardTitle = "常胜将军"
            });
            
            achievements.Add(new Achievement
            {
                achievementName = "五杀传说",
                description = "获得一次五杀",
                targetValue = 1,
                currentValue = 0,
                rewardTitle = "收割者"
            });
            
            achievements.Add(new Achievement
            {
                achievementName = "英雄收集者",
                description = "解锁10个英雄",
                targetValue = 10,
                currentValue = 0,
                rewardTitle = "收藏家"
            });
            
            achievements.Add(new Achievement
            {
                achievementName = "富可敌国",
                description = "累计获得100000金币",
                targetValue = 100000,
                currentValue = 0,
                rewardTitle = "财神"
            });
            
            achievements.Add(new Achievement
            {
                achievementName = "神装达人",
                description = "完成6神装购买",
                targetValue = 100,
                currentValue = 0,
                rewardTitle = "装备大师"
            });
        }
        
        public void UpdateQuestProgress(string questName, int amount)
        {
            foreach (Quest quest in activeQuests)
            {
                if (quest.questName == questName && !quest.isCompleted)
                {
                    quest.currentValue += amount;
                    
                    if (quest.currentValue >= quest.targetValue)
                    {
                        CompleteQuest(quest);
                    }
                    break;
                }
            }
        }
        
        void CompleteQuest(Quest quest)
        {
            quest.isCompleted = true;
            activeQuests.Remove(quest);
            completedQuests.Add(quest);
            
            // 发放奖励
            ShopManager.Instance?.AddGold(quest.rewardGold);
            
            // 显示完成提示
            Debug.Log($"任务完成: {quest.questName}");
        }
        
        public void UpdateAchievement(string achievementName, int amount)
        {
            foreach (Achievement achievement in achievements)
            {
                if (achievement.achievementName == achievementName && !achievement.isUnlocked)
                {
                    achievement.currentValue += amount;
                    
                    if (achievement.currentValue >= achievement.targetValue)
                    {
                        UnlockAchievement(achievement);
                    }
                    break;
                }
            }
        }
        
        void UnlockAchievement(Achievement achievement)
        {
            achievement.isUnlocked = true;
            Debug.Log($"成就解锁: {achievement.achievementName} - {achievement.rewardTitle}");
        }
    }
    
    [System.Serializable]
    public class Quest
    {
        public string questName;
        public string description;
        public QuestType questType;
        public int targetValue;
        public int currentValue;
        public bool isCompleted;
        public int rewardGold;
        public int rewardExp;
    }
    
    public enum QuestType
    {
        Tutorial,
        Combat,
        Objective,
        Daily
    }
    
    [System.Serializable]
    public class Achievement
    {
        public string achievementName;
        public string description;
        public int targetValue;
        public int currentValue;
        public bool isUnlocked;
        public string rewardTitle;
    }
}
