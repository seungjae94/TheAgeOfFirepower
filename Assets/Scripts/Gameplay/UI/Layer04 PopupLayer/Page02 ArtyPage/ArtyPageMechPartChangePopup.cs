using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using UnityEngine.Serialization;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class ArtyPageMechPartChangePopup : PopupPresenter
    {
        private const float k_openDuration = 0.25f;

        // Alias
        ArtyPage ArtyPage => Find<ArtyPage>();
        InventoryState InventoryState => GameState.Inst.inventoryState;

        // View
        [SerializeField]
        private TextMeshProUGUI titleText;
        
        [SerializeField]
        private Button okButton;
        
        [SerializeField]
        private Button cancelButton;

        [SerializeField]
        private MechPartBasicView currentMechPartView;
        
        [SerializeField]
        private MechPartBasicView selectedMechPartView;
        
        [SerializeField]
        private RectTransform windowTransform;
        
        // Field
        private Tween openTween;
        private Tween closeTween;
        private EMechPartType slotType = EMechPartType.Barrel;

        private readonly CompositeDisposable disposables = new ();
        
        MechPartModel selectedMechPart = null;

        // 이벤트 함수
        public void Setup(EMechPartType pSlotType)
        {
            slotType = pSlotType;
        }
        
        public override void Initialize()
        {
            base.Initialize();

            openTween = windowTransform.DOScale(new Vector3(1f, 1f, 1f), k_openDuration)
                .From(new Vector3(0f, 0f, 1f))
                .SetAutoKill(false)
                .Pause();
            
            closeTween = windowTransform.DOScale(new Vector3(0f, 0f, 1f), k_openDuration)
                .From(new Vector3(1f, 1f, 1f))
                .SetAutoKill(false)
                .Pause();
        }

        void OnDestroy()
        {
            openTween.Kill();
            closeTween.Kill();
            
            disposables.Dispose();
        }
        
        public override async UniTask OpenWithAnimation()
        {
            base.OpenWithAnimation();

            // 이벤트 구독
            okButton.OnClickAsObservable()
                .Subscribe(OnClickOKButton)
                .AddTo(disposables);

            cancelButton.OnClickAsObservable()
                .Subscribe(OnClickCancelButton)
                .AddTo(disposables);
            
            // 뷰 초기화
            InitializeView();
            
            // 애니메이션
            openTween.Restart();
            await openTween.AwaitForComplete();
        }

        public override async UniTask CloseWithAnimation()
        {
            disposables.Clear();
            
            closeTween.Restart();
            await closeTween.AwaitForComplete();
            
            base.CloseWithAnimation();
        }

        // 유저 이벤트 처리
        private void OnClickOKButton(Unit _)
        {
            // BatteryPage.SelectedArty
            //     .Equip(m_slotType, selectedMechPart);

            CloseWithAnimation().Forget();
        }

        private void OnClickCancelButton(Unit _)
        {
            CloseWithAnimation().Forget();
        }

        // void OnClickUnequipButton(Unit _)
        // {
        //     selectedMechPart = null;
        //
        //     UpdateSelectedEquipmentView();
        //     UpdateFlex();
        // }

        // 뷰 업데이트
        private void InitializeView()
        {
            titleText.text = $"{slotType.ToDisplayName()} 교체";

            MechPartModel currentMechPart = ArtyPage.SelectedArty.mechPartSlotsRx[slotType];
            currentMechPartView.Setup(currentMechPart, "부품을 장착하지 않았습니다.");
            currentMechPartView.Draw();
            
            selectedMechPartView.Setup(null);
            selectedMechPartView.Draw();
        }

        void UpdateSelectedEquipmentView()
        {
            // string equipTypeString = EquipmentTypeToString(slotType);
            //
            // if (selectedMechPart == null)
            // {
            //     selectedMechPartIcon.enabled = false;
            //     selectedMechPartIcon.sprite = null;
            //     selectedMechPartNameText.text = "";
            //     selectedMechPartDescriptionText.text = $"<style=\"WarningPrimaryColor\">{equipTypeString}�� �������� �ʽ��ϴ�.</style>";
            // }
            // else
            // {
            //     selectedMechPartIcon.enabled = true;
            //     selectedMechPartIcon.sprite = selectedMechPart.Icon;
            //     selectedMechPartNameText.text = selectedMechPart.DisplayName;
            //
            //     if (selectedMechPart.Owner != null)
            //         selectedMechPartDescriptionText.text = $"<style=\"NoticePrimaryColor\">{selectedMechPart.Owner.Value.DisplayName} ���� ��</style>\n";
            //     else
            //         selectedMechPartDescriptionText.text = "";
            //
            //     selectedMechPartDescriptionText.text += selectedMechPart.Description;
            // }
        }

        void UpdateFlex()
        {
            // var itemDatas = inventoryState
            //         .GetSortedEquipmentList(m_slotType)
            //         .Select(item => new InventoryFlexItemData(item, item == m_selectedEquipment))
            //         .ToList();
            //
            // m_flex.Draw(itemDatas, OnClickFlexItem);
        }
        
        // public UniTask Show(EMechPartType slotType)
        // {
        //     this.slotType = slotType;
        //
        //     MechPartModel currentMechPart = BatteryPage.SelectedArty.GetMechPartAtSlot(slotType);
        //     selectedMechPart = currentMechPart;
        //
        //     UpdateCurrentEquipmentView(currentMechPart);
        //     UpdateSelectedEquipmentView();
        //     UpdateFlex();
        //     
        // }

        // void OnClickFlexItem(MechPartModel mechPart)
        // {
        //     selectedMechPart = mechPart;
        //     UpdateSelectedEquipmentView();
        //     UpdateFlex();
        // }
    }
}
