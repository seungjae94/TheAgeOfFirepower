using UnityEngine;
using VContainer;
using UniRx;
using Mathlife.ProjectL.Utils;
using UniRx.Triggers;
using UnityEngine.EventSystems;
using System;

namespace Mathlife.ProjectL.Gameplay
{
    public class CharacterScrollViewPresenter : Presenter
    {
        Transform m_content;
        CanvasGroup m_removeMemberGuide;

        ObservableDropTrigger m_dropTrigger;
        ObservablePointerClickTrigger m_pointerClickTrigger;

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
            TeamPage teamPage = m_worldSceneManager.GetPage<TeamPage>();

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
                .team.SubscribeMemberChange(_ => UpdateView())
                .AddTo(gameObject);

            m_worldSceneManager
                .GetPage<TeamPage>()
                .SubscribeDragMemberCardEvent(() => m_removeMemberGuide.Show(), () => m_removeMemberGuide.Hide())
                .AddTo(gameObject);

            // Render
            UpdateView();
        }

        new void UpdateView()
        {
            m_removeMemberGuide.Hide();

            foreach (Transform cardTrans in m_content)
            {
                Destroy(cardTrans.gameObject);
            }

            foreach (CharacterModel character in m_characterRepository.GetSortedList())
            {
                if (m_characterRepository.team.Contains(character))
                    continue;

                CharacterCardPresenter card = InstantiateWithInjection<CharacterCardPresenter>(EPrefabId.CharacterCard, m_content);
                card.Initialize(character);
            }
        }

        void OnDrop(PointerEventData eventData)
        {
            var newCharacter = eventData.pointerDrag?
                .GetComponent<CharacterCardPresenter>()?
                .GetCharacterModel();

            if (null == newCharacter)
                return;

            // 멤버를 스크롤 뷰에 놓는 경우만 처리
            if (m_characterRepository.team.Contains(newCharacter))
            {
                m_characterRepository.team.Remove(newCharacter);
            }
        }

        void OnPointerClick(PointerEventData eventData)
        {
            m_worldSceneManager.GetPage<TeamPage>().selectedCharacter = null;
        }
    }
}