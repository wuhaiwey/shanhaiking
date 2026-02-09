using UnityEngine;
using System.Collections;

namespace ShanHaiKing.Network
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }
        
        [Header("网络设置")]
        public string serverIP = "127.0.0.1";
        public int serverPort = 7777;
        
        private bool isConnected = false;
        
        void Awake()
        {
            Instance = this;
        }
        
        public void ConnectToServer()
        {
            Debug.Log($"连接到服务器: {serverIP}:{serverPort}");
            // 实现网络连接逻辑
            isConnected = true;
        }
        
        public void Disconnect()
        {
            Debug.Log("断开连接");
            isConnected = false;
        }
        
        public void SendPlayerPosition(Vector3 position)
        {
            if (!isConnected) return;
            // 发送位置同步数据
        }
        
        public void SendSkillCast(int skillId, Vector3 targetPos)
        {
            if (!isConnected) return;
            Debug.Log($"释放技能 {skillId} 到 {targetPos}");
        }
    }
}
