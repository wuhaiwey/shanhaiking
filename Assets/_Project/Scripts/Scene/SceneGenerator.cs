using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace ShanHaiKing.Scene
{
    /// <summary>
    /// 程序化场景生成器
    /// </summary>
    public class SceneGenerator : MonoBehaviour
    {
        public static SceneGenerator Instance { get; private set; }
        
        [Header("地形设置")]
        public int terrainWidth = 100;
        public int terrainHeight = 100;
        public float terrainScale = 10f;
        public float heightMultiplier = 5f;
        
        [Header("地形材质")]
        public Material groundMaterial;
        public Material grassMaterial;
        public Material pathMaterial;
        public Material waterMaterial;
        
        [Header("地图元素")]
        public GameObject treePrefab;
        public GameObject rockPrefab;
        public GameObject grassPrefab;
        public GameObject bushPrefab;
        public GameObject flowerPrefab;
        
        [Header("地图装饰")]
        public GameObject[] ancientRuins;
        public GameObject[] totems;
        public GameObject[] stoneLanterns;
        
        [Header("环境设置")]
        public Light directionalLight;
        public Material skyboxMaterial;
        public Color ambientLightColor = new Color(0.8f, 0.8f, 0.9f);
        
        private Mesh terrainMesh;
        private GameObject terrainObject;
        
        void Awake()
        {
            Instance = this;
        }
        
        void Start()
        {
            GenerateCompleteScene();
        }
        
        public void GenerateCompleteScene()
        {
            Debug.Log("开始生成场景...");
            
            // 生成地形
            GenerateTerrain();
            
            // 生成路径
            GeneratePaths();
            
            // 生成河流
            GenerateRiver();
            
            // 生成植被
            GenerateVegetation();
            
            // 生成装饰
            GenerateDecorations();
            
            // 设置环境
            SetupEnvironment();
            
            Debug.Log("场景生成完成！");
        }
        
        void GenerateTerrain()
        {
            terrainObject = new GameObject("Terrain");
            
            MeshFilter meshFilter = terrainObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = terrainObject.AddComponent<MeshRenderer>();
            MeshCollider meshCollider = terrainObject.AddComponent<MeshCollider>();
            
            terrainMesh = new Mesh();
            terrainMesh.name = "ProceduralTerrain";
            
            // 生成顶点
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> triangles = new List<int>();
            
            int resolution = 50;
            float stepX = (float)terrainWidth / resolution;
            float stepZ = (float)terrainHeight / resolution;
            
            // 生成高度图
            float[,] heights = new float[resolution + 1, resolution + 1];
            for (int x = 0; x <= resolution; x++)
            {
                for (int z = 0; z <= resolution; z++)
                {
                    float worldX = x * stepX - terrainWidth / 2f;
                    float worldZ = z * stepZ - terrainHeight / 2f;
                    
                    // 使用Perlin噪声生成高度
                    float noise1 = Mathf.PerlinNoise(x * 0.1f, z * 0.1f);
                    float noise2 = Mathf.PerlinNoise(x * 0.3f + 100, z * 0.3f) * 0.5f;
                    float noise3 = Mathf.PerlinNoise(x * 0.05f + 200, z * 0.05f) * 0.25f;
                    
                    // 中间平坦（战场区域）
                    float distanceFromCenter = Mathf.Sqrt(worldX * worldX + worldZ * worldZ);
                    float centerFactor = Mathf.Clamp01(distanceFromCenter / 20f);
                    
                    heights[x, z] = (noise1 + noise2 + noise3) * heightMultiplier * centerFactor;
                    
                    vertices.Add(new Vector3(worldX, heights[x, z], worldZ));
                    uvs.Add(new Vector2((float)x / resolution, (float)z / resolution));
                }
            }
            
            // 生成三角形
            for (int x = 0; x < resolution; x++)
            {
                for (int z = 0; z < resolution; z++)
                {
                    int topLeft = x * (resolution + 1) + z;
                    int topRight = topLeft + 1;
                    int bottomLeft = (x + 1) * (resolution + 1) + z;
                    int bottomRight = bottomLeft + 1;
                    
                    triangles.Add(topLeft);
                    triangles.Add(bottomLeft);
                    triangles.Add(topRight);
                    
                    triangles.Add(topRight);
                    triangles.Add(bottomLeft);
                    triangles.Add(bottomRight);
                }
            }
            
            terrainMesh.vertices = vertices.ToArray();
            terrainMesh.uv = uvs.ToArray();
            terrainMesh.triangles = triangles.ToArray();
            terrainMesh.RecalculateNormals();
            terrainMesh.RecalculateBounds();
            
            meshFilter.mesh = terrainMesh;
            meshCollider.sharedMesh = terrainMesh;
            
            if (groundMaterial != null)
            {
                meshRenderer.material = groundMaterial;
            }
        }
        
        void GeneratePaths()
        {
            // 生成三路兵线
            CreatePath(new Vector3(-40, 0, 20), new Vector3(40, 0, -20), "TopLane");
            CreatePath(new Vector3(-40, 0, 0), new Vector3(40, 0, 0), "MidLane");
            CreatePath(new Vector3(-40, 0, -20), new Vector3(40, 0, 20), "BotLane");
        }
        
        void CreatePath(Vector3 start, Vector3 end, string pathName)
        {
            GameObject path = new GameObject(pathName);
            
            // 创建路径平面
            GameObject pathObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            pathObj.name = "PathMesh";
            pathObj.transform.SetParent(path.transform);
            
            Vector3 direction = end - start;
            float length = direction.magnitude;
            
            pathObj.transform.position = (start + end) / 2f;
            pathObj.transform.rotation = Quaternion.LookRotation(direction);
            pathObj.transform.localScale = new Vector3(0.6f, 1, length / 10f);
            
            if (pathMaterial != null)
            {
                pathObj.GetComponent<Renderer>().material = pathMaterial;
            }
            
            Destroy(pathObj.GetComponent<Collider>());
        }
        
        void GenerateRiver()
        {
            // 生成弯曲的河流
            GameObject river = new GameObject("River");
            
            int riverPoints = 20;
            Vector3[] riverPath = new Vector3[riverPoints];
            
            for (int i = 0; i < riverPoints; i++)
            {
                float t = (float)i / (riverPoints - 1);
                float x = Mathf.Lerp(-50, 50, t);
                float z = Mathf.Sin(t * Mathf.PI * 2) * 15f;
                
                riverPath[i] = new Vector3(x, -1f, z);
            }
            
            // 创建河流网格
            CreateRiverMesh(riverPath, river);
        }
        
        void CreateRiverMesh(Vector3[] path, GameObject parent)
        {
            for (int i = 0; i < path.Length - 1; i++)
            {
                GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                segment.name = $"RiverSegment_{i}";
                segment.transform.SetParent(parent.transform);
                
                Vector3 midPoint = (path[i] + path[i + 1]) / 2f;
                float length = Vector3.Distance(path[i], path[i + 1]);
                
                segment.transform.position = midPoint;
                segment.transform.rotation = Quaternion.LookRotation(path[i + 1] - path[i]) * Quaternion.Euler(90, 0, 0);
                segment.transform.localScale = new Vector3(4f, length / 2f, 0.5f);
                
                if (waterMaterial != null)
                {
                    segment.GetComponent<Renderer>().material = waterMaterial;
                }
                
                Destroy(segment.GetComponent<Collider>());
            }
        }
        
        void GenerateVegetation()
        {
            // 在野区生成树木
            GenerateTreesInArea(new Vector3(-30, 0, 20), 15, 10);
            GenerateTreesInArea(new Vector3(30, 0, -20), 15, 10);
            GenerateTreesInArea(new Vector3(-35, 0, 10), 10, 8);
            GenerateTreesInArea(new Vector3(35, 0, -10), 10, 8);
            
            // 生成草丛
            GenerateBushes();
        }
        
        void GenerateTreesInArea(Vector3 center, float radius, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 randomCircle = Random.insideUnitCircle * radius;
                Vector3 position = center + new Vector3(randomCircle.x, 0, randomCircle.y);
                
                if (treePrefab != null)
                {
                    GameObject tree = Instantiate(treePrefab, position, Quaternion.Euler(0, Random.Range(0, 360), 0));
                    tree.transform.localScale = Vector3.one * Random.Range(0.8f, 1.5f);
                }
            }
        }
        
        void GenerateBushes()
        {
            // 在地图边缘生成草丛
            for (int i = 0; i < 30; i++)
            {
                Vector3 position = new Vector3(
                    Random.Range(-45, 45),
                    0,
                    Random.Range(-45, 45)
                );
                
                // 避开路径
                if (Mathf.Abs(position.z) > 5 || Mathf.Abs(position.x) > 40)
                {
                    if (bushPrefab != null)
                    {
                        Instantiate(bushPrefab, position, Quaternion.identity);
                    }
                }
            }
        }
        
        void GenerateDecorations()
        {
            // 在基地生成装饰
            GenerateBaseDecorations(new Vector3(-40, 0, 0), MinionAI.Team.Blue);
            GenerateBaseDecorations(new Vector3(40, 0, 0), MinionAI.Team.Red);
            
            // 生成古代遗迹
            GenerateAncientRuins();
            
            // 生成石灯笼
            GenerateStoneLanterns();
        }
        
        void GenerateBaseDecorations(Vector3 basePosition, MinionAI.Team team)
        {
            // 基地中心建筑
            GameObject nexus = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            nexus.name = $"Nexus_{team}";
            nexus.transform.position = basePosition + Vector3.up * 2f;
            nexus.transform.localScale = new Vector3(6f, 4f, 6f);
            
            Renderer renderer = nexus.GetComponent<Renderer>();
            renderer.material.color = team == MinionAI.Team.Blue ? Color.blue : Color.red;
            
            // 基地周围的装饰
            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60f * Mathf.Deg2Rad;
                Vector3 pos = basePosition + new Vector3(Mathf.Cos(angle) * 10, 0, Mathf.Sin(angle) * 10);
                
                if (totems.Length > 0)
                {
                    GameObject totem = Instantiate(totems[Random.Range(0, totems.Length)], pos, Quaternion.identity);
                    totem.transform.Rotate(0, Random.Range(0, 360), 0);
                }
            }
        }
        
        void GenerateAncientRuins()
        {
            // 在野区生成古代遗迹
            Vector3[] ruinPositions = new Vector3[]
            {
                new Vector3(-25, 0, 25),
                new Vector3(25, 0, -25),
                new Vector3(-15, 0, -15),
                new Vector3(15, 0, 15)
            };
            
            foreach (Vector3 pos in ruinPositions)
            {
                if (ancientRuins.Length > 0)
                {
                    Instantiate(ancientRuins[Random.Range(0, ancientRuins.Length)], pos, Quaternion.Euler(0, Random.Range(0, 360), 0));
                }
            }
        }
        
        void GenerateStoneLanterns()
        {
            // 沿着路径生成石灯笼
            for (int i = 1; i < 10; i++)
            {
                float t = (float)i / 10f;
                
                // 上路
                Vector3 topLanePos = Vector3.Lerp(new Vector3(-40, 0, 20), new Vector3(40, 0, -20), t);
                topLanePos += new Vector3(0, 0, 4);
                
                // 下路
                Vector3 botLanePos = Vector3.Lerp(new Vector3(-40, 0, -20), new Vector3(40, 0, 20), t);
                botLanePos += new Vector3(0, 0, -4);
                
                if (stoneLanterns.Length > 0)
                {
                    Instantiate(stoneLanterns[Random.Range(0, stoneLanterns.Length)], topLanePos, Quaternion.identity);
                    Instantiate(stoneLanterns[Random.Range(0, stoneLanterns.Length)], botLanePos, Quaternion.identity);
                }
            }
        }
        
        void SetupEnvironment()
        {
            // 设置主光源
            if (directionalLight == null)
            {
                GameObject lightObj = new GameObject("DirectionalLight");
                directionalLight = lightObj.AddComponent<Light>();
                directionalLight.type = LightType.Directional;
            }
            
            directionalLight.transform.rotation = Quaternion.Euler(50, -30, 0);
            directionalLight.intensity = 1.2f;
            directionalLight.color = new Color(1, 0.95f, 0.8f);
            
            // 设置环境光
            RenderSettings.ambientLight = ambientLightColor;
            RenderSettings.ambientIntensity = 1.0f;
            
            // 设置天空盒
            if (skyboxMaterial != null)
            {
                RenderSettings.skybox = skyboxMaterial;
            }
        }
    }
}
