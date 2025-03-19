using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class CurrencyBar : OverlayPresenter
    {
        InventoryState InventoryState => GameState.Inst.inventoryState;
        
        [FormerlySerializedAs("m_goldText")]
        [SerializeField] TextMeshProUGUI goldText;
        
        [FormerlySerializedAs("m_diamondText")]
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
