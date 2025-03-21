using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class InventoryPage : Page
    {
        public override string PageName => "인벤토리";
        
        // View
        [SerializeField]
        private InventoryTabMenuBar tabMenuBar;
        
        // TODO: InventoryPageMechPartTabView
        // TODO: InventoryPageOtherItemView
        
        // Field
        // TODO: 선택된 탭 상태 관리
        
        
        protected override void OnOpen()
        {
            // Overlay
            NavigateBackOverlay navBackOverlay = Find<NavigateBackOverlay>();
            navBackOverlay.Activate();
            
            // 뷰 초기화
            var inventoryTabMenuItemDataList = new List<InventoryTabMenuItemData>()
            {
                new InventoryTabMenuItemData() { displayName = "부품" },
                new InventoryTabMenuItemData() { displayName = "소모품" },
                new InventoryTabMenuItemData() { displayName = "전투" },
            };
            
            tabMenuBar.Setup(inventoryTabMenuItemDataList);
            tabMenuBar.SelectCell(0);
            
            // OnSelectTabMenu(0);
            // OnClickFlexItem(null);
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

        protected override void OnClose()
        {
            
        }
        
        // 이벤트 구독 콜백
        void OnSelectTabMenu(int index)
        {
            // m_tabMenus[m_selectedTab].Default();
            // m_tabMenus[index].Select();
            //
            // m_selectedTab = index;
            // m_selectedEquipment = null;
            //
            // UpdateFlex();
            // UpdateSelectedEquipmentView();
        }
        
        void UpdateFlex()
        {
            // var itemDatas = inventoryState
            //         .GetSortedEquipmentList((EEquipmentType)m_selectedTab)
            //         .Select(item => new InventoryFlexItemData(item, item == m_selectedEquipment))
            //         .ToList();
            //
            // m_flex.Draw(itemDatas, OnClickFlexItem);
        }
    }
}