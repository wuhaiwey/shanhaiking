using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace ShanHaiKing.Audio
{
    /// <summary>
    /// 高级音频管理器
    /// </summary>
    public class AdvancedAudioManager : MonoBehaviour
    {
        public static AdvancedAudioManager Instance { get; private set; }
        
        [Header("音频混合器")]
        public AudioMixer mainMixer;
        public AudioMixerGroup bgmGroup;
        public AudioMixerGroup sfxGroup;
        public AudioMixerGroup voiceGroup;
        public AudioMixerGroup ambientGroup;
        
        [Header("音频源")]
        public AudioSource bgmSource;
        public AudioSource[] sfxSources;
        public AudioSource voiceSource;
        public AudioSource ambientSource;
        
        [Header("音频剪辑库")]
        public AudioClip[] bgmClips;
        public AudioClip[] attackSounds;
        public AudioClip[] skillSounds;
        public AudioClip[] hitSounds;
        public AudioClip[] deathSounds;
        public AudioClip[] uiSounds;
        public AudioClip[] ambientSounds;
        public AudioClip[] voiceLines;
        
        [Header("音量设置")]
        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float bgmVolume = 0.7f;
        [Range(0f, 1f)] public float sfxVolume = 1f;
        [Range(0f, 1f)] public float voiceVolume = 1f;
        [Range(0f, 1f)] public float ambientVolume = 0.5f;
        
        private int currentSfxIndex = 0;
        private Dictionary<string, AudioClip> audioLibrary = new Dictionary<string, AudioClip>();
        private Queue<AudioSource> sfxPool;
        
        void Awake()
        {
            Instance = this;
            InitializeAudioLibrary();
            InitializeSFXPool();
        }
        
        void Start()
        {
            ApplyVolumeSettings();
        }
        
        void InitializeAudioLibrary()
        {
            // 注册所有音频剪辑
            RegisterClips(bgmClips, "BGM_");
            RegisterClips(attackSounds, "ATK_");
            RegisterClips(skillSounds, "SKILL_");
            RegisterClips(hitSounds, "HIT_");
            RegisterClips(deathSounds, "DEATH_");
            RegisterClips(uiSounds, "UI_");
            RegisterClips(ambientSounds, "AMB_");
            RegisterClips(voiceLines, "VOICE_");
        }
        
        void RegisterClips(AudioClip[] clips, string prefix)
        {
            if (clips == null) return;
            
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] != null)
                {
                    string key = prefix + clips[i].name;
                    audioLibrary[key] = clips[i];
                }
            }
        }
        
        void InitializeSFXPool()
        {
            sfxPool = new Queue<AudioSource>();
            
            // 创建额外的SFX源
            for (int i = 0; i < 10; i++)
            {
                GameObject go = new GameObject($"SFX_Source_{i}");
                go.transform.SetParent(transform);
                AudioSource source = go.AddComponent<AudioSource>();
                source.outputAudioMixerGroup = sfxGroup;
                source.playOnAwake = false;
                sfxPool.Enqueue(source);
            }
        }
        
        #region BGM控制
        
        public void PlayBGM(string clipName, bool fade = true)
        {
            AudioClip clip = GetClip("BGM_" + clipName);
            if (clip != null)
            {
                if (fade)
                {
                    StartCoroutine(FadeBGM(clip));
                }
                else
                {
                    bgmSource.clip = clip;
                    bgmSource.Play();
                }
            }
        }
        
        public void PlayBGM(int index, bool fade = true)
        {
            if (index >= 0 && index < bgmClips.Length)
            {
                PlayBGM(bgmClips[index].name, fade);
            }
        }
        
        public void StopBGM(bool fade = true)
        {
            if (fade)
            {
                StartCoroutine(FadeOutBGM());
            }
            else
            {
                bgmSource.Stop();
            }
        }
        
        public void PauseBGM()
        {
            bgmSource.Pause();
        }
        
        public void ResumeBGM()
        {
            bgmSource.UnPause();
        }
        
        System.Collections.IEnumerator FadeBGM(AudioClip newClip)
        {
            float fadeTime = 1f;
            float startVolume = bgmSource.volume;
            
            // 淡出
            for (float t = 0; t < fadeTime; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
                yield return null;
            }
            
            bgmSource.clip = newClip;
            bgmSource.Play();
            
            // 淡入
            for (float t = 0; t < fadeTime; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(0, bgmVolume, t / fadeTime);
                yield return null;
            }
            
            bgmSource.volume = bgmVolume;
        }
        
        System.Collections.IEnumerator FadeOutBGM()
        {
            float fadeTime = 1f;
            float startVolume = bgmSource.volume;
            
            for (float t = 0; t < fadeTime; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
                yield return null;
            }
            
            bgmSource.Stop();
            bgmSource.volume = bgmVolume;
        }
        
        #endregion
        
        #region SFX控制
        
        public void PlaySFX(string clipName, float volume = 1f, float pitch = 1f)
        {
            AudioClip clip = GetClip(clipName);
            if (clip != null)
            {
                PlaySFX(clip, volume, pitch);
            }
        }
        
        public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            AudioSource source = GetAvailableSFXSource();
            if (source != null)
            {
                source.clip = clip;
                source.volume = volume * sfxVolume;
                source.pitch = pitch;
                source.Play();
                
                // 播放完毕后回收
                StartCoroutine(ReturnSFXSource(source, clip.length));
            }
        }
        
        public void PlaySFXAtPosition(string clipName, Vector3 position, float volume = 1f)
        {
            AudioClip clip = GetClip(clipName);
            if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, position, volume * sfxVolume);
            }
        }
        
        AudioSource GetAvailableSFXSource()
        {
            if (sfxPool.Count > 0)
            {
                return sfxPool.Dequeue();
            }
            
            // 创建临时源
            GameObject go = new GameObject("TempSFX");
            go.transform.SetParent(transform);
            return go.AddComponent<AudioSource>();
        }
        
        System.Collections.IEnumerator ReturnSFXSource(AudioSource source, float delay)
        {
            yield return new WaitForSeconds(delay);
            sfxPool.Enqueue(source);
        }
        
        #endregion
        
        #region 便捷方法
        
        public void PlayAttackSound(string heroName)
        {
            PlaySFX($"ATK_{heroName}");
        }
        
        public void PlaySkillSound(string skillName)
        {
            PlaySFX($"SKILL_{skillName}");
        }
        
        public void PlayHitSound()
        {
            if (hitSounds.Length > 0)
            {
                PlaySFX(hitSounds[Random.Range(0, hitSounds.Length)]);
            }
        }
        
        public void PlayDeathSound(string heroName)
        {
            PlaySFX($"DEATH_{heroName}");
        }
        
        public void PlayUISound(string uiAction)
        {
            PlaySFX($"UI_{uiAction}");
        }
        
        public void PlayVoiceLine(string heroName, string lineType)
        {
            string key = $"VOICE_{heroName}_{lineType}";
            AudioClip clip = GetClip(key);
            
            if (clip != null && !voiceSource.isPlaying)
            {
                voiceSource.clip = clip;
                voiceSource.Play();
            }
        }
        
        #endregion
        
        #region 音量控制
        
        public void SetMasterVolume(float volume)
        {
            masterVolume = volume;
            mainMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        }
        
        public void SetBGMVolume(float volume)
        {
            bgmVolume = volume;
            mainMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
        }
        
        public void SetSFXVolume(float volume)
        {
            sfxVolume = volume;
            mainMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        }
        
        public void SetVoiceVolume(float volume)
        {
            voiceVolume = volume;
            mainMixer.SetFloat("VoiceVolume", Mathf.Log10(volume) * 20);
        }
        
        public void SetAmbientVolume(float volume)
        {
            ambientVolume = volume;
            mainMixer.SetFloat("AmbientVolume", Mathf.Log10(volume) * 20);
        }
        
        void ApplyVolumeSettings()
        {
            SetMasterVolume(masterVolume);
            SetBGMVolume(bgmVolume);
            SetSFXVolume(sfxVolume);
            SetVoiceVolume(voiceVolume);
            SetAmbientVolume(ambientVolume);
        }
        
        #endregion
        
        AudioClip GetClip(string key)
        {
            if (audioLibrary.ContainsKey(key))
            {
                return audioLibrary[key];
            }
            return null;
        }
    }
}
