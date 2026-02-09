using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ShanHaiKing.Effects
{
    /// <summary>
    /// 粒子效果管理器
    /// </summary>
    public class ParticleManager : MonoBehaviour
    {
        public static ParticleManager Instance { get; private set; }
        
        [Header("技能特效")]
        public GameObject fireballEffect;
        public GameObject iceShardEffect;
        public GameObject lightningEffect;
        public GameObject healingEffect;
        public GameObject shieldEffect;
        public GameObject teleportEffect;
        
        [Header("命中特效")]
        public GameObject hitSparkEffect;
        public GameObject bloodEffect;
        public GameObject magicHitEffect;
        public GameObject critHitEffect;
        
        [Header("环境特效")]
        public GameObject dustEffect;
        public GameObject smokeEffect;
        public GameObject steamEffect;
        public GameObject leavesEffect;
        
        [Header("UI特效")]
        public GameObject levelUpUIEffect;
        public GameObject goldGainEffect;
        public GameObject damagePopupEffect;
        
        [Header("对象池")]
        private Dictionary<string, Queue<GameObject>> effectPools = new Dictionary<string, Queue<GameObject>>();
        public int poolSizePerEffect = 10;
        
        void Awake()
        {
            Instance = this;
            InitializePools();
        }
        
        void InitializePools()
        {
            // 初始化所有特效的对象池
            string[] effectNames = { "Fireball", "IceShard", "Lightning", "Healing", "Shield", 
                                    "HitSpark", "Blood", "MagicHit", "CritHit", "Dust" };
            
            foreach (string name in effectNames)
            {
                effectPools[name] = new Queue<GameObject>();
                
                for (int i = 0; i < poolSizePerEffect; i++)
                {
                    GameObject obj = CreateEffect(name);
                    obj.SetActive(false);
                    effectPools[name].Enqueue(obj);
                }
            }
        }
        
        GameObject CreateEffect(string effectName)
        {
            GameObject prefab = null;
            
            switch (effectName)
            {
                case "Fireball": prefab = fireballEffect; break;
                case "IceShard": prefab = iceShardEffect; break;
                case "Lightning": prefab = lightningEffect; break;
                case "Healing": prefab = healingEffect; break;
                case "Shield": prefab = shieldEffect; break;
                case "HitSpark": prefab = hitSparkEffect; break;
                case "Blood": prefab = bloodEffect; break;
                case "MagicHit": prefab = magicHitEffect; break;
                case "CritHit": prefab = critHitEffect; break;
                case "Dust": prefab = dustEffect; break;
            }
            
            if (prefab != null)
            {
                return Instantiate(prefab, transform);
            }
            
            return new GameObject($"Effect_{effectName}");
        }
        
        public GameObject PlayEffect(string effectName, Vector3 position, Quaternion rotation)
        {
            GameObject effect = GetFromPool(effectName);
            
            if (effect != null)
            {
                effect.transform.position = position;
                effect.transform.rotation = rotation;
                effect.SetActive(true);
                
                // 自动回收
                StartCoroutine(ReturnToPoolAfterDelay(effectName, effect, 2f));
            }
            
            return effect;
        }
        
        public GameObject PlayEffect(string effectName, Vector3 position)
        {
            return PlayEffect(effectName, position, Quaternion.identity);
        }
        
        GameObject GetFromPool(string effectName)
        {
            if (effectPools.ContainsKey(effectName) && effectPools[effectName].Count > 0)
            {
                return effectPools[effectName].Dequeue();
            }
            
            // 池空了，创建新的
            return CreateEffect(effectName);
        }
        
        void ReturnToPool(string effectName, GameObject effect)
        {
            effect.SetActive(false);
            
            if (!effectPools.ContainsKey(effectName))
            {
                effectPools[effectName] = new Queue<GameObject>();
            }
            
            effectPools[effectName].Enqueue(effect);
        }
        
        IEnumerator ReturnToPoolAfterDelay(string effectName, GameObject effect, float delay)
        {
            yield return new WaitForSeconds(delay);
            ReturnToPool(effectName, effect);
        }
        
        // 特殊效果方法
        public void PlayFireEffect(Vector3 position, float scale = 1f)
        {
            GameObject effect = PlayEffect("Fireball", position);
            if (effect != null)
                effect.transform.localScale = Vector3.one * scale;
        }
        
        public void PlayIceEffect(Vector3 position, float scale = 1f)
        {
            GameObject effect = PlayEffect("IceShard", position);
            if (effect != null)
                effect.transform.localScale = Vector3.one * scale;
        }
        
        public void PlayLightningEffect(Vector3 startPos, Vector3 endPos)
        {
            GameObject effect = PlayEffect("Lightning", startPos);
            if (effect != null)
            {
                effect.transform.LookAt(endPos);
                float distance = Vector3.Distance(startPos, endPos);
                effect.transform.localScale = new Vector3(1, 1, distance);
            }
        }
        
        public void PlayHealEffect(Vector3 position)
        {
            PlayEffect("Healing", position);
        }
        
        public void PlayShieldEffect(Vector3 position)
        {
            PlayEffect("Shield", position);
        }
        
        public void PlayHitEffect(Vector3 position, Core.DamageType damageType, bool isCrit)
        {
            string effectName = isCrit ? "CritHit" : "HitSpark";
            
            switch (damageType)
            {
                case Core.DamageType.Magic:
                    effectName = "MagicHit";
                    break;
                case Core.DamageType.Physical:
                    effectName = isCrit ? "CritHit" : "HitSpark";
                    break;
            }
            
            PlayEffect(effectName, position);
        }
        
        public void PlayDustEffect(Vector3 position)
        {
            PlayEffect("Dust", position);
        }
        
        public void PlayTeleportEffect(Vector3 position)
        {
            PlayEffect("Teleport", position);
        }
        
        public void PlayLevelUpEffect(Vector3 position)
        {
            if (levelUpUIEffect != null)
            {
                GameObject effect = Instantiate(levelUpUIEffect, position, Quaternion.identity);
                Destroy(effect, 3f);
            }
        }
    }
}
