using System;
using Coffee.UIEffects;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class InventorySelectedCountableItemView : AbstractView
    {
        // View
        [SerializeField]
        private CountableItemBasicView basicView;
        
        // Field
        private ItemStackModel itemStack;

        public void Setup(ItemStackModel pItemStack)
        {
            itemStack = pItemStack;
        }
        
        public override void Draw()
        {
            base.Draw();
            
            basicView.Setup(itemStack);
            basicView.Draw();
        }

        public override void Clear()
        {
            base.Clear();
            basicView.Clear();
        }
    }
}