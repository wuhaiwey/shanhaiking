using UnityEngine;

namespace ShanHaiKing.Core
{
    /// <summary>
    /// 游戏摄像机控制器
    /// </summary>
    public class GameCameraController : MonoBehaviour
    {
        public static GameCameraController Instance { get; private set; }
        
        [Header("目标")]
        public Transform target;
        
        [Header("跟随设置")]
        public float followSpeed = 5f;
        public float targetHeight = 10f;
        public float targetDistance = 15f;
        public Vector3 offset = new Vector3(0, 15, -20);
        
        [Header("视角限制")]
        public float minZoom = 10f;
        public float maxZoom = 40f;
        public float currentZoom = 20f;
        
        [Header("边缘滚动")]
        public bool edgeScrolling = true;
        public float edgeSize = 50f;
        public float scrollSpeed = 20f;
        
        [Header("视角锁定")]
        public bool lockOnTarget = true;
        
        [Header("平滑设置")]
        public float smoothTime = 0.3f;
        private Vector3 velocity = Vector3.zero;
        
        private Vector3 desiredPosition;
        
        void Awake()
        {
            Instance = this;
        }
        
        void LateUpdate()
        {
            if (target != null)
            {
                FollowTarget();
            }
            else
            {
                HandleFreeMovement();
            }
            
            HandleZoom();
        }
        
        void FollowTarget()
        {
            // 计算目标位置
            desiredPosition = target.position + offset;
            desiredPosition = AdjustForZoom(desiredPosition);
            
            // 平滑移动
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
            
            // 看向目标
            transform.LookAt(target.position + Vector3.up * targetHeight);
        }
        
        void HandleFreeMovement()
        {
            if (!edgeScrolling) return;
            
            Vector3 moveDirection = Vector3.zero;
            
            // 检测鼠标位置
            Vector2 mousePos = Input.mousePosition;
            
            if (mousePos.x < edgeSize)
                moveDirection.x = -1;
            else if (mousePos.x > Screen.width - edgeSize)
                moveDirection.x = 1;
                
            if (mousePos.y < edgeSize)
                moveDirection.z = -1;
            else if (mousePos.y > Screen.height - edgeSize)
                moveDirection.z = 1;
            
            if (moveDirection != Vector3.zero)
            {
                moveDirection.Normalize();
                Vector3 worldMove = transform.TransformDirection(moveDirection);
                worldMove.y = 0;
                
                transform.position += worldMove * scrollSpeed * Time.deltaTime;
            }
        }
        
        void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                currentZoom -= scroll * 10f;
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
                
                // 调整偏移量
                offset.y = currentZoom;
                offset.z = -currentZoom * 0.8f;
            }
        }
        
        Vector3 AdjustForZoom(Vector3 position)
        {
            // 根据缩放调整位置
            return position;
        }
        
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
        
        public void ShakeCamera(float duration, float magnitude)
        {
            StartCoroutine(Shake(duration, magnitude));
        }
        
        System.Collections.IEnumerator Shake(float duration, float magnitude)
        {
            Vector3 originalPos = transform.localPosition;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;
                
                transform.localPosition = originalPos + new Vector3(x, y, 0);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            transform.localPosition = originalPos;
        }
        
        public void FocusOnPosition(Vector3 position, float duration)
        {
            StartCoroutine(FocusRoutine(position, duration));
        }
        
        System.Collections.IEnumerator FocusRoutine(Vector3 position, float duration)
        {
            Vector3 originalOffset = offset;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                // 临时偏移到关注位置
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            offset = originalOffset;
        }
    }
}
