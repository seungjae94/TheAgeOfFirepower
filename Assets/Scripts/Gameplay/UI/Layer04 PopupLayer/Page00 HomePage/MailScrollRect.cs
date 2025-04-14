using System;
using System.Collections.Generic;
using UniRx;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class Mail
    {
        public string title;
        public string content;
        public Reward reward;
    }

    public class MailScrollRectContext : SimpleScrollRectContext
    {
        public Action<int> removeTestMail = null;
        public Action updateContents = null;
    }

    public class MailScrollRect
        : SimpleScrollRect<Mail, MailScrollRectContext>
    {
        List<Mail> testMails = new List<Mail>();
        
        protected override void Initialize()
        {
            base.Initialize();

            // 테스트용 데이터
            testMails.Add(new Mail()
                { title = "사전 예약 보상", content = "컨텐츠 1", reward = new Reward() { gold = 100000 } });
            testMails.Add(new Mail()
                { title = "사전 예약 보상", content = "컨텐츠 2", reward = new Reward() { diamond = 3000 } });
            testMails.Add(new Mail()
                { title = "사전 예약 보상", content = "컨텐츠 3", reward = new Reward() { diamond = 2000 } });
            testMails.Add(new Mail()
                { title = "사전 예약 보상", content = "컨텐츠 4", reward = new Reward() { diamond = 1000 } });
            testMails.Add(new Mail()
                { title = "사전 예약 보상", content = "컨텐츠 5", reward = new Reward() { diamond = 500 } });
            
            Context.removeTestMail = (index) => testMails.RemoveAt(index); 
            Context.updateContents = UpdateContentsAuto;
        }

        public void UpdateContentsAuto()
        {
            // TODO: 메일 데이터 가져와서...
            UpdateContents(testMails);
        }
    }
}