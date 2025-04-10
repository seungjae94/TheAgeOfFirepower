using System;
using System.Collections.Generic;
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
        
        [SerializeField]
        private InventorySelectedMechPartView selectedMechPartView;
        
        // Field
        private readonly CompositeDisposable disposables = new();
        private List<MechPartModel> mechParts;
        
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
            mechParts = InventoryState.GetSortedMechPartList();
            gridView.UpdateContents(mechParts);
            gridView.ResetPosition();
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
            selectedMechPartView.Clear();
            selectedMechPartView.Setup(index >= 0 ? mechParts[index] : null);
            selectedMechPartView.Draw();
        }
    }
}