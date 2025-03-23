using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class InventoryPage : Page
    {
        public override string PageName => "인벤토리";
        
        // View
        [SerializeField]
        private InventoryTabMenuBar tabMenuBar;
        
        [SerializeField]
        private InventoryMechPartTabView mechPartTabView;
        
        [SerializeField]
        private InventoryCountableItemTabView countableItemTabView;
        
        // Field
        public readonly ReactiveProperty<int> selectedTabIndexRx = new(0);
        private readonly CompositeDisposable disposables = new();
        
        protected override void OnOpen()
        {
            // Overlay
            NavigateBackOverlay navBackOverlay = Find<NavigateBackOverlay>();
            navBackOverlay.Activate();
            
            // 뷰 초기화
            mechPartTabView.gameObject.SetActive(false);
            countableItemTabView.gameObject.SetActive(false);
            
            selectedTabIndexRx.Value = 0;
            selectedTabIndexRx
                .DistinctUntilChanged()
                .Subscribe(OnSelectTab)
                .AddTo(disposables);
            
            var inventoryTabMenuItemDataList = new List<InventoryTabMenuItemData>()
            {
                new InventoryTabMenuItemData() { displayName = "부품" },
                new InventoryTabMenuItemData() { displayName = "재료" },
                new InventoryTabMenuItemData() { displayName = "전투" },
            };
            
            tabMenuBar.UpdateContents(inventoryTabMenuItemDataList);
            tabMenuBar.SelectCell(0);
        }

        protected override void OnClose()
        {
            if (false == mechPartTabView.IsClear)
                mechPartTabView.Clear();
            if (false == countableItemTabView.IsClear)
                countableItemTabView.Clear();
            
            disposables.Clear();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }

        // 이벤트 구독 콜백
        private void OnSelectTab(int index)
        {
            if (false == mechPartTabView.IsClear)
                mechPartTabView.Clear();
            if (false == countableItemTabView.IsClear)
                countableItemTabView.Clear();

            if (index == 0)
                mechPartTabView.Draw();
            else
                countableItemTabView.Draw();
        }
    }
}