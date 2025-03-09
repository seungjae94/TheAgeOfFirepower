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
    public class ShopPage : Page
    {
        public override EPageId pageId => EPageId.ShopPage;

        [Inject] MainSceneManager m_mainSceneManager;
        [Inject] InventoryState inventoryState;
        [Inject] GameDataLoader gameDataLoader;

        [SerializeField] Button m_navBackButton;
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

            m_ingameCurrencyBar.Initialize();
        }

        protected override void SubscribeUserInteractions()
        {
            m_navBackButton.OnClickAsObservable()
                .Subscribe(_ => m_mainSceneManager.NavigateBack())
                .AddTo(gameObject);
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

        public void OnClickBuyButton(EquipmentGameData equipmentGameData)
        {
            bool buyResult = inventoryState.BuyItem(equipmentGameData.id);

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
            m_flexView.Draw(gameDataLoader.GetShopSO().GetItemCatalog((EEquipmentType)m_selectedTab), OnClickBuyButton);
        }
    }
}
