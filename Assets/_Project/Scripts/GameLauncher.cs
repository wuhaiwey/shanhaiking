using UnityEngine;
using UnityEngine.UI;

namespace ShanHaiKing
{
    /// <summary>
    /// 游戏启动器 - 创建所有必要的游戏对象
    /// </summary>
    public class GameLauncher : MonoBehaviour
    {
        [Header("游戏 prefab")]
        public GameObject playerPrefab;
        public GameObject joystickPrefab;
        
        void Start()
        {
            CreateGameWorld();
        }
        
        void CreateGameWorld()
        {
            // 1. 创建地面
            CreateGround();
            
            // 2. 创建玩家
            CreatePlayer();
            
            // 3. 设置摄像机
            SetupCamera();
            
            // 4. 创建 UI
            CreateUI();
            
            // 5. 创建光照
            CreateLighting();
            
            Debug.Log("游戏世界创建完成！");
        }
        
        void CreateGround()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(10, 1, 10);
            
            // 创建材质
            Material groundMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            groundMat.color = new Color(0.2f, 0.3f, 0.2f);
            ground.GetComponent<Renderer>().material = groundMat;
        }
        
        void CreatePlayer()
        {
            // 创建一个简单的玩家胶囊体
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.tag = "Player";
            player.transform.position = new Vector3(0, 1, 0);
            
            // 添加刚体
            Rigidbody rb = player.AddComponent<Rigidbody>();
            rb.freezeRotation = true;
            
            // 添加角色控制器脚本
            player.AddComponent<SimplePlayerController>();
            
            // 创建材质
            Material playerMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            playerMat.color = Color.red;
            player.GetComponent<Renderer>().material = playerMat;
        }
        
        void SetupCamera()
        {
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject camObj = new GameObject("MainCamera");
                mainCam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
            }
            
            // 设置摄像机位置和角度
            mainCam.transform.position = new Vector3(0, 10, -10);
            mainCam.transform.rotation = Quaternion.Euler(45, 0, 0);
            
            // 添加摄像机跟随脚本
            mainCam.gameObject.AddComponent<CameraFollow>();
        }
        
        void CreateUI()
        {
            // 创建 Canvas
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 创建移动摇杆（左下角）
            CreateJoystick(canvas.transform);
            
            // 创建技能按钮（右下角）
            CreateSkillButtons(canvas.transform);
            
            // 创建血条（顶部）
            CreateHealthBar(canvas.transform);
        }
        
        void CreateJoystick(Transform canvas)
        {
            // 摇杆背景
            GameObject joystickBg = new GameObject("JoystickBG");
            joystickBg.transform.SetParent(canvas);
            RectTransform bgRect = joystickBg.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 0);
            bgRect.anchorMax = new Vector2(0, 0);
            bgRect.pivot = new Vector2(0, 0);
            bgRect.anchoredPosition = new Vector2(50, 50);
            bgRect.sizeDelta = new Vector2(150, 150);
            
            Image bgImage = joystickBg.AddComponent<Image>();
            bgImage.color = new Color(1, 1, 1, 0.3f);
            
            // 摇杆柄
            GameObject joystickHandle = new GameObject("JoystickHandle");
            joystickHandle.transform.SetParent(joystickBg.transform);
            RectTransform handleRect = joystickHandle.AddComponent<RectTransform>();
            handleRect.anchorMin = new Vector2(0.5f, 0.5f);
            handleRect.anchorMax = new Vector2(0.5f, 0.5f);
            handleRect.pivot = new Vector2(0.5f, 0.5f);
            handleRect.anchoredPosition = Vector2.zero;
            handleRect.sizeDelta = new Vector2(60, 60);
            
            Image handleImage = joystickHandle.AddComponent<Image>();
            handleImage.color = new Color(1, 1, 1, 0.8f);
        }
        
        void CreateSkillButtons(Transform canvas)
        {
            string[] skillNames = { "Q", "W", "E", "R" };
            Color[] skillColors = { Color.red, Color.blue, Color.green, Color.yellow };
            
            for (int i = 0; i < 4; i++)
            {
                GameObject btn = new GameObject($"Skill_{skillNames[i]}");
                btn.transform.SetParent(canvas);
                
                RectTransform rect = btn.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(1, 0);
                rect.anchoredPosition = new Vector2(-50 - i * 90, 50);
                rect.sizeDelta = new Vector2(80, 80);
                
                Image img = btn.AddComponent<Image>();
                img.color = skillColors[i];
                
                // 添加文字
                GameObject txtObj = new GameObject("Text");
                txtObj.transform.SetParent(btn.transform);
                Text txt = txtObj.AddComponent<Text>();
                txt.text = skillNames[i];
                txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                txt.fontSize = 32;
                txt.color = Color.white;
                txt.alignment = TextAnchor.MiddleCenter;
                
                RectTransform txtRect = txtObj.GetComponent<RectTransform>();
                txtRect.anchorMin = Vector2.zero;
                txtRect.anchorMax = Vector2.one;
                txtRect.offsetMin = Vector2.zero;
                txtRect.offsetMax = Vector2.zero;
            }
        }
        
        void CreateHealthBar(Transform canvas)
        {
            // 血条背景
            GameObject healthBg = new GameObject("HealthBar_BG");
            healthBg.transform.SetParent(canvas);
            
            RectTransform bgRect = healthBg.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0.5f, 1);
            bgRect.anchorMax = new Vector2(0.5f, 1);
            bgRect.pivot = new Vector2(0.5f, 1);
            bgRect.anchoredPosition = new Vector2(0, -20);
            bgRect.sizeDelta = new Vector2(300, 30);
            
            Image bgImg = healthBg.AddComponent<Image>();
            bgImg.color = Color.black;
            
            // 血条前景
            GameObject healthFill = new GameObject("HealthBar_Fill");
            healthFill.transform.SetParent(healthBg.transform);
            
            RectTransform fillRect = healthFill.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(2, 2);
            fillRect.offsetMax = new Vector2(-2, -2);
            
            Image fillImg = healthFill.AddComponent<Image>();
            fillImg.color = Color.red;
        }
        
        void CreateLighting()
        {
            // 主光源
            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.5f;
            light.color = Color.white;
            lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);
            
            // 环境光
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.3f, 0.3f, 0.4f);
        }
    }
    
    /// <summary>
    /// 简单的玩家控制器
    /// </summary>    public class SimplePlayerController : MonoBehaviour
    {
        public float moveSpeed = 10f;
        public float rotationSpeed = 10f;
        
        private Rigidbody rb;
        private Vector3 moveDirection;
        
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }
        
        void Update()
        {
            // 获取输入
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            
            // 计算移动方向
            moveDirection = new Vector3(h, 0, v).normalized;
            
            // 旋转朝向
            if (moveDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        
        void FixedUpdate()
        {
            // 移动
            rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
        }
    }
    
    /// <summary>
    /// 摄像机跟随
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0, 10, -10);
        public float smoothSpeed = 5f;
        
        void LateUpdate()
        {
            if (target == null)
            {
                // 自动查找玩家
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null) target = player.transform;
                return;
            }
            
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
            
            transform.LookAt(target);
        }
    }
}
