using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class MailPopup : PopupPresenter
    {
        // Component
        [SerializeField]
        private DOTweenAnimation windowSlideAnimation;

        [SerializeField]
        private MailScrollRect scrollRect;
        
        [SerializeField]
        private TextMeshProUGUI mailCapacityText;
        
        [SerializeField]
        private Button closeButton;
        
        [SerializeField]
        private Button recvAllButton;
        
        // Field
        private readonly CompositeDisposable disposables = new();

        public override async UniTask OpenWithAnimation()
        {
            base.OpenWithAnimation();
            
            // 블러 적용
            BlurPopup blurPopup = Find<BlurPopup>();
            blurPopup.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
            await blurPopup.OpenWithAnimation();
            
            closeButton.OnClickAsObservable()
                .Subscribe(OnClickCloseButton)
                .AddTo(disposables);
            
            // TODO: recvAllButton 구독
            // TODO: mailCountText 구독
            
            scrollRect.UpdateContentsAuto();
            
            windowSlideAnimation.DOPlay();
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
        
        private void OnClickCloseButton(Unit _)
        {
            CloseWithAnimation().Forget();
        }
    }
}