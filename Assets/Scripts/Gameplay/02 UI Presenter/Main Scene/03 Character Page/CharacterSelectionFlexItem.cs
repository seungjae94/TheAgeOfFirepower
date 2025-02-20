using System;
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
    : IFlexItemFactory<CharacterModel, Action<CharacterModel>, CharacterSelectionFlexItem>
    {
        [Inject] IObjectResolver m_container;
        [Inject] GameDataDB m_gameDataDB;

        public CharacterSelectionFlexItem Create(Transform parent, CharacterModel character, Action<CharacterModel> onClick)
        {
            CharacterSelectionFlexItem flexItem
                = m_gameDataDB.Instantiate<CharacterSelectionFlexItem>(EPrefabId.CharacterSelectionFlexItem, parent);

            flexItem.Initialize(character, onClick);

            m_container.Inject(flexItem);

            return flexItem;
        }
    }


    [RequireComponent(typeof(ObservablePointerClickTrigger))]
    class CharacterSelectionFlexItem : Presenter<CharacterModel, Action<CharacterModel>>
    {
        ObservablePointerClickTrigger m_clickTrigger;
        [SerializeField] Image m_portraitImage;
        [SerializeField] TMP_Text m_levelText;
        [SerializeField] TMP_Text m_nameText;

        CharacterModel m_character;
        Action<CharacterModel> m_onClick;

        void Awake()
        {
            m_clickTrigger = GetComponent<ObservablePointerClickTrigger>(); 
        }

        protected override void Store(CharacterModel character, Action<CharacterModel> onClick)
        {
            m_character = character;
            m_onClick = onClick;
        }

        protected override void SubscribeDataChange()
        {
            m_character.SubscribeLevelChangeEvent(UpdateLevelText)
                .AddTo(gameObject);
        }

        protected override void SubscribeUserInteractions()
        {
            m_clickTrigger.OnPointerClickAsObservable()
                .Subscribe(ev => m_onClick(m_character))
                .AddTo(gameObject);
        }

        protected override void InitializeView()
        {
            m_portraitImage.sprite = m_character.portrait;
            m_nameText.text = m_character.displayName;
            UpdateLevelText(m_character.level);
        }

        // 뷰 업데이트
        void UpdateLevelText(int level)
        {
            m_levelText.text = m_character.level.ToString();
        }
    }
}
