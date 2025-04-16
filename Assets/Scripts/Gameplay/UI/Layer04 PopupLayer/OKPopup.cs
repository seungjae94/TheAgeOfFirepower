using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class OKPopup : PopupPresenter
    {
        private const float k_openDuration = 0.25f;
        
        // View
        [SerializeField]
        private RectTransform windowTransform;

        [SerializeField]
        private TextMeshProUGUI titleText;
        
        [SerializeField]
        private TextMeshProUGUI messageText;
        
        [SerializeField]
        private Button okButton;
        
        // Field
        private Tween openTween;
        private Tween closeTween;
        private readonly CompositeDisposable disposables = new();
        
        // State
        public readonly Subject<Unit> okClickSubject = new();  

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

        public void Setup(string title, string message)
        {
            titleText.text = title;
            messageText.text = message;
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
            okClickSubject.OnNext(_);
            CloseWithAnimation().Forget();
        }
    }
}