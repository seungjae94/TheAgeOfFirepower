using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mathlife.ProjectL.Gameplay.Gameplay.Data.Model;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class GameSettingsPopup : PopupPresenter
    {
        private const float OPEN_DURATION = 0.35f;
        private const float CLOSE_DURATION = 0.25f;

        // Alias
        private GameSettingState GameSettingState => GameState.Inst.gameSettingState;

        // Component
        [SerializeField]
        private RectTransform panelTrans;

        [SerializeField]
        private TMP_Dropdown resolutionDropdown;

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
        private Tween openTween;
        private Tween closeTween;
        private readonly CompositeDisposable disposables = new();

        public override void OnSceneInitialize()
        {
            base.OnSceneInitialize();

            openTween = panelTrans.DOAnchorPosY(0f, OPEN_DURATION)
                .SetAutoKill(false)
                .Pause();

            closeTween = panelTrans.DOAnchorPosY(1000f, CLOSE_DURATION)
                .SetAutoKill(false)
                .Pause();

            List<TMP_Dropdown.OptionData> optionDatas = new();
            foreach (var option in DisplayManager.resolutionOptions)
            {
                optionDatas.Add(option.fullScreen
                    ? new TMP_Dropdown.OptionData($"전체 화면")
                    : new TMP_Dropdown.OptionData($"창 모드 ({option.width} x {option.height})"));
            }

            resolutionDropdown.AddOptions(optionDatas);
        }

        public override async UniTask OpenWithAnimation()
        {
            AudioManager.Inst.PlaySE(ESoundEffectId.PopupOpen);
            base.OpenWithAnimation();

            // 블러 적용
            BlurPopup blurPopup = Find<BlurPopup>();
            blurPopup.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
            await blurPopup.OpenWithAnimation();

            if (Application.isMobilePlatform)
            {
                resolutionDropdown.enabled = false;
                resolutionDropdown.value = 0;
                resolutionDropdown.RefreshShownValue();
            }
            else
            {
                resolutionDropdown.enabled = true;
                resolutionDropdown.value = GameSettingState.resolutionOptionIndex.Value;
                resolutionDropdown.onValueChanged
                    .AsObservable()
                    .DistinctUntilChanged()
                    .Subscribe(OnResolutionSelected)
                    .AddTo(disposables);
            }

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

            openTween.Restart();
            await openTween.AwaitForComplete();
        }

        public override async UniTask CloseWithAnimation()
        {
            AudioManager.Inst.PlaySE(ESoundEffectId.PopupClose);
            disposables.Clear();

            closeTween.Restart();
            await closeTween.AwaitForComplete();

            // 블러 제거
            Find<BlurPopup>().CloseWithAnimation().Forget();

            await base.CloseWithAnimation();
        }

        private void OnDestroy()
        {
            openTween.Kill();
            closeTween.Kill();
            disposables.Dispose();
        }

        public void OnResolutionSelected(int optionIdx)
        {
            GameSettingState.resolutionOptionIndex.Value = optionIdx;

            DisplayManager.Inst.Adapt().Forget();
            GameState.Inst.Save();
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