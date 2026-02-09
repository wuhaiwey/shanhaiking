using UnityEngine;

namespace ShanHaiKing.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        [Header("音效")]
        public AudioClip attackSound;
        public AudioClip skillSound;
        public AudioClip hitSound;
        public AudioClip deathSound;
        
        [Header("背景音乐")]
        public AudioClip bgmBattle;
        public AudioClip bgmLobby;
        
        private AudioSource sfxSource;
        private AudioSource bgmSource;
        
        void Awake()
        {
            Instance = this;
            
            sfxSource = gameObject.AddComponent<AudioSource>();
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
        }
        
        public void PlaySFX(AudioClip clip)
        {
            if (clip != null)
                sfxSource.PlayOneShot(clip);
        }
        
        public void PlayBGM(AudioClip clip)
        {
            if (clip != null && bgmSource.clip != clip)
            {
                bgmSource.clip = clip;
                bgmSource.Play();
            }
        }
        
        public void PlayAttackSound()
        {
            PlaySFX(attackSound);
        }
        
        public void PlaySkillSound()
        {
            PlaySFX(skillSound);
        }
    }
}
