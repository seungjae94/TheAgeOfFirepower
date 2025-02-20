using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    class CharacterSelectionFlexItemFactory
    : IFlexItemFactory<CharacterModel, CharacterSelectionFlexItem>
    {
        [Inject] IObjectResolver m_container;
        [Inject] GameDataDB m_gameDataDB;

        public CharacterSelectionFlexItem Create(Transform parent, CharacterModel character)
        {
            CharacterSelectionFlexItem flexItem
                = m_gameDataDB.Instantiate<CharacterSelectionFlexItem>(EPrefabId.CharacterSelectionFlexItem, parent);

            flexItem.Initialize(character);

            m_container.Inject(flexItem);

            return flexItem;
        }
    }

    [RequireComponent(typeof(ObservablePointerClickTrigger))]
    class CharacterSelectionFlexItem : Presenter<CharacterModel>
    {
        [Inject] PartyPage m_partyPage;
        [Inject] CharacterRepository m_characterRepository;

        ObservablePointerClickTrigger m_clickTrigger;
        [SerializeField] Image m_portraitImage;
        [SerializeField] TMP_Text m_levelText;

        CharacterModel m_character;

        void Awake()
        {
            m_clickTrigger = GetComponent<ObservablePointerClickTrigger>(); 
        }

        protected override void Store(CharacterModel character)
        {
            m_character = character;
        }

        protected override void SubscribeDataChange()
        {
            m_character.SubscribeLevelChangeEvent(UpdateLevelText)
                .AddTo(gameObject);
        }

        protected override void SubscribeUserInteractions()
        {
            m_clickTrigger.OnPointerClickAsObservable()
                .Subscribe(OnClick)
                .AddTo(gameObject);
        }

        protected override void InitializeView()
        {
            m_portraitImage.sprite = m_character.portrait;
            UpdateLevelText(m_character.level);
        }

        // 상호작용
        async void OnClick(PointerEventData ev)
        {
            m_characterRepository.party.Add(m_partyPage.selectedSlotIndex.GetState(), m_character);
            m_partyPage.selectedSlotIndex.SetState(-1);

            await m_partyPage.partyMemberChangeModal.Hide();
        }

        // 뷰 업데이트
        void UpdateLevelText(int level)
        {
            m_levelText.text = m_character.level.ToString();
        }
    }
}
