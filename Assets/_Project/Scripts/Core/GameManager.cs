using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Core
{
    /// <summary>
    /// 游戏管理器 - 单例模式
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("游戏状态")]
        public GameState currentState = GameState.MainMenu;
        public float gameTime = 0f;
        
        [Header("玩家信息")]
        public Hero.HeroBase localPlayer;
        public List<Hero.HeroBase> allPlayers = new List<Hero.HeroBase>();
        
        [Header("队伍分数")]
        public int blueTeamScore = 0;
        public int redTeamScore = 0;
        
        [Header("事件")]
        public System.Action OnGameStart;
        public System.Action OnGameEnd;
        public System.Action<GameState> OnGameStateChanged;
        
        void Awake()
        {
            // 单例模式
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        void Update()
        {
            if (currentState == GameState.Playing)
            {
                gameTime += Time.deltaTime;
            }
        }
        
        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            SetGameState(GameState.Playing);
            gameTime = 0f;
            OnGameStart?.Invoke();
        }
        
        /// <summary>
        /// 结束游戏
        /// </summary>
        public void EndGame(bool blueTeamWins)
        {
            SetGameState(GameState.GameOver);
            OnGameEnd?.Invoke();
            
            // TODO: 显示结算界面
        }
        
        /// <summary>
        /// 设置游戏状态
        /// </summary>
        public void SetGameState(GameState newState)
        {
            currentState = newState;
            OnGameStateChanged?.Invoke(newState);
        }
        
        /// <summary>
        /// 注册玩家
        /// </summary>
        public void RegisterPlayer(Hero.HeroBase player)
        {
            if (!allPlayers.Contains(player))
            {
                allPlayers.Add(player);
            }
        }
        
        /// <summary>
        /// 移除玩家
        /// </summary>
        public void RemovePlayer(Hero.HeroBase player)
        {
            allPlayers.Remove(player);
        }
        
        /// <summary>
        /// 获取格式化游戏时间
        /// </summary>
        public string GetFormattedTime()
        {
            int minutes = Mathf.FloorToInt(gameTime / 60);
            int seconds = Mathf.FloorToInt(gameTime % 60);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    
    /// <summary>
    /// 游戏状态枚举
    /// </summary>
    public enum GameState
    {
        MainMenu,       // 主菜单
        HeroSelect,     // 英雄选择
        Loading,        // 加载中
        Playing,        // 游戏中
        Paused,         // 暂停
        GameOver        // 游戏结束
    }
}
