using System;
using Coffee.UIEffects;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class InventoryMechPartGridViewCell
        : SimpleGridViewCell<MechPartModel, SimpleGridViewContext>
    {
        // View
        [SerializeField]
        private GameObject selectionGameObject;

        [SerializeField]
        private GameObject equippedMarkGameObject;

        [SerializeField]
        private UIEffect uiEffect;

        [SerializeField]
        private Image iconImage;

        // Field
        private MechPartModel mechPart;

        public override void UpdateContent(MechPartModel itemData)
        {
            mechPart = itemData;

            if (mechPart == null)
            {
                throw new ArgumentNullException("[InventoryMechPartGridViewCell] null 부품 데이터가 인자로 들어왔습니다.");
            }

            uiEffect.LoadPreset(mechPart.Rarity.ToGradientPresetName());
            iconImage.sprite = mechPart.Icon;

            equippedMarkGameObject.SetActive(mechPart.Owner.Value != null);
            selectionGameObject.SetActive(Context.selectedIndex == Index);
        }
        
        protected override void OnClick(Unit _)
        {
            base.OnClick(_);
            AudioManager.Inst.PlaySE(ESoundEffectId.Ok);
        }
    }
}