using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class CurrencyBar : OverlayPresenter
    {
        InventoryState InventoryState => GameState.Inst.inventoryState;
        
        [SerializeField] TextMeshProUGUI goldText;
        
        [SerializeField] TextMeshProUGUI diamondText;

        public override void Activate()
        {
            base.Activate();
            
            InventoryState
                .goldRx
                .Subscribe(gold => goldText.text = gold.ToString())
                .AddTo(gameObject);
            
            goldText.text = InventoryState.goldRx.Value.ToString();
            diamondText.text = 0.ToString();
        }
    }
}
