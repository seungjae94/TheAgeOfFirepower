using Mathlife.ProjectL.Utils;
using System;
using Mathlife.ProjectL.Gameplay.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    internal class BatteryPageSelectedArtyView : MonoBehaviour, IView
    {
        // Alias
        ArtyRosterState ArtyRosterState => GameState.Inst.artyRosterState;
        private BatteryPage BatteryPage => Presenter.Find<BatteryPage>();

        // View
        [SerializeField]
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
        
        // IView
        public void Initialize()
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

        public void Clear()
        {
            disposables.Clear();
        }
        
        // 이벤트 함수
        private void OnDestroy()
        {
            disposables.Dispose();
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
            Presenter.Find<BattlePageArtySlotChangePopup>().Open();
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