using System;
using UniRx;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Mathlife.ProjectL.Gameplay
{
    public class ArtyModel
    {
        ArtyGameData gameData;
        ExpGameData expGameData;
        
        public ArtyModel(
            ArtyGameData gameData,
            ExpGameData expGameData,
            int level, long totalExp
        )
        {
            this.gameData = gameData;
            this.expGameData = expGameData;
            levelRx = new(level);
            totalExpRx = new(totalExp);

            equipmentsRx.Add(EEquipmentType.Barrel, null);
            equipmentsRx.Add(EEquipmentType.Armor, null);
            equipmentsRx.Add(EEquipmentType.Engine, null);
        }

        

        public int id => gameData.id;
        public string displayName => gameData.displayName;
        public Sprite Sprite => gameData.sprite;

        public readonly ReactiveProperty<int> levelRx;
        public readonly ReactiveProperty<long> totalExpRx;

        public long needExp => expGameData.characterNeedExpAtLevelList[levelRx.Value];

        public long currentLevelExp
        {
            get
            {
                long totalExpAtCurrentLevel = expGameData.characterTotalExpAtLevelList[levelRx.Value];
                return totalExpRx.Value - totalExpAtCurrentLevel;
            }
        }

        public readonly ReactiveDictionary<EEquipmentType, MechPartModel> equipmentsRx = new();

        public MechPartModel Weapon { 
            get => equipmentsRx[EEquipmentType.Barrel];
            private set
            {
                if (value != null && value.type != EEquipmentType.Barrel)
                    return;

                equipmentsRx[EEquipmentType.Barrel] = value;
            }
        }

        public MechPartModel Armor { 
            get => equipmentsRx[EEquipmentType.Armor];
            private set
            {
                if (value != null && value.type != EEquipmentType.Armor)
                    return;

                equipmentsRx[EEquipmentType.Armor] = value;
            }
        }

        public MechPartModel Artifact { 
            get => equipmentsRx[EEquipmentType.Engine];
            private set
            {
                if (value != null && value.type != EEquipmentType.Engine)
                    return;

                equipmentsRx[EEquipmentType.Engine] = value;
            }
        }

        public MechPartModel GetEquipment(EEquipmentType type)
        {
            return equipmentsRx[type];
        }

        public IDisposable SubscribeEquipmentChangeEvent(EEquipmentType type, Action<MechPartModel> action)
        {
            return equipmentsRx
                .ObserveEveryValueChanged(dic => dic[type])
                .Subscribe(equip => action(equip));
        }

        public int GetMaxHp()
        {
            int value = gameData.maxHp + (int)(gameData.maxHpGrowth * (levelRx.Value - 1));
            value += (Weapon?.stat.maxHp ?? 0) + (Armor?.stat.maxHp ?? 0) + (Artifact?.stat.maxHp ?? 0);
            return value;
        }

        public int GetAtk()
        {
            int value = gameData.atk + (int)(gameData.atkGrowth * (levelRx.Value - 1));
            value += (Weapon?.stat.atk ?? 0) + (Armor?.stat.atk ?? 0) + (Artifact?.stat.atk ?? 0);
            return value;
        }

        public int GetDef()
        {
            int value = gameData.def + (int)(gameData.defGrowth * (levelRx.Value - 1));
            value += (Weapon?.stat.def ?? 0) + (Armor?.stat.def ?? 0) + (Artifact?.stat.def ?? 0);
            return value;
        }

        public int GetSpd()
        {
            int value = gameData.spd + (int)(gameData.spdGrowth * (levelRx.Value - 1));
            value += (Weapon?.stat.spd ?? 0) + (Armor?.stat.spd ?? 0) + (Artifact?.stat.spd ?? 0);
            return value;
        }

        public void Equip(EEquipmentType type, MechPartModel mechPart)
        {
            UnEquip(type);

            if (mechPart == null)
                return;

            mechPart.owner?.UnEquip(type);
            mechPart.owner = this;
            equipmentsRx[type] = mechPart;
        }

        public void UnEquip(EEquipmentType type)
        {
            if (equipmentsRx[type] == null)
                return;

            equipmentsRx[type].owner = null;
            equipmentsRx[type] = null;
        }
    }

}
