using UnityEngine;
using System;
using System.Collections.Generic;

namespace ShanHaiKing.Core
{
    /// <summary>
    /// 全局事件系统
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }
        
        private Dictionary<GameEventType, Action<object[]>> eventDictionary = new Dictionary<GameEventType, Action<object[]>>();
        
        void Awake()
        {
            Instance = this;
        }
        
        /// <summary>
        /// 订阅事件
        /// </summary>
        public void Subscribe(GameEventType eventType, Action<object[]> callback)
        {
            if (!eventDictionary.ContainsKey(eventType))
            {
                eventDictionary[eventType] = callback;
            }
            else
            {
                eventDictionary[eventType] += callback;
            }
        }
        
        /// <summary>
        /// 取消订阅
        /// </summary>
        public void Unsubscribe(GameEventType eventType, Action<object[]> callback)
        {
            if (eventDictionary.ContainsKey(eventType))
            {
                eventDictionary[eventType] -= callback;
            }
        }
        
        /// <summary>
        /// 触发事件
        /// </summary>
        public void TriggerEvent(GameEventType eventType, params object[] parameters)
        {
            if (eventDictionary.ContainsKey(eventType))
            {
                eventDictionary[eventType]?.Invoke(parameters);
            }
        }
        
        #region 便捷方法
        
        public void OnPlayerKill(int killerId, int victimId, bool isFirstBlood)
        {
            TriggerEvent(GameEventType.PlayerKill, killerId, victimId, isFirstBlood);
        }
        
        public void OnTowerDestroyed(int destroyerId, MinionAI.Team team)
        {
            TriggerEvent(GameEventType.TowerDestroyed, destroyerId, team);
        }
        
        public void OnMinionKill(int killerId, int goldReward)
        {
            TriggerEvent(GameEventType.MinionKill, killerId, goldReward);
        }
        
        public void OnLevelUp(int playerId, int newLevel)
        {
            TriggerEvent(GameEventType.LevelUp, playerId, newLevel);
        }
        
        public void OnGameStart()
        {
            TriggerEvent(GameEventType.GameStart);
        }
        
        public void OnGameEnd(MinionAI.Team winner)
        {
            TriggerEvent(GameEventType.GameEnd, winner);
        }
        
        public void OnSkillCast(int playerId, string skillName)
        {
            TriggerEvent(GameEventType.SkillCast, playerId, skillName);
        }
        
        public void OnItemPurchase(int playerId, string itemName, int cost)
        {
            TriggerEvent(GameEventType.ItemPurchase, playerId, itemName, cost);
        }
        
        #endregion
    }
    
    public enum GameEventType
    {
        PlayerKill,
        MinionKill,
        TowerDestroyed,
        GameStart,
        GameEnd,
        LevelUp,
        SkillCast,
        ItemPurchase,
        BuffApplied,
        BuffRemoved,
        HeroSpawn,
        HeroDeath,
        WaveSpawn,
        NexusDestroyed
    }
}
