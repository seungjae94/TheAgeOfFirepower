using Mathlife.ProjectL.Utils;
using System;
using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class ArtyPageMechPartSlot : AbstractView
    {
        // Alias
        private ArtyPage ArtyPage => Presenter.Find<ArtyPage>();
        private ArtyRosterState ArtyRosterState => GameState.Inst.artyRosterState; 
        
        // View
        [SerializeField]
        private Button button;

        [SerializeField]
        private UIEffect uiEffect; 

        [SerializeField]
        private Image iconImage;

        // Field
        private readonly CompositeDisposable disposables = new();
        private EMechPartType slotType;

        public void Setup(EMechPartType pSlotType)
        {
            slotType = pSlotType;
        }

        public override void Draw()
        {
            base.Draw();
            
            // 이벤트 구독
            button.OnClickAsObservable()
                .Subscribe(OnClick)
                .AddTo(disposables);
            
            // 데이터 구독
            ArtyPage.SelectedArty.mechPartSlotsRx
                .ObserveEveryValueChanged(equipments => equipments[slotType])
                .Subscribe(UpdateView)
                .AddTo(disposables);
            
            // 뷰 초기화
            UpdateView(ArtyPage.SelectedArty.mechPartSlotsRx[slotType]);
        }

        public override void Clear()
        {
            base.Clear();
            
            disposables.Clear();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }

        private void OnClick(Unit _)
        {
            ArtyPageMechPartChangePopup popup = Presenter.Find<ArtyPageMechPartChangePopup>();
            popup.Setup(slotType);
            popup.OpenWithAnimation().Forget();
        }

        private void UpdateView(MechPartModel mechPart)
        {
            if (mechPart == null)
            {
                uiEffect.gameObject.SetActive(false);
                return;
            }
            
            uiEffect.gameObject.SetActive(true);
            uiEffect.LoadPreset(mechPart.Rarity.ToGradientPresetName());
            iconImage.sprite = mechPart.Icon;
        }
    }
}