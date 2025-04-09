using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

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
        
        // Field
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
            BlurPopup blurPopup = Find<BlurPopup>();
            await blurPopup.CloseWithAnimation();
            await base.CloseWithAnimation();
        }

        private void InitializeWinView()
        {
            titleText.text = "WIN";
            winBoxObject.SetActive(true);
            loseBoxObject.SetActive(false);
        }
        
        private void InitializeLoseView()
        {
            titleText.text = "LOSE";
            winBoxObject.SetActive(false);
            loseBoxObject.SetActive(true);
        }
    }
}