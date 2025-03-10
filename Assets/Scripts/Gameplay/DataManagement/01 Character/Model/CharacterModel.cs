using System;
using UniRx;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Mathlife.ProjectL.Gameplay
{
    public class CharacterModel
    {
        public CharacterModel(
            CharacterGameData dataAsset,
            ExpGameData expGameData,
            int level, long totalExp
        )
        {
            gameData = dataAsset;
            expGameData = expGameData;
            m_level = new(level);
            m_totalExp = new(totalExp);

            m_equipments.Add(EEquipmentType.Weapon, null);
            m_equipments.Add(EEquipmentType.Armor, null);
            m_equipments.Add(EEquipmentType.Artifact, null);
        }

        CharacterGameData gameData;
        ExpGameData expGameData;

        public int id => gameData.id;
        public string displayName => gameData.displayName;
        public Sprite portrait => gameData.portrait;
        public Sprite battler => gameData.battler;

        IntReactiveProperty m_level;
        public int level { get => m_level.Value; private set => m_level.Value = value; }
        public IDisposable SubscribeLevelChangeEvent(Action<int> onLevelChangedAction)
        {
            return m_level.Subscribe(onLevelChangedAction);
        }


        LongReactiveProperty m_totalExp;
        public long totalExp { get => m_totalExp.Value; private set => m_totalExp.Value = value; }
        public IDisposable SubscribeTotalExpChangeEvent(Action<long> onTotalExpChangedAction)
        {
            return m_totalExp.Subscribe(onTotalExpChangedAction);
        }

        public long needExp => expGameData.characterNeedExpAtLevelList[level];

        public long currentLevelExp
        {
            get
            {
                long totalExpAtCurrentLevel = expGameData.characterTotalExpAtLevelList[level];
                return totalExp - totalExpAtCurrentLevel;
            }
        }

        ReactiveDictionary<EEquipmentType, EquipmentModel> m_equipments = new();

        public EquipmentModel weapon { 
            get => m_equipments[EEquipmentType.Weapon];
            private set
            {
                if (value != null && value.type != EEquipmentType.Weapon)
                    return;

                m_equipments[EEquipmentType.Weapon] = value;
            }
        }

        public EquipmentModel armor { 
            get => m_equipments[EEquipmentType.Armor];
            private set
            {
                if (value != null && value.type != EEquipmentType.Armor)
                    return;

                m_equipments[EEquipmentType.Armor] = value;
            }
        }

        public EquipmentModel artifact { 
            get => m_equipments[EEquipmentType.Artifact];
            private set
            {
                if (value != null && value.type != EEquipmentType.Artifact)
                    return;

                m_equipments[EEquipmentType.Artifact] = value;
            }
        }

        public EquipmentModel GetEquipment(EEquipmentType type)
        {
            return m_equipments[type];
        }

        public IDisposable SubscribeEquipmentChangeEvent(EEquipmentType type, Action<EquipmentModel> action)
        {
            return m_equipments
                .ObserveEveryValueChanged(dic => dic[type])
                .Subscribe(equip => action(equip));
        }

        public int GetMaxHp()
        {
            int value = gameData.maxHp + (int)(gameData.maxHpGrowth * (m_level.Value - 1));
            value += (weapon?.stat.maxHp ?? 0) + (armor?.stat.maxHp ?? 0) + (artifact?.stat.maxHp ?? 0);
            return value;
        }

        public int GetMaxEnergy()
        {
            int value = 3;
            value += (weapon?.stat.maxEnergy ?? 0) + (armor?.stat.maxEnergy ?? 0) + (artifact?.stat.maxEnergy ?? 0);
            return value;
        }

        public int GetEnergyRecovery()
        {
            int value = 1;
            value += (weapon?.stat.energyRecovery ?? 0) + (armor?.stat.energyRecovery ?? 0) + (artifact?.stat.energyRecovery ?? 0);
            return value;
        }

        public int GetAtk()
        {
            int value = gameData.atk + (int)(gameData.atkGrowth * (m_level.Value - 1));
            value += (weapon?.stat.atk ?? 0) + (armor?.stat.atk ?? 0) + (artifact?.stat.atk ?? 0);
            return value;
        }

        public int GetDef()
        {
            int value = gameData.def + (int)(gameData.defGrowth * (m_level.Value - 1));
            value += (weapon?.stat.def ?? 0) + (armor?.stat.def ?? 0) + (artifact?.stat.def ?? 0);
            return value;
        }

        public int GetMag()
        {
            int value = gameData.mag + (int)(gameData.magGrowth * (m_level.Value - 1));
            value += (weapon?.stat.mag ?? 0) + (armor?.stat.mag ?? 0) + (artifact?.stat.mag ?? 0);
            return value;
        }

        public int GetSpd()
        {
            int value = gameData.spd + (int)(gameData.spdGrowth * (m_level.Value - 1));
            value += (weapon?.stat.spd ?? 0) + (armor?.stat.spd ?? 0) + (artifact?.stat.spd ?? 0);
            return value;
        }

        public void Equip(EEquipmentType type, EquipmentModel equipment)
        {
            UnEquip(type);

            if (equipment == null)
                return;

            equipment.owner?.UnEquip(type);
            equipment.owner = this;
            m_equipments[type] = equipment;
        }

        public void UnEquip(EEquipmentType type)
        {
            if (m_equipments[type] == null)
                return;

            m_equipments[type].owner = null;
            m_equipments[type] = null;
        }
    }

}
