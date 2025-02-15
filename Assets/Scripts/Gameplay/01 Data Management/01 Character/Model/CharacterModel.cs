using System;
using UniRx;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Mathlife.ProjectL.Gameplay
{
    public class CharacterModel
    {
        public CharacterModel(
            CharacterSO dataAsset,
            ExpDataAsset expSO,
            int level, long totalExp
        )
        {
            m_so = dataAsset;
            m_expSO = expSO;
            m_level = new(level);
            m_totalExp = new(totalExp);

            m_equipments.Add(EEquipmentType.Weapon, null);
            m_equipments.Add(EEquipmentType.Armor, null);
            m_equipments.Add(EEquipmentType.Artifact, null);
        }

        CharacterSO m_so;
        ExpDataAsset m_expSO;

        public ECharacterId characterId => m_so.id;
        public string displayName => m_so.displayName;
        public Sprite portrait => m_so.portrait;
        public Sprite battler => m_so.battler;

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

        public long needExp => m_expSO.characterNeedExpAtLevelList[level];

        public long currentLevelExp
        {
            get
            {
                long totalExpAtCurrentLevel = m_expSO.characterTotalExpAtLevelList[level];
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
            get => m_equipments[EEquipmentType.Weapon];
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
            int value = m_so.maxHp + (int)(m_so.maxHpGrowth * (m_level.Value - 1));
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
            int value = m_so.atk + (int)(m_so.atkGrowth * (m_level.Value - 1));
            value += (weapon?.stat.atk ?? 0) + (armor?.stat.atk ?? 0) + (artifact?.stat.atk ?? 0);
            return value;
        }

        public int GetDef()
        {
            int value = m_so.def + (int)(m_so.defGrowth * (m_level.Value - 1));
            value += (weapon?.stat.def ?? 0) + (armor?.stat.def ?? 0) + (artifact?.stat.def ?? 0);
            return value;
        }

        public int GetMag()
        {
            int value = m_so.mag + (int)(m_so.magGrowth * (m_level.Value - 1));
            value += (weapon?.stat.mag ?? 0) + (armor?.stat.mag ?? 0) + (artifact?.stat.mag ?? 0);
            return value;
        }

        public int GetSpd()
        {
            int value = m_so.spd + (int)(m_so.spdGrowth * (m_level.Value - 1));
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
