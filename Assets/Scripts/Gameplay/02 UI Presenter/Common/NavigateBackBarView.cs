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
    internal class NavigateBackBarView : MonoBehaviour
    {
        [SerializeField] Button m_backButton;

        public void Initialize(Action navigateBackAction)
        {
            m_backButton.OnClickAsObservable()
                .Subscribe(_ => navigateBackAction())
                .AddTo(gameObject);
        }
    }
}
