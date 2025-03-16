using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class SortedCharacterListSubscription
    {
        CompositeDisposable m_characterLevelChangeSubscriptions;
        IDisposable m_countChangeSubscription;

        public SortedCharacterListSubscription(CompositeDisposable characterLevelChangeSubscriptions, IDisposable countChangedSubscription)
        {
            m_characterLevelChangeSubscriptions = characterLevelChangeSubscriptions;
            m_countChangeSubscription = countChangedSubscription;
        }

        public void Dispose()
        {
            m_characterLevelChangeSubscriptions.Dispose();
            m_countChangeSubscription.Dispose();
        }
    }

    public class ArtyRosterState : IPersistable
    {
        SaveDataManager saveDataManager;
        GameDataLoader gameDataLoader;
        
        public async UniTask Load()
        {
            saveDataManager = GameState.Inst.SaveDataManager;
            gameDataLoader = GameState.Inst.GameDataLoader;

            // Validate that InventoryState was created before creating CharacterState.
            if (GameState.Inst.InventoryState == null)
            {
                Debug.LogError("[CharacterRosterState] InventoryState is null.");
                throw new Exception("[CharacterRosterState] InventoryState is null.");
            }

            if (GameState.Inst.SaveDataManager.DoesSaveFileExist())
            {
                LoadFromSaveFile();
            }
            else
            {
                LoadFromStarterData();
            }
        }

        public async UniTask Save()
        {
            throw new NotImplementedException();
        }

        // From Save File
        void LoadFromSaveFile()
        {
            ArtyRosterSaveFile artyRosterSaveFile = saveDataManager.artyRoster;

            foreach (var characterSaveData in artyRosterSaveFile.artyRoster)
            {
                ArtyModel model = new ArtyModel(
                    gameDataLoader.GetCharacterData(characterSaveData.id),
                    gameDataLoader.GetExpSO(),
                    characterSaveData.level,
                    characterSaveData.totalExp
                );

                Equip(model, characterSaveData.weaponId);
                Equip(model, characterSaveData.armorId);
                Equip(model, characterSaveData.artifactId);

                m_characters.Add(characterSaveData.id, model);
            }

            // ���� ĳ���Ͱ� ����. �����Ͱ� ������ �ջ�Ǿ���.
            if (m_characters.Count == 0)
            {
                LoadFromStarterData();
                return;
            }

            BatterySaveData teamSaveData = artyRosterSaveFile.battery;

            // �� �����Ͱ� �߸� �����Ǿ� �ִ�. ���� Ȥ�� ����� ����.
            if (false == teamSaveData.IsHealthy())
            {
                ConstructBestTeam();
                return;
            }

            // Construct team by data.
            List<ArtyModel> members = teamSaveData.members
                .Select((memberId) => (memberId == -1) ? null : m_characters[memberId])
                .ToList();

            Battery = new(members);
        }

        void LoadFromStarterData()
        {
            StarterGameData starterData = gameDataLoader.GetStarterSO();
            ExpGameData expData = gameDataLoader.GetExpSO();

            var starterParty = starterData.GetStarterBattery();
            var starterCharactersNotInParty = starterData.GetStarterMechParts();

            List<ArtyPreset> starterCharacters = new();
            starterCharacters.AddRange(starterParty);
            starterCharacters.AddRange(starterCharactersNotInParty);

            foreach (var starterCharacter in starterCharacters)
            {
                if (starterCharacter.arty == null)
                    continue;

                int level = starterCharacter.level;
                long totalExp = expData.characterTotalExpAtLevelList[level] + starterCharacter.currentLevelExp;
                ArtyModel model = new(starterCharacter.arty, expData, level, totalExp);

                Equip(model, starterCharacter.barrel?.id ?? -1);
                Equip(model, starterCharacter.armor?.id ?? -1);
                Equip(model, starterCharacter.engine?.id ?? -1);

                m_characters.Add(starterCharacter.arty.id, model);
            }

            List<ArtyModel> teamMembers = new();
            foreach (var starterMember in starterParty)
            {
                if (starterMember.arty == null)
                {
                    teamMembers.Add(null);
                    continue;
                }

                int id = starterMember.arty.id;

                if (m_characters.TryGetValue(id, out var character) == false)
                {
                    Debug.LogError("��Ÿ�� ���ÿ� ������ �ֽ��ϴ�. ����� ĳ���� ��Ͽ� �������� �ʽ��ϴ�.");
                    continue;
                }

                teamMembers.Add(character);
            }

            Battery = new(teamMembers);
        }
        
        public void Equip(ArtyModel arty, int equipmentId)
        {
            EEquipmentType equipmentType = (EEquipmentType)(equipmentId / 1000);
            arty.Equip(equipmentType, GameState.Inst.InventoryState.FindEquipment(equipmentType, equipmentId));
        }

        void ConstructBestTeam()
        {
            List<ArtyModel> members = new();
            for (int i = 0; i < Constants.PartyMemberCount; ++i)
                members.Add(null);

            Battery = new(members);

            BuildBestTeam();
        }

        public BatteryModel Battery { get; private set; }

        public void BuildBestTeam()
        {
            int memberCount = Mathf.Min(m_characters.Count, Constants.PartyMemberCount);

            List<ArtyModel> members = m_characters
                .OrderByDescending(kv => kv.Value.levelRx)
                .ThenBy(kv => kv.Key)
                .Take(memberCount)
                .Select(kv => kv.Value)
                .ToList();

            for (int i = memberCount; i < Constants.PartyMemberCount; ++i)
            {
                members.Add(null);
            }

            Battery.Rebuild(members);
        }

        ReactiveDictionary<int, ArtyModel> m_characters = new();
        List<SortedCharacterListSubscription> subscriptions = new();

        public ArtyModel this[int id]
        {
            get => m_characters[id];
        }

        public ArtyModel Add(int id, int level = 1, int totalExp = 0)
        {
            ArtyModel arty = new(
                gameDataLoader.GetCharacterData(id),
                gameDataLoader.GetExpSO(),
                level, totalExp);
            m_characters.Add(id, arty);

            return arty;
        }

        public bool Remove(int id)
        {
            if (m_characters.ContainsKey(id) == false)
                return false;

            m_characters.Remove(id);
            Battery.Remove(m_characters[id]);

            return true;
        }

        public List<ArtyModel> GetSortedList()
        {
            return m_characters
                .Select(kv => kv.Value)
                .OrderByDescending(character => character.levelRx)
                .ThenBy(character => character.id)
                .ToList();
        }

        public SortedCharacterListSubscription SubscribeSortedCharacterList(Action onCharacterListChangedAction)
        {
            CompositeDisposable characterLevelChangeSubscriptions = new();
            foreach (var kv in m_characters)
            {
                ArtyModel arty = kv.Value;
                IDisposable characterLevelChangeSubscription = arty.levelRx
                    .Subscribe(level => onCharacterListChangedAction?.Invoke());
                characterLevelChangeSubscriptions.Add(characterLevelChangeSubscription);
            }

            IDisposable countChangedSubscription = m_characters.ObserveCountChanged()
                .Subscribe(_ => OnCountChanged(onCharacterListChangedAction, characterLevelChangeSubscriptions));

            return new(characterLevelChangeSubscriptions, countChangedSubscription);
        }

        void OnCountChanged(Action onCharacterListChangedAction, CompositeDisposable characterLevelChangeSubscriptions)
        {
            // ��� ĳ���� �� �ٽ� ����
            characterLevelChangeSubscriptions.Clear();
            foreach (var kv in m_characters)
            {
                ArtyModel arty = kv.Value;
                IDisposable characterLevelChangeSubscription = arty.levelRx
                    .Subscribe(level => onCharacterListChangedAction?.Invoke());
                characterLevelChangeSubscriptions.Add(characterLevelChangeSubscription);
            }

            // �׼� ����
            onCharacterListChangedAction?.Invoke();
        }
    }
}
