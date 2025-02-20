using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PartyMemberChangeModal : Presenter
    {
        const float k_fadeTime = 0.25f;

        [Inject] PartyPage m_partyPage;
        [Inject] CharacterRepository m_characterRepository;

        CanvasGroup m_canvasGroup;
        [SerializeField] Button m_closeButton;
        [SerializeField] ObservablePointerClickTrigger m_excludeButton;
        [SerializeField] CharacterSelectionFlex m_flex;

        int m_selectedSlotIndex;

        void Awake()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();    
        }

        public async UniTask Show()
        {
            // TODO: 이동식으로 구현

            m_selectedSlotIndex = m_partyPage.selectedSlotIndex.GetState();

            if (m_partyPage.IsSelectedSlotIndexInRange() == false || m_characterRepository.party[m_selectedSlotIndex] != null)
            {
                m_excludeButton.gameObject.SetActive(true);
            }
            else
            {
                m_excludeButton.gameObject.SetActive(false);
            }

            await m_canvasGroup.Show(k_fadeTime);
        }

        public async UniTask Hide()
        {
            // TODO: 이동식으로 구현

            await m_canvasGroup.Hide(k_fadeTime);
        }

        protected override void InitializeView()
        {
            m_canvasGroup.Hide();
        }

        protected override void InitializeChildren()
        {
            m_flex.Initialize();
        }

        protected override void SubscribeUserInteractions()
        {
            m_closeButton.OnClickAsObservable()
                .Subscribe(async _ => await Hide())
                .AddTo(gameObject);

            m_excludeButton.OnPointerClickAsObservable()
                .Subscribe(OnClickExcludeButton)
                .AddTo(gameObject);
        }

        async void OnClickExcludeButton(PointerEventData ev)
        {
            m_partyPage.selectedSlotIndex.SetState(m_selectedSlotIndex);
            m_characterRepository.party.RemoveAt(m_selectedSlotIndex);
            await m_partyPage.partyMemberChangeModal.Hide();
        }
    }
}
