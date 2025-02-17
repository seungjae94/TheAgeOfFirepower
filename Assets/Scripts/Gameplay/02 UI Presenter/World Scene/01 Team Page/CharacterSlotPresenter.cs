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
        ObservableDropTrigger m_dropTrigger;

        int m_index = 0;

        void Awake()
        {
            m_index = transform.GetSiblingIndex();

            // Event Triggers
            m_dropTrigger = GetComponent<ObservableDropTrigger>();
        }

        public void Initialize()
        {
            // Subscribe Event Triggers
            m_dropTrigger
                .OnDropAsObservable()
                .Subscribe(OnDrop);

            // Subscribe Models
            m_characterRepository.team
                .ObserveEveryValueChanged(team => team[m_index])
                .Subscribe(_ => UpdateView())
                .AddTo(gameObject);

            // Render
            UpdateView();
        }

        new void UpdateView()
        {
            // Destroy old card
            foreach (var oldCard in transform.FindAllRecursive<CharacterCardPresenter>())
            {
                Destroy(oldCard.gameObject);
            }

            CharacterModel member = m_characterRepository.team[m_index];

            if (member != null)
            {
                // Instantiate new card
                CharacterCardPresenter card = InstantiateWithInjection<CharacterCardPresenter>(EPrefabId.CharacterCard, transform);
                card.Initialize(member);
            }
        }

        void OnDrop(PointerEventData eventData)
        {
            var newCharacter = eventData.pointerDrag?
                .GetComponent<CharacterCardPresenter>()?
                .GetCharacterModel();
            
            if (null == newCharacter)
                return;


            var oldCharacter = m_characterRepository.team[m_index];

            if (oldCharacter == newCharacter)
                return;

            // 멤버 <-> 멤버 스왑
            if (m_characterRepository.team.Contains(newCharacter))
            {
                int otherIndex = m_characterRepository.team.IndexOf(newCharacter);
                m_characterRepository.team.Swap(m_index, otherIndex);
            }
            // 보유 캐릭터 <-> 멤버 스왑
            else
                m_characterRepository.team.Add(m_index, newCharacter);
        }
    }
}
