using System;
using System.Collections.Generic;
using System.Linq;
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

    public class CharacterRepository : AbstractRepository
    {
        public CharacterRepository
            (RuntimeDB runtimeDB, GameDataDB gameDataDB, SaveDataDB saveDataDB) : base(runtimeDB, gameDataDB, saveDataDB)
        {
            // Validate that InventoryRepository was created before creating CharacterRepository.
            if (runtimeDB.inventoryRepository == null)
            {
                Debug.LogError("�κ��丮 �������丮�� ���� �����ؾ� �մϴ�.");
                throw new Exception("�κ��丮 �������丮�� ���� �����ؾ� �մϴ�.");
            }

            if (saveDataDB.DoesSaveFileExist())
            {
                ConstructFromSaveFile();
            }
            else
            {
                ConstructFromStarterData();
            }
        }

        public void Equip(CharacterModel character, EEquipmentId equipmentId)
        {
            EEquipmentType equipmentType = (EEquipmentType)((int)equipmentId / 1000);
            character.Equip(equipmentType, m_runtimeDB.inventoryRepository.FindEquipment(equipmentType, equipmentId));
        }

        // From Save File
        void ConstructFromSaveFile()
        {
            CharacterSaveFile characterSaveFile = m_saveDataDB.character;

            foreach (var characterSaveData in characterSaveFile.characters)
            {
                CharacterModel model = new CharacterModel(
                    m_gameDataDB.GetCharacterData(characterSaveData.id),
                    m_gameDataDB.GetExpSO(),
                    characterSaveData.level,
                    characterSaveData.totalExp
                );

                Equip(model, characterSaveData.weapon);
                Equip(model, characterSaveData.armor);
                Equip(model, characterSaveData.artifact);

                m_characters.Add(characterSaveData.id, model);
            }

            // ���� ĳ���Ͱ� ����. �����Ͱ� ������ �ջ�Ǿ���.
            if (m_characters.Count == 0)
            {
                ConstructFromStarterData();
                return;
            }

            PartySaveData teamSaveData = characterSaveFile.party;

            // �� �����Ͱ� �߸� �����Ǿ� �ִ�. ���� Ȥ�� ����� ����.
            if (false == teamSaveData.IsHealthy())
            {
                ConstructBestTeam();
                return;
            }

            // Construct team by data.
            List<CharacterModel> members = teamSaveData.members
                .Select((memberId) => (memberId == ECharacterId.None) ? null : m_characters[memberId])
                .ToList();

            party = new(members);
        }

        void ConstructFromStarterData()
        {
            StarterSO starterData = m_gameDataDB.GetStarterSO();
            ExpSO expData = m_gameDataDB.GetExpSO();

            var starterParty = starterData.GetStarterParty();
            var starterCharactersNotInParty = starterData.GetStarterCharactersNotInParty();

            List<CharacterState> starterCharacters = new();
            starterCharacters.AddRange(starterParty);
            starterCharacters.AddRange(starterCharactersNotInParty);

            foreach (var starterCharacter in starterCharacters)
            {
                if (starterCharacter.character == null)
                    continue;

                int level = starterCharacter.level;
                long totalExp = expData.characterTotalExpAtLevelList[level] + starterCharacter.currentLevelExp;
                CharacterModel model = new(starterCharacter.character, expData, level, totalExp);

                Equip(model, starterCharacter.weapon?.id ?? EEquipmentId.None);
                Equip(model, starterCharacter.armor?.id ?? EEquipmentId.None);
                Equip(model, starterCharacter.artifact?.id ?? EEquipmentId.None);

                m_characters.Add(starterCharacter.character.id, model);
            }

            List<CharacterModel> teamMembers = new();
            foreach (var starterMember in starterParty)
            {
                if (starterMember.character == null)
                {
                    teamMembers.Add(null);
                    continue;
                }

                ECharacterId id = starterMember.character.id;

                if (m_characters.ContainsKey(id) == false)
                {
                    Debug.LogError("��Ÿ�� ���ÿ� ������ �ֽ��ϴ�. ����� ĳ���� ��Ͽ� �������� �ʽ��ϴ�.");
                    continue;
                }

                teamMembers.Add(m_characters[id]);
            }

            party = new(teamMembers);
        }

        void ConstructBestTeam()
        {
            List<CharacterModel> members = new();
            for (int i = 0; i < Constants.TeamMemberMaxCount; ++i)
                members.Add(null);

            party = new(members);

            BuildBestTeam();
        }

        public PartyModel party { get; private set; }

        public void BuildBestTeam()
        {
            int memberCount = Mathf.Min(m_characters.Count, Constants.TeamMemberMaxCount);

            List<CharacterModel> members = m_characters
                .OrderByDescending(kv => kv.Value.level)
                .ThenBy(kv => kv.Key)
                .Take(memberCount)
                .Select(kv => kv.Value)
                .ToList();

            for (int i = memberCount; i < Constants.TeamMemberMaxCount; ++i)
            {
                members.Add(null);
            }

            party.Rebuild(members);
        }

        ReactiveDictionary<ECharacterId, CharacterModel> m_characters = new();
        List<SortedCharacterListSubscription> subscriptions = new();

        public CharacterModel this[ECharacterId id]
        {
            get => m_characters[id];
        }

        public CharacterModel Add(ECharacterId id, int level = 1, int totalExp = 0)
        {
            CharacterModel character = new(
                m_gameDataDB.GetCharacterData(id),
                m_gameDataDB.GetExpSO(),
                level, totalExp);
            m_characters.Add(id, character);

            return character;
        }

        public bool Remove(ECharacterId id)
        {
            if (m_characters.ContainsKey(id) == false)
                return false;

            m_characters.Remove(id);
            party.Remove(m_characters[id]);

            return true;
        }

        public List<CharacterModel> GetSortedList()
        {
            return m_characters
                .Select(kv => kv.Value)
                .OrderByDescending(character => character.level)
                .ThenBy(character => character.characterId)
                .ToList();
        }

        public SortedCharacterListSubscription SubscribeSortedCharacterList(Action onCharacterListChangedAction)
        {
            CompositeDisposable characterLevelChangeSubscriptions = new();
            foreach (var kv in m_characters)
            {
                IDisposable characterLevelChangeSubscription = kv.Value.SubscribeLevelChangeEvent(level => onCharacterListChangedAction?.Invoke());
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
                IDisposable characterLevelChangeSubscription = kv.Value.SubscribeLevelChangeEvent(level => onCharacterListChangedAction?.Invoke());
                characterLevelChangeSubscriptions.Add(characterLevelChangeSubscription);
            }

            // �׼� ����
            onCharacterListChangedAction?.Invoke();
        }
    }
}
