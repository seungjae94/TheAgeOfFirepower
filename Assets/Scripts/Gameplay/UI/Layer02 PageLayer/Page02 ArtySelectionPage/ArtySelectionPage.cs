using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class ArtySelectionPage : Page
    {
        public override string PageName => "화포 선택";
        
        ArtyRosterState artyRosterState;

        [SerializeField] Button m_navBackButton;
        //[SerializeField] CharacterSelectionFlex m_flex;

        public readonly ReactiveProperty<ArtyModel> selectedCharacterRx = new();

        public override void Activate()
        {
            // m_flex.Initialize();
            //
            // m_sortedCharacterListChangeSubs = characterRosterState
            //     .SubscribeSortedCharacterList(UpdateView);
 
            // m_navBackButton.OnClickAsObservable()
            //     .Subscribe(_ => lobbySceneGameMode.NavigateBack())
            //     .AddTo(gameObject);
        }

        // 유저 상호작용
        void OnClickFlexItem(ArtyModel arty)
        {
            selectedCharacterRx.Value = arty;
            //lobbySceneGameMode.Navigate(EPageId.CharacterDetailPage);
        }

        // 뷰 업데이트
        void UpdateView()
        {
            UpdateFlex();
        }

        void UpdateFlex()
        {
            // var itemDatas = characterRosterState
            //     .GetSortedList()
            //     .ToList();
            //
            // m_flex.Draw(itemDatas, OnClickFlexItem);
        }
    }
}
