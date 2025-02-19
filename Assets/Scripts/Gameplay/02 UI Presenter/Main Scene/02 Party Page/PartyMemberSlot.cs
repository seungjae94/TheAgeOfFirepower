using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using Mathlife.ProjectL.Utils;

namespace Mathlife.ProjectL.Gameplay
{
    public class PartyMemberSlot : Presenter
    {
        [Inject] PartyPage m_partyPage;
        [Inject] CharacterRepository m_characterRepository;

        ObservableDropTrigger m_dropTrigger;
        ObservablePointerClickTrigger m_clickTrigger;

        [SerializeField] int m_index = 0;
        [SerializeField] PartyMemberSlotItem m_slotItem;
        [SerializeField] CanvasGroup m_addMemberGuideCanvasGroup;

        void Awake()
        {
            m_dropTrigger = GetComponent<ObservableDropTrigger>();
            m_clickTrigger = GetComponent<ObservablePointerClickTrigger>();
        }

        protected override void SubscribeDataChange()
        {
            m_characterRepository.party
                .SubscribeMemberChange(OnSlotMemberChange)
                .AddTo(gameObject);
        }

        protected override void SubscribeUserInteractions()
        {
            m_dropTrigger
                .OnDropAsObservable()
                .Subscribe(OnDrop)
                .AddTo(gameObject);

            m_clickTrigger
                .OnPointerClickAsObservable()
                .Subscribe(_ => Debug.Log("Click!"))
                .AddTo(gameObject);
        }

        protected override void InitializeView()
        {
            CharacterModel character = m_characterRepository.party[m_index];

            if (character != null)
            {
                m_addMemberGuideCanvasGroup.Hide();
            }
            else
            {
                m_addMemberGuideCanvasGroup.Show();
            }
        }

        protected override void InitializeChildren()
        {
            m_slotItem.Initialize(m_index);
        }

        // 데이터 변경 구독 메서드
        void OnSlotMemberChange(CharacterModel character)
        {
            if (character != null)
            {
                m_addMemberGuideCanvasGroup.Hide();
            }
            else
            {
                m_addMemberGuideCanvasGroup.Show();
            }
        }

        // 유저 상호작용
        void OnDrop(PointerEventData eventData)
        {
            var newCharacter = eventData.pointerDrag?
                .GetComponent<PartyMemberSlotItem>()?
                .GetCharacterModel();
            
            // 파티 멤버 슬롯 아이템을 드래그하는 경우만 처리
            if (null == newCharacter)
                return;

            var oldCharacter = m_characterRepository.party[m_index];

            // i번 슬롯에서 i번 슬롯으로 드래그 한 경우 무시
            if (oldCharacter == newCharacter)
                return;

            // 멤버 스왑
            if (m_characterRepository.party.Contains(newCharacter))
            {
                int otherIndex = m_characterRepository.party.IndexOf(newCharacter);
                m_characterRepository.party.Swap(m_index, otherIndex);
            }

            // 선택된 슬롯 초기화
            m_partyPage.selectedCharacter.SetState(null);
        }
    }
}
