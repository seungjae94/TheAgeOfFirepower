using UniRx;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace Mathlife.ProjectL.Gameplay.UI.ArtyPagePopup
{
    public class ItemData
    {
        public MechPartModel mechPart;
    }

    public class Context : FancyGridViewContext
    {
        public int selectedIndex = -1;
        public readonly Subject<int> onCellClickRx = new();
    }

    public class ArtyPageMechPartChangeScrollView
        : FancyGridView<ItemData, Context>
    {
        // Alias
        ArtyPageMechPartChangePopup Popup => Presenter.Find<ArtyPageMechPartChangePopup>();
        
        // Override
        class CellGroup : DefaultCellGroup
        {
        }

        protected override void SetupCellTemplate() => Setup<CellGroup>(cellPrefab);

        protected override void Initialize()
        {
            base.Initialize();
            
            Context.onCellClickRx
                .Subscribe(SelectCell)
                .AddTo(gameObject);
        }
        
        public void SelectCell(int index)
        {
            if (index == Context.selectedIndex)
                return;

            // 스크롤뷰 업데이트
            Context.selectedIndex = index;
            Refresh();

            // 팝업 뷰 업데이트
            Popup.selectedIndexRx.Value = index;
        }
        
        // Field
        [SerializeField]
        ArtyPageMechPartChangeScrollViewCell cellPrefab;
    }
}