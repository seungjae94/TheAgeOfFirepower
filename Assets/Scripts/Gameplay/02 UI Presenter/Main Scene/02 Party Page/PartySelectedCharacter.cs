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
        [Inject] CharacterRepository m_characterRepository;

        [SerializeField] CanvasGroup m_canvasGroup;
        [SerializeField] Image m_portraitImage;
        [SerializeField] TMP_Text m_levelText;
        [SerializeField] TMP_Text m_nameText;
        [SerializeField] Button m_detailInfoButton;
        [SerializeField] Button m_memberChangeButton;

        IDisposable m_characterSub;

        protected override void InitializeView()
        {
            UpdateView();
        }

        protected override void SubscribeDataChange()
        {
            m_partyPage.selectedSlotIndex
                .SubscribeChangeEvent(OnSelectedSlotIndexChange)
                .AddTo(gameObject);
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

        // 데이터 변경 구독 메서드
        void OnSelectedSlotIndexChange(int selectedSlotIndex)
        {
            if (m_characterSub != null)
                m_characterSub.Dispose();

            if (m_partyPage.IsSelectedSlotIndexInRange() == true && m_partyPage.GetSelectedCharacter() != null)
            {
                m_characterSub = m_partyPage.GetSelectedCharacter()
                    .SubscribeLevelChangeEvent(level => m_levelText.text = level.ToString());
            }

            UpdateView();
        }

        // 뷰 업데이트
        void UpdateView()
        {
            if (m_partyPage.IsSelectedSlotIndexInRange() == false || m_partyPage.GetSelectedCharacter() == null)
            {
                m_canvasGroup.Hide();
                return;
            }
            m_canvasGroup.Show();

            m_portraitImage.sprite = m_partyPage.GetSelectedCharacter().portrait;
            m_levelText.text = m_partyPage.GetSelectedCharacter().level.ToString();
            m_nameText.text = m_partyPage.GetSelectedCharacter().displayName;
        }
    }
}
