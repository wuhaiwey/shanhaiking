using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Effects
{
    /// <summary>
    /// 屏幕特效管理器
    /// </summary>
    public class ScreenEffectManager : MonoBehaviour
    {
        public static ScreenEffectManager Instance { get; private set; }
        
        [Header("屏幕效果材质")]
        public Material hurtMaterial;
        public Material healMaterial;
        public Material buffMaterial;
        public Material debuffMaterial;
        public Material ultimateMaterial;
        
        [Header("后处理设置")]
        public float effectDuration = 0.5f;
        public float fadeSpeed = 2f;
        
        private Dictionary<ScreenEffectType, float> activeEffects = new Dictionary<ScreenEffectType, float>();
        private Material currentMaterial;
        private float currentIntensity = 0f;
        
        void Awake()
        {
            Instance = this;
        }
        
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (currentMaterial != null && currentIntensity > 0.01f)
            {
                currentMaterial.SetFloat("_Intensity", currentIntensity);
                Graphics.Blit(source, destination, currentMaterial);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }
        
        void Update()
        {
            // 更新效果强度
            if (currentIntensity > 0)
            {
                currentIntensity -= fadeSpeed * Time.deltaTime;
                if (currentIntensity < 0) currentIntensity = 0;
            }
            
            // 处理活跃效果
            List<ScreenEffectType> toRemove = new List<ScreenEffectType>();
            foreach (var kvp in activeEffects)
            {
                float newDuration = kvp.Value - Time.deltaTime;
                if (newDuration <= 0)
                {
                    toRemove.Add(kvp.Key);
                }
                else
                {
                    activeEffects[kvp.Key] = newDuration;
                }
            }
            
            foreach (var effect in toRemove)
            {
                activeEffects.Remove(effect);
            }
        }
        
        public void PlayHurtEffect(float intensity = 1f)
        {
            currentMaterial = hurtMaterial;
            currentIntensity = intensity;
            activeEffects[ScreenEffectType.Hurt] = effectDuration;
        }
        
        public void PlayHealEffect(float intensity = 1f)
        {
            currentMaterial = healMaterial;
            currentIntensity = intensity;
            activeEffects[ScreenEffectType.Heal] = effectDuration;
        }
        
        public void PlayBuffEffect(float intensity = 1f)
        {
            currentMaterial = buffMaterial;
            currentIntensity = intensity;
            activeEffects[ScreenEffectType.Buff] = effectDuration;
        }
        
        public void PlayDebuffEffect(float intensity = 1f)
        {
            currentMaterial = debuffMaterial;
            currentIntensity = intensity;
            activeEffects[ScreenEffectType.Debuff] = effectDuration;
        }
        
        public void PlayUltimateEffect(float intensity = 1f)
        {
            currentMaterial = ultimateMaterial;
            currentIntensity = intensity;
            activeEffects[ScreenEffectType.Ultimate] = effectDuration * 2f;
        }
        
        public void ShakeScreen(float duration, float magnitude)
        {
            StartCoroutine(ScreenShake(duration, magnitude));
        }
        
        System.Collections.IEnumerator ScreenShake(float duration, float magnitude)
        {
            Vector3 originalPos = Camera.main.transform.position;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;
                
                Camera.main.transform.position = originalPos + new Vector3(x, y, 0);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            Camera.main.transform.position = originalPos;
        }
        
        public void FlashScreen(Color color, float duration)
        {
            StartCoroutine(ScreenFlash(color, duration));
        }
        
        System.Collections.IEnumerator ScreenFlash(Color color, float duration)
        {
            // 创建全屏闪光
            GameObject flashObj = new GameObject("ScreenFlash");
            SpriteRenderer renderer = flashObj.AddComponent<SpriteRenderer>();
            
            // 创建全屏白色精灵
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            renderer.sprite = sprite;
            renderer.sortingOrder = 9999;
            
            // 设置大小覆盖全屏
            float worldHeight = Camera.main.orthographicSize * 2f;
            float worldWidth = worldHeight * Camera.main.aspect;
            flashObj.transform.localScale = new Vector3(worldWidth, worldHeight, 1);
            flashObj.transform.position = Camera.main.transform.position + Vector3.forward;
            
            // 淡出
            float elapsed = 0f;
            Color startColor = color;
            
            while (elapsed < duration)
            {
                float alpha = 1f - (elapsed / duration);
                renderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            Destroy(flashObj);
        }
        
        public void SlowMotion(float slowFactor, float duration)
        {
            StartCoroutine(SlowMotionEffect(slowFactor, duration));
        }
        
        System.Collections.IEnumerator SlowMotionEffect(float slowFactor, float duration)
        {
            Time.timeScale = slowFactor;
            Time.fixedDeltaTime = 0.02f * slowFactor;
            
            yield return new WaitForSecondsRealtime(duration);
            
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }
        
        public void FreezeFrame(float duration)
        {
            StartCoroutine(FreezeEffect(duration));
        }
        
        System.Collections.IEnumerator FreezeEffect(float duration)
        {
            Time.timeScale = 0.001f;
            
            yield return new WaitForSecondsRealtime(duration);
            
            Time.timeScale = 1f;
        }
    }
    
    public enum ScreenEffectType
    {
        Hurt,
        Heal,
        Buff,
        Debuff,
        Ultimate,
        Stun,
        Silence
    }
}
