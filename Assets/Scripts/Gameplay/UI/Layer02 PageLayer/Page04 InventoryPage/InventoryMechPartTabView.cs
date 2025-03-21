using System;
using UniRx;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class InventoryMechPartTabView : AbstractView
    {
        // Alias
        private static InventoryState InventoryState => GameState.Inst.inventoryState; 
        
        // View
        [SerializeField]
        private InventoryMechPartGridView gridView;
        // TODO: InventorySelectedMechPartView
        
        // Field
        private readonly ReactiveProperty<int> selectedIndexRx = new(-1);
        private readonly CompositeDisposable disposables = new();
        
        public override void Draw()
        {
            base.Draw();
            gameObject.SetActive(true);
            
            // 구독
            selectedIndexRx
                .DistinctUntilChanged()
                .Subscribe(OnSelectCell)
                .AddTo(disposables);
            
            // 뷰 초기화
            var items = InventoryState.GetSortedMechPartList();
            
            gridView.Setup(this);
            gridView.UpdateContents(items);
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
            // TODO: InventorySelectedMechPartView Re-draw
        }
    }
}