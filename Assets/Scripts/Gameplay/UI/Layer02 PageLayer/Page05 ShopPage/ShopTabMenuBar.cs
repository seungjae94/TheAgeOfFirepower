using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class ShopTabMenuItemData
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
    
    public class ShopTabMenuBar : SimpleScrollRect<ShopTabMenuItemData, SimpleScrollRectContext>
    {
        protected override void OnSelectCell(SimpleScrollRectSelectionData<ShopTabMenuItemData> cellData)
        {
            Presenter.Find<ShopPage>().selectedTabIndexRx.Value = cellData.index;
        }
    }
}
