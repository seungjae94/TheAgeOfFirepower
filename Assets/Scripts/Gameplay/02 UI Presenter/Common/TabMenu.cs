using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Utils;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public class TabMenu : MonoBehaviour
    {
        [SerializeField] Button m_button;
        [SerializeField] CanvasGroup m_defaultView;
        [SerializeField] CanvasGroup m_selectedView;

        int m_index = 0;

        void Awake()
        {
            m_index = transform.parent.GetSiblingIndex();
        }


        public void BindSelectAction(Action<int> selectAction)
        {
            m_button.OnClickAsObservable()
                .Subscribe(_ => selectAction(m_index))
                .AddTo(gameObject);
        }

        public void Default()
        {
            m_defaultView.Show();
            m_selectedView.Hide();
        }

        public void Select()
        {
            m_defaultView.Hide();
            m_selectedView.Show();
        }
    }
}
