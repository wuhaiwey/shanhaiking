using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Effects
{
    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance { get; private set; }
        
        [Header("技能特效预制体")]
        public GameObject fireballEffect;
        public GameObject iceCrystalEffect;
        public GameObject lightningEffect;
        public GameObject healEffect;
        
        void Awake()
        {
            Instance = this;
        }
        
        public void PlayEffect(string effectName, Vector3 position, Quaternion rotation)
        {
            GameObject effect = null;
            
            if (effectName == "fireball") effect = fireballEffect;
            else if (effectName == "ice") effect = iceCrystalEffect;
            else if (effectName == "lightning") effect = lightningEffect;
            else if (effectName == "heal") effect = healEffect;
            
            if (effect != null)
            {
                GameObject instance = Instantiate(effect, position, rotation);
                Destroy(instance, 3f);
            }
        }
    }
}
