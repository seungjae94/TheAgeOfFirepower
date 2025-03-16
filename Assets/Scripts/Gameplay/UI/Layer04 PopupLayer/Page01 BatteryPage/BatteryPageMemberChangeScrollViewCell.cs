using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Mathlife.ProjectL.Gameplay.UI.BatteryPagePopup
{
    public class BatteryPageMemberChangeScrollViewCell
        : FancyGridViewCell<ItemData, Context>
    {
        // Alias
        private BatteryPage BatteryPage => Presenter.Find<BatteryPage>();
        private ArtyRosterState ArtyRosterState => GameState.Inst.artyRosterState;

        // View
        [SerializeField]
        private CanvasGroup artyViewCanvasGroup;

        [SerializeField]
        private Button artyViewButton;

        [SerializeField]
        private Image portraitImage;

        [SerializeField]
        private TextMeshProUGUI levelText;

        [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField]
        private Button emptyViewButton;

        // Field
        private ArtyModel arty;

        // Override
        public override void Initialize()
        {
            artyViewButton.OnClickAsObservable()
                .Subscribe(OnClick)
                .AddTo(gameObject);

            emptyViewButton.OnClickAsObservable()
                .Subscribe(OnClick)
                .AddTo(gameObject);
        }

        public override void UpdateContent(ItemData itemData)
        {
            arty = itemData.arty;

            if (arty == null)
            {
                artyViewCanvasGroup.Hide();
                return;
            }

            artyViewCanvasGroup.Show();
            portraitImage.sprite = itemData.arty?.Sprite;
            levelText.text = itemData.arty?.levelRx.Value.ToString();
            nameText.text = itemData.arty?.displayName;
        }

        // 상호작용 콜백
        private void OnClick(Unit _)
        {
            ArtyRosterState.Battery.Add(BatteryPage.selectedSlotIndexRx.Value, arty);
            Presenter.Find<BatteryPageMemberChangePopup>().CloseWithAnimation().Forget();
        }
    }
}