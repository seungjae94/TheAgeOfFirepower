using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mathlife.ProjectL.Gameplay.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public class BatteryPageValidationPopup : PopupPresenter
    {
        private const float k_openDuration = 0.25f;
        
        // Alias
        private static ArtyRosterState ArtyRosterState => GameState.Inst.artyRosterState;

        // View
        [SerializeField]
        private RectTransform windowTransform;
        
        [SerializeField]
        private Button okButton;

        [SerializeField]
        private Button cancelButton;
        
        // Field
        private Tween openTween;
        private Tween closeTween;
        private readonly CompositeDisposable disposables = new();

        public override void Initialize()
        {
            base.Initialize();

            openTween = windowTransform.DOScale(new Vector3(1f, 1f, 1f), k_openDuration)
                .From(new Vector3(0f, 0f, 1f))
                .SetAutoKill(false)
                .Pause();

            closeTween = windowTransform.DOScale(new Vector3(0f, 0f, 1f), k_openDuration)
                .From(new Vector3(1f, 1f, 1f))
                .SetAutoKill(false)
                .Pause();
        }

        public override async UniTask OpenWithAnimation()
        {
            AudioManager.Inst.PlaySE(ESoundEffectId.PopupOpen);
            
            // 블러 적용
            BlurPopup blurPopup = Find<BlurPopup>();
            blurPopup.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
            await blurPopup.OpenWithAnimation();
            
            await base.OpenWithAnimation();
            
            okButton.OnClickAsObservable()
                .Subscribe(OnClickOKButton)
                .AddTo(disposables);

            cancelButton.OnClickAsObservable()
                .Subscribe(OnClickCancelButton)
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
            await Find<BlurPopup>().CloseWithAnimation();
            
            await base.CloseWithAnimation();
        }

        private void OnDestroy()
        {
            openTween.Kill();
            closeTween.Kill();
            disposables.Dispose();
        }

        private void OnClickOKButton(Unit _)
        {
            ArtyRosterState.BuildBestTeam();

            // 변경사항 저장
            GameState.Inst.Save();
            
            CloseWithAnimation()
                .ContinueWith(() => Presenter.Find<BatteryPage>().Close())
                .Forget();
        }

        private void OnClickCancelButton(Unit _)
        {
            AudioManager.Inst.PlaySE(ESoundEffectId.PopupClose);
            CloseWithAnimation().Forget();
        }
    }
}