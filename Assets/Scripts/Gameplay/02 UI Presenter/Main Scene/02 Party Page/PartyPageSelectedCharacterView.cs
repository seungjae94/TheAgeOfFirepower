using Mathlife.ProjectL.Utils;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    internal class PartyPageSelectedCharacterView : MonoBehaviour
    {
        [SerializeField] CanvasGroup m_canvasGroup;
        [SerializeField] Image m_portraitImage;
        [SerializeField] TMP_Text m_levelText;
        [SerializeField] TMP_Text m_nameText;
        [SerializeField] Button m_detailInfoButton;
        [SerializeField] Button m_memberChangeButton;

        public void BindEvents(Action onClickDetailInfoButton, Action onClickPartyMemberChangeButton)
        {
            m_detailInfoButton.OnClickAsObservable()
                .Subscribe(_ => onClickDetailInfoButton())
                .AddTo(gameObject);

            m_memberChangeButton.OnClickAsObservable()
                .Subscribe(_ => onClickPartyMemberChangeButton())
                .AddTo(gameObject);
        }

        public void Render(CharacterModel character)
        {
            if (character == null)
            {
                m_canvasGroup.Hide();
                return;
            }

            m_canvasGroup.Show();

            m_portraitImage.sprite = character.portrait;
            m_levelText.text = character.level.ToString();
            m_nameText.text = character.displayName;

            m_detailInfoButton.onClick.GetPersistentEventCount();
        }
    }
}
