using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class StageSelectionPage : Page
    {
        public override string PageName => "전투";
        
        // View
        [SerializeField]
        private WorldMapView worldMapView;
        
        // Field
        private readonly CompositeDisposable disposables = new();
        
        protected override void OnOpen()
        {
            // Overlay
            Find<NavigateBackOverlay>().Activate();
            
            worldMapView.Setup(1);
            worldMapView.Draw();
        }

        protected override void OnClose()
        {
            worldMapView.Clear();
            disposables.Clear();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}
