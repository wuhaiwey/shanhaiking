using UnityEngine;
using System;

namespace ShanHaiKing.Input
{
    /// <summary>
    /// 输入管理器 - 支持键盘/手柄/触屏
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        
        [Header("输入类型")]
        public InputType currentInputType = InputType.Keyboard;
        
        [Header("按键绑定")]
        public KeyBinding keyBinding;
        
        [Header("轴输入")]
        public float horizontal;
        public float vertical;
        
        [Header("鼠标位置")]
        public Vector3 mouseWorldPosition;
        public Vector2 mouseScreenPosition;
        
        // 事件定义
        public event Action OnAttackPressed;
        public event Action<int> OnSkillPressed;
        public event Action OnInteractPressed;
        public event Action OnRecallPressed;
        public event Action OnShopTogglePressed;
        public event Action OnScoreboardPressed;
        
        void Awake()
        {
            Instance = this;
            keyBinding = new KeyBinding();
        }
        
        void Update()
        {
            UpdateAxisInput();
            UpdateMousePosition();
            UpdateKeyInput();
            UpdateTouchInput();
        }
        
        void UpdateAxisInput()
        {
            switch (currentInputType)
            {
                case InputType.Keyboard:
                    horizontal = UnityEngine.Input.GetAxisRaw("Horizontal");
                    vertical = UnityEngine.Input.GetAxisRaw("Vertical");
                    break;
                    
                case InputType.Gamepad:
                    horizontal = UnityEngine.Input.GetAxis("Horizontal");
                    vertical = UnityEngine.Input.GetAxis("Vertical");
                    break;
                    
                case InputType.Touch:
                    // 虚拟摇杆处理
                    break;
            }
        }
        
        void UpdateMousePosition()
        {
            if (currentInputType == InputType.Keyboard)
            {
                mouseScreenPosition = UnityEngine.Input.mousePosition;
                
                Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
                {
                    mouseWorldPosition = hit.point;
                }
            }
        }
        
        void UpdateKeyInput()
        {
            if (currentInputType != InputType.Keyboard) return;
            
            // 技能键
            if (UnityEngine.Input.GetKeyDown(keyBinding.skill1)) OnSkillPressed?.Invoke(0);
            if (UnityEngine.Input.GetKeyDown(keyBinding.skill2)) OnSkillPressed?.Invoke(1);
            if (UnityEngine.Input.GetKeyDown(keyBinding.skill3)) OnSkillPressed?.Invoke(2);
            if (UnityEngine.Input.GetKeyDown(keyBinding.skill4)) OnSkillPressed?.Invoke(3);
            
            // 其他按键
            if (UnityEngine.Input.GetKeyDown(keyBinding.attack)) OnAttackPressed?.Invoke();
            if (UnityEngine.Input.GetKeyDown(keyBinding.interact)) OnInteractPressed?.Invoke();
            if (UnityEngine.Input.GetKeyDown(keyBinding.recall)) OnRecallPressed?.Invoke();
            if (UnityEngine.Input.GetKeyDown(keyBinding.shop)) OnShopTogglePressed?.Invoke();
            if (UnityEngine.Input.GetKeyDown(keyBinding.scoreboard)) OnScoreboardPressed?.Invoke();
        }
        
        void UpdateTouchInput()
        {
            if (currentInputType != InputType.Touch) return;
            
            // 处理触摸输入
            for (int i = 0; i < UnityEngine.Input.touchCount; i++)
            {
                Touch touch = UnityEngine.Input.GetTouch(i);
                
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        HandleTouchBegan(touch);
                        break;
                    case TouchPhase.Moved:
                        HandleTouchMoved(touch);
                        break;
                    case TouchPhase.Ended:
                        HandleTouchEnded(touch);
                        break;
                }
            }
        }
        
        void HandleTouchBegan(Touch touch)
        {
            // 触摸开始处理
        }
        
        void HandleTouchMoved(Touch touch)
        {
            // 触摸移动处理
        }
        
        void HandleTouchEnded(Touch touch)
        {
            // 触摸结束处理
        }
        
        public void SetInputType(InputType type)
        {
            currentInputType = type;
            Debug.Log($"输入类型切换为: {type}");
        }
    }
    
    public enum InputType
    {
        Keyboard,
        Gamepad,
        Touch
    }
    
    [System.Serializable]
    public class KeyBinding
    {
        public KeyCode skill1 = KeyCode.Q;
        public KeyCode skill2 = KeyCode.W;
        public KeyCode skill3 = KeyCode.E;
        public KeyCode skill4 = KeyCode.R;
        public KeyCode attack = KeyCode.A;
        public KeyCode interact = KeyCode.F;
        public KeyCode recall = KeyCode.B;
        public KeyCode shop = KeyCode.P;
        public KeyCode scoreboard = KeyCode.Tab;
        public KeyCode pause = KeyCode.Escape;
    }
}
