using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Utils;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PartyMemberChangeModal : Presenter
    {
        const float k_fadeTime = 0.25f;

        [Inject] PartyPage m_partyPage;
        [Inject] CharacterRosterState characterRosterState;

        CanvasGroup m_canvasGroup;
        [SerializeField] Button m_closeButton;
        [SerializeField] ObservablePointerClickTrigger m_excludeButton;
        [SerializeField] CharacterSelectionFlex m_flex;

        int m_selectedSlotIndex;

        SortedCharacterListSubscription m_sortedCharacterListChangeSubs;

        void Awake()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
        }

        void OnDestroy()
        {
            m_sortedCharacterListChangeSubs?.Dispose();
        }

        protected override void InitializeView()
        {
            m_canvasGroup.Hide();
        }

        protected override void InitializeChildren()
        {
            m_flex.Initialize();
        }

        protected override void SubscribeDataChange()
        {
            m_sortedCharacterListChangeSubs = characterRosterState
               .SubscribeSortedCharacterList(UpdateView);

            characterRosterState.party
                .SubscribeMemberChange(_ => UpdateView())
                .AddTo(gameObject);
        }

        protected override void SubscribeUserInteractions()
        {
            m_closeButton.OnClickAsObservable()
                .Subscribe(async _ => await Hide())
                .AddTo(gameObject);

            m_excludeButton.OnPointerClickAsObservable()
                .Subscribe(OnClickExcludeButton)
                .AddTo(gameObject);
        }

        // 유저 상호작용
        public async UniTask Show()
        {
            // TODO: 이동식으로 구현

            m_selectedSlotIndex = m_partyPage.selectedSlotIndexRx.Value;

            if (m_partyPage.IsSelectedSlotIndexInRange() == false || characterRosterState.party[m_selectedSlotIndex] != null)
            {
                m_excludeButton.gameObject.SetActive(true);
            }
            else
            {
                m_excludeButton.gameObject.SetActive(false);
            }

            UpdateFlex();
            await m_canvasGroup.Show(k_fadeTime);
        }

        public async UniTask Hide()
        {
            // TODO: 이동식으로 구현

            await m_canvasGroup.Hide(k_fadeTime);
        }

        async void OnClickExcludeButton(PointerEventData ev)
        {
            m_partyPage.selectedSlotIndexRx.Value = m_selectedSlotIndex;
            characterRosterState.party.RemoveAt(m_selectedSlotIndex);
            await m_partyPage.partyMemberChangeModal.Hide();
        }

        async void OnClickFlexItem(CharacterModel character)
        {
            characterRosterState.party.Add(m_partyPage.selectedSlotIndexRx.Value, character);
            m_partyPage.selectedSlotIndexRx.Value = -1;

            await Hide();
        }

        // 뷰 업데이트
        void UpdateView()
        {
            UpdateFlex();
        }

        void UpdateFlex()
        {
            var itemDatas = characterRosterState
                .GetSortedList()
                .Where(character =>
                    characterRosterState.party.Contains(character) == false)
                .ToList();

            m_flex.Draw(itemDatas, OnClickFlexItem);
        }
    }
}
