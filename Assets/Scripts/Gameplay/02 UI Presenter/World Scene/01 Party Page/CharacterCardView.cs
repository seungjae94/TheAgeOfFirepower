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
    public class CharacterCardView : MonoBehaviour
    {
        [SerializeField] CanvasGroup m_canvasGroup;
        [SerializeField] Image m_portraitImage;
        [SerializeField] TMP_Text m_nameText;
        [SerializeField] TMP_Text m_levelText;
        [SerializeField] CanvasGroup m_selectionOverlayCanvasGroup;
        [SerializeField] Button m_detailInfoButton;
        [SerializeField] RectTransform m_dragArea;
        [SerializeField] ObservableBeginDragTrigger m_beginDragTrigger;
        [SerializeField] ObservableDragTrigger m_dragTrigger;
        [SerializeField] ObservableEndDragTrigger m_endDragTrigger;
        [SerializeField] ObservablePointerClickTrigger m_pointerClickTrigger;

        CharacterModel m_character = null;
        DragCharacterCardView m_dragCard = null;

        public void Render(CharacterModel character)
        {
            m_character = character;

            m_portraitImage.sprite = character.portrait;
            m_levelText.text = character.level.ToString();
            m_nameText.text = character.displayName;
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

            // Subscribe model change events
            character.SubscribeLevelChangeEvent(OnLevelChanged)
                .AddTo(gameObject);

            //m_worldSceneManager
            //    .GetPage<PartyPage>()
            //    .SubscribeSelectedCharacterChangeEvent(OnSelectedCharacterChanged)
            //    .AddTo(gameObject);

            // Render
            InitializeView();
        }

        new void InitializeView()
        {
            m_portraitImage.sprite = m_character.portrait;
            m_levelText.text = m_character.level.ToString();

            m_selectionOverlayCanvasGroup.Hide();
        }

        public CharacterModel GetCharacterModel()
        {
            return m_character;
        }

        void OnBeginDrag(PointerEventData eventData)
        {
            //m_dragCard = InstantiateWithInjection<DragCharacterCardPresenter>(EPrefabId.DragCharacterCard, m_dragArea);
            //m_dragCard.Initialize(m_character);
            //m_dragCard.transform.position = transform.position;

            //m_canvasGroup.alpha = 0.25f;
            //m_canvasGroup.blocksRaycasts = false;
            //m_canvasGroup.interactable = false;

            //if (m_characterRepository.team.Contains(m_character))
            //    m_worldSceneManager.GetPage<PartyPage>().isDraggingMemberCard = true;
        }

        void OnDrag(PointerEventData eventData)
        {
            m_dragCard.GetComponent<RectTransform>().position = eventData.position;
        }

        void OnEndDrag(PointerEventData eventData)
        {
            //if (m_worldSceneManager.GetPage<PartyPage>().isDraggingMemberCard)
            //    m_worldSceneManager.GetPage<PartyPage>().isDraggingMemberCard = false;

            //Destroy(m_dragCard.gameObject);
            //m_canvasGroup.alpha = 1.0f;
            //m_canvasGroup.blocksRaycasts = true;
            //m_canvasGroup.interactable = true;
        }

        void OnPointerClick(PointerEventData eventData)
        {
            //m_worldSceneManager.GetPage<PartyPage>().selectedCharacter = m_character;
        }

        void OnClickDetailInfoButton()
        {
            //m_worldSceneManager.Navigate(EPageId.CharacterPage);
        }

        void OnLevelChanged(int value)
        {
            m_levelText.text = value.ToString();
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