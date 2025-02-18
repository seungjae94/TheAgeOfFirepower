using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using Mathlife.ProjectL.Utils;
using UniRx;
using UniRx.Triggers;
using VContainer;
using UnityEngine.EventSystems;
using System;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class PartyPage : Page
    {
        public override EPageId pageId => EPageId.TeamPage;
        [Inject] CharacterRepository m_characterRepository;

        [SerializeField] NavigateBackBarView m_navigateBackBar;
        [SerializeField] Image m_background;
        [SerializeField] Button m_buildBestTeamButton;
        [SerializeField] Button m_excludeAllButton;
        [SerializeField] List<CharacterSlotPresenter> m_slots;
        [SerializeField] CharacterScrollViewPresenter m_scrollView;
        [SerializeField] TeamValidationModal m_teamValidationModal;

        // 상태 - 선택한 캐릭터
        ReactiveProperty<CharacterModel> m_selectedCharacter = new(null);
        public CharacterModel selectedCharacter { get => m_selectedCharacter.Value; set => m_selectedCharacter.Value = value; }

        public IDisposable SubscribeSelectedCharacterChangeEvent(Action<CharacterModel> action)
        {
            return m_selectedCharacter.Subscribe(action);
        }

        // 상태 - 드래그 상태
        BoolReactiveProperty m_isDraggingMemberCard = new(false);
        public bool isDraggingMemberCard
        {
            get => m_isDraggingMemberCard.Value;
            set => m_isDraggingMemberCard.Value = value;
        }
        public CompositeDisposable SubscribeDragMemberCardEvent(Action beginDragAction, Action endDragAction)
        {
            CompositeDisposable subscriptions = new();

            IDisposable beginDragSubscription = m_isDraggingMemberCard
                .Where(v => v)
                .Subscribe(_ => beginDragAction?.Invoke());

            IDisposable endDragSubscription = m_isDraggingMemberCard
                .Where(v => !v)
                .Subscribe(_ => endDragAction?.Invoke());

            subscriptions.Add(beginDragSubscription);
            subscriptions.Add(endDragSubscription);
            return subscriptions;
        }

        // 초기화
        public override void Initialize()
        {
            InitializeChildren();

            Close();
        }

        protected override void InitializeChildren()
        {
            m_navigateBackBar.Initialize(OnClickBackButton);

            m_background
                .OnPointerClickAsObservable()
                .Subscribe(OnClickBackground)
                .AddTo(gameObject);

            m_buildBestTeamButton
                .OnClickAsObservable()
                .Subscribe(OnClickBuildBestTeamButton)
                .AddTo(gameObject);

            m_excludeAllButton
                .OnClickAsObservable()
                .Subscribe(OnClickExcludeAllButton)
                .AddTo(gameObject);

            for (int i = 0; i < Constants.TeamMemberMaxCount; ++i)
            {
                m_slots[i].Initialize();
            }

            m_scrollView.Initialize();

            m_teamValidationModal.Initialize();
        }

        // 유저 상호 작용
        async void OnClickBackButton()
        {
            if (m_characterRepository.party.Validate())
                m_worldSceneManager.NavigateBack();
            else
                await m_teamValidationModal.Show();
        }

        void OnClickBackground(PointerEventData _)
        {
            selectedCharacter = null;
        }

        void OnClickExcludeAllButton(Unit _)
        {
            m_characterRepository.party.Clear();
        }

        void OnClickBuildBestTeamButton(Unit _)
        {
            m_characterRepository.BuildBestTeam();
        }
    }
}