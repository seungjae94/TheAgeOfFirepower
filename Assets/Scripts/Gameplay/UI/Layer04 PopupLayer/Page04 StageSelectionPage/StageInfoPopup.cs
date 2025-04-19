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
        private bool isOpen;
        
        public void Setup(StageGameData pStageGameData)
        {
            stageGameData = pStageGameData;
            isOpen = false;
        }

        public override UniTask OpenWithAnimation()
        {
            disposables.Clear();
            
            AudioManager.Inst.PlaySE(ESoundEffectId.Ok);
            
            titleText.text = $"스테이지 {stageGameData.displayName}";
            
            enterButton.OnClickAsObservable()
                .Subscribe(OnClickEnterButton)
                .AddTo(disposables);
            
            closeButton.OnClickAsObservable()
                .Subscribe(_ => CloseWithAnimation().Forget())
                .AddTo(disposables);
            
            enemyScrollRect.UpdateContents(stageGameData.enemyList);
            rewardScrollRect.UpdateContents(stageGameData.rewardList);
            enemyScrollRect.ResetPosition();
            rewardScrollRect.ResetPosition();
            
            return base.OpenWithAnimation();
        }

        public override UniTask CloseWithAnimation()
        {
            AudioManager.Inst.PlaySE(ESoundEffectId.Ok);
            
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
            AudioManager.Inst.PlaySE(ESoundEffectId.Ok);
            
            GameState.Inst.battleState.stageGameData = stageGameData;
            GameManager.Inst.ChangeScene(SceneNames.PLAY_SCENE).Forget();
        }
    }
}