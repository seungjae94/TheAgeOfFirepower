using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class CharacterPage : Page
    {
        public override EPageId pageId => EPageId.CharacterPage;

        [Inject] MainSceneManager m_mainSceneManager;

        [SerializeField] Button m_navBackButton;



        protected override void SubscribeUserInteractions()
        {
            m_navBackButton.OnClickAsObservable()
                .Subscribe(_ => m_mainSceneManager.NavigateBack())
                .AddTo(gameObject);
        }

    }
}
