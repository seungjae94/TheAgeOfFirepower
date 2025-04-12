using Cysharp.Threading.Tasks;
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
        public float SEVolume => seSource.volume;

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

        public void ReplayBGM()
        {
            bgmSource.Play();
        }

        public void PlaySE(AudioClip clip)
        {
            seSource.PlayOneShot(clip, SEVolume);
        }
        
        public void StopSE()
        {
            seSource.Stop();
        }

        public void PauseBGM()
        {
            bgmSource.Pause();
        }
        
        public void ResumeBGM()
        {
            bgmSource.UnPause();
        }

        public void SetBGMVolume(float volume)
        {
            float clamped = Mathf.Clamp(volume, 0.001f, 1f);
            audioMixer.SetFloat("BGM.Volume", Mathf.Log10(clamped) * MAX_DECIBEL);
        }
        
        public void SetSEVolume(float volume)
        {
            float clamped = Mathf.Clamp(volume, 0.001f, 1f);
            audioMixer.SetFloat("SE.Volume", Mathf.Log10(clamped) * MAX_DECIBEL);
        }

        public void MuteBGM()
        {
            bgmSource.mute = true;
        }

        public void UnmuteBGM()
        {
            bgmSource.mute = false;
        }
    }
}