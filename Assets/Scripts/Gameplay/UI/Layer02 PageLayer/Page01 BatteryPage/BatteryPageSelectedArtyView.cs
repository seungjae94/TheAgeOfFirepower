using Mathlife.ProjectL.Utils;
using System;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.UI.BatteryPagePopup;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    internal class BatteryPageSelectedArtyView : AbstractView
    {
        // Alias
        ArtyRosterState ArtyRosterState => GameState.Inst.artyRosterState;
        private BatteryPage BatteryPage => Presenter.Find<BatteryPage>();

        // View
        private CanvasGroup viewCanvasGroup;

        [SerializeField]
        private Image portraitImage;

        [SerializeField]
        private TMP_Text artyLevelText;

        [SerializeField]
        private TMP_Text artyNameText;

        [SerializeField]
        private Button navToArtyPageButton;

        [SerializeField]
        private Button artySlotChangeButton;

        // Field
        private readonly CompositeDisposable disposables = new();
        private IDisposable artyModelSub = null;
        
        // 이벤트 루프
        public override void Initialize()
        {
            base.Initialize();
            
            viewCanvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
        
        public override void Draw()
        {
            BatteryPage.selectedSlotIndexRx
                .Subscribe(OnSelectedSlotIndexChange)
                .AddTo(disposables);

            navToArtyPageButton.OnClickAsObservable()
                .Subscribe(OnClickNavToArtyPageButton)
                .AddTo(disposables);

            artySlotChangeButton.OnClickAsObservable()
                .Subscribe(OnClickArtySlotChangeButton)
                .AddTo(disposables);

            UpdateView();
        }

        public override void Clear()
        {
            disposables.Clear();
        }

        // 모델 구독 콜백
        void OnSelectedSlotIndexChange(int selectedSlotIndex)
        {
            if (artyModelSub != null)
                artyModelSub.Dispose();

            if (BatteryPage.SelectedArty != null)
            {
                artyModelSub = BatteryPage.SelectedArty
                    .levelRx
                    .Subscribe(level => artyLevelText.text = level.ToString());
            }

            UpdateView();
        }

        // 이벤트 구독 콜백
        private void OnClickNavToArtyPageButton(Unit _)
        {
            ArtyPage artyPage = Presenter.Find<ArtyPage>();
            // TODO: artyPage.TargetArty = ...;
            artyPage.Open();
        }

        private void OnClickArtySlotChangeButton(Unit _)
        {
            Presenter.Find<BatteryPageMemberChangePopup>()
                .OpenWithAnimation().Forget();
        }

        // 뷰 업데이트
        void UpdateView()
        {
            if (BatteryPage.SelectedArty == null)
            {
                viewCanvasGroup.Hide();
                return;
            }

            viewCanvasGroup.Show();

            portraitImage.sprite = BatteryPage.SelectedArty.Sprite;
            artyLevelText.text = BatteryPage.SelectedArty.levelRx.Value.ToString();
            artyNameText.text = BatteryPage.SelectedArty.displayName;
        }
    }
}