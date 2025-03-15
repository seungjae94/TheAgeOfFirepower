// using Cysharp.Threading.Tasks;
// using Mathlife.ProjectL.Utils;
// using System;
// using UniRx;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace Mathlife.ProjectL.Gameplay
// {
//     public class TabMenu : Presenter<int, Action<int>>
//     {
//         [SerializeField] Button m_button;
//         [SerializeField] CanvasGroup m_defaultView;
//         [SerializeField] CanvasGroup m_selectedView;
//
//         int m_index;
//         Action<int> m_selectAction;
//
//         protected override void Store(int index, Action<int> selectAction)
//         {
//             m_index = index;
//             m_selectAction = selectAction;
//         }
//
//         protected override void SubscribeUserInteractions()
//         {
//             m_button.OnClickAsObservable()
//                 .Subscribe(_ => m_selectAction(m_index))
//                 .AddTo(gameObject);
//         }
//
//         public void Default()
//         {
//             m_defaultView.Show();
//             m_selectedView.Hide();
//         }
//
//         public void Select()
//         {
//             m_defaultView.Hide();
//             m_selectedView.Show();
//         }
//     }
// }
