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
        [Inject] MainSceneManager m_mainSceneManager;
        [Inject] CharacterRepository m_characterRepository;

        CanvasGroup m_canvasGroup;
        Button m_okButton;
        Button m_cancelButton;

        const float k_fadeTime = 0.25f;

        void Awake()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
            m_okButton = transform.FindRecursiveByName<Button>("OK Button");
            m_cancelButton = transform.FindRecursiveByName<Button>("Cancel Button");
        }

        public void Initialize()
        {
            m_okButton.OnClickAsObservable()
                .Subscribe(OnClickOKButton)
                .AddTo(gameObject);

            m_cancelButton.OnClickAsObservable()
                .Subscribe(OnClickCancelButton)
                .AddTo(gameObject);

            m_canvasGroup.Hide();
        }

        async void OnClickOKButton(Unit _)
        {
            m_characterRepository.BuildBestTeam();

            await m_canvasGroup.Hide(k_fadeTime);
            
            m_mainSceneManager.NavigateBack();
        }

        void OnClickCancelButton(Unit _)
        {
            UniTask task = m_canvasGroup.Hide(k_fadeTime);
        }

        public async UniTask Show()
        {
            await m_canvasGroup.Show(k_fadeTime);
        }

        protected override void SubscribeDataChange()
        {
            throw new NotImplementedException();
        }

        protected override void SubscribeUserInteractions()
        {
            throw new NotImplementedException();
        }

        protected override void InitializeView()
        {
            throw new NotImplementedException();
        }
    }
}
