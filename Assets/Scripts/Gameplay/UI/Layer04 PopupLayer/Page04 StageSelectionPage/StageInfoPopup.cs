using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class StageInfoPopup : PopupPresenter
    {
        // View
        [SerializeField]
        private TextMeshProUGUI titleText;

        [SerializeField]
        private Button enterButton;

        [SerializeField]
        private Button closeButton;
        
        [SerializeField]
        private StageInfoEnemyScrollRect enemyScrollRect;
        
        [SerializeField]
        private StageInfoRewardScrollRect rewardScrollRect;
        
        // Field
        private StageGameData stageGameData;
        private readonly CompositeDisposable disposables = new();
        
        public void Setup(StageGameData pStageGameData)
        {
            stageGameData = pStageGameData;
        }

        public override UniTask OpenWithAnimation()
        {
            titleText.text = $"스테이지 {stageGameData.displayName}";
            
            enterButton.OnClickAsObservable()
                .Subscribe(OnClickEnterButton)
                .AddTo(disposables);
            
            closeButton.OnClickAsObservable()
                .Subscribe(_ => CloseWithAnimation().Forget())
                .AddTo(disposables);
            
            enemyScrollRect.UpdateContents(stageGameData.enemyList);
            rewardScrollRect.UpdateContents(stageGameData.rewardList);
            
            return base.OpenWithAnimation();
        }

        public override UniTask CloseWithAnimation()
        {
            Find<StageSelectionPage>().onSelectStage.OnNext(null);
            
            disposables.Clear();
            return base.CloseWithAnimation();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
        
        // Callback
        private void OnClickEnterButton(Unit _)
        {
            GameState.Inst.battleState.stageGameData = stageGameData;
            GameManager.Inst.ChangeScene(SceneNames.PlayScene).Forget();
        }
    }
}