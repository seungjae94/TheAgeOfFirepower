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
        [Inject] CharacterRepository m_characterRepository;

        ObservableDropTrigger m_dropTrigger;

        [SerializeField] int m_index = 0;
        [SerializeField] PartyMemberSlotItem m_slotItem;

        void Awake()
        {
            m_dropTrigger = GetComponent<ObservableDropTrigger>();
        }

        protected override void SubscribeUserInteractions()
        {
            m_dropTrigger
                .OnDropAsObservable()
                .Subscribe(OnDrop);
        }

        protected override void InitializeChildren()
        {
            m_slotItem.Initialize(m_index);
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
        }
    }
}
