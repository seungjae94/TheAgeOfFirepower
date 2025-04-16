using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI.Extensions.EasingCore;

namespace Mathlife.ProjectL.Gameplay.UI.ArtyPageView
{
    public class ItemData
    {
        public ArtyModel arty;
    }

    public class Context : FancyScrollRectContext
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

        private int prevSnapIndex = 0;
        
        protected override void Initialize()
        {
            base.Initialize();

            Context.onCellClickRx
                .Subscribe(i => SelectCell(i))
                .AddTo(gameObject);
            
            scroller = GetComponent<Scroller>();
            scroller.OnValueChanged(OnScrollValueChanged);
            scroller.OnSelectionChanged(i => UpdateSelection(i));
        }

        public void Setup(List<ArtyModel> items)
        {
            UpdateContents(items.Select(arty => new ItemData { arty = arty }).ToList());
            scroller.SetTotalCount(items.Count);
            Context.cellInterval = cellInterval;
            Context.scrollOffset = scrollOffset;
        }

        public void SelectCell(int index, bool internalCall = true)
        {
            if (index == Context.selectedIndex)
                return;

            UpdateSelection(index, internalCall);
            scroller.ScrollTo(index, 0.35f, Ease.OutCubic);
        }

        private void UpdateSelection(int index, bool internalCall = true)
        {
            if (Context.selectedIndex == index)
                return;

            if (internalCall)
            {
                AudioManager.Inst.PlaySE(ESoundEffectId.Ok);
            }
            Context.selectedIndex = index;
            ArtyPage.selectedArtyIndexRx.Value = index;
            Refresh();
        }

        private void OnScrollValueChanged(float value)
        {
            UpdatePosition(value);

            float frac = value - Mathf.Floor(value);
            
            if (frac is < 0.1f or > 0.9f)
            {
                int currentSnapIndex = Mathf.RoundToInt(value);

                if (prevSnapIndex != currentSnapIndex)
                {
                    AudioManager.Inst.PlaySE(ESoundEffectId.BeginDrag);
                }

                prevSnapIndex = currentSnapIndex;
            }
        }
    }
}