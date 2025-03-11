using UniRx;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class EquipmentModel
    {
        public EquipmentModel(MechPartGameData gameData)
        {
            this.gameData = gameData;
        }

        private readonly MechPartGameData gameData;

        public int id => gameData.id;

        public EEquipmentType type => gameData.type;

        public string displayName => gameData.displayName;

        public string description => gameData.description;

        public BasicStat stat => gameData.stat;
        
        public Sprite icon => gameData.icon;

        ReactiveProperty<CharacterModel> m_owner = new();
        public CharacterModel owner { get => m_owner.Value; set => m_owner.Value = value; } 
    }
}
