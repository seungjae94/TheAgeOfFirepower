using System;
using System.Collections.Generic;
using UniRx;
using Mathlife.ProjectL.Utils;
using System.Linq;

namespace Mathlife.ProjectL.Gameplay
{
    public class TeamModel
    {
        #region Constructors
        public TeamModel(CharacterModel leader, List<CharacterModel> members)
        {
            m_leader = new(leader);
            m_members = new(members);
        }
        #endregion

        #region Leader
        ReactiveProperty<CharacterModel> m_leader;
        public CharacterModel leader { get => m_leader.Value; private set => m_leader.Value = value; }

        public IDisposable SubscribeLeaderChangeEvent(Action<CharacterModel> onLeaderChangedAction)
        {
            return m_leader.Subscribe(onLeaderChangedAction);
        }

        public IDisposable SubscribeLeaderNotNullChangeEvent(Action<CharacterModel> onLeaderChangedAction)
        {
            return m_leader
                .Where(leader => leader != null)
                .Subscribe(onLeaderChangedAction);
        }

        public void AppointLeader(CharacterModel character)
        {
            if (null == character || Contains(character) == false)
                throw new ArgumentException($"Failed to appoint leader. Given character is null or not a member.");

            leader = character;
        }

        public void AppointLeaderAuto()
        {
            leader = null;

            for (int i = 0; i < m_members.Count; i++)
            {
                if (m_members[i] == null)
                    continue;

                if (leader == null)
                {
                    leader = m_members[i];
                    continue;
                }

                if (m_members[i].level > leader.level)
                {
                    leader = m_members[i];
                    continue;
                }

                if (m_members[i].level == leader.level && m_members[i].characterId < leader.characterId)
                {
                    leader = m_members[i];
                    continue;
                }
            }
        }
        #endregion

        #region Members
        ReactiveCollection<CharacterModel> m_members;

        public CharacterModel this[int i] => m_members[i];

        public CompositeDisposable SubscribeMemberChange(Action<CharacterModel> onMemberChangedAction)
        {
            CompositeDisposable subscriptions = new();

            for (int i = 0; i < m_members.Count; i++)
            {
                int iCapture = i;
                IDisposable subscription = m_members
                    .ObserveEveryValueChanged(members => members[iCapture])
                    .Subscribe(character => onMemberChangedAction(character));
                subscriptions.Add(subscription);
            }

            return subscriptions;
        }

        public bool Contains(CharacterModel character)
        {
            if (null == character)
                throw new ArgumentNullException("Tried to check if team contains null.");

            return m_members.Contains(character);
        }

        public int IndexOf(CharacterModel character)
        {
            return m_members.IndexOf(character);
        }

        // ¸â¹ö <-> ¸â¹ö
        public void Swap(int i, int j)
        {
            m_members.Swap(i, j);
        }

        // º¸À¯ Ä³¸¯ÅÍ -> ¸â¹ö
        public void Add(int index, CharacterModel character)
        {
            if (null == character)
                throw new ArgumentNullException("Failed to add a member. Given character was null.");

            if (Contains(character))
                throw new ArgumentException("Failed to add a member. Given character was already in the team.");

            var oldCharacter = m_members[index];

            m_members[index] = character;

            if (oldCharacter == leader)
                AppointLeaderAuto();
        }

        // ¸â¹ö -> º¸À¯ Ä³¸¯ÅÍ
        public void RemoveAt(int index)
        {
            Remove(m_members[index]);
        }

        // ¸â¹ö -> º¸À¯ Ä³¸¯ÅÍ
        public void Remove(CharacterModel character)
        {
            if (null == character)
                throw new ArgumentNullException("Failed to remove a member. Given character was null.");

            if (false == Contains(character))
                throw new ArgumentException("Failed to remove a member. Given character was not a member.");

            m_members[m_members.IndexOf(character)] = null;

            if (leader == character)
                AppointLeaderAuto();
        }

        public void Clear()
        {
            for (int i = 0; i < m_members.Count; ++i)
            {
                m_members[i] = null;
            }

            leader = null;
        }

        public bool IsEmpty()
        {
            return memberCount == 0;
        }

        public void Rebuild(CharacterModel leader, List<CharacterModel> members)
        {
            Clear();
            this.leader = leader;
            for (int i = 0; i < m_members.Count; ++i)
            {
                m_members[i] = members[i];
            }
        }

        int memberCount
        {
            get
            {
                int count = 0;
                foreach (var member in m_members)
                {
                    if (member == null)
                        continue;
                    count++;
                }
                return count;
            }
        }
        #endregion

        public bool Validate()
        {
            return m_members.Count > 0 && leader != null && m_members.Contains(leader);
        }
    }
}
