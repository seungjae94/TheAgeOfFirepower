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

namespace Mathlife.ProjectL.Gameplay
{
    class CharacterSelectionFlex
        : AbstractFlex<CharacterModel, CharacterSelectionFlexItem>
    {
        Transform m_content;
        CanvasGroup m_removeMemberGuide;

        ObservableDropTrigger m_dropTrigger;
        ObservablePointerClickTrigger m_pointerClickTrigger;

        [Inject] MainSceneManager m_mainSceneManager;
        [Inject] CharacterRepository m_characterRepository;

        SortedCharacterListSubscription m_subscription;

        void Awake()
        {
            m_content = transform.FindRecursiveByName<Transform>("Content");
            m_removeMemberGuide = transform.FindRecursiveByName<CanvasGroup>("Remove Member Guide");

            m_dropTrigger = GetComponent<ObservableDropTrigger>();
            m_pointerClickTrigger = GetComponent<ObservablePointerClickTrigger>();
        }

        void OnDestroy()
        {
            m_subscription?.Dispose();
        }

        public void Initialize()
        {
            PartyPage teamPage = m_mainSceneManager.GetPage<PartyPage>();

            // Subscribe user interactions
            m_dropTrigger.OnDropAsObservable()
                .Subscribe(OnDrop)
                .AddTo(gameObject);

            m_pointerClickTrigger.OnPointerClickAsObservable()
                .Subscribe(OnPointerClick)
                .AddTo(gameObject);

            // Subscribe model change events
            m_subscription = m_characterRepository
               .SubscribeSortedCharacterList(UpdateView);

            m_characterRepository
                .party.SubscribeMemberChange(_ => UpdateView())
                .AddTo(gameObject);

            m_mainSceneManager
                .GetPage<PartyPage>()
                .SubscribeDragMemberCardEvent(() => m_removeMemberGuide.Show(), () => m_removeMemberGuide.Hide())
                .AddTo(gameObject);

            // Render
            UpdateView();
        }

        new void UpdateView()
        {
            //m_removeMemberGuide.Hide();

            //foreach (Transform cardTrans in m_content)
            //{
            //    Destroy(cardTrans.gameObject);
            //}

            //foreach (CharacterModel character in m_characterRepository.GetSortedList())
            //{
            //    if (m_characterRepository.party.Contains(character))
            //        continue;

            //    CharacterCardView card = InstantiateWithInjection<CharacterCardView>(EPrefabId.CharacterCard, m_content);
            //    card.Initialize(character);
            //}
        }

        void OnDrop(PointerEventData eventData)
        {
            var newCharacter = eventData.pointerDrag?
                .GetComponent<CharacterCardView>()?
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
            m_mainSceneManager.GetPage<PartyPage>().selectedCharacter = null;
        }

        protected override void SubscribeDataChange()
        {
            throw new NotImplementedException();
        }

        protected override void SubscribeUserInteractions()
        {
            throw new NotImplementedException();
        }

        protected override void InitializeView()
        {
            throw new NotImplementedException();
        }
    }
}