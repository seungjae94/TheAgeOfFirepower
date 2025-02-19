using Mathlife.ProjectL.Utils;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    internal class PartySelectedCharacter : Presenter
    {
        [Inject] PartyPage m_partyPage;

        [SerializeField] CanvasGroup m_canvasGroup;
        [SerializeField] Image m_portraitImage;
        [SerializeField] TMP_Text m_levelText;
        [SerializeField] TMP_Text m_nameText;
        [SerializeField] Button m_detailInfoButton;
        [SerializeField] Button m_memberChangeButton;


        protected override void InitializeView()
        {
            if (m_partyPage.selectedCharacter.GetState() == null)
            {
                m_canvasGroup.Hide();
                return;
            }
            m_canvasGroup.Show();

            m_portraitImage.sprite = m_partyPage.selectedCharacter.GetState().portrait;
            m_levelText.text = m_partyPage.selectedCharacter.GetState().level.ToString();
            m_nameText.text = m_partyPage.selectedCharacter.GetState().displayName;
        }

        protected override void SubscribeUserInteractions()
        {
            m_detailInfoButton.OnClickAsObservable()
                .Subscribe(m_partyPage.OnClickDetailInfoButton)
                .AddTo(gameObject);

            m_memberChangeButton.OnClickAsObservable()
                .Subscribe(m_partyPage.OnClickPartyMemberChangeButton)
                .AddTo(gameObject);
        }
    }
}
