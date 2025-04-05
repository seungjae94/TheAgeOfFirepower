using System;
using UniRx;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Mathlife.ProjectL.Gameplay
{
    public class ArtyModel
    {
        // Alias
        private InventoryState InventoryState => GameState.Inst.inventoryState;
        private GameDataLoader GameDataLoader => GameState.Inst.gameDataLoader;
        private ExpGameData ExpGameData => GameDataLoader.GetExpData();
        
        public ArtyModel(
            ArtyGameData gameData,
            int level, long totalExp
        )
        {
            this.gameData = gameData;
            levelRx = new(level);
            totalExpRx = new(totalExp);

            mechPartSlotsRx.Add(EMechPartType.Barrel, null);
            mechPartSlotsRx.Add(EMechPartType.Armor, null);
            mechPartSlotsRx.Add(EMechPartType.Engine, null);
        }
        
        // Field - Basic Info
        private readonly ArtyGameData gameData;
        public readonly ReactiveProperty<int> levelRx;
        public readonly ReactiveProperty<long> totalExpRx;
        
        public int Id => gameData.id;
        public string DisplayName => gameData.displayName;
        public Sprite Sprite => gameData.sprite;
        public Sprite EnemySprite => gameData.enemySprite;
        
        public GameObject BattlerPrefab => gameData.battlerPrefab;
        
        public long NeedExp => ExpGameData.characterNeedExpAtLevelList[levelRx.Value];
        public long CurrentLevelExp => totalExpRx.Value - ExpGameData.characterTotalExpAtLevelList[levelRx.Value]; 

        // Field - Mech Parts
        public readonly ReactiveDictionary<EMechPartType, MechPartModel> mechPartSlotsRx = new();

        public MechPartModel Barrel => mechPartSlotsRx[EMechPartType.Barrel];

        public MechPartModel Armor => mechPartSlotsRx[EMechPartType.Armor];

        public MechPartModel Engine => mechPartSlotsRx[EMechPartType.Engine];

        public MechPartModel GetMechPartAtSlot(EMechPartType type) => mechPartSlotsRx[type];
        
        // Method
        public int GetMaxHp()
        {
            int value = gameData.maxHp + (int)(gameData.maxHpGrowth * (levelRx.Value - 1));
            value += (Barrel?.Stat.maxHp ?? 0) + (Armor?.Stat.maxHp ?? 0) + (Engine?.Stat.maxHp ?? 0);
            return value;
        }

        public int GetAtk()
        {
            int value = gameData.atk + (int)(gameData.atkGrowth * (levelRx.Value - 1));
            value += (Barrel?.Stat.atk ?? 0) + (Armor?.Stat.atk ?? 0) + (Engine?.Stat.atk ?? 0);
            return value;
        }

        public int GetDef()
        {
            int value = gameData.def + (int)(gameData.defGrowth * (levelRx.Value - 1));
            value += (Barrel?.Stat.def ?? 0) + (Armor?.Stat.def ?? 0) + (Engine?.Stat.def ?? 0);
            return value;
        }

        public int GetMobility()
        {
            int value = gameData.mob + (int)(gameData.mobGrowth * (levelRx.Value - 1));
            value += (Barrel?.Stat.mob ?? 0) + (Armor?.Stat.mob ?? 0) + (Engine?.Stat.mob ?? 0);
            return value;
        }

        public int GetThreatLevel()
        {
            return Mathf.CeilToInt(GetAtk() * gameData.shell.damage / 100f);
        }

        public void Equip(EMechPartType slotType, MechPartModel mechPart)
        {
            if (mechPart == null)
            {
                UnEquip(slotType);
                return;
            }
            
            // 검증
            if (slotType != mechPart.Type)
            {
                Debug.LogError($"[ArtyRosterState] 슬롯 {slotType}에 {mechPart.Type} 타입의 부품을 장착하려고 합니다.");
                return;
            }
            
            // 장착 중이던 부품이 있다면 해제
            UnEquip(slotType);

            // 주인 변경
            mechPart.Owner.Value?.UnEquip(slotType);
            mechPart.Owner.Value = this;
            
            // 장착
            mechPartSlotsRx[slotType] = mechPart;
        }
        
        public void Equip(EMechPartType slotType, int mechPartId)
        {
            if (mechPartId < 0)
            {
                Equip(slotType, null);
                return;
            }
            
            MechPartModel mechPart = InventoryState.FindBackupMechPart(slotType, mechPartId);
            Equip(slotType, mechPart);
        }

        private void UnEquip(EMechPartType slotType)
        {
            if (mechPartSlotsRx[slotType] == null)
                return;

            mechPartSlotsRx[slotType].Owner.Value = null;
            mechPartSlotsRx[slotType] = null;
        }
    }

}
