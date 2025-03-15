using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public class BatteryPageArtySlotDragItem : MonoBehaviour
    {
        [SerializeField] Image m_portraitImage;
        [SerializeField] TMP_Text m_levelText;
        [SerializeField] TMP_Text m_nameText;

        ArtyModel arty = null;

        // protected override void Store(CharacterModel character)
        // {
        //     m_character = character;
        // }
        //
        // protected override void InitializeView()
        // {
        //     m_portraitImage.sprite = m_character.Sprite;
        //     m_levelText.text = m_character.levelRx.ToString();
        //     m_nameText.text = m_character.displayName;
        // }

        public ArtyModel GetCharacterModel()
        {
            return arty;
        }
    }
}