using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class MailScrollRectContext : SimpleScrollRectContext
    {
        public Action updateContents = null;
    }

    public class MailScrollRect
        : SimpleScrollRect<Mail, MailScrollRectContext>
    {
        protected override void Initialize()
        {
            base.Initialize();
            Context.updateContents = UpdateContentsAuto;
        }

        public void UpdateContentsAuto()
        {
            UpdateContents(GameState.Inst.gameProgressState.mailsRx.ToList());
        }
    }
}