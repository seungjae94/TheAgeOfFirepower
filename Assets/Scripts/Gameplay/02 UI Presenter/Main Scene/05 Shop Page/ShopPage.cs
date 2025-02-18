using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class ShopPage : Page
    {
        public override EPageId pageId => EPageId.ShopPage;

        [Inject] MainSceneManager m_mainSceneManager;
        [Inject] InventoryRepository m_inventoryRepository;
        [Inject] GameDataDB m_gameDataDB;

        [SerializeField] SimpleActionButton m_navigateBackButton;
        [SerializeField] IngameCurrencyBar m_ingameCurrencyBar;

        // 탭 선택 기능
        [SerializeField] Transform m_tabMenuBar;
        List<TabMenu> m_tabMenus = new();
        int m_selectedTab = 0;

        // 카탈로그 표시 기능
        [SerializeField] ShopFlex m_flexView;

        // 초기화
        protected override void Awake()
        {
            base.Awake();

            foreach (Transform tabMenuTrans in m_tabMenuBar)
            {
                TabMenu tabMenu = tabMenuTrans.GetComponent<TabMenu>();
                m_tabMenus.Add(tabMenu);
            }
        }

        protected override void InitializeChildren()
        {
            m_tabMenus[0].Initialize(0, OnSelectTabMenu);
            m_tabMenus[0].Select();

            for (int i = 1; i < m_tabMenus.Count; ++i)
            {
                m_tabMenus[i].Initialize(i, OnSelectTabMenu);
                m_tabMenus[i].Default();
            }

            m_navigateBackButton.Initialize(m_mainSceneManager.NavigateBack);
            m_ingameCurrencyBar.Initialize();
        }

        // 유저 상호작용
        public override void Open()
        {
            base.Open();

            UpdateFlexView();
        }

        void OnSelectTabMenu(int index)
        {
            m_tabMenus[m_selectedTab].Default();
            m_tabMenus[index].Select();

            m_selectedTab = index;

            UpdateFlexView();
        }

        public void OnClickBuyButton(EquipmentSO equipmentSO)
        {
            bool buyResult = m_inventoryRepository.BuyItem(equipmentSO.id);

            if (buyResult)
            {
                // 구매 이펙트
                Debug.Log("구매 이펙트");
            }
            else
            {
                // 구매 실패 알림
                Debug.Log("구매 실패 알림");
            }
        }

        // 뷰 업데이트
        void UpdateFlexView()
        {
            m_flexView.Draw(m_gameDataDB.GetShopSO().GetItemCatalog((EEquipmentType)m_selectedTab), OnClickBuyButton);
        }
    }
}
