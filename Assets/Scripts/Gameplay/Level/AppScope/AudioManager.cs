using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.Gameplay.Data.Model;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Audio;

namespace Mathlife.ProjectL.Gameplay
{
    public enum ESoundEffectId
    {
        Ok = 0,
        Cancel,
        PopupOpen,
        PopupClose,
        BeginDrag,
        Buy,
        Engine,
        Aim,
        Fire,
        Explosion,
    }

    [Serializable]
    public struct SoundEffectData
    {
        public ESoundEffectId id;
        public AudioClip clip;
    }

    public class AudioManager : MonoSingleton<AudioManager>
    {
        // Alias
        private GameSettingState GameSettingState => GameState.Inst.gameSettingState;

        // 상수
        private const float CONVERT_COEFF = 20f; // log(0.0001) * 20 = -80dB, log(1) * 20 = 0dB
        private const int POOL_SIZE = 10;

        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Inherit;

        [SerializeField]
        private List<SoundEffectData> soundEffects = new();

        // 컴포넌트
        [SerializeField]
        private AudioMixer audioMixer;

        [SerializeField]
        private AudioSource bgmSource;

        [SerializeField]
        private AudioSource seSource;

        [SerializeField]
        private AudioSource seSourceLooped;

        [SerializeField]
        private AudioSource audioSourcePrefab;

        private readonly Queue<AudioSource> pool = new();

        // 필드
        public float BGMVolume
        {
            get
            {
                audioMixer.GetFloat("BGM.Volume", out float volume);
                return Mathf.Pow(10, volume / CONVERT_COEFF);
            }
        }

        public float SEVolume
        {
            get
            {
                audioMixer.GetFloat("SE.Volume", out float volume);
                return Mathf.Pow(10, volume / CONVERT_COEFF);
            }
        }

        protected override void OnRegistered()
        {
            soundEffects.Sort((s1, s2) => s1.id - s2.id);

            for (int i = 0; i < POOL_SIZE; ++i)
            {
                var source = CreateAudioSource();
                pool.Enqueue(source);
            }
        }

        private AudioSource CreateAudioSource()
        {
            AudioSource source = Instantiate<AudioSource>(audioSourcePrefab, transform);
            source.gameObject.SetActive(false);
            return source;
        }

        public AudioSource BorrowAudioSource()
        {
            var source = pool.Count == 0 ? CreateAudioSource() : pool.Dequeue();
            source.gameObject.SetActive(true);
            return source;
        }

        public void ReturnAudioSource(AudioSource source)
        {
            source.Stop();
            source.gameObject.SetActive(false);
            pool.Enqueue(source);
        }

        public UniTaskVoid PlayOneShotOnAudioPool(ESoundEffectId id)
        {
            var clip = soundEffects[(int)id].clip;
            return PlayOneShotOnAudioPool(clip);
        }

        public async UniTaskVoid PlayOneShotOnAudioPool(AudioClip clip)
        {
            var source = BorrowAudioSource();
            source.PlayOneShot(clip);
            await UniTask.WaitWhile(source, s => s != null && s.isPlaying);
            ReturnAudioSource(source);
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

        public void PlaySE(ESoundEffectId clipId, bool loop = false)
        {
            PlaySE(soundEffects[(int)clipId].clip, loop);
        }

        public void PlaySE(AudioClip clip, bool loop = false)
        {
            if (loop)
            {
                seSourceLooped.Stop();
                
                seSourceLooped.clip = clip;
                seSourceLooped.Play();
            }
            else
            {
                seSource.Stop();
                seSource.PlayOneShot(clip);
            }
        }

        public void StopSE(bool looped = false)
        {
            if (looped)
            {
                seSourceLooped.Stop();
                seSourceLooped.clip = null;
            }
            else
            {
                seSource.Stop();
            }
        }

        public void StopAllSE()
        {
            seSource.Stop();
            seSourceLooped.Stop();
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
            audioMixer.SetFloat("BGM.Volume", Mathf.Log10(clamped) * CONVERT_COEFF);
            GameSettingState.bgmVolume.Value = volume;
        }

        public void SetSEVolume(float volume)
        {
            float clamped = Mathf.Clamp(volume, 0.001f, 1f);
            audioMixer.SetFloat("SE.Volume", Mathf.Log10(clamped) * CONVERT_COEFF);
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