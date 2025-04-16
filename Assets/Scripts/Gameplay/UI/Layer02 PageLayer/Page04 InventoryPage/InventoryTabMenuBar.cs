using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class InventoryTabMenuItemData
    {
        public string displayName;

        // Spacing 100당 1글자에 해당
        // -> (n-1)x + (n-2) * 100 = 300
        public int GetCharacterSpacing()
        {
            int n = displayName.Length;
            return Mathf.FloorToInt((float)(300 - 100 * (n - 2)) / (n - 1));
        }
    }
    
    public class InventoryTabMenuBar : SimpleScrollRect<InventoryTabMenuItemData, SimpleScrollRectContext>
    {
        protected override void OnSelectCell(SimpleScrollRectSelectionData<InventoryTabMenuItemData> cellData)
        {
            Presenter.Find<InventoryPage>().selectedTabIndexRx.Value = cellData.index;
        }
    }
}
