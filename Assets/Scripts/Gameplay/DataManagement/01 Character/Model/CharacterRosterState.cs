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

    public class CharacterRosterState : IState
    {
        SaveDataManager saveDataManager;
        GameDataLoader gameDataLoader;
        
        public async UniTask Load()
        {
            saveDataManager = GameState.Inst.SaveDataManager;
            gameDataLoader = GameState.Inst.GameDataLoader;

            await UniTask.SwitchToThreadPool();
            
            // Validate that InventoryRepository was created before creating CharacterRepository.
            if (GameState.Inst.InventoryState == null)
            {
                Debug.LogError("�κ��丮 �������丮�� ���� �����ؾ� �մϴ�.");
                throw new Exception("�κ��丮 �������丮�� ���� �����ؾ� �մϴ�.");
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
            CharacterSaveFile characterSaveFile = saveDataManager.character;

            foreach (var characterSaveData in characterSaveFile.characters)
            {
                CharacterModel model = new CharacterModel(
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

            PartySaveData teamSaveData = characterSaveFile.party;

            // �� �����Ͱ� �߸� �����Ǿ� �ִ�. ���� Ȥ�� ����� ����.
            if (false == teamSaveData.IsHealthy())
            {
                ConstructBestTeam();
                return;
            }

            // Construct team by data.
            List<CharacterModel> members = teamSaveData.members
                .Select((memberId) => (memberId == -1) ? null : m_characters[memberId])
                .ToList();

            party = new(members);
        }

        void LoadFromStarterData()
        {
            StarterSO starterData = gameDataLoader.GetStarterSO();
            ExpSO expData = gameDataLoader.GetExpSO();

            var starterParty = starterData.GetStarterParty();
            var starterCharactersNotInParty = starterData.GetStarterCharactersNotInParty();

            List<CharacterPreset> starterCharacters = new();
            starterCharacters.AddRange(starterParty);
            starterCharacters.AddRange(starterCharactersNotInParty);

            foreach (var starterCharacter in starterCharacters)
            {
                if (starterCharacter.character == null)
                    continue;

                int level = starterCharacter.level;
                long totalExp = expData.characterTotalExpAtLevelList[level] + starterCharacter.currentLevelExp;
                CharacterModel model = new(starterCharacter.character, expData, level, totalExp);

                Equip(model, starterCharacter.weapon?.id ?? -1);
                Equip(model, starterCharacter.armor?.id ?? -1);
                Equip(model, starterCharacter.artifact?.id ?? -1);

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

                int id = starterMember.character.id;

                if (m_characters.TryGetValue(id, out var character) == false)
                {
                    Debug.LogError("��Ÿ�� ���ÿ� ������ �ֽ��ϴ�. ����� ĳ���� ��Ͽ� �������� �ʽ��ϴ�.");
                    continue;
                }

                teamMembers.Add(character);
            }

            party = new(teamMembers);
        }
        
        public void Equip(CharacterModel character, int equipmentId)
        {
            EEquipmentType equipmentType = (EEquipmentType)(equipmentId / 1000);
            character.Equip(equipmentType, GameState.Inst.InventoryState.FindEquipment(equipmentType, equipmentId));
        }

        void ConstructBestTeam()
        {
            List<CharacterModel> members = new();
            for (int i = 0; i < Constants.PartyMemberCount; ++i)
                members.Add(null);

            party = new(members);

            BuildBestTeam();
        }

        public PartyModel party { get; private set; }

        public void BuildBestTeam()
        {
            int memberCount = Mathf.Min(m_characters.Count, Constants.PartyMemberCount);

            List<CharacterModel> members = m_characters
                .OrderByDescending(kv => kv.Value.level)
                .ThenBy(kv => kv.Key)
                .Take(memberCount)
                .Select(kv => kv.Value)
                .ToList();

            for (int i = memberCount; i < Constants.PartyMemberCount; ++i)
            {
                members.Add(null);
            }

            party.Rebuild(members);
        }

        ReactiveDictionary<int, CharacterModel> m_characters = new();
        List<SortedCharacterListSubscription> subscriptions = new();

        public CharacterModel this[int id]
        {
            get => m_characters[id];
        }

        public CharacterModel Add(int id, int level = 1, int totalExp = 0)
        {
            CharacterModel character = new(
                gameDataLoader.GetCharacterData(id),
                gameDataLoader.GetExpSO(),
                level, totalExp);
            m_characters.Add(id, character);

            return character;
        }

        public bool Remove(int id)
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
                .ThenBy(character => character.id)
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
