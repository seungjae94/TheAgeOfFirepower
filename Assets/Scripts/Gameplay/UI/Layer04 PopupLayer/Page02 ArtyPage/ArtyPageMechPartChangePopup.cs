using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using Mathlife.ProjectL.Gameplay.UI.ArtyPagePopup;
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

        [SerializeField]
        private ArtyPageMechPartChangeScrollView scrollView;
        
        // Field
        private Tween openTween;
        private Tween closeTween;
        private EMechPartType slotType = EMechPartType.Barrel;

        private readonly CompositeDisposable disposables = new ();

        public readonly ReactiveProperty<int> selectedIndexRx = new(0);
        private List<MechPartModel> mechPartList;

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
            // 블러 적용
            await Find<BlurPopup>().OpenWithAnimation();
            
            base.OpenWithAnimation();
            
            // 뷰 초기화
            selectedIndexRx.Value = 0;
            InitializeView();
            
            // 이벤트 구독
            okButton.OnClickAsObservable()
                .Subscribe(OnClickOKButton)
                .AddTo(disposables);

            cancelButton.OnClickAsObservable()
                .Subscribe(OnClickCancelButton)
                .AddTo(disposables);
            
            selectedIndexRx
                .Subscribe(OnSelectedIndexChanged)
                .AddTo(disposables);
            
            // 애니메이션
            openTween.Restart();
            await openTween.AwaitForComplete();
        }

        public override async UniTask CloseWithAnimation()
        {
            disposables.Clear();
            
            closeTween.Restart();
            await closeTween.AwaitForComplete();
            
            await Find<BlurPopup>().CloseWithAnimation();
            base.CloseWithAnimation();
        }

        // 이벤트 구독 콜백
        private void OnClickOKButton(Unit _)
        {
            // BatteryPage.SelectedArty
            //     .Equip(m_slotType, selectedMechPart);

            MechPartModel mechPart = mechPartList[selectedIndexRx.Value];
            ArtyPage.SelectedArty.Equip(slotType, mechPart);
            
            CloseWithAnimation().Forget();
        }

        private void OnClickCancelButton(Unit _)
        {
            CloseWithAnimation().Forget();
        }

        private void OnSelectedIndexChanged(int selectedIndex)
        {
            int targetIndex = selectedIndex < 0 ? 0 : selectedIndex;

            if (targetIndex >= mechPartList.Count)
                return;
            
            selectedMechPartView.Setup(mechPartList[targetIndex]);
            selectedMechPartView.Draw();
        }

        // 뷰 업데이트
        private void InitializeView()
        {
            // Fetch Data
            MechPartModel currentMechPart = ArtyPage.SelectedArty.mechPartSlotsRx[slotType];
            bool ExcludeFilter(MechPartModel mechPart) => mechPart == currentMechPart;

            mechPartList = InventoryState
                .GetSortedMechPartList(slotType, ExcludeFilter);

            if (currentMechPart != null)
                mechPartList.Insert(0, currentMechPart);
            mechPartList.Insert(0, null);
            
            var items = mechPartList
                .Select(mechPart => new ItemData() { mechPart = mechPart })
                .ToList();
            
            selectedIndexRx.Value = (currentMechPart != null) ? 1 : 0;
            
            // 일반 뷰 초기화
            titleText.text = $"{slotType.ToDisplayName()} 교체";

            currentMechPartView.Setup(currentMechPart, "부품을 장착하지 않았습니다.");
            currentMechPartView.Draw();
            
            // 스크롤 뷰 초기화
            scrollView.UpdateContents(items);
            scrollView.SelectCell(selectedIndexRx.Value);
        }
        
    }
}
