using UniRx;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class InventoryMechPartGridView
        : SimpleGridView<MechPartModel, SimpleGridViewContext>
    {
        private InventoryMechPartTabView tabView;
        
        // Override
        [SerializeField]
        private InventoryMechPartGridViewCell cellPrefab;
        
        private class CellGroup : DefaultCellGroup {}
        protected override void SetupCellTemplate() => Setup<CellGroup>(cellPrefab);
        
        // Impl
        public void Setup(InventoryMechPartTabView pTabView)
        {
            tabView = pTabView;
        }
        
        protected override void OnSelectCell(SimpleGridViewSelectionData<MechPartModel> selectionData)
        {
            tabView.selectedIndexRx.Value = selectionData.index;
        }
    }
}