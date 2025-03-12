using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mathlife.ProjectL.Utils;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class PartyValidationModal : Presenter
    {
        [Inject] LobbySceneGameMode lobbySceneGameMode;
        [Inject] CharacterRosterState characterRosterState;

        CanvasGroup m_canvasGroup;
        [SerializeField] Button m_okButton;
        [SerializeField] Button m_cancelButton;

        const float k_fadeTime = 0.25f;

        void Awake()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void SubscribeUserInteractions()
        {
            m_okButton.OnClickAsObservable()
                .Subscribe(OnClickOKButton)
                .AddTo(gameObject);

            m_cancelButton.OnClickAsObservable()
                .Subscribe(OnClickCancelButton)
                .AddTo(gameObject);
        }

        protected override void InitializeView()
        {
            m_canvasGroup.Hide();
        }


        async void OnClickOKButton(Unit _)
        {
            characterRosterState.BuildBestTeam();

            await m_canvasGroup.Hide(k_fadeTime);
            
            lobbySceneGameMode.NavigateBack();
        }

        void OnClickCancelButton(Unit _)
        {
            UniTask task = m_canvasGroup.Hide(k_fadeTime);
        }

        public async UniTask Show()
        {
            await m_canvasGroup.Show(k_fadeTime);
        }
    }
}
