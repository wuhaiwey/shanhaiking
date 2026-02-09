using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace ShanHaiKing.UI
{
    /// <summary>
    /// 伤害数字浮动显示系统
    /// </summary>
    public class DamageTextManager : MonoBehaviour
    {
        public static DamageTextManager Instance { get; private set; }
        
        [Header("预制体")]
        public GameObject damageTextPrefab;
        public GameObject critDamageTextPrefab;
        public GameObject healTextPrefab;
        
        [Header("设置")]
        public int poolSize = 30;
        public float displayDuration = 1f;
        public float moveSpeed = 2f;
        public float spreadRadius = 0.5f;
        
        private Queue<GameObject> damageTextPool = new Queue<GameObject>();
        private Queue<GameObject> critTextPool = new Queue<GameObject>();
        private Queue<GameObject> healTextPool = new Queue<GameObject>();
        
        void Awake()
        {
            Instance = this;
            InitializePool();
        }
        
        void InitializePool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                CreateDamageText();
                CreateCritText();
                CreateHealText();
            }
        }
        
        void CreateDamageText()
        {
            GameObject obj = Instantiate(damageTextPrefab, transform);
            obj.SetActive(false);
            damageTextPool.Enqueue(obj);
        }
        
        void CreateCritText()
        {
            GameObject obj = Instantiate(critDamageTextPrefab, transform);
            obj.SetActive(false);
            critTextPool.Enqueue(obj);
        }
        
        void CreateHealText()
        {
            GameObject obj = Instantiate(healTextPrefab, transform);
            obj.SetActive(false);
            healTextPool.Enqueue(obj);
        }
        
        public void ShowDamage(Vector3 worldPosition, float damage, bool isCrit, Core.DamageType damageType)
        {
            GameObject textObj = isCrit ? critTextPool.Dequeue() : damageTextPool.Dequeue();
            
            textObj.SetActive(true);
            textObj.transform.position = worldPosition + Random.insideUnitSphere * spreadRadius;
            
            Text text = textObj.GetComponent<Text>();
            text.text = $"{damage:0}";
            
            // 根据伤害类型设置颜色
            switch (damageType)
            {
                case Core.DamageType.Physical:
                    text.color = Color.white;
                    break;
                case Core.DamageType.Magic:
                    text.color = new Color(0.5f, 0.2f, 1f); // 紫色
                    break;
                case Core.DamageType.True:
                    text.color = Color.yellow;
                    break;
            }
            
            // 暴击显示更大
            if (isCrit)
            {
                text.transform.localScale = Vector3.one * 1.5f;
            }
            else
            {
                text.transform.localScale = Vector3.one;
            }
            
            StartCoroutine(AnimateText(textObj, text, isCrit ? critTextPool : damageTextPool));
        }
        
        public void ShowHeal(Vector3 worldPosition, float healAmount)
        {
            GameObject textObj = healTextPool.Dequeue();
            
            textObj.SetActive(true);
            textObj.transform.position = worldPosition + Vector3.up * 0.5f;
            
            Text text = textObj.GetComponent<Text>();
            text.text = $"+{healAmount:0}";
            text.color = Color.green;
            
            StartCoroutine(AnimateText(textObj, text, healTextPool));
        }
        
        IEnumerator AnimateText(GameObject obj, Text text, Queue<GameObject> pool)
        {
            float elapsed = 0f;
            Vector3 startPos = obj.transform.position;
            
            while (elapsed < displayDuration)
            {
                // 向上移动
                obj.transform.position = startPos + Vector3.up * (elapsed * moveSpeed);
                
                // 淡出
                Color color = text.color;
                color.a = 1f - (elapsed / displayDuration);
                text.color = color;
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // 重置并回收
            obj.SetActive(false);
            Color resetColor = text.color;
            resetColor.a = 1f;
            text.color = resetColor;
            
            pool.Enqueue(obj);
        }
    }
}
