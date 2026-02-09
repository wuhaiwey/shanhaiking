using UnityEngine;
using UnityEngine.EventSystems;

namespace ShanHaiKing.Core
{
    /// <summary>
    /// 虚拟摇杆控制器
    /// </summary>
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("UI组件")]
        public RectTransform joystickBackground;
        public RectTransform joystickHandle;
        
        [Header("设置")]
        public float maxRange = 100f; // 摇杆最大移动范围
        public bool isFixed = true;   // 是否固定位置
        
        [Header("输出")]
        public Vector2 inputDirection { get; private set; }
        public bool isDragging { get; private set; }
        
        private Vector2 originPosition;
        private Vector2 currentPointerPosition;
        
        void Start()
        {
            originPosition = joystickBackground.position;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
            
            if (!isFixed)
            {
                // 跟随手指位置
                joystickBackground.position = eventData.position;
            }
            
            OnDrag(eventData);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;
            
            currentPointerPosition = eventData.position;
            Vector2 direction = currentPointerPosition - (Vector2)joystickBackground.position;
            
            // 限制范围
            if (direction.magnitude > maxRange)
            {
                direction = direction.normalized * maxRange;
            }
            
            // 移动摇杆柄
            joystickHandle.position = (Vector2)joystickBackground.position + direction;
            
            // 计算输入方向（-1 到 1）
            inputDirection = direction / maxRange;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
            inputDirection = Vector2.zero;
            
            // 复位
            if (!isFixed)
            {
                joystickBackground.position = originPosition;
            }
            joystickHandle.position = joystickBackground.position;
        }
    }
}
