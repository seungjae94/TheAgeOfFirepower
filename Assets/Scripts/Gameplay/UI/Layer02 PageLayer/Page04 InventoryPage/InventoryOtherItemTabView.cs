using System;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class InventoryOtherItemTabView : AbstractView
    {
        // Alias
        private static InventoryPage InventoryPage => Presenter.Find<InventoryPage>();
        
        // View
        // TODO: InventoryOtherItemScrollView
        
        // Field
        private bool isClear;
        private EItemType itemType;
        
        public override void Draw()
        {
            base.Draw();
            gameObject.SetActive(true);

            itemType = (EItemType) InventoryPage.selectedTabIndexRx.Value;
            switch (itemType)
            {
                case EItemType.MechPart:
                    throw new ArgumentOutOfRangeException("");
                case EItemType.ConsumableItem:
                    Debug.Log("소모품");
                    break;
                case EItemType.BattleItem:
                    Debug.Log("전투");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("");
            }
            
        }

        public override void Clear()
        {
            base.Clear();
            gameObject.SetActive(false);
        }
    }
}