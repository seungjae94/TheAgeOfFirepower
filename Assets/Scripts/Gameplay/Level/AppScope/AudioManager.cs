using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.Gameplay.Data.Model;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using UnityEngine;
using UnityEngine.Audio;

namespace Mathlife.ProjectL.Gameplay
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        // Alias
        private GameSettingState GameSettingState => GameState.Inst.gameSettingState;
        
        // 상수
        private const float MAX_DECIBEL = 20f;
        
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Inherit;
        
        // 컴포넌트
        [SerializeField]
        private AudioMixer audioMixer;
        
        [SerializeField]
        private AudioSource bgmSource;
        
        [SerializeField]
        private AudioSource seSource;
        
        // 필드
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
            GameSettingState.bgmVolume.Value = volume;
        }
        
        public void SetSEVolume(float volume)
        {
            float clamped = Mathf.Clamp(volume, 0.001f, 1f);
            audioMixer.SetFloat("SE.Volume", Mathf.Log10(clamped) * MAX_DECIBEL);
            GameSettingState.seVolume.Value = volume;
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