using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    [RequireComponent(typeof(CanvasGroup))]
    internal class PartyMemberChangeModal : Presenter
    {
        const float k_fadeTime = 0.25f;

        [Inject] PartyPage m_partyPage;

        CanvasGroup m_canvasGroup;
        [SerializeField] Button m_closeButton;
        [SerializeField] CharacterSelectionFlex m_flex;

        int m_selectedSlotIndex;

        void Awake()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();    
        }

        public async UniTask Show()
        {
            // TODO: 이동식으로 구현

            m_selectedSlotIndex = m_partyPage.GetSelectedSlotIndex();
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
        }
    }
}
