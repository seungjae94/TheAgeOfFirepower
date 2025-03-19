using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI.Extensions.EasingCore;

namespace Mathlife.ProjectL.Gameplay.UI.ArtyPageView
{
    public class ItemData
    {
        public ArtyModel arty;
    }

    public class Context : FancyGridViewContext
    {
        public float cellInterval = 1f;
        public float scrollOffset = 1f;
        public int selectedIndex = -1;
        public readonly Subject<int> onCellClickRx = new();
    }

    public class ArtyPageScrollView
        : FancyScrollView<ItemData, Context>
    {
        // Alias
        private ArtyPage ArtyPage => Presenter.Find<ArtyPage>();
        
        // Override
        protected override GameObject CellPrefab => cellPrefab.gameObject;

        // Field
        private Scroller scroller;

        [SerializeField]
        ArtyPageScrollViewCell cellPrefab;

        protected override void Initialize()
        {
            base.Initialize();

            Context.onCellClickRx
                .Subscribe(SelectCell)
                .AddTo(gameObject);
            
            scroller = GetComponent<Scroller>();
            scroller.OnValueChanged(UpdatePosition);
            scroller.OnSelectionChanged(UpdateSelection);
        }

        public void Setup(List<ArtyModel> items)
        {
            UpdateContents(items.Select(arty => new ItemData { arty = arty }).ToList());
            scroller.SetTotalCount(items.Count);
            Context.cellInterval = cellInterval;
            Context.scrollOffset = scrollOffset;
        }

        public void SelectCell(int index)
        {
            if (index == Context.selectedIndex)
                return;

            UpdateSelection(index);
            scroller.ScrollTo(index, 0.35f, Ease.OutCubic);
        }

        private void UpdateSelection(int index)
        {
            if (Context.selectedIndex == index)
                return;

            Context.selectedIndex = index;
            ArtyPage.selectedArtyIndexRx.Value = index;
            Refresh();
        }
    }
}