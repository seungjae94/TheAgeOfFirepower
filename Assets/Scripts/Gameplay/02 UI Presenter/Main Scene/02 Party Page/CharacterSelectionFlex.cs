using UnityEngine;
using VContainer;
using UniRx;
using Mathlife.ProjectL.Utils;
using UniRx.Triggers;
using UnityEngine.EventSystems;
using System;
using Unity.Android.Gradle.Manifest;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace Mathlife.ProjectL.Gameplay
{
    class CharacterSelectionFlex
        : AbstractFlex<CharacterModel, CharacterSelectionFlexItem>
    {
        CanvasGroup m_removeMemberGuide;
        ObservableDropTrigger m_dropTrigger;
        ObservablePointerClickTrigger m_pointerClickTrigger;

        [Inject] PartyPage m_partyPage;
        [Inject] CharacterRepository m_characterRepository;


        SortedCharacterListSubscription m_subscription;

        void Awake()
        {
            m_removeMemberGuide = transform.FindRecursiveByName<CanvasGroup>("Remove Member Guide");

            m_dropTrigger = GetComponent<ObservableDropTrigger>();
            m_pointerClickTrigger = GetComponent<ObservablePointerClickTrigger>();
        }

        void OnDestroy()
        {
            m_subscription?.Dispose();
        }

        protected override void SubscribeDataChange()
        {
            m_subscription = m_characterRepository
               .SubscribeSortedCharacterList(UpdateView);

            m_characterRepository
                .party.SubscribeMemberChange(_ => UpdateView())
                .AddTo(gameObject);

            m_partyPage
                .SubscribePartyMemberSlotItemDragEvent(() => m_removeMemberGuide.Show(), () => m_removeMemberGuide.Hide())
                .AddTo(gameObject);
        }

        protected override void SubscribeUserInteractions()
        {
            m_dropTrigger.OnDropAsObservable()
                .Subscribe(OnDrop)
                .AddTo(gameObject);

            m_pointerClickTrigger.OnPointerClickAsObservable()
                .Subscribe(OnPointerClick)
                .AddTo(gameObject);
        }

        protected override void InitializeView()
        {
            UpdateView();
        }

        void UpdateView()
        {
            m_removeMemberGuide.Hide();

            Draw(m_characterRepository.GetSortedList()
                .Where(character => m_characterRepository.party.Contains(character) == false)
                .ToList());
        }

        void OnDrop(PointerEventData eventData)
        {
            var newCharacter = eventData.pointerDrag?
                .GetComponent<PartyMemberSlotItem>()?
                .GetCharacterModel();

            if (null == newCharacter)
                return;

            // 멤버를 스크롤 뷰에 놓는 경우만 처리
            if (m_characterRepository.party.Contains(newCharacter))
            {
                m_characterRepository.party.Remove(newCharacter);
            }
        }

        void OnPointerClick(PointerEventData eventData)
        {
            m_partyPage.selectedCharacter.SetState(null);
        }
    }
}