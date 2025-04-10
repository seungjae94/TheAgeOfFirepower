using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Mathlife.ProjectL.Utils;
using System.Linq;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class BatteryModel : IEnumerable<ArtyModel>
    {
        private readonly ReactiveCollection<ArtyModel> membersRx;
        public BatteryModel(List<ArtyModel> members)
        {
            membersRx = new(members);
        }

        public ArtyModel this[int i] => membersRx[i];
        
        public bool Contains(ArtyModel arty)
        {
            if (null == arty)
                throw new ArgumentNullException("Tried to check if team contains null.");

            return membersRx.Contains(arty);
        }

        public int IndexOf(ArtyModel arty)
        {
            return membersRx.IndexOf(arty);
        }

        // 슬롯 스왑
        public void Swap(int i, int j)
        {
            membersRx.Swap(i, j);
        }

        // 멤버 추가
        public void Add(int index, ArtyModel arty)
        {
            if (null == arty)
            {
                RemoveAt(index);
                return;
            }

            if (Contains(arty))
                throw new ArgumentException("Failed to add a member. Given character was already in the team.");

            var oldCharacter = membersRx[index];

            membersRx[index] = arty;
        }

        // 멤버 제거 (인덱스)
        public void RemoveAt(int index)
        {
            Remove(membersRx[index]);
        }

        // 멤버 제거 (모델)
        public void Remove(ArtyModel arty)
        {
            if (null == arty)
                throw new ArgumentNullException("Failed to remove a member. Given character was null.");

            if (false == Contains(arty))
                throw new ArgumentException("Failed to remove a member. Given character was not a member.");

            membersRx[membersRx.IndexOf(arty)] = null;
        }

        // 멤버 전부 제거
        public void Clear()
        {
            for (int i = 0; i < membersRx.Count; ++i)
            {
                membersRx[i] = null;
            }
        }

        public void Rebuild(List<ArtyModel> members)
        {
            Clear();

            for (int i = 0; i < membersRx.Count; ++i)
            {
                membersRx[i] = members[i];
            }
        }

        public bool Validate()
        {
            return membersRx.Count(arty => arty != null) > 0;
        }

        public IEnumerator<ArtyModel> GetEnumerator()
        {
            return membersRx.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
