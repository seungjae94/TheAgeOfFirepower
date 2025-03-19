using System;
using UniRx;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Mathlife.ProjectL.Gameplay
{
    public class ArtyModel
    {
        private readonly ArtyGameData gameData;
        private readonly ExpGameData expGameData;
        
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

            equipmentsRx.Add(EMechPartType.Barrel, null);
            equipmentsRx.Add(EMechPartType.Armor, null);
            equipmentsRx.Add(EMechPartType.Engine, null);
        }

        

        public int Id => gameData.id;
        public string DisplayName => gameData.displayName;
        public Sprite Sprite => gameData.sprite;

        public readonly ReactiveProperty<int> levelRx;
        public readonly ReactiveProperty<long> totalExpRx;

        public long NeedExp => expGameData.characterNeedExpAtLevelList[levelRx.Value];

        public long CurrentLevelExp
        {
            get
            {
                long totalExpAtCurrentLevel = expGameData.characterTotalExpAtLevelList[levelRx.Value];
                return totalExpRx.Value - totalExpAtCurrentLevel;
            }
        }

        public readonly ReactiveDictionary<EMechPartType, MechPartModel> equipmentsRx = new();

        public MechPartModel Weapon { 
            get => equipmentsRx[EMechPartType.Barrel];
            private set
            {
                if (value != null && value.Type != EMechPartType.Barrel)
                    return;

                equipmentsRx[EMechPartType.Barrel] = value;
            }
        }

        public MechPartModel Armor { 
            get => equipmentsRx[EMechPartType.Armor];
            private set
            {
                if (value != null && value.Type != EMechPartType.Armor)
                    return;

                equipmentsRx[EMechPartType.Armor] = value;
            }
        }

        public MechPartModel Artifact { 
            get => equipmentsRx[EMechPartType.Engine];
            private set
            {
                if (value != null && value.Type != EMechPartType.Engine)
                    return;

                equipmentsRx[EMechPartType.Engine] = value;
            }
        }

        public MechPartModel GetEquipment(EMechPartType type)
        {
            return equipmentsRx[type];
        }

        public IDisposable SubscribeEquipmentChangeEvent(EMechPartType type, Action<MechPartModel> action)
        {
            return equipmentsRx
                .ObserveEveryValueChanged(dic => dic[type])
                .Subscribe(equip => action(equip));
        }

        public int GetMaxHp()
        {
            int value = gameData.maxHp + (int)(gameData.maxHpGrowth * (levelRx.Value - 1));
            value += (Weapon?.Stat.maxHp ?? 0) + (Armor?.Stat.maxHp ?? 0) + (Artifact?.Stat.maxHp ?? 0);
            return value;
        }

        public int GetAtk()
        {
            int value = gameData.atk + (int)(gameData.atkGrowth * (levelRx.Value - 1));
            value += (Weapon?.Stat.atk ?? 0) + (Armor?.Stat.atk ?? 0) + (Artifact?.Stat.atk ?? 0);
            return value;
        }

        public int GetDef()
        {
            int value = gameData.def + (int)(gameData.defGrowth * (levelRx.Value - 1));
            value += (Weapon?.Stat.def ?? 0) + (Armor?.Stat.def ?? 0) + (Artifact?.Stat.def ?? 0);
            return value;
        }

        public int GetMobility()
        {
            int value = gameData.mob + (int)(gameData.mobGrowth * (levelRx.Value - 1));
            value += (Weapon?.Stat.spd ?? 0) + (Armor?.Stat.spd ?? 0) + (Artifact?.Stat.spd ?? 0);
            return value;
        }

        public void Equip(EMechPartType type, MechPartModel mechPart)
        {
            UnEquip(type);

            if (mechPart == null)
                return;

            mechPart.Owner.Value?.UnEquip(type);
            mechPart.Owner.Value = this;
            equipmentsRx[type] = mechPart;
        }

        public void UnEquip(EMechPartType type)
        {
            if (equipmentsRx[type] == null)
                return;

            equipmentsRx[type].Owner.Value = null;
            equipmentsRx[type] = null;
        }
    }

}
