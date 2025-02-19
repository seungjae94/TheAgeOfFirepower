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
    public class PartyMemberSlotDragItem : Presenter<CharacterModel>
    {
        [SerializeField] Image m_portraitImage;
        [SerializeField] TMP_Text m_levelText;
        [SerializeField] TMP_Text m_nameText;

        CharacterModel m_character = null;

        protected override void Store(CharacterModel character)
        {
            m_character = character;
        }

        protected override void InitializeView()
        {
            m_portraitImage.sprite = m_character.portrait;
            m_levelText.text = m_character.level.ToString();
            m_nameText.text = m_character.displayName;
        }

        public CharacterModel GetCharacterModel()
        {
            return m_character;
        }
    }
}