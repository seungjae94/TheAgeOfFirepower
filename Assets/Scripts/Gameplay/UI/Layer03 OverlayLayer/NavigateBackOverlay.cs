using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class NavigateBackOverlay : OverlayPresenter
    {
        // View
        private TextMeshProUGUI titleText;
        private Button navBackButton;
        
        // Field
        public readonly Subject<NavigateBackOverlay> onNavigateBack = new();
        private readonly CompositeDisposable disposables = new();
        private bool shouldNavigate = false;
        
        private void Awake()
        {
            titleText = transform.FindRecursiveByName<TextMeshProUGUI>("Title");
            navBackButton = transform.FindRecursive<Button>();
        }

        public override void Activate()
        {
            base.Activate();

            navBackButton
                .OnClickAsObservable()
                .Subscribe(OnClick)
                .AddTo(disposables);
            
            titleText.text = Page.CurrentPage.PageName;
        }
        
        public override void Deactivate()
        {
            disposables.Clear();
            base.Deactivate();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }

        private void OnClick(Unit _)
        {
            shouldNavigate = true;
            onNavigateBack.OnNext(this);
            
            if (shouldNavigate)
                Page.CurrentPage.Close();
        }

        public void StopNavigation()
        {
            shouldNavigate = false;
        }
    }
}