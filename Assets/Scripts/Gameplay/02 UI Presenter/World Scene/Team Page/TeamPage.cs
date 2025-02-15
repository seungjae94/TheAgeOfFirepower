using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using Mathlife.ProjectL.Utils;
using UniRx;
using UniRx.Triggers;
using VContainer;
using UnityEngine.EventSystems;
using System;

namespace Mathlife.ProjectL.Gameplay
{
    public class TeamPage : Page
    {
        #region View
        Button m_backButton;
        Image m_background;
        Button m_buildBestTeamButton;
        Button m_excludeAllButton;

        List<CharacterSlotPresenter> m_slots;
        CharacterScrollViewPresenter m_scrollView;
        TeamValidationModal m_teamValidationModal;
        #endregion

        [Inject] CharacterRepository m_characterRepository;
        public override EPageId pageId => EPageId.TeamPage;

        #region State - Selected Character
        ReactiveProperty<CharacterModel> m_selectedCharacter = new(null);
        public CharacterModel selectedCharacter { get => m_selectedCharacter.Value; set => m_selectedCharacter.Value = value; }

        public IDisposable SubscribeSelectedCharacterChangeEvent(Action<CharacterModel> action)
        {
            return m_selectedCharacter.Subscribe(action);
        }
        #endregion

        #region State - isDraggingMemberCard
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
        #endregion

        protected override void Awake()
        {
            base.Awake();

            m_backButton = transform.FindRecursiveByName<Button>("Back Button");
            m_background = transform.FindRecursiveByName<Image>("Background");
            m_buildBestTeamButton = transform.FindRecursiveByName<Button>("Build Best Team Button");
            m_excludeAllButton = transform.FindRecursiveByName<Button>("Exclude All Button");

            m_slots = transform.FindAllRecursive<CharacterSlotPresenter>();
            m_scrollView = transform.FindRecursive<CharacterScrollViewPresenter>();
            m_teamValidationModal = transform.FindRecursive<TeamValidationModal>();
        }

        public override void Initialize()
        {
            // Subscribe Event Triggers
            m_backButton
                .OnClickAsObservable()
                .Subscribe(OnClickBackButton)
                .AddTo(gameObject);

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

            InitializeChildren();

            Close();
        }

        async void OnClickBackButton(Unit _)
        {
            if (m_characterRepository.team.Validate())
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
            m_characterRepository.team.Clear();
        }

        void OnClickBuildBestTeamButton(Unit _)
        {
            m_characterRepository.BuildBestTeam();
        }

        protected override void InitializeChildren()
        {
            for (int i = 0; i < Constants.TeamMemberMaxCount; ++i)
            {
                m_slots[i].Initialize();
            }

            m_scrollView.Initialize();

            m_teamValidationModal.Initialize();
        }
    }
}