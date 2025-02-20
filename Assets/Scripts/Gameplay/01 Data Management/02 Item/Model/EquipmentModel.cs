using UniRx;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class EquipmentModel
    {
        public EquipmentModel(EquipmentSO so)
        {
            m_so = so;
        }

        protected EquipmentSO m_so;

        public EEquipmentId id => m_so.id;

        public EEquipmentType type => m_so.type;

        public string displayName => m_so.displayName;

        public string description => m_so.description;

        public BasicStat stat => m_so.stat;


        public ConditionalBattleEffect battleEffect => m_so.battleEffect;

        public Sprite icon => m_so.icon;

        ReactiveProperty<CharacterModel> m_owner = new();
        public CharacterModel owner { get => m_owner.Value; set => m_owner.Value = value; } 
    }
}
