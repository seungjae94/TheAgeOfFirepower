using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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
        
        // Field
        private readonly CompositeDisposable disposables = new();
        
        private bool didWin = true;
        private StageGameData stageGameData;
        
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
            
            // 뷰 초기화
            if (didWin)
            {
                InitializeWinView();
            }
            else
            {
                InitializeLoseView();
            }
            
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
            titleText.text = "WIN";
            winBoxObject.SetActive(true);
            loseBoxObject.SetActive(false);
            
            rewardScrollRect.UpdateContents(stageGameData.rewardList);

            for (int i = 0; i < Constants.BatterySize; ++i)
            {
                var battery = GameState.Inst.artyRosterState.Battery;
                var arty = battery[i];
                artyViews[i].Setup(arty);
                artyViews[i].Draw();
            }
        }
        
        private void InitializeLoseView()
        {
            titleText.text = "LOSE";
            winBoxObject.SetActive(false);
            loseBoxObject.SetActive(true);
        }
    }
}