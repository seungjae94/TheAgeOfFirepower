using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class InventoryCountableItemTabView : AbstractView
    {
        // Alias
        private static InventoryPage InventoryPage => Presenter.Find<InventoryPage>();
        private static InventoryState InventoryState => GameState.Inst.inventoryState;
        
        // View
        [SerializeField]
        private InventoryCountableItemGridView gridView;
        
        [SerializeField]
        private InventorySelectedCountableItemView selectedItemView;
        
        // Field
        private readonly CompositeDisposable disposables = new();
        private List<ItemStackModel> itemStacks;
        
        public override void Draw()
        {
            base.Draw();
            gameObject.SetActive(true);

            // 구독
            gridView.onSelectedIndexSubject
                .DistinctUntilChanged()
                .Subscribe(OnSelectCell)
                .AddTo(disposables);
            
            // 뷰 초기화
            EItemType itemType = (EItemType) InventoryPage.selectedTabIndexRx.Value;
            if (itemType != EItemType.MaterialItem && itemType != EItemType.BattleItem)
                throw new ArgumentOutOfRangeException("Selected item type is not countable.");
            
            itemStacks = InventoryState.GetSortedItemStackList(itemType);
            gridView.UpdateContents(itemStacks);
            gridView.SelectCell(-1);
        }

        public override void Clear()
        {
            base.Clear();
            gameObject.SetActive(false);
            disposables.Clear();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
        
        private void OnSelectCell(int index)
        {
            selectedItemView.Clear();
            selectedItemView.Setup(index >= 0 ? itemStacks[index] : null);
            selectedItemView.Draw();
        }
    }
}