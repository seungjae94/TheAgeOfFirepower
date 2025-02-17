using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class CharacterCardPresenter : Presenter
    {
        CanvasGroup m_canvasGroup;
        Image m_portraitImage;
        TMP_Text m_levelText;
        CanvasGroup m_leaderMark;

        CanvasGroup m_selectionOverlayCanvasGroup;
        Button m_detailInfoButton;
        Button m_leaderAppointmentButton;

        RectTransform m_dragArea;

        ObservableBeginDragTrigger m_beginDragTrigger;
        ObservableDragTrigger m_dragTrigger;
        ObservableEndDragTrigger m_endDragTrigger;
        ObservablePointerClickTrigger m_pointerClickTrigger;

        CharacterModel m_character = null;
        DragCharacterCardPresenter m_dragCard = null;

        [Inject] CharacterRepository m_characterRepository;

        void Awake()
        {
            // Views
            m_canvasGroup = GetComponent<CanvasGroup>();
            m_portraitImage = transform.FindRecursiveByName<Image>("Portrait");
            m_levelText = transform.FindRecursiveByName<TMP_Text>("Level Text");

            m_selectionOverlayCanvasGroup = transform.FindRecursiveByName<CanvasGroup>("Selection Overlay");
            m_detailInfoButton = transform.FindRecursiveByName<Button>("Detail Info Button");
            m_leaderAppointmentButton = transform.FindRecursiveByName<Button>("Leader Appointment Button");

            m_leaderMark = transform.FindRecursiveByName<CanvasGroup>("Leader Mark");
            m_dragArea = transform.root.FindRecursive<PartyPage>()
                .transform.FindRecursiveByName<RectTransform>("Drag Area");

            // Event Triggers
            m_beginDragTrigger = GetComponent<ObservableBeginDragTrigger>();
            m_dragTrigger = GetComponent<ObservableDragTrigger>();
            m_endDragTrigger = GetComponent<ObservableEndDragTrigger>();
            m_pointerClickTrigger = GetComponent<ObservablePointerClickTrigger>();
        }

        public void Initialize(CharacterModel character)
        {
            m_character = character;

            // Subscribe user interactions
            m_beginDragTrigger
                .OnBeginDragAsObservable()
                .Subscribe(OnBeginDrag)
                .AddTo(gameObject);

            m_dragTrigger
                .OnDragAsObservable()
                .Subscribe(OnDrag)
                .AddTo(gameObject);

            m_endDragTrigger
                .OnEndDragAsObservable()
                .Subscribe(OnEndDrag)
                .AddTo(gameObject);

            m_pointerClickTrigger
                .OnPointerClickAsObservable()
                .Subscribe(OnPointerClick)
                .AddTo(gameObject);

            m_detailInfoButton
                .OnClickAsObservable()
                .Subscribe(_ => OnClickDetailInfoButton())
                .AddTo(gameObject);

            m_leaderAppointmentButton
                .OnClickAsObservable()
                .Subscribe(_ => OnClickLeaderAppointmentButton())
                .AddTo(gameObject);

            // Subscribe model change events
            character.SubscribeLevelChangeEvent(OnLevelChanged)
                .AddTo(gameObject);

            m_characterRepository.team
                .SubscribeLeaderChangeEvent(OnLeaderChanged)
                .AddTo(gameObject);

            m_worldSceneManager
                .GetPage<PartyPage>()
                .SubscribeSelectedCharacterChangeEvent(OnSelectedCharacterChanged)
                .AddTo(gameObject);

            // Render
            InitializeView();
        }

        new void InitializeView()
        {
            m_portraitImage.sprite = m_character.portrait;
            m_levelText.text = m_character.level.ToString();

            m_selectionOverlayCanvasGroup.Hide();

            // 리더 마크 및 리더 위임 버튼
            CharacterModel leader = m_characterRepository.team.leader;
            if (leader == m_character)
            {
                m_leaderMark.Show();
            }
            else if (m_characterRepository.team.Contains(m_character))
            {
                m_leaderMark.Hide();
                m_leaderAppointmentButton.gameObject.SetActive(true);
            }
            else
            {
                m_leaderMark.Hide();
                m_leaderAppointmentButton.gameObject.SetActive(false);
            }
        }

        public CharacterModel GetCharacterModel()
        {
            return m_character;
        }

        void OnBeginDrag(PointerEventData eventData)
        {
            m_dragCard = InstantiateWithInjection<DragCharacterCardPresenter>(EPrefabId.DragCharacterCard, m_dragArea);
            m_dragCard.Initialize(m_character);
            m_dragCard.transform.position = transform.position;

            m_canvasGroup.alpha = 0.25f;
            m_canvasGroup.blocksRaycasts = false;
            m_canvasGroup.interactable = false;

            if (m_characterRepository.team.Contains(m_character))
                m_worldSceneManager.GetPage<PartyPage>().isDraggingMemberCard = true;
        }

        void OnDrag(PointerEventData eventData)
        {
            m_dragCard.GetComponent<RectTransform>().position = eventData.position;
        }

        void OnEndDrag(PointerEventData eventData)
        {
            if (m_worldSceneManager.GetPage<PartyPage>().isDraggingMemberCard)
                m_worldSceneManager.GetPage<PartyPage>().isDraggingMemberCard = false;

            Destroy(m_dragCard.gameObject);
            m_canvasGroup.alpha = 1.0f;
            m_canvasGroup.blocksRaycasts = true;
            m_canvasGroup.interactable = true;
        }

        void OnPointerClick(PointerEventData eventData)
        {
            m_worldSceneManager.GetPage<PartyPage>().selectedCharacter = m_character;
        }

        void OnClickDetailInfoButton()
        {
            m_worldSceneManager.Navigate(EPageId.CharacterPage);
        }

        void OnClickLeaderAppointmentButton()
        {
            m_characterRepository.team.AppointLeader(m_character);
            m_worldSceneManager.GetPage<PartyPage>().selectedCharacter = null;
        }

        void OnLevelChanged(int value)
        {
            m_levelText.text = value.ToString();
        }

        void OnLeaderChanged(CharacterModel leader)
        {
            if (leader == m_character)
                m_leaderMark.Show();
            else
                m_leaderMark.Hide();
        }

        void OnSelectedCharacterChanged(CharacterModel selectedCharacter)
        {
            // This character is selected.
            if (m_character == selectedCharacter)
            {
                m_selectionOverlayCanvasGroup.Show();
            }
            // Another character is selected.
            else
            {
                m_selectionOverlayCanvasGroup.Hide();
            }
        }
    }
}