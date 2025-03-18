using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.UI.BatteryPagePopup;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Mathlife.ProjectL.Gameplay.UI.ArtyPageView
{
    public class ArtyPageScrollViewCell
        : FancyCell<ItemData, Context>
    {
        // Alias
        private BatteryPage BatteryPage => Presenter.Find<BatteryPage>();
        private ArtyRosterState ArtyRosterState => GameState.Inst.artyRosterState;

        // View
        private UIEffect uiEffect;
        private Animator animator;

        [SerializeField]
        private Button cellButton;

        [SerializeField]
        private Image portraitImage;

        [SerializeField]
        private TextMeshProUGUI levelText;

        [SerializeField]
        private TextMeshProUGUI nameText;

        // Field
        private ArtyModel arty;
        
        static class AnimatorHash
        {
            public static readonly int Scroll = Animator.StringToHash("ArtyPageScrollViewCell Scroll");
        }

        // Override
        public override void Initialize()
        {
            uiEffect = GetComponent<UIEffect>();
            animator = GetComponent<Animator>();
            
            cellButton.OnClickAsObservable()
                .Subscribe(OnClick)
                .AddTo(gameObject);
        }

        public override void UpdateContent(ItemData itemData)
        {
            arty = itemData.arty;
            portraitImage.sprite = itemData.arty?.Sprite;
            levelText.text = itemData.arty?.levelRx.Value.ToString();
            nameText.text = itemData.arty?.displayName;

            uiEffect.LoadPreset(Context.selectedIndex == Index
                ? "GradientArtyCellSelected"
                : "GradientArtyCellDefault");
        }

        public override void UpdatePosition(float position)
        {
            animator.Play(AnimatorHash.Scroll, -1, position);
            animator.speed = 0;
        }

        // 상호작용 콜백
        private void OnClick(Unit _)
        {
            Context.onCellClickRx.OnNext(Index);

            Debug.Log($"{Index}번 {arty.displayName} 클릭");
            // ArtyRosterState.Battery.Add(BatteryPage.selectedSlotIndexRx.Value, arty);
            // Presenter.Find<BatteryPageMemberChangePopup>().CloseWithAnimation().Forget();
        }
    }
}