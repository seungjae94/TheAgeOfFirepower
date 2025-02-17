using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.TextCore.Text;

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
                Debug.LogError("인벤토리 리포지토리를 먼저 생성해야 합니다.");
                throw new Exception("인벤토리 리포지토리를 먼저 생성해야 합니다.");
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

            // 보유 캐릭터가 없다. 데이터가 완전히 손상되었다.
            if (m_characters.Count == 0)
            {
                ConstructFromStarterData();
                return;
            }

            TeamSaveData teamSaveData = characterSaveFile.team;

            // 팀 데이터가 잘못 설정되어 있다. 리더 혹은 멤버가 없다.
            if (false == teamSaveData.IsHealthy())
            {
                ConstructBestTeam();
                return;
            }

            // Construct team by data.
            CharacterModel leader = m_characters[teamSaveData.leader];
            List<CharacterModel> members = teamSaveData.members
                .Select((memberId) => (memberId == ECharacterId.None) ? null : m_characters[memberId])
                .ToList();

            team = new(leader, members);
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

            CharacterModel leader = null;
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
                    Debug.LogError("스타터 세팅에 오류가 있습니다. 멤버가 캐릭터 목록에 존재하지 않습니다.");
                    continue;
                }

                teamMembers.Add(m_characters[id]);

                if (null == leader)
                {
                    leader = m_characters[id];
                }
            }

            team = new(leader, teamMembers);
        }

        void ConstructBestTeam()
        {
            CharacterModel leader = null;
            List<CharacterModel> members = new();
            for (int i = 0; i < Constants.TeamMemberMaxCount; ++i)
                members.Add(null);

            team = new(leader, members);

            BuildBestTeam();
        }

        public TeamModel team { get; private set; }

        public void BuildBestTeam()
        {
            int memberCount = Mathf.Min(m_characters.Count, Constants.TeamMemberMaxCount);

            List<CharacterModel> members = m_characters
                .OrderByDescending(kv => kv.Value.level)
                .ThenBy(kv => kv.Key)
                .Take(memberCount)
                .Select(kv => kv.Value)
                .ToList();

            CharacterModel leader = members[0];

            for (int i = memberCount; i < Constants.TeamMemberMaxCount; ++i)
            {
                members.Add(null);
            }

            team.Rebuild(leader, members);
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
            team.Remove(m_characters[id]);

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
            // 모든 캐릭터 모델 다시 구독
            characterLevelChangeSubscriptions.Clear();
            foreach (var kv in m_characters)
            {
                IDisposable characterLevelChangeSubscription = kv.Value.SubscribeLevelChangeEvent(level => onCharacterListChangedAction?.Invoke());
                characterLevelChangeSubscriptions.Add(characterLevelChangeSubscription);
            }

            // 액션 실행
            onCharacterListChangedAction?.Invoke();
        }
    }
}
