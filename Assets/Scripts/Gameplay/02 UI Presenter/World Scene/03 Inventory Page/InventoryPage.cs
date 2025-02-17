using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public class InventoryPage : Page
    {
        public override EPageId pageId => EPageId.InventoryPage;

        [SerializeField] Button m_backButton;
        [SerializeField] Transform m_tabMenuBar;
        List<TabMenu> m_tabMenus = new();

        int m_selectedTab = 0;

        protected override void Awake()
        {
            base.Awake();

            foreach (Transform tabMenuTrans in m_tabMenuBar)
            {
                m_tabMenus.Add(tabMenuTrans.GetComponent<TabMenu>());
            }
        }

        public override void Initialize()
        {
            m_backButton.OnClickAsObservable()
                .Subscribe(OnClickBackButton)
                .AddTo(gameObject);

            InitializeChildren();
            Close();
        }

        protected override void InitializeChildren()
        {
            InitializeTabMenu();
        }

        void InitializeTabMenu()
        {
            m_tabMenus[0].BindSelectAction(0, OnSelectTabMenu);
            m_tabMenus[0].Select();

            for (int i = 1; i < m_tabMenus.Count; ++i)
            {
                m_tabMenus[i].BindSelectAction(i, OnSelectTabMenu);
                m_tabMenus[i].Default();
            }
        }

        void OnClickBackButton(Unit _)
        {
            m_worldSceneManager.NavigateBack();
        }

        void OnSelectTabMenu(int index)
        {
            m_tabMenus[m_selectedTab].Default();
            m_tabMenus[index].Select();
            
            m_selectedTab = index;
        }
    }
}