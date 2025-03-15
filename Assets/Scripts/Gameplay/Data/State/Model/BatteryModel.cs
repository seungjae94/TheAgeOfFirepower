using System;
using System.Collections.Generic;
using UniRx;
using Mathlife.ProjectL.Utils;
using System.Linq;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class BatteryModel
    {
        public BatteryModel(List<ArtyModel> members)
        {
            m_members = new(members);
        }

        ReactiveCollection<ArtyModel> m_members;

        public ArtyModel this[int i] => m_members[i];

        public CompositeDisposable SubscribeMemberChange(Action<ArtyModel> onMemberChangedAction)
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

        public bool Contains(ArtyModel arty)
        {
            if (null == arty)
                throw new ArgumentNullException("Tried to check if team contains null.");

            return m_members.Contains(arty);
        }

        public int IndexOf(ArtyModel arty)
        {
            return m_members.IndexOf(arty);
        }

        // ??? <-> ???
        public void Swap(int i, int j)
        {
            m_members.Swap(i, j);
        }

        // ???? ĳ???? -> ???
        public void Add(int index, ArtyModel arty)
        {
            if (null == arty)
            {
                RemoveAt(index);
                return;
            }

            if (Contains(arty))
                throw new ArgumentException("Failed to add a member. Given character was already in the team.");

            var oldCharacter = m_members[index];

            m_members[index] = arty;
        }

        // ??? -> ???? ĳ????
        public void RemoveAt(int index)
        {
            Remove(m_members[index]);
        }

        // ??? -> ???? ĳ????
        public void Remove(ArtyModel arty)
        {
            if (null == arty)
                throw new ArgumentNullException("Failed to remove a member. Given character was null.");

            if (false == Contains(arty))
                throw new ArgumentException("Failed to remove a member. Given character was not a member.");

            m_members[m_members.IndexOf(arty)] = null;
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

        public void Rebuild(List<ArtyModel> members)
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
