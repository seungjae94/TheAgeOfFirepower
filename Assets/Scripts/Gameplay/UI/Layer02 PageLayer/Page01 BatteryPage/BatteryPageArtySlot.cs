using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using Mathlife.ProjectL.Utils;
using UnityEngine.Serialization;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class BatteryPageArtySlot : MonoBehaviour, IView
    {
        BatteryPage batteryPage;
        ArtyRosterState artyRosterState;

        ObservableDropTrigger m_dropTrigger;
        ObservablePointerClickTrigger m_clickTrigger;
        int m_slotIndex = 0;

        [FormerlySerializedAs("mSlotItemView")]
        [FormerlySerializedAs("m_slotItem")]
        [SerializeField] BatteryPageArtySlotItem mSlotItem;
        [SerializeField] CanvasGroup m_addMemberGuideCanvasGroup;

        // 이벤트 함수
        public void Initialize()
        {
            m_slotIndex = transform.GetSiblingIndex();
            m_dropTrigger = GetComponent<ObservableDropTrigger>();
            m_clickTrigger = GetComponent<ObservablePointerClickTrigger>();
        }
        
        public void Draw()
        {
            // 뷰 초기화
            ArtyModel arty = artyRosterState.Battery[m_slotIndex];

            if (arty != null)
                m_addMemberGuideCanvasGroup.Hide();
            else
                m_addMemberGuideCanvasGroup.Show();
            
            mSlotItem.Setup(m_slotIndex);
            
            // 모델 구독
            artyRosterState.Battery
                .ObserveEveryValueChanged(party => party[m_slotIndex])
                .Subscribe(OnSlotMemberChange)
                .AddTo(gameObject);
            
            // 이벤트 구독
            m_dropTrigger
                .OnDropAsObservable()
                .Subscribe(OnDrop)
                .AddTo(gameObject);

            m_clickTrigger
                .OnPointerClickAsObservable()
                .Subscribe(OnClickSlot)
                .AddTo(gameObject);
        }

        public void Clear()
        {

        }
        
        // 모델 구독 콜백
        void OnSlotMemberChange(ArtyModel arty)
        {
            if (arty != null)
            {
                m_addMemberGuideCanvasGroup.Hide();
            }
            else
            {
                m_addMemberGuideCanvasGroup.Show();
            }
        }

        // 이벤트 구독 콜백
        void OnDrop(PointerEventData eventData)
        {
            var newCharacter = eventData.pointerDrag?
                .GetComponent<BatteryPageArtySlotItem>()?
                .GetCharacterModel();
            
            // 파티 멤버 슬롯 아이템을 드래그하는 경우만 처리
            if (null == newCharacter)
                return;

            var oldCharacter = artyRosterState.Battery[m_slotIndex];

            // i번 슬롯에서 i번 슬롯으로 드래그 한 경우 무시
            if (oldCharacter == newCharacter)
                return;

            // 멤버 스왑
            if (artyRosterState.Battery.Contains(newCharacter))
            {
                int otherIndex = artyRosterState.Battery.IndexOf(newCharacter);
                artyRosterState.Battery.Swap(m_slotIndex, otherIndex);
            }

            // 선택된 슬롯 초기화
            batteryPage.selectedSlotIndexRx.Value = -1;
        }

        async void OnClickSlot(PointerEventData ev)
        {
            batteryPage.selectedSlotIndexRx.Value = m_slotIndex;
            //await batteryPage.partyMemberChangeModal.Show();
        }
    }
}
