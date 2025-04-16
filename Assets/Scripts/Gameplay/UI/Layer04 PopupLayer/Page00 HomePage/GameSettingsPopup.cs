using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DhafinFawwaz.AnimationUILib;
using Mathlife.ProjectL.Gameplay.Gameplay.Data.Model;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class GameSettingsPopup : PopupPresenter
    {
        // Alias
        private GameSettingState GameSettingState => GameState.Inst.gameSettingState;

        // Component
        [SerializeField]
        private DOTweenAnimation windowSlideAnimation;

        [SerializeField]
        private ToggleButton drawTrajectoryToggleButton;

        [SerializeField]
        private Slider bgmVolumeSlider;

        [SerializeField]
        private TextMeshProUGUI bgmVolumeText;

        [SerializeField]
        private Slider seVolumeSlider;

        [SerializeField]
        private TextMeshProUGUI seVolumeText;

        [SerializeField]
        private Button okButton;

        [SerializeField]
        private Button gameQuitButton;

        // Field
        private readonly CompositeDisposable disposables = new();

        public override async UniTask OpenWithAnimation()
        {
            base.OpenWithAnimation();

            // 블러 적용
            BlurPopup blurPopup = Find<BlurPopup>();
            blurPopup.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
            await blurPopup.OpenWithAnimation();

            drawTrajectoryToggleButton.IsOn = GameSettingState.drawTrajectory.Value;
            bgmVolumeSlider.value = GameSettingState.bgmVolume.Value;
            seVolumeSlider.value = GameSettingState.seVolume.Value;

            drawTrajectoryToggleButton.OnClickAsObservable()
                .DistinctUntilChanged()
                .Subscribe(OnToggleButtonClick)
                .AddTo(disposables);

            bgmVolumeSlider.OnValueChangedAsObservable()
                .DistinctUntilChanged()
                .Subscribe(OnBGMVolumeSliderValueChanged)
                .AddTo(disposables);

            seVolumeSlider.OnValueChangedAsObservable()
                .DistinctUntilChanged()
                .Subscribe(OnSEVolumeSliderValueChanged)
                .AddTo(disposables);

            okButton.OnClickAsObservable()
                .Subscribe(OnClickOKButton)
                .AddTo(disposables);

            gameQuitButton.OnClickAsObservable()
                .Subscribe(OnClickQuitButton)
                .AddTo(disposables);

            windowSlideAnimation.DORestart();
        }

        public override async UniTask CloseWithAnimation()
        {
            disposables.Clear();

            windowSlideAnimation.DOPlayBackwards();
            await windowSlideAnimation.tween.AwaitForRewind();

            // 블러 제거
            Find<BlurPopup>().CloseWithAnimation().Forget();

            await base.CloseWithAnimation();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
            windowSlideAnimation.tween.Kill();
        }

        private void OnToggleButtonClick(Unit _)
        {
            GameSettingState.drawTrajectory.Value = drawTrajectoryToggleButton.IsOn;
        }

        private void OnBGMVolumeSliderValueChanged(float value)
        {
            bgmVolumeText.text = $"{Mathf.Round(value * 100)}%";
            AudioManager.Inst.SetBGMVolume(value);
        }

        private void OnSEVolumeSliderValueChanged(float value)
        {
            seVolumeText.text = $"{Mathf.Round(value * 100)}%";
            AudioManager.Inst.SetSEVolume(value);
        }

        private void OnClickOKButton(Unit _)
        {
            AudioManager.Inst.SetBGMVolume(bgmVolumeSlider.value);
            AudioManager.Inst.SetSEVolume(seVolumeSlider.value);
            GameState.Inst.Save();

            CloseWithAnimation().Forget();
        }

        private void OnClickQuitButton(Unit _)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}