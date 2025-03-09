using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    internal class SimpleActionButton : Presenter<Action>
    {
        [SerializeField] Button m_backButton;
        Action m_navigateBackAction;

        protected override void Store(Action navigateBackAction)
        {
            m_navigateBackAction = navigateBackAction;
        }

        protected override void SubscribeUserInteractions()
        {
            m_backButton.OnClickAsObservable()
                .Subscribe(_ => m_navigateBackAction())
                .AddTo(gameObject);
        }
    }
}
