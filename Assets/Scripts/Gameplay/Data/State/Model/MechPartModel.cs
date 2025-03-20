using DG.DemiEditor.DeGUINodeSystem;
using UniRx;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class MechPartModel
    {
        public MechPartModel(MechPartGameData gameData)
        {
            this.gameData = gameData;
        }

        private readonly MechPartGameData gameData;

        public int Id => gameData.id;
        
        public EItemRarity Rarity => gameData.rarity;

        public EMechPartType Type => gameData.type;

        public string DisplayName => gameData.displayName;

        public string Description => gameData.description;

        public BasicStat Stat => gameData.stat;
        
        public Sprite Icon => gameData.icon;

        public readonly ReactiveProperty<ArtyModel> Owner = new();
    }
}
