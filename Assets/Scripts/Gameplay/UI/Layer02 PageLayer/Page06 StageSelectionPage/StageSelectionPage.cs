using Coffee.UIExtensions;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class StageSelectionPage : Page
    {
        public override string PageName => "스테이지 선택";

        // View
        [SerializeField]
        private ScrollRect scrollRect; 
        
        [SerializeField]
        private WorldMapView worldMapView;

        [SerializeField]
        private UIParticle selectionParticleSystem;

        // Field
        private readonly CompositeDisposable disposables = new();
        public readonly Subject<StageInfoView> onSelectStage = new();

        protected override void OnOpen()
        {
            // Overlay
            Find<NavigateBackOverlay>().Activate();

            // 뷰 초기화
            scrollRect.horizontalNormalizedPosition = 0f;
            
            worldMapView.Setup(1);
            worldMapView.Draw();

            selectionParticleSystem.enabled = false;
            
            // 이벤트 구독
            onSelectStage
                .Subscribe(OnSelectStage)
                .AddTo(disposables);
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
        
        // Callback
        private void OnSelectStage(StageInfoView stageInfoView)
        {
            if (stageInfoView == null)
            {
                selectionParticleSystem.Clear();
                return;
            }
            
            selectionParticleSystem.transform.position = stageInfoView.transform.position;
            selectionParticleSystem.enabled = true;
            selectionParticleSystem.Play();
        }
    }
}