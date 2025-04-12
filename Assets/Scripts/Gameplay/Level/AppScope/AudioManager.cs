using Mathlife.ProjectL.Gameplay.ObjectBase;
using UnityEngine;
using UnityEngine.Audio;

namespace Mathlife.ProjectL.Gameplay
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        private const float MAX_DECIBEL = 20f;
        
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Inherit;
        
        [SerializeField]
        private AudioMixer audioMixer;
        
        [SerializeField]
        private AudioSource bgmSource;
        
        [SerializeField]
        private AudioSource seSource;
        
        public float BGMVolume => bgmSource.volume;
        public float SEVolume { get; private set; } = 1f;

        protected override void OnRegistered()
        {
            bgmSource = GetComponent<AudioSource>();
        }
        
        public void PlayBGM(AudioClip clip, bool forceReplay = false)
        {
            if (forceReplay == false && clip == bgmSource.clip)
                return;
            
            bgmSource.clip = clip;
            bgmSource.Play();
        }

        public void PlaySE(AudioClip clip)
        {
            bgmSource.PlayOneShot(clip, SEVolume);
        }

        public void SetBGMVolume(float volume)
        {
            float clamped = Mathf.Clamp(volume, 0f, 1f);
            audioMixer.SetFloat("BGM", Mathf.Log10(clamped) * MAX_DECIBEL);
        }
        
        public void SetSEVolume(float volume)
        {
            float clamped = Mathf.Clamp(volume, 0f, 1f);
            audioMixer.SetFloat("SE", Mathf.Log10(clamped) * MAX_DECIBEL);
        }
    }
}