using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class SimpleGridViewSelectionData<TItemData>
    {
        public int index;
        public TItemData itemData;
    }

    public class SimpleGridViewContext : FancyGridViewContext
    {
        public int selectedIndex = -1;
        public readonly Subject<int> onCellClickRx = new();
    }

    public abstract class SimpleGridView<TItemData, TContext>
        : FancyGridView<TItemData, TContext>
        where TContext : SimpleGridViewContext, new()
    {
        // Override
        protected override void Initialize()
        {
            base.Initialize();

            Context.onCellClickRx
                .Subscribe(SelectCell)
                .AddTo(gameObject);
        }

        // Method
        public void SelectCell(int index)
        {
            Context.selectedIndex = index;

            TItemData itemData = default;

            if (index >= 0)
            {
                int rowNo = index / startAxisCellCount;
                int colNo = index % startAxisCellCount;
                itemData = ItemsSource[rowNo][colNo];
            }

            OnSelectCell(new SimpleGridViewSelectionData<TItemData>
            {
                index = index, 
                itemData = itemData
            });
            
            Refresh();
        }

        public void ResetPosition()
        {
            JumpTo(0, 0.5f);
            Refresh();
        }

        protected abstract void OnSelectCell(SimpleGridViewSelectionData<TItemData> selectionData);
    }
}