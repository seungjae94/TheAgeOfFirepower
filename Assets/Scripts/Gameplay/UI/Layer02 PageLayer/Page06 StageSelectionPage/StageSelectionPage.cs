using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class StageSelectionPage : Page
    {
        public override string PageName => "전투";
        
        private readonly CompositeDisposable disposables = new();
        
        protected override void OnOpen()
        {
            // Overlay
            Find<NavigateBackOverlay>().Activate();
        }

        protected override void OnClose()
        {
            disposables.Clear();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}
