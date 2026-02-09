using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Battle
{
    /// <summary>
    /// 游戏地图管理器 - 管理三条兵线
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }
        
        [Header("地图设置")]
        public float mapWidth = 100f;
        public float mapHeight = 100f;
        public Vector3 blueBasePosition = new Vector3(-40, 0, 0);
        public Vector3 redBasePosition = new Vector3(40, 0, 0);
        
        [Header("三路设置")]
        public Lane topLane;
        public Lane midLane;
        public Lane botLane;
        
        [Header("防御塔设置")]
        public List<TowerController> blueTowers = new List<TowerController>();
        public List<TowerController> redTowers = new List<TowerController>();
        
        [Header("野区")]
        public List<Transform> blueJungleCamps = new List<Transform>();
        public List<Transform> redJungleCamps = new List<Transform>();
        
        void Awake()
        {
            Instance = this;
        }
        
        void Start()
        {
            InitializeLanes();
            InitializeTowers();
        }
        
        void InitializeLanes()
        {
            // 上路
            topLane = new Lane
            {
                name = "TopLane",
                waypoints = new Transform[5],
                laneType = LaneType.Top
            };
            
            // 中路
            midLane = new Lane
            {
                name = "MidLane",
                waypoints = new Transform[5],
                laneType = LaneType.Mid
            };
            
            // 下路
            botLane = new Lane
            {
                name = "BotLane",
                waypoints = new Transform[5],
                laneType = LaneType.Bot
            };
        }
        
        void InitializeTowers()
        {
            // 初始化蓝方防御塔
            for (int i = 0; i < 3; i++)
            {
                Vector3 pos = Vector3.Lerp(blueBasePosition, Vector3.zero, (i + 1) * 0.25f);
                CreateTower(pos, MinionAI.Team.Blue, i);
            }
            
            // 初始化红方防御塔
            for (int i = 0; i < 3; i++)
            {
                Vector3 pos = Vector3.Lerp(redBasePosition, Vector3.zero, (i + 1) * 0.25f);
                CreateTower(pos, MinionAI.Team.Red, i);
            }
        }
        
        void CreateTower(Vector3 position, MinionAI.Team team, int index)
        {
            GameObject towerObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            towerObj.name = $"Tower_{team}_{index}";
            towerObj.transform.position = position;
            towerObj.transform.localScale = new Vector3(2, 5, 2);
            
            TowerController tower = towerObj.AddComponent<TowerController>();
            tower.team = team;
            
            // 设置颜色
            Renderer renderer = towerObj.GetComponent<Renderer>();
            Material mat = new Material(renderer.material);
            mat.color = team == MinionAI.Team.Blue ? Color.blue : Color.red;
            renderer.material = mat;
            
            if (team == MinionAI.Team.Blue)
                blueTowers.Add(tower);
            else
                redTowers.Add(tower);
        }
        
        /// <summary>
        /// 获取某方的基地位置
        /// </summary>
        public Vector3 GetBasePosition(MinionAI.Team team)
        {
            return team == MinionAI.Team.Blue ? blueBasePosition : redBasePosition;
        }
        
        /// <summary>
        /// 检查位置是否在地图范围内
        /// </summary>
        public bool IsValidPosition(Vector3 position)
        {
            return Mathf.Abs(position.x) <= mapWidth / 2f && 
                   Mathf.Abs(position.z) <= mapHeight / 2f;
        }
        
        /// <summary>
        /// 获取最近的防御塔
        /// </summary>
        public TowerController GetNearestTower(Vector3 position, MinionAI.Team team)
        {
            List<TowerController> towers = team == MinionAI.Team.Blue ? blueTowers : redTowers;
            TowerController nearest = null;
            float minDistance = float.MaxValue;
            
            foreach (TowerController tower in towers)
            {
                if (tower == null) continue;
                float distance = Vector3.Distance(position, tower.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = tower;
                }
            }
            
            return nearest;
        }
        
        void OnDrawGizmos()
        {
            // 绘制地图边界
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapWidth, 1, mapHeight));
            
            // 绘制三路
            if (topLane != null && topLane.waypoints != null)
                DrawLaneGizmos(topLane, Color.cyan);
            if (midLane != null && midLane.waypoints != null)
                DrawLaneGizmos(midLane, Color.white);
            if (botLane != null && botLane.waypoints != null)
                DrawLaneGizmos(botLane, Color.magenta);
        }
        
        void DrawLaneGizmos(Lane lane, Color color)
        {
            Gizmos.color = color;
            for (int i = 0; i < lane.waypoints.Length - 1; i++)
            {
                if (lane.waypoints[i] != null && lane.waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(lane.waypoints[i].position, lane.waypoints[i + 1].position);
                }
            }
        }
    }
    
    [System.Serializable]
    public class Lane
    {
        public string name;
        public Transform[] waypoints;
        public LaneType laneType;
        public MinionAI.Team pushedBy = MinionAI.Team.Blue; // 哪方推线优势
        public float pushAdvantage = 0f; // 推线优势值
    }
    
    public enum LaneType
    {
        Top,
        Mid,
        Bot
    }
}
