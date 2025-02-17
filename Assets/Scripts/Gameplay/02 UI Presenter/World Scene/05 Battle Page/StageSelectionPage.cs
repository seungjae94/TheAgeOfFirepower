using Mathlife.ProjectL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class StageSelectionPage : Page
    {
        Image m_portrait;
        TMP_Text m_battleNameText;

        Button m_enterBattleButton;
        Button m_backButton;

        CompositeDisposable m_subscriptions_page = new();

        public override EPageId pageId => EPageId.StageSelectionPage;

        protected override void Awake()
        {
            base.Awake();

            m_portrait = transform.FindRecursiveByName<Image>("Portrait");
            m_battleNameText = transform.FindRecursiveByName<TMP_Text>("Battle Name Text");

            m_enterBattleButton = transform.FindRecursiveByName<Button>("Enter Button");
            m_backButton = transform.FindRecursiveByName<Button>("Back Button");
        }

        public override void Initialize()
        {
            m_backButton
                .OnClickAsObservable()
                .Subscribe(_ => OnClickBackButton())
                .AddTo(m_subscriptions_page);

            InitializeChildren();

            Close();
        }

        void OnClickBackButton()
        {
            m_worldSceneManager.NavigateBack();
        }

        protected override void InitializeChildren()
        {
            
        }
    }
}
