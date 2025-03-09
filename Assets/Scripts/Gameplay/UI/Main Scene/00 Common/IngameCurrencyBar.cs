using TMPro;
using UnityEngine;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    internal class IngameCurrencyBar : Presenter
    {
        [Inject] InventoryState inventoryState;

        [SerializeField] TMP_Text m_goldText;
        [SerializeField] TMP_Text m_diamondText;

        protected override void InitializeView()
        {
            m_goldText.text = inventoryState.gold.ToString();
            m_diamondText.text = 0.ToString();
        }

        protected override void SubscribeDataChange()
        {
            inventoryState
                .SubscribeGoldChange(gold => m_goldText.text = gold.ToString());
        }
    }
}
