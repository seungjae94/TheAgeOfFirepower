using UniRx;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class InventoryCountableItemGridView
        : SimpleGridView<ItemStackModel, SimpleGridViewContext>
    {
        // Override
        [SerializeField]
        private InventoryCountableItemGridViewCell cellPrefab;
        
        private class CellGroup : DefaultCellGroup {}
        protected override void SetupCellTemplate() => Setup<CellGroup>(cellPrefab);
        
        // Impl
        public readonly Subject<int> onSelectedIndexSubject = new();
        
        protected override void OnSelectCell(SimpleGridViewSelectionData<ItemStackModel> selectionData)
        {
            onSelectedIndexSubject.OnNext(selectionData.index);
        }
    }
}