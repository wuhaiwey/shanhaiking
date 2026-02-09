using UnityEngine;
using System.Collections.Generic;

namespace ShanHaiKing.Core
{
    /// <summary>
    /// 音频管理器 - 管理所有游戏音效
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        [Header("音频源")]
        public AudioSource bgmSource;
        public AudioSource sfxSource;
        public AudioSource voiceSource;
        
        [Header("音效库")]
        public AudioClip[] bgmClips;
        public AudioClip attackClip;
        public AudioClip skillClip;
        public AudioClip hitClip;
        public AudioClip deathClip;
        public AudioClip victoryClip;
        public AudioClip defeatClip;
        
        [Header("音量设置")]
        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float bgmVolume = 0.7f;
        [Range(0f, 1f)] public float sfxVolume = 1f;
        
        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        void Start()
        {
            PlayBGM(0);
        }
        
        public void PlayBGM(int index)
        {
            if (index >= 0 && index < bgmClips.Length)
            {
                bgmSource.clip = bgmClips[index];
                bgmSource.volume = bgmVolume * masterVolume;
                bgmSource.loop = true;
                bgmSource.Play();
            }
        }
        
        public void PlaySFX(AudioClip clip)
        {
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
            }
        }
        
        public void PlayAttack()
        {
            PlaySFX(attackClip);
        }
        
        public void PlaySkill()
        {
            PlaySFX(skillClip);
        }
        
        public void PlayHit()
        {
            PlaySFX(hitClip);
        }
        
        public void PlayDeath()
        {
            PlaySFX(deathClip);
        }
    }
}
