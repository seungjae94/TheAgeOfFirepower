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
    public class DragCharacterCardView : MonoBehaviour
    {
        [SerializeField] CanvasGroup m_canvasGroup;
        [SerializeField] Image m_portraitImage;
        //[SerializeField] TMP_Text m_levelText;
        //[SerializeField] TMP_Text m_nameText;

        CharacterModel m_character = null;

        public void Initialize(CharacterModel character)
        {
            m_character = character;

            // Initialize View
            InitializeView();
        }

        new void InitializeView()
        {
            m_portraitImage.sprite = m_character.portrait;
            //m_levelText.text = m_character.level.ToString();
        }

        public CharacterModel GetCharacterModel()
        {
            return m_character;
        }
    }
}