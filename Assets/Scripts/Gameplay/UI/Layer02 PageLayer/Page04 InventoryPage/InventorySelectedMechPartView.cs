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
        
        // Field
        private MechPartModel mechPart;
        
        public void Setup(MechPartModel pMechPart)
        {
            mechPart = pMechPart;
        }
        
        public override void Draw()
        {
            base.Draw();
            
            basicView.Setup(mechPart);
            basicView.Draw();
        }

        public override void Clear()
        {
            base.Clear();
            basicView.Clear();
        }
    }
}