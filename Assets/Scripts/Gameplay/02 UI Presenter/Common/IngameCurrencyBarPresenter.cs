using TMPro;
using UnityEngine;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    internal class IngameCurrencyBarPresenter : Presenter
    {
        [Inject] InventoryRepository m_inventoryRepository;

        [SerializeField] TMP_Text m_goldText;
        [SerializeField] TMP_Text m_diamondText;

        public void Initilize()
        {
            m_inventoryRepository
                .SubscribeGoldChange(gold => m_goldText.text = gold.ToString());
        }
    }
}
