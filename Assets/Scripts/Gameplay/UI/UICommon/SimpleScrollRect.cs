using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class SimpleScrollRectSelectionData<TItemData>
    {
        public int index = 0;
        public TItemData itemData;
    }
    
    public class SimpleScrollRectContext : FancyScrollRectContext
    {
        public int selectedIndex = -1;
        public readonly Subject<int> onCellClickRx = new();
    }

    // FancyScrollView: 사용자가 직접 배치, 무한 스크롤 지원 O, 스냅 지원 O
    // FancyScrollRect: Vertical 혹은 Horizontal로 자동 배치, 무한 스크롤 지원 X, 스냅 지원 X
    // (참고) FancyScrollRect extends FancyScrollView
    public abstract class SimpleScrollRect<TItemData, TContext> : FancyScrollRect<TItemData, TContext>
        where TContext : SimpleScrollRectContext, new() 
    {
        [SerializeField]
        protected GameObject cellPrefab;

        [SerializeField]
        protected float cellSize = 100f;

        protected override GameObject CellPrefab => cellPrefab;
        protected override float CellSize => cellSize;

        protected override void Initialize()
        {
            base.Initialize();

            Context.onCellClickRx
                .Subscribe(SelectCell)
                .AddTo(gameObject);
        }

        public new virtual void UpdateContents(IList<TItemData> itemDatas)
        {
            base.UpdateContents(itemDatas);
        }
        
        public void SelectCell(int index)
        {
            if (index == Context.selectedIndex)
                return;
            
            Context.selectedIndex = index;
            OnSelectCell(new SimpleScrollRectSelectionData<TItemData>
            {
                index = Context.selectedIndex, itemData = ItemsSource[index]
            });
            Refresh();
        }

        protected virtual void OnSelectCell(SimpleScrollRectSelectionData<TItemData> selectionData)
        {
        }
    }
}