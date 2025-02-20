using System;
using System.Collections.Generic;
using UniRx;
using Mathlife.ProjectL.Utils;
using System.Linq;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class PartyModel
    {
        public PartyModel(List<CharacterModel> members)
        {
            m_members = new(members);
        }

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
            {
                RemoveAt(index);
                return;
            }

            if (Contains(character))
                throw new ArgumentException("Failed to add a member. Given character was already in the team.");

            var oldCharacter = m_members[index];

            m_members[index] = character;
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
        }

        public void Clear()
        {
            for (int i = 0; i < m_members.Count; ++i)
            {
                m_members[i] = null;
            }
        }

        public bool IsEmpty()
        {
            return memberCount == 0;
        }

        public void Rebuild(List<CharacterModel> members)
        {
            Clear();

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

        public bool Validate()
        {
            return m_members.Count > 0;
        }
    }
}
