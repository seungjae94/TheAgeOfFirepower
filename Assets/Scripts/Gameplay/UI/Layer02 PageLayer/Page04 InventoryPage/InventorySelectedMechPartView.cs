using System;
using Coffee.UIEffects;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class InventorySelectedMechPartView : AbstractView
    {
        // View
        [SerializeField]
        private MechPartBasicView basicView;

        [SerializeField]
        private Button equipButton;
        
        [SerializeField]
        private Button sellButton;
        
        // Field
        private MechPartModel mechPart;
        private readonly CompositeDisposable disposables = new();
        
        public void Setup(MechPartModel pMechPart)
        {
            mechPart = pMechPart;
        }
        
        public override void Draw()
        {
            base.Draw();

            equipButton.OnClickAsObservable()
                .Subscribe(OnClickEquipButton)
                .AddTo(disposables);
            
            sellButton.OnClickAsObservable()
                .Subscribe(OnClickSellButton)
                .AddTo(disposables);
            
            basicView.Setup(mechPart);
            basicView.Draw();
        }

        public override void Clear()
        {
            base.Clear();
            disposables.Clear();
            basicView.Clear();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
        
        // 이벤트 처리
        private void OnClickEquipButton(Unit _)
        {
            Debug.Log("Equip!");
        }
        
        private void OnClickSellButton(Unit _)
        {
            Debug.Log("Sell!");
        }
    }
}