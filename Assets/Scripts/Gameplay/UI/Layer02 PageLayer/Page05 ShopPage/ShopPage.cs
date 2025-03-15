using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class ShopPage : Page
    {
        public override string PageName => "상점";
        
        [SerializeField] Button m_navBackButton;
        [SerializeField] CurrencyBar m_ingameCurrencyBar;

        // 탭 선택 기능
        [SerializeField] Transform m_tabMenuBar;
        //List<TabMenu> m_tabMenus = new();
        int m_selectedTab = 0;

        // 카탈로그 표시 기능
        //[SerializeField] ShopFlex m_flexView;

        // 초기화
        protected void Awake()
        {
            // foreach (Transform tabMenuTrans in m_tabMenuBar)
            // {
            //     TabMenu tabMenu = tabMenuTrans.GetComponent<TabMenu>();
            //     m_tabMenus.Add(tabMenu);
            // }
        }

        // 유저 상호작용
        public override void Activate()
        {
            // base.Open();
            //
            // UpdateFlexView();
            //
            // m_tabMenus[0].Initialize(0, OnSelectTabMenu);
            // m_tabMenus[0].Select();
            //
            // for (int i = 1; i < m_tabMenus.Count; ++i)
            // {
            //     m_tabMenus[i].Initialize(i, OnSelectTabMenu);
            //     m_tabMenus[i].Default();
            // }

            //m_ingameCurrencyBar.Initialize();
        
            // m_navBackButton.OnClickAsObservable()
            //     .Subscribe(_ => lobbySceneGameMode.NavigateBack())
            //     .AddTo(gameObject);
        }

        void OnSelectTabMenu(int index)
        {
            // m_tabMenus[m_selectedTab].Default();
            // m_tabMenus[index].Select();
            //
            // m_selectedTab = index;
            //
            // UpdateFlexView();
        }

        public void OnClickBuyButton(MechPartGameData mechPartGameData)
        {
            // bool buyResult = inventoryState.BuyItem(mechPartGameData.id);
            //
            // if (buyResult)
            // {
            //     // 구매 이펙트
            //     Debug.Log("구매 이펙트");
            // }
            // else
            // {
            //     // 구매 실패 알림
            //     Debug.Log("구매 실패 알림");
            // }
        }

        // 뷰 업데이트
        void UpdateFlexView()
        {
            //m_flexView.Draw(gameDataLoader.GetShopSO().GetItemCatalog((EEquipmentType)m_selectedTab), OnClickBuyButton);
        }
    }
}
