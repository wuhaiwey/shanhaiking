using UnityEngine;

namespace ShanHaiKing.Effects
{
    /// <summary>
    /// 特效管理器 - 技能特效、命中特效
    /// </summary>
    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance { get; private set; }
        
        [Header("特效预制体")]
        public GameObject[] skillEffects;
        public GameObject hitEffectPrefab;
        public GameObject levelUpEffectPrefab;
        public GameObject deathEffectPrefab;
        public GameObject spawnEffectPrefab;
        
        [Header("特效池")]
        public int poolSize = 20;
        
        void Awake()
        {
            Instance = this;
        }
        
        public void PlaySkillEffect(int effectIndex, Vector3 position, Quaternion rotation)
        {
            if (effectIndex >= 0 && effectIndex < skillEffects.Length)
            {
                GameObject effect = Instantiate(skillEffects[effectIndex], position, rotation);
                Destroy(effect, 3f); // 3秒后销毁
            }
        }
        
        public void PlayHitEffect(Vector3 position)
        {
            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
                Destroy(effect, 1f);
            }
        }
        
        public void PlayLevelUpEffect(Vector3 position)
        {
            if (levelUpEffectPrefab != null)
            {
                GameObject effect = Instantiate(levelUpEffectPrefab, position, Quaternion.identity);
                Destroy(effect, 2f);
            }
        }
        
        public void PlayDeathEffect(Vector3 position)
        {
            if (deathEffectPrefab != null)
            {
                GameObject effect = Instantiate(deathEffectPrefab, position, Quaternion.identity);
                Destroy(effect, 3f);
            }
        }
        
        public void PlaySpawnEffect(Vector3 position)
        {
            if (spawnEffectPrefab != null)
            {
                GameObject effect = Instantiate(spawnEffectPrefab, position, Quaternion.identity);
                Destroy(effect, 2f);
            }
        }
    }
}
