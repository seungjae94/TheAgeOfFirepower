using Cysharp.Threading.Tasks;
using System.Linq;
using DG.Tweening;
using Mathlife.ProjectL.Gameplay.UI;
using Sirenix.OdinInspector;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using DG.DOTweenEditor;
#endif

namespace Mathlife.ProjectL.Gameplay
{
    [RequireComponent(typeof(RectTransform))]
    public class BattlePageArtySlotChangePopup : PopupPresenter
    {
        [SerializeField]
        private float slideDuration = 0.5f;

        // Alias
        private BatteryPage BatteryPage => Find<BatteryPage>();
        private ArtyRosterState ArtyRosterState => GameState.Inst.artyRosterState;

        // View
        Button m_closeButton;
        Button m_excludeButton;
        //[SerializeField] CharacterSelectionFlex m_flex;

        int m_selectedSlotIndex;

        SortedCharacterListSubscription m_sortedCharacterListChangeSubs;

        // Tween
        private Tween slideInTween;
        private Tween slideOutTween;

        private void CreateTweens()
        {
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
        void Awake()
        {
            CreateTweens();
        }

        void OnDestroy()
        {
            m_sortedCharacterListChangeSubs?.Dispose();
        }

        // Open / Close
        public override async UniTask OpenWithAnimation()
        {
            base.Activate();

            // 뷰 초기화
            m_excludeButton.gameObject.SetActive(BatteryPage.SelectedArty != null);

            //m_flex.Initialize();
            UpdateFlex();

            // 이벤트 구독
            m_closeButton.OnClickAsObservable()
                .Subscribe(OnClickCloseButton)
                .AddTo(gameObject);

            m_excludeButton.OnPointerClickAsObservable()
                .Subscribe(OnClickExcludeButton)
                .AddTo(gameObject);

            // 모델 구독
            m_sortedCharacterListChangeSubs = ArtyRosterState
                .SubscribeSortedCharacterList(UpdateView);

            ArtyRosterState.Battery
                .SubscribeMemberChange(_ => UpdateView())
                .AddTo(gameObject);

            m_selectedSlotIndex = Find<BatteryPage>().selectedSlotIndexRx.Value;

            // 슬라이드 애니메이션
            slideInTween.Restart();
            await slideInTween;
        }

        public override async UniTask CloseWithAnimation()
        {
            slideOutTween.Restart();
            await slideOutTween;

            base.Deactivate();
        }

#if UNITY_EDITOR
        [Button("Preview SlideIn")]
        private void PreviewSlideIn()
        {
            DOTweenEditorPreview.Stop();
            CreateTweens();
            DOTweenEditorPreview.PrepareTweenForPreview(slideInTween);
            DOTweenEditorPreview.Start();
        }

        [Button("Preview SlideOut")]
        private void PreviewSlideOut()
        {
            DOTweenEditorPreview.Stop();
            CreateTweens();
            DOTweenEditorPreview.PrepareTweenForPreview(slideOutTween);
            DOTweenEditorPreview.Start();
        }
#endif

        // 유저 상호작용
        private void OnClickCloseButton(Unit _)
        {
            CloseWithAnimation().Forget();
        }

        private void OnClickExcludeButton(PointerEventData ev)
        {
            Find<BatteryPage>().selectedSlotIndexRx.Value = m_selectedSlotIndex;
            ArtyRosterState.Battery.RemoveAt(m_selectedSlotIndex);
            //await Find<BatteryPage>().partyMemberChangeModal.Hide();
        }

        private void OnClickFlexItem(ArtyModel arty)
        {
            ArtyRosterState.Battery.Add(Find<BatteryPage>().selectedSlotIndexRx.Value, arty);
            Find<BatteryPage>().selectedSlotIndexRx.Value = -1;

            CloseWithAnimation().Forget();
        }

        // 뷰 업데이트
        void UpdateView()
        {
            UpdateFlex();
        }

        void UpdateFlex()
        {
            var itemDatas = ArtyRosterState
                .GetSortedList()
                .Where(character =>
                    ArtyRosterState.Battery.Contains(character) == false)
                .ToList();

            //m_flex.Draw(itemDatas, OnClickFlexItem);
        }
    }
}