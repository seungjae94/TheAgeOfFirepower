using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mathlife.ProjectL.Gameplay.Gameplay.Data.Model;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class MailPopup : PopupPresenter
    {
        private const float OPEN_DURATION = 0.35f;
        private const float CLOSE_DURATION = 0.25f;
        
        // Alias
        private static GameProgressState GameProgressState => GameState.Inst.gameProgressState;
        
        // Component
        [SerializeField]
        private RectTransform panelTrans;
        
        [SerializeField]
        private MailScrollRect scrollRect;
        
        [SerializeField]
        private TextMeshProUGUI mailCapacityText;
        
        [SerializeField]
        private Button closeButton;
        
        [SerializeField]
        private Button recvAllButton;
        
        // Field
        private Tween openTween;
        private Tween closeTween;
        private readonly CompositeDisposable disposables = new();

        public override void Initialize()
        {
            base.Initialize();

            openTween = panelTrans.DOAnchorPosY(0f, OPEN_DURATION)
                .SetAutoKill(false)
                .Pause();

            closeTween = panelTrans.DOAnchorPosY(1000f, CLOSE_DURATION)
                .SetAutoKill(false)
                .Pause();
        }
        
        public override async UniTask OpenWithAnimation()
        {
            AudioManager.Inst.PlaySE(ESoundEffectId.PopupOpen);
            base.OpenWithAnimation();
            
            // 블러 적용
            BlurPopup blurPopup = Find<BlurPopup>();
            blurPopup.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
            await blurPopup.OpenWithAnimation();
            
            closeButton.OnClickAsObservable()
                .Subscribe(OnClickCloseButton)
                .AddTo(disposables);
            
            recvAllButton.OnClickAsObservable()
                .Subscribe(OnClickRecvAllButton)
                .AddTo(disposables);
            
            GameProgressState.mailsRx
                .ObserveCountChanged()
                .DistinctUntilChanged()
                .Subscribe(UpdateBottomView);
            
            // 뷰 초기화
            scrollRect.UpdateContentsAuto();
            UpdateBottomView(GameProgressState.mailsRx.Count);
            
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

        private void UpdateBottomView(int mailCount)
        {
            mailCapacityText.text = $"우편 보유 수량 ({mailCount}/200)";
            recvAllButton.interactable = mailCount > 0;
        }
        
        private void OnClickCloseButton(Unit _)
        {
            CloseWithAnimation().Forget();
        }

        private void OnClickRecvAllButton(Unit _)
        {
            GameProgressState.ReceiveAllMailRewards();
            scrollRect.UpdateContentsAuto();
            recvAllButton.interactable = false;
            GameState.Inst.Save();
        }
    }
}