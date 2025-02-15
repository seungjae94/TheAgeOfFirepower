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
    public class DragCharacterCardPresenter : Presenter
    {
        #region View
        CanvasGroup m_canvasGroup;
        Image m_portraitImage;
        TMP_Text m_levelText;
        #endregion

        #region Field
        CharacterModel m_character = null;
        #endregion

        void Awake()
        {
            // Views
            m_canvasGroup = GetComponent<CanvasGroup>();
            m_portraitImage = transform.FindRecursiveByName<Image>("Portrait");
            m_levelText = transform.FindRecursiveByName<TMP_Text>("Level Text");
        }

        public void Initialize(CharacterModel character)
        {
            m_character = character;

            // Initialize View
            InitializeView();
        }

        new void InitializeView()
        {
            m_portraitImage.sprite = m_character.portrait;
            m_levelText.text = m_character.level.ToString();
        }

        public CharacterModel GetCharacterModel()
        {
            return m_character;
        }
    }
}