using System;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using Mathlife.ProjectL.Gameplay.UI;
using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Mathlife.ProjectL.Gameplay.UI.BatteryPagePopup
{
    public class ItemData
    {
        public ArtyModel arty;
    }

    public class Context : FancyGridViewContext
    {
        public int selectedIndex = -1;
    }

    public class BatteryPageMemberChangeScrollView
        : FancyGridView<ItemData, Context>
    {
        // Override
        class CellGroup : DefaultCellGroup
        {
        }

        protected override void SetupCellTemplate() => Setup<CellGroup>(cellPrefab);

        // Field
        [SerializeField]
        BatteryPageMemberChangeScrollViewCell cellPrefab;
    }
}