using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Core
{
    /// <summary>
    /// 资源管理器 - 管理游戏所有资源加载
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }
        
        [Header("资源缓存")]
        private Dictionary<string, Object> resourceCache = new Dictionary<string, Object>();
        
        [Header("预制体")]
        public GameObject heroPrefab;
        public GameObject minionPrefab;
        public GameObject towerPrefab;
        public GameObject projectilePrefab;
        
        [Header("材质")]
        public Material blueTeamMaterial;
        public Material redTeamMaterial;
        public Material neutralMaterial;
        
        [Header("特效")]
        public GameObject levelUpEffect;
        public GameObject deathEffect;
        public GameObject spawnEffect;
        public GameObject hitEffect;
        public GameObject critEffect;
        
        [Header("音效")]
        public AudioClip[] attackSounds;
        public AudioClip[] skillSounds;
        public AudioClip[] hitSounds;
        public AudioClip[] deathSounds;
        public AudioClip[] uiSounds;
        
        [Header("图标")]
        public Sprite[] skillIcons;
        public Sprite[] itemIcons;
        public Sprite[] heroIcons;
        
        void Awake()
        {
            Instance = this;
        }
        
        /// <summary>
        /// 加载资源（带缓存）
        /// </summary>
        public T Load<T>(string path) where T : Object
        {
            // 检查缓存
            if (resourceCache.ContainsKey(path))
            {
                return resourceCache[path] as T;
            }
            
            // 从Resources加载
            T resource = Resources.Load<T>(path);
            
            if (resource != null)
            {
                resourceCache[path] = resource;
            }
            else
            {
                Debug.LogWarning($"Resource not found: {path}");
            }
            
            return resource;
        }
        
        /// <summary>
        /// 实例化预制体
        /// </summary>
        public GameObject InstantiatePrefab(string prefabName, Vector3 position, Quaternion rotation)
        {
            GameObject prefab = Load<GameObject>($"Prefabs/{prefabName}");
            
            if (prefab != null)
            {
                return Instantiate(prefab, position, rotation);
            }
            
            return null;
        }
        
        /// <summary>
        /// 获取音效
        /// </summary>
        public AudioClip GetSound(string soundName)
        {
            return Load<AudioClip>($"Audio/{soundName}");
        }
        
        /// <summary>
        /// 获取图标
        /// </summary>
        public Sprite GetIcon(string iconName)
        {
            return Load<Sprite>($"Icons/{iconName}");
        }
        
        /// <summary>
        /// 获取材质
        /// </summary>
        public Material GetMaterial(Battle.MinionAI.Team team)
        {
            switch (team)
            {
                case Battle.MinionAI.Team.Blue:
                    return blueTeamMaterial;
                case Battle.MinionAI.Team.Red:
                    return redTeamMaterial;
                default:
                    return neutralMaterial;
            }
        }
        
        /// <summary>
        /// 获取特效
        /// </summary>
        public GameObject GetEffect(string effectName)
        {
            switch (effectName.ToLower())
            {
                case "levelup": return levelUpEffect;
                case "death": return deathEffect;
                case "spawn": return spawnEffect;
                case "hit": return hitEffect;
                case "crit": return critEffect;
                default: return null;
            }
        }
        
        /// <summary>
        /// 预加载资源
        /// </summary>
        public void PreloadResources()
        {
            // 预加载常用资源
            // 这可以在加载画面时调用
        }
        
        /// <summary>
        /// 清理缓存
        /// </summary>
        public void ClearCache()
        {
            resourceCache.Clear();
            Resources.UnloadUnusedAssets();
        }
        
        /// <summary>
        /// 异步加载
        /// </summary>
        public void LoadAsync<T>(string path, System.Action<T> callback) where T : Object
        {
            StartCoroutine(LoadAsyncCoroutine(path, callback));
        }
        
        System.Collections.IEnumerator LoadAsyncCoroutine<T>(string path, System.Action<T> callback) where T : Object
        {
            ResourceRequest request = Resources.LoadAsync<T>(path);
            
            while (!request.isDone)
            {
                yield return null;
            }
            
            callback?.Invoke(request.asset as T);
        }
    }
}
