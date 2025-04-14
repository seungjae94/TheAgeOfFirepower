using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class RewardSaveData
    {
        public int gold;
        public int diamond;
        public int itemType;
        public int itemId;
        public int itemAmount;

        public override bool Equals(object obj)
        {
            if (obj is not RewardSaveData other) return false;
            if (ReferenceEquals(this, other)) return true;
            return gold == other.gold
                   && diamond == other.diamond
                   && itemType == other.itemType
                   && itemId == other.itemId
                   && itemAmount == other.itemAmount;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(gold, diamond, itemType, itemId, itemAmount);
        }

        public Reward ToReward()
        {
            Reward reward = new();
            reward.gold = gold;
            reward.diamond = diamond;
            reward.itemAmount = itemAmount;

            if (itemType >= 0)
            {
                if (itemType == (int)EItemType.MechPart)
                {
                    reward.itemGameData = GameState.Inst.gameDataLoader.GetMechPartData(itemId);
                }
                else
                {
                    reward.itemGameData = GameState.Inst.gameDataLoader.GetCountableItemData((EItemType)itemType, itemId);
                }   
            }
            
            return reward;
        }
    }

    [Serializable]
    public class MailSaveData
    {
        public string title;
        public string content;
        public RewardSaveData reward;

        public override bool Equals(object obj)
        {
            if (obj is not MailSaveData other) return false;
            if (ReferenceEquals(this, other)) return true;
            return title == other.title
                   && content == other.content
                   && reward.Equals(other.reward);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(title, content, reward.GetHashCode());
        }

        public Mail ToMail()
        {
            Mail mail = new();
            mail.title = title;
            mail.content = content;
            mail.reward = reward.ToReward();
            return mail;
        }
    }

    [Serializable]
    public class GameProgressSaveFile : SaveFile
    {
        public int unlockWorld = 1;
        public int unlockStage = 1;
        public string userName = "";
        public List<MailSaveData> mails = new();

        public override bool Equals(object obj)
        {
            if (obj is not GameProgressSaveFile other) return false;
            if (ReferenceEquals(this, other)) return true;
            return unlockWorld == other.unlockWorld
                   && unlockStage == other.unlockStage
                   && userName == other.userName
                   && mails.SequenceEqual(other.mails);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(unlockWorld, unlockStage, userName, ListToHashCode(mails));
        }
    }
}