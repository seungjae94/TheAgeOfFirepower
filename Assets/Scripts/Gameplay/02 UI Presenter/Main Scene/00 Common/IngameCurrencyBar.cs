using TMPro;
using UnityEngine;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    internal class IngameCurrencyBar : Presenter
    {
        [Inject] InventoryRepository m_inventoryRepository;

        [SerializeField] TMP_Text m_goldText;
        [SerializeField] TMP_Text m_diamondText;

        protected override void InitializeView()
        {
            m_goldText.text = m_inventoryRepository.gold.ToString();
            m_diamondText.text = 0.ToString();
        }

        protected override void SubscribeDataChange()
        {
            m_inventoryRepository
                .SubscribeGoldChange(gold => m_goldText.text = gold.ToString());
        }
    }
}
