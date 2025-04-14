using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class Reward
    {
        [HorizontalGroup("Gold", LabelWidth = 50, Width = 150)]
        [LabelText("골드")]
        [LabelWidth(50)]
        public int gold;

        [HorizontalGroup("Diamond", LabelWidth = 50, Width = 150)]
        [LabelText("다이아")]
        [LabelWidth(50)]
        public int diamond;

#if UNITY_EDITOR
        [HorizontalGroup("Item", LabelWidth = 50, Width = 100)]
        [LabelText("아이템")]
        [PreviewField(ObjectFieldAlignment.Left, PreviewGetter = nameof(GetPreview))]
#endif
        public ItemGameData itemGameData;

        [HorizontalGroup("Item", Gap = 50, Width = 100)]
        [LabelText("개수")]
        public int itemAmount;

        public RewardSaveData ToRewardSaveData()
        {
            RewardSaveData saveData = new();
            saveData.gold = gold;
            saveData.diamond = diamond;
            saveData.itemType = itemGameData != null ? (int)itemGameData.ItemType : -1;
            saveData.itemId = itemGameData != null ? itemGameData.id : -1;
            saveData.itemAmount = itemAmount;
            return saveData;
        }
        
#if UNITY_EDITOR
        private Sprite GetPreview()
        {
            return itemGameData?.icon;
        }
#endif
    }
}