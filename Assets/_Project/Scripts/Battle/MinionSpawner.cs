using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ShanHaiKing.Battle
{
    /// <summary>
    /// 小兵生成管理器
    /// </summary>
    public class MinionSpawner : MonoBehaviour
    {
        [Header("生成设置")]
        public GameObject minionPrefab;
        public float spawnInterval = 30f; // 每30秒一波
        public int minionsPerWave = 6; // 每波6个小兵
        
        [Header("生成点")]
        public Transform blueSpawnPoint;
        public Transform redSpawnPoint;
        
        [Header("路点")]
        public Transform[] blueWaypoints; // 蓝方路线
        public Transform[] redWaypoints;  // 红方路线
        
        [Header("游戏时间")]
        public float gameTime = 0f;
        public int currentWave = 0;
        
        private List<MinionAI> activeMinions = new List<MinionAI>();
        
        void Start()
        {
            // 游戏开始30秒后第一波
            Invoke(nameof(SpawnWave), 30f);
        }
        
        void Update()
        {
            gameTime += Time.deltaTime;
            
            // 清理已死亡的小兵
            activeMinions.RemoveAll(m => m == null);
        }
        
        void SpawnWave()
        {
            currentWave++;
            
            // 生成蓝方小兵
            StartCoroutine(SpawnTeamMinions(MinionAI.Team.Blue));
            
            // 生成红方小兵
            StartCoroutine(SpawnTeamMinions(MinionAI.Team.Red));
            
            // 下一波
            Invoke(nameof(SpawnWave), spawnInterval);
        }
        
        IEnumerator SpawnTeamMinions(MinionAI.Team team)
        {
            Transform spawnPoint = team == MinionAI.Team.Blue ? blueSpawnPoint : redSpawnPoint;
            Transform[] waypoints = team == MinionAI.Team.Blue ? blueWaypoints : redWaypoints;
            
            for (int i = 0; i < minionsPerWave; i++)
            {
                SpawnMinion(team, spawnPoint, waypoints);
                yield return new WaitForSeconds(0.5f); // 间隔生成
            }
        }
        
        void SpawnMinion(MinionAI.Team team, Transform spawnPoint, Transform[] waypoints)
        {
            if (minionPrefab == null)
            {
                // 如果没有预制体，创建默认小兵
                GameObject minion = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                minion.transform.position = spawnPoint.position + Random.insideUnitSphere * 2f;
                minion.transform.position = new Vector3(minion.transform.position.x, 1f, minion.transform.position.z);
                
                // 添加组件
                MinionAI ai = minion.AddComponent<MinionAI>();
                ai.team = team;
                ai.waypoints = waypoints;
                
                // 设置阵营颜色
                Renderer renderer = minion.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material mat = new Material(renderer.material);
                    mat.color = team == MinionAI.Team.Blue ? Color.blue : Color.red;
                    renderer.material = mat;
                }
                
                // 添加刚体
                Rigidbody rb = minion.AddComponent<Rigidbody>();
                rb.freezeRotation = true;
                
                // 添加碰撞器
                if (minion.GetComponent<Collider>() == null)
                {
                    minion.AddComponent<CapsuleCollider>();
                }
                
                activeMinions.Add(ai);
            }
            else
            {
                // 使用预制体
                GameObject minion = Instantiate(minionPrefab, 
                    spawnPoint.position + Random.insideUnitSphere * 2f, 
                    Quaternion.identity);
                
                MinionAI ai = minion.GetComponent<MinionAI>();
                if (ai != null)
                {
                    ai.team = team;
                    ai.waypoints = waypoints;
                    activeMinions.Add(ai);
                }
            }
        }
        
        /// <summary>
        /// 根据游戏时间增强小兵
        /// </summary>
        void EnhanceMinionsOverTime()
        {
            float timeMultiplier = 1f + (gameTime / 600f); // 每10分钟增强10%
            
            foreach (var minion in activeMinions)
            {
                if (minion != null)
                {
                    minion.attackDamage = 50f * timeMultiplier;
                    minion.maxHealth = 500f * timeMultiplier;
                }
            }
        }
        
        /// <summary>
        /// 清除所有小兵
        /// </summary>
        public void ClearAllMinions()
        {
            foreach (var minion in activeMinions)
            {
                if (minion != null)
                {
                    Destroy(minion.gameObject);
                }
            }
            activeMinions.Clear();
        }
        
        /// <summary>
        /// 获取某方小兵数量
        /// </summary>
        public int GetMinionCount(MinionAI.Team team)
        {
            int count = 0;
            foreach (var minion in activeMinions)
            {
                if (minion != null && minion.team == team)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
