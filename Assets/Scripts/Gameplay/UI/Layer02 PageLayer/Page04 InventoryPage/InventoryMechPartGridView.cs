using UniRx;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class InventoryMechPartGridView
        : SimpleGridView<MechPartModel, SimpleGridViewContext>
    {
        // Override
        [SerializeField]
        private InventoryMechPartGridViewCell cellPrefab;
        
        private class CellGroup : DefaultCellGroup {}
        protected override void SetupCellTemplate() => Setup<CellGroup>(cellPrefab);
        
        // Impl
        public readonly Subject<int> onSelectedIndexSubject = new();
        
        protected override void OnSelectCell(SimpleGridViewSelectionData<MechPartModel> selectionData)
        {
            onSelectedIndexSubject.OnNext(selectionData.index);
        }
    }
}