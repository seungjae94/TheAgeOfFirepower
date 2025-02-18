using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using Mathlife.ProjectL.Utils;

namespace Mathlife.ProjectL.Gameplay
{
    public class CharacterSlotPresenter : Presenter
    {
        [Inject] CharacterRepository m_characterRepository;

        [SerializeField] int m_index = 0;
        [SerializeField] ObservableDropTrigger m_dropTrigger;
        [SerializeField] CharacterCardView m_characterCard;

        //public void Initialize()
        //{
        //    // Subscribe Event Triggers
        //    m_dropTrigger
        //        .OnDropAsObservable()
        //        .Subscribe(OnDrop);

        //    // Subscribe Models
        //    m_characterRepository.party
        //        .ObserveEveryValueChanged(party => party[m_index])
        //        .Subscribe(_ => UpdateView())
        //        .AddTo(gameObject);

        //    //m_characterRepository.party
        //    //    .ObserveEveryValueChanged(party => party[m_index].level)
        //    //    .Subscribe(_ => UpdateView())
        //    //    .AddTo(gameObject);

        //    // Render
        //    //UpdateView();
        //}

        new void UpdateView()
        {
            CharacterModel member = m_characterRepository.party[m_index];

            if (member == null)
            {
                m_characterCard.Hide();
            }
            if (member != null)
            {
                m_characterCard.Show();
                m_characterCard.Render(member);
            }
        }

        void OnDrop(PointerEventData eventData)
        {
            var newCharacter = eventData.pointerDrag?
                .GetComponent<CharacterCardView>()?
                .GetCharacterModel();
            
            if (null == newCharacter)
                return;


            var oldCharacter = m_characterRepository.party[m_index];

            if (oldCharacter == newCharacter)
                return;

            // 멤버 <-> 멤버 스왑
            if (m_characterRepository.party.Contains(newCharacter))
            {
                int otherIndex = m_characterRepository.party.IndexOf(newCharacter);
                m_characterRepository.party.Swap(m_index, otherIndex);
            }
            // 보유 캐릭터 <-> 멤버 스왑
            else
                m_characterRepository.party.Add(m_index, newCharacter);
        }

        protected override void SubscribeDataChange()
        {
        }

        protected override void SubscribeUserInteractions()
        {
        }

        protected override void InitializeView()
        {
        }
    }
}
