using UnityEngine;
using VContainer;
using UniRx;
using Mathlife.ProjectL.Utils;
using UniRx.Triggers;
using UnityEngine.EventSystems;
using System;
using Unity.Android.Gradle.Manifest;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace Mathlife.ProjectL.Gameplay
{
    class CharacterSelectionFlex
        : AbstractFlex<CharacterModel, CharacterSelectionFlexItem>
    {
        [Inject] CharacterRepository m_characterRepository;

        SortedCharacterListSubscription m_subscription;

        void OnDestroy()
        {
            m_subscription?.Dispose();
        }

        protected override void SubscribeDataChange()
        {
            m_subscription = m_characterRepository
               .SubscribeSortedCharacterList(UpdateView);

            m_characterRepository
                .party.SubscribeMemberChange(_ => UpdateView())
                .AddTo(gameObject);
        }

        protected override void InitializeView()
        {
            UpdateView();
        }

        public void UpdateView()
        {
            Draw(m_characterRepository.GetSortedList()
                .Where(character => m_characterRepository.party.Contains(character) == false)
                .ToList());
        }
    }
}