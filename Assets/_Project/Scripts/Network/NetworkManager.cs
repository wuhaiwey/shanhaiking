using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Network
{
    /// <summary>
    /// 网络同步管理器 - Photon PUN2 准备
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }
        
        [Header("网络设置")]
        public bool isOnline = false;
        public bool isHost = false;
        public string roomName = "";
        public int maxPlayers = 10;
        
        [Header("玩家列表")]
        public List<NetworkPlayer> connectedPlayers = new List<NetworkPlayer>();
        
        [Header("延迟信息")]
        public int ping = 0;
        public float serverTime = 0f;
        
        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        // 连接到服务器
        public void ConnectToServer()
        {
            Debug.Log("连接到服务器...");
            // PhotonNetwork.ConnectUsingSettings();
            isOnline = true;
        }
        
        // 创建房间
        public void CreateRoom(string name)
        {
            roomName = name;
            isHost = true;
            Debug.Log($"创建房间: {name}");
            // PhotonNetwork.CreateRoom(name, new RoomOptions { MaxPlayers = (byte)maxPlayers });
        }
        
        // 加入房间
        public void JoinRoom(string name)
        {
            roomName = name;
            isHost = false;
            Debug.Log($"加入房间: {name}");
            // PhotonNetwork.JoinRoom(name);
        }
        
        // 离开房间
        public void LeaveRoom()
        {
            Debug.Log("离开房间");
            // PhotonNetwork.LeaveRoom();
            isOnline = false;
            isHost = false;
            connectedPlayers.Clear();
        }
        
        // 同步玩家位置
        public void SyncPlayerPosition(int playerId, Vector3 position, Quaternion rotation)
        {
            // 发送位置数据到其他客户端
            // photonView.RPC("UpdatePlayerPosition", RpcTarget.Others, playerId, position, rotation);
        }
        
        // 同步玩家状态
        public void SyncPlayerState(int playerId, float health, float mana, int level)
        {
            // photonView.RPC("UpdatePlayerState", RpcTarget.Others, playerId, health, mana, level);
        }
        
        // 同步技能释放
        public void SyncSkillCast(int playerId, int skillIndex, Vector3 targetPos)
        {
            // photonView.RPC("CastSkill", RpcTarget.Others, playerId, skillIndex, targetPos);
        }
        
        // 同步伤害
        public void SyncDamage(int attackerId, int targetId, float damage)
        {
            // photonView.RPC("ApplyDamage", RpcTarget.All, attackerId, targetId, damage);
        }
        
        void Update()
        {
            if (isOnline)
            {
                // 更新延迟
                // ping = PhotonNetwork.GetPing();
                // serverTime = (float)PhotonNetwork.Time;
            }
        }
    }
    
    [System.Serializable]
    public class NetworkPlayer
    {
        public int playerId;
        public string playerName;
        public bool isLocal;
        public int team;
        public Hero.HeroBase hero;
        public int ping;
    }
}
