using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    [Serializable]
    public class Mail
    {
        [LabelText("제목")]
        [LabelWidth(50)]
        public string title;
        
        [LabelText("내용")]
        [LabelWidth(50)]
        [Multiline(2)]
        public string content;
        
        [InlineProperty]
        [LabelWidth(50)]
        [LabelText("보상")]
        public Reward reward;

        public MailSaveData ToMailSaveData()
        {
            MailSaveData saveData = new();
            saveData.title = title;
            saveData.content = content;
            saveData.reward = reward.ToRewardSaveData();
            return saveData;
        }
    }
}