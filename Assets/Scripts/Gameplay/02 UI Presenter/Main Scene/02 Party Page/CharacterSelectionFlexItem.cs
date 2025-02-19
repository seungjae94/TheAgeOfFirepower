using Mathlife.ProjectL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
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

    class CharacterSelectionFlexItem : Presenter<CharacterModel>
    {
        [SerializeField] Image m_portraitImage;
        [SerializeField] TMP_Text m_levelText;
        [SerializeField] Button m_button;
        [SerializeField] CanvasGroup m_overlayCanvasGroup;

        [SerializeField] Button m_detailButton;
        [SerializeField] Button m_includeButton;

        CharacterModel m_character;

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
            m_button.OnClickAsObservable()
                .Subscribe(_ => m_overlayCanvasGroup.Show())
                .AddTo(gameObject);

            // On Select <-- 얘는 바깥에서 입력받아서...
        }

        protected override void InitializeView()
        {
            m_portraitImage.sprite = m_character.portrait;
            UpdateLevelText(m_character.level);
        }

        // 뷰 업데이트
        void UpdateLevelText(int level)
        {
            m_levelText.text = m_character.level.ToString();
        }
    }
}
