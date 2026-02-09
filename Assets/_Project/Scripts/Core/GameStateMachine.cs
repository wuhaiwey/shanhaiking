using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ShanHaiKing.Core
{
    /// <summary>
    /// 游戏状态机
    /// </summary>
    public class GameStateMachine : MonoBehaviour
    {
        public static GameStateMachine Instance { get; private set; }
        
        [Header("当前状态")]
        public GameState currentState = GameState.None;
        
        [Header("状态历史")]
        public List<GameState> stateHistory = new List<GameState>();
        
        [Header("状态计时器")]
        public float stateTimer = 0f;
        
        // 状态事件
        public System.Action<GameState, GameState> OnStateChanged;
        
        void Awake()
        {
            Instance = this;
        }
        
        void Update()
        {
            stateTimer += Time.deltaTime;
            UpdateCurrentState();
        }
        
        void UpdateCurrentState()
        {
            switch (currentState)
            {
                case GameState.Loading:
                    UpdateLoadingState();
                    break;
                case GameState.MainMenu:
                    UpdateMainMenuState();
                    break;
                case GameState.HeroSelect:
                    UpdateHeroSelectState();
                    break;
                case GameState.MatchMaking:
                    UpdateMatchMakingState();
                    break;
                case GameState.Playing:
                    UpdatePlayingState();
                    break;
                case GameState.Paused:
                    UpdatePausedState();
                    break;
                case GameState.GameOver:
                    UpdateGameOverState();
                    break;
            }
        }
        
        #region 状态更新
        
        void UpdateLoadingState()
        {
            // 加载进度检查
        }
        
        void UpdateMainMenuState()
        {
            // 主菜单逻辑
        }
        
        void UpdateHeroSelectState()
        {
            // 英雄选择逻辑
        }
        
        void UpdateMatchMakingState()
        {
            // 匹配逻辑
        }
        
        void UpdatePlayingState()
        {
            // 游戏进行中的逻辑
        }
        
        void UpdatePausedState()
        {
            // 暂停逻辑
        }
        
        void UpdateGameOverState()
        {
            // 游戏结束逻辑
        }
        
        #endregion
        
        #region 状态切换
        
        public void ChangeState(GameState newState)
        {
            if (currentState == newState) return;
            
            GameState previousState = currentState;
            
            // 退出当前状态
            ExitState(currentState);
            
            // 记录历史
            stateHistory.Add(currentState);
            if (stateHistory.Count > 10)
                stateHistory.RemoveAt(0);
            
            // 进入新状态
            currentState = newState;
            stateTimer = 0f;
            
            EnterState(newState);
            
            // 触发事件
            OnStateChanged?.Invoke(previousState, newState);
            
            Debug.Log($"游戏状态切换: {previousState} -> {newState}");
        }
        
        void EnterState(GameState state)
        {
            switch (state)
            {
                case GameState.Loading:
                    OnEnterLoading();
                    break;
                case GameState.MainMenu:
                    OnEnterMainMenu();
                    break;
                case GameState.HeroSelect:
                    OnEnterHeroSelect();
                    break;
                case GameState.MatchMaking:
                    OnEnterMatchMaking();
                    break;
                case GameState.Playing:
                    OnEnterPlaying();
                    break;
                case GameState.Paused:
                    OnEnterPaused();
                    break;
                case GameState.GameOver:
                    OnEnterGameOver();
                    break;
            }
        }
        
        void ExitState(GameState state)
        {
            switch (state)
            {
                case GameState.Loading:
                    OnExitLoading();
                    break;
                case GameState.MainMenu:
                    OnExitMainMenu();
                    break;
                case GameState.HeroSelect:
                    OnExitHeroSelect();
                    break;
                case GameState.MatchMaking:
                    OnExitMatchMaking();
                    break;
                case GameState.Playing:
                    OnExitPlaying();
                    break;
                case GameState.Paused:
                    OnExitPaused();
                    break;
                case GameState.GameOver:
                    OnExitGameOver();
                    break;
            }
        }
        
        #endregion
        
        #region 状态进入回调
        
        void OnEnterLoading()
        {
            Time.timeScale = 1f;
            // 显示加载界面
        }
        
        void OnEnterMainMenu()
        {
            Time.timeScale = 1f;
            // 加载主菜单场景
        }
        
        void OnEnterHeroSelect()
        {
            Time.timeScale = 1f;
            // 加载英雄选择场景
        }
        
        void OnEnterMatchMaking()
        {
            Time.timeScale = 1f;
            // 开始匹配
        }
        
        void OnEnterPlaying()
        {
            Time.timeScale = 1f;
            // 加载游戏场景
        }
        
        void OnEnterPaused()
        {
            Time.timeScale = 0f;
            // 显示暂停菜单
        }
        
        void OnEnterGameOver()
        {
            Time.timeScale = 0.5f;
            // 显示结果界面
        }
        
        #endregion
        
        #region 状态退出回调
        
        void OnExitLoading()
        {
            // 清理加载资源
        }
        
        void OnExitMainMenu()
        {
            // 保存设置
        }
        
        void OnExitHeroSelect()
        {
            // 记录选择
        }
        
        void OnExitMatchMaking()
        {
            // 停止匹配
        }
        
        void OnExitPlaying()
        {
            // 保存游戏数据
        }
        
        void OnExitPaused()
        {
            Time.timeScale = 1f;
        }
        
        void OnExitGameOver()
        {
            Time.timeScale = 1f;
        }
        
        #endregion
        
        #region 便捷方法
        
        public void GoToMainMenu()
        {
            ChangeState(GameState.MainMenu);
        }
        
        public void StartGame()
        {
            ChangeState(GameState.Playing);
        }
        
        public void PauseGame()
        {
            if (currentState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
            }
        }
        
        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
            }
        }
        
        public void EndGame(bool victory)
        {
            ChangeState(GameState.GameOver);
        }
        
        public bool IsPlaying()
        {
            return currentState == GameState.Playing;
        }
        
        public bool IsPaused()
        {
            return currentState == GameState.Paused;
        }
        
        #endregion
    }
    
    public enum GameState
    {
        None,
        Loading,
        MainMenu,
        HeroSelect,
        MatchMaking,
        Playing,
        Paused,
        GameOver
    }
}
