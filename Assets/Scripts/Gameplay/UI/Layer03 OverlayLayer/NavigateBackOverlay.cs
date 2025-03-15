using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class NavigateBackOverlay : OverlayPresenter
    {
        private TextMeshProUGUI titleText;
        private Button navBackButton;
        
        private readonly CompositeDisposable disposables = new();
        
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
                .Subscribe(_ => Page.CurrentPage.Close())
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
    }
}