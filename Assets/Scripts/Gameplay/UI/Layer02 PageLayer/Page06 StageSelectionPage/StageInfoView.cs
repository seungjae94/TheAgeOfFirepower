using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.Gameplay.Data.Model;
using Mathlife.ProjectL.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Mathlife.ProjectL.Gameplay.UI
{
    public class StageInfoView : AbstractView
    {
        // Alias
        private static GameProgressState GameProgressState => GameState.Inst.gameProgressState;
        
        // View
        [SerializeField]
        private CanvasGroup canvasGroup;
        
        [SerializeField]
        private Button button;

        // Field
        private StageGameData stageGameData;
        private readonly CompositeDisposable disposables = new();
        
        public void Setup(StageGameData pStageGameData)
        {
            stageGameData = pStageGameData;
        }

        public override void Draw()
        {
            base.Draw();
            
            button.OnClickAsObservable()
                .Subscribe(OnClick)
                .AddTo(disposables);
            
            GameProgressState.unlockWorldRx
                .DistinctUntilChanged()
                .Subscribe(_ => UpdateView())
                .AddTo(disposables);
            
            GameProgressState.unlockStageRx
                .DistinctUntilChanged()
                .Subscribe(_ => UpdateView())
                .AddTo(disposables);
            
            UpdateView();
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
        private void UpdateView()
        {
            if (GameProgressState.unlockWorldRx.Value > stageGameData.worldNo)
            {
                canvasGroup.Show();
            }
            else if (GameProgressState.unlockWorldRx.Value < stageGameData.worldNo)
            {
                canvasGroup.Hide();
            }
            else if (GameProgressState.unlockStageRx.Value >= stageGameData.stageNo)
            {
                canvasGroup.Show();
            }
            else
            {
                canvasGroup.Hide();
            }
        }
        
        private void OnClick(Unit _)
        {
            var stageSelectionPage = Presenter.Find<StageSelectionPage>();
            stageSelectionPage.onSelectStage.OnNext(this);
            
            var popup = Presenter.Find<StageInfoPopup>();
            popup.Setup(stageGameData);
            popup.OpenWithAnimation().Forget();
        }
    }
}