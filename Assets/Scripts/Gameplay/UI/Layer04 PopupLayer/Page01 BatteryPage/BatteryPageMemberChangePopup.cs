using System;
using Cysharp.Threading.Tasks;
using System.Linq;
using DG.Tweening;
using Mathlife.ProjectL.Gameplay.UI;
using Sirenix.OdinInspector;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

#if UNITY_EDITOR
using Mathlife.ProjectL.Gameplay.UI.Editor;
#endif

namespace Mathlife.ProjectL.Gameplay.UI.BatteryPagePopup
{
    [RequireComponent(typeof(RectTransform))]
    public class BatteryPageMemberChangePopup : PopupPresenter
    {
        [SerializeField]
        private float slideDuration = 0.5f;

        // Alias
        private BatteryPage BatteryPage => Find<BatteryPage>();
        private ArtyRosterState ArtyRosterState => GameState.Inst.artyRosterState;

        // View
        [SerializeField]
        private Button closeButton;

        [SerializeField]
        BatteryPageMemberChangeScrollView scrollView;

        // Field
        private readonly CompositeDisposable disposables = new();

        // Tween
        private Tween slideInTween;
        private Tween slideOutTween;

        private void CreateTweens()
        {
            slideInTween?.Kill();
            slideOutTween?.Kill();

            Vector2 outPos = new Vector2(rectTransform.rect.width, 0);
            slideInTween = rectTransform.DOAnchorPos(Vector2.zero, slideDuration, false)
                .From(outPos)
                .SetAutoKill(false)
                .Pause();
            slideOutTween = rectTransform.DOAnchorPos(outPos, slideDuration, false)
                .From(Vector2.zero)
                .SetAutoKill(false)
                .Pause();
        }


        // 이벤트 함수
        public override void Initialize()
        {
            base.Initialize();

            CreateTweens();
        }

        void OnDestroy()
        {
            slideInTween.Kill();
            slideOutTween.Kill();
            disposables.Dispose();
        }

        public override async UniTask OpenWithAnimation()
        {
            await base.OpenWithAnimation();

            // 이벤트 구독
            closeButton.OnClickAsObservable()
                .Subscribe(OnClickCloseButton)
                .AddTo(disposables);

            // 모델 구독
            BatteryPage.selectedSlotIndexRx
                .Subscribe(selectedSlot => UpdateScrollView())
                .AddTo(disposables);

            // 뷰 초기화
            UpdateScrollView();

            // 슬라이드 애니메이션
            slideInTween.Restart();
            await slideInTween.AwaitForComplete();
        }

        public override async UniTask CloseWithAnimation()
        {
            disposables.Clear();

            BatteryPage.selectedSlotIndexRx.Value = -1;
            
            slideOutTween.Restart();
            await slideOutTween.AwaitForComplete();

            await base.CloseWithAnimation();
        }

#if UNITY_EDITOR
        [Button("Preview SlideIn")]
        private void PreviewSlideIn()
        {
            CreateTweens();
            MyDOTweenEditorPreview.Start(slideInTween, gameObject);
        }

        [Button("Preview SlideOut")]
        private void PreviewSlideOut()
        {
            CreateTweens();
            MyDOTweenEditorPreview.Start(slideOutTween, gameObject);
        }

        [Button("Stop Preview")]
        private void StopPreview()
        {
            MyDOTweenEditorPreview.Stop(gameObject);
        }
#endif

        // 유저 상호작용
        private void OnClickCloseButton(Unit _)
        {
            CloseWithAnimation().Forget();
        }

        // 뷰 업데이트
        private void UpdateScrollView()
        {
            bool ExcludeFilter(ArtyModel arty) => ArtyRosterState.Battery.Contains(arty);

            var sortedArtyList = ArtyRosterState
                .GetSortedList(ExcludeFilter);

            if (BatteryPage.SelectedArty != null)
            {
                sortedArtyList.Insert(0, null);
            }

            var items = sortedArtyList
                .Select(arty => new ItemData() { arty = arty })
                .ToList();

            scrollView.UpdateContents(items);
        }
    }
}