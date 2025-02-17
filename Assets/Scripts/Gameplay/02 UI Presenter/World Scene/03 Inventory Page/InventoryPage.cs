using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class InventoryPage : Page
    {
        public override EPageId pageId => EPageId.InventoryPage;

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
            InitializeChildren();
            Close();
        }

        protected override void InitializeChildren()
        {
            InitializeTabMenu();
        }

        void InitializeTabMenu()
        {
            m_tabMenus[0].BindSelectAction(OnSelectTabMenu);
            m_tabMenus[0].Select();

            for (int i = 1; i < m_tabMenus.Count; ++i)
            {
                m_tabMenus[i].BindSelectAction(OnSelectTabMenu);
                m_tabMenus[i].Default();
            }
        }

        void OnSelectTabMenu(int index)
        {
            m_tabMenus[m_selectedTab].Default();
            m_tabMenus[index].Default();

            m_selectedTab = index;
        }
    }
}