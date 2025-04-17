using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.Gameplay.Data.Model;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class BattleResultPopup : PopupPresenter
    {
        [SerializeField]
        private TextMeshProUGUI titleText;

        [SerializeField]
        private GameObject winBoxObject;
        
        [SerializeField]
        private GameObject loseBoxObject;

        [SerializeField]
        private List<BattleResultArtyView> artyViews = new();
        
        [SerializeField]
        private BattleRewardScrollRect rewardScrollRect;
        
        [SerializeField]
        private Button backToLobbyButton; 
        
        // Audio
        [SerializeField]
        private AudioClip winSE;
        
        [SerializeField]
        private AudioClip loseSE;
        
        // Field
        private readonly CompositeDisposable disposables = new();
        
        private bool didWin = true;
        private StageGameData stageGameData;
        
        private float bgmVolume = 1f;
        
        public void Setup(bool didWin, StageGameData stageGameData)
        {
            this.didWin = didWin;
            this.stageGameData = stageGameData;
        }
        
        public override async UniTask OpenWithAnimation()
        {
            // 블러 적용
            BlurPopup blurPopup = Find<BlurPopup>();
            blurPopup.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
            await blurPopup.OpenWithAnimation();
            
            // 볼륨 조절
            bgmVolume = AudioManager.Inst.BGMVolume;
            AudioManager.Inst.SetBGMVolume(bgmVolume / 2f);
            
            // 뷰 초기화
            if (didWin)
            {
                InitializeWinView();
            }
            else
            {
                InitializeLoseView();
            }
            
            backToLobbyButton.
                OnClickAsObservable()
                .Subscribe(_ => OnClickButton().Forget())
                .AddTo(disposables);
            
            await base.OpenWithAnimation();
        }

        public override async UniTask CloseWithAnimation()
        {
            disposables.Clear();
            
            BlurPopup blurPopup = Find<BlurPopup>();
            await blurPopup.CloseWithAnimation();
            await base.CloseWithAnimation();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }

        private void InitializeWinView()
        {
            // SE 재생
            AudioManager.Inst.PauseBGM();
            AudioManager.Inst.PlaySE(winSE);
            UniTask.Delay((int)(winSE.length * 1000))
                .ContinueWith(() =>
                {
                    AudioManager.Inst.ResumeBGM();
                })
                .Forget();
            
            // 실제 승리 처리
            InventoryState inventoryState = GameState.Inst.inventoryState;
            var rewardList = stageGameData.rewardList;

            foreach (var reward in rewardList)
            {
                inventoryState.GainReward(reward);
            }

            int stageCount = GameState.Inst.gameDataLoader.GetStageCount(stageGameData.worldNo);
            
            GameProgressState gameProgressState = GameState.Inst.gameProgressState;
            
            // 처음 플레이할 경우 다음 스테이지 언락
            if (gameProgressState.unlockWorldRx.Value == stageGameData.worldNo
                && gameProgressState.unlockStageRx.Value == stageGameData.stageNo)
            {
                int nextWorldNo = stageGameData.worldNo; 
                int nextStageNo = stageGameData.stageNo + 1;
                if (nextStageNo > stageCount)
                {
                    ++nextWorldNo;
                    nextStageNo = 1;
                }

                gameProgressState.unlockWorldRx.Value = nextWorldNo; 
                gameProgressState.unlockStageRx.Value = nextStageNo;

                Debug.Log($"Unlock {nextWorldNo}-{nextStageNo}");
            }
            
            // UI 업데이트
            titleText.text = "WIN";
            winBoxObject.SetActive(true);
            loseBoxObject.SetActive(false);
            
            rewardScrollRect.UpdateContents(rewardList);

            for (int i = 0; i < Constants.BatterySize; ++i)
            {
                var battery = GameState.Inst.artyRosterState.Battery;
                var arty = battery[i];
                artyViews[i].Setup(arty);
                artyViews[i].Draw();
            }
            
            // 변경사항 저장
            GameState.Inst.Save();
        }
        
        private void InitializeLoseView()
        {
            // SE 재생
            AudioManager.Inst.PauseBGM();
            AudioManager.Inst.PlaySE(loseSE);
            UniTask.Delay((int)(loseSE.length * 1000))
                .ContinueWith(() =>
                {
                    AudioManager.Inst.ResumeBGM();
                })
                .Forget();
            
            titleText.text = "LOSE";
            winBoxObject.SetActive(false);
            loseBoxObject.SetActive(true);
        }

        private async UniTaskVoid OnClickButton()
        {
            AudioManager.Inst.PlaySE(ESoundEffectId.Ok);
            
            await UniTask.NextFrame();
            GameManager.Inst.ChangeScene(SceneNames.LobbyScene)
                .ContinueWith(() => AudioManager.Inst.SetBGMVolume(bgmVolume))
                .Forget();
        }
    }
}