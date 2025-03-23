using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Mathlife.ProjectL.Gameplay.UI.ArtyPagePopup
{
    public class ArtyPageMechPartChangeScrollViewCell
        : FancyGridViewCell<ItemData, Context>
    {
        // View
        [SerializeField]
        private GameObject itemSlotGameObject;

        [SerializeField]
        private GameObject selectionGameObject;

        [SerializeField]
        private GameObject equippedMarkGameObject;

        [SerializeField]
        private UIEffect uiEffect;

        [SerializeField]
        private Button itemViewButton;

        [SerializeField]
        private Image iconImage;

        [SerializeField]
        private Button emptyViewButton;

        // Field
        private MechPartModel mechPart;

        // Override
        public override void Initialize()
        {
            itemViewButton.OnClickAsObservable()
                .Subscribe(OnClick)
                .AddTo(gameObject);

            emptyViewButton.OnClickAsObservable()
                .Subscribe(OnClick)
                .AddTo(gameObject);
        }

        public override void UpdateContent(ItemData itemData)
        {
            mechPart = itemData.mechPart;

            if (mechPart == null)
            {
                itemSlotGameObject.SetActive(false);
                selectionGameObject.SetActive(false);
                equippedMarkGameObject.SetActive(false);
                return;
            }

            itemSlotGameObject.SetActive(true);
            uiEffect.LoadPreset(mechPart.Rarity.ToGradientPresetName());
            iconImage.sprite = mechPart.Icon;

            equippedMarkGameObject.SetActive(mechPart.Owner.Value != null);
            selectionGameObject.SetActive(Context.selectedIndex == Index);
        }

        // 상호작용 콜백
        private void OnClick(Unit _)
        {
            Context.onCellClickRx.OnNext(Index);
        }
    }
}