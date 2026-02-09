using UnityEngine;

namespace ShanHaiKing.Core
{
    /// <summary>
    /// 游戏摄像机控制器
    /// </summary>
    public class GameCamera : MonoBehaviour
    {
        [Header("跟随目标")]
        public Transform target;
        
        [Header("跟随设置")]
        public Vector3 offset = new Vector3(0, 15, -10);
        public float followSpeed = 5f;
        public float rotationSpeed = 3f;
        
        [Header("视角设置")]
        public float minZoom = 5f;
        public float maxZoom = 25f;
        public float currentZoom = 15f;
        public float zoomSpeed = 2f;
        
        [Header("边界限制")]
        public bool useBounds = true;
        public Vector2 mapBounds = new Vector2(50, 50);
        
        private Camera cam;
        
        void Start()
        {
            cam = GetComponent<Camera>();
            if (cam == null) cam = Camera.main;
            
            // 初始位置
            if (target != null)
            {
                transform.position = target.position + offset;
                transform.LookAt(target);
            }
        }
        
        void LateUpdate()
        {
            if (target == null) return;
            
            HandleZoom();
            FollowTarget();
        }
        
        void FollowTarget()
        {
            // 计算目标位置
            Vector3 targetPosition = target.position + offset.normalized * currentZoom;
            
            // 边界限制
            if (useBounds)
            {
                targetPosition.x = Mathf.Clamp(targetPosition.x, -mapBounds.x, mapBounds.x);
                targetPosition.z = Mathf.Clamp(targetPosition.z, -mapBounds.y, mapBounds.y);
            }
            
            // 平滑跟随
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                followSpeed * Time.deltaTime
            );
            
            // 看向目标
            transform.LookAt(target.position + Vector3.up * 2f);
        }
        
        void HandleZoom()
        {
            // 鼠标滚轮缩放
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                currentZoom -= scroll * zoomSpeed * 10f;
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
            }
            
            // 双指缩放（移动端）
            if (Input.touchCount == 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);
                
                Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
                Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
                
                float prevDistance = Vector2.Distance(touch0PrevPos, touch1PrevPos);
                float currentDistance = Vector2.Distance(touch0.position, touch1.position);
                
                float deltaDistance = currentDistance - prevDistance;
                currentZoom -= deltaDistance * zoomSpeed * 0.01f;
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
            }
        }
        
        /// <summary>
        /// 设置跟随目标
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
        
        /// <summary>
        /// 震屏效果
        /// </summary>
        public void ShakeCamera(float duration, float intensity)
        {
            StartCoroutine(ShakeCoroutine(duration, intensity));
        }
        
        private System.Collections.IEnumerator ShakeCoroutine(float duration, float intensity)
        {
            Vector3 originalPosition = transform.localPosition;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * intensity;
                float y = Random.Range(-1f, 1f) * intensity;
                
                transform.localPosition = originalPosition + new Vector3(x, y, 0);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            transform.localPosition = originalPosition;
        }
    }
}
