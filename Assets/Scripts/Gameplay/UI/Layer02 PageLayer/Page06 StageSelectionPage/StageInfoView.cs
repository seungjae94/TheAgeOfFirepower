using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Mathlife.ProjectL.Gameplay.UI
{
    public class StageInfoView : AbstractView
    {
        // View
        private Button button;

        // Field
        private StageGameData stageGameData;
        private readonly CompositeDisposable disposables = new();
        
        public void Setup(StageGameData pStageGameData)
        {
            stageGameData = pStageGameData;

            if (button == null)
            {
                button = GetComponent<Button>();
            }
        }

        public override void Draw()
        {
            base.Draw();
            
            // TODO: Locked 여부 및 선택 여부에 따라 다르게 렌더링
            
            button.OnClickAsObservable()
                .Subscribe(OnClick)
                .AddTo(disposables);
        }

        public override void Clear()
        {
            base.Clear();
            
            disposables.Clear();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
        
        // Callback
        private void OnClick(Unit _)
        {
            var popup = Presenter.Find<StageInfoPopup>();
            popup.Setup(stageGameData);
            popup.OpenWithAnimation().Forget();
        }
    }
}