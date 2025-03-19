using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class InventoryPage : Page
    {
        public override string PageName => "인벤토리";
        
        LobbySceneGameMode lobbySceneGameMode;
        InventoryState inventoryState;

        [SerializeField] Button m_navBackButton;
        [SerializeField] CurrencyBar m_ingameCurrencyBar;
        [SerializeField] Transform m_tabMenuBar;
        //List<TabMenu> m_tabMenus = new();
        int m_selectedTab = 0;
        //[SerializeField] InventoryFlex m_flex;
        [SerializeField] Image m_selectedEquipmentIcon;
        [SerializeField] TMP_Text m_selectedEquipmentName;
        [SerializeField] TMP_Text m_selectedEquipmentDescription;
        MechPartModel selectedMechPart = null;
        
        protected override void OnOpen()
        {
            // OnSelectTabMenu(0);
            // OnClickFlexItem(null);
            //
            // m_tabMenus[0].Initialize(0, OnSelectTabMenu);
            // m_tabMenus[0].Select();
            //
            // for (int i = 1; i < m_tabMenus.Count; ++i)
            // {
            //     m_tabMenus[i].Initialize(i, OnSelectTabMenu);
            //     m_tabMenus[i].Default();
            // }

            //m_ingameCurrencyBar.Initialize();
        
            // m_navBackButton.OnClickAsObservable()
            //     .Subscribe(_ => lobbySceneGameMode.NavigateBack())
            //     .AddTo(gameObject);
        }

        protected override void OnClose()
        {
            throw new System.NotImplementedException();
        }
        
        // 이벤트 구독 콜백
        void OnSelectTabMenu(int index)
        {
            // m_tabMenus[m_selectedTab].Default();
            // m_tabMenus[index].Select();
            //
            // m_selectedTab = index;
            // m_selectedEquipment = null;
            //
            // UpdateFlex();
            // UpdateSelectedEquipmentView();
        }

        void OnClickFlexItem(MechPartModel mechPart)
        {
            selectedMechPart = mechPart;

            UpdateSelectedEquipmentView();
            UpdateFlex();
        }

        // �� ������Ʈ
        void UpdateFlex()
        {
            // var itemDatas = inventoryState
            //         .GetSortedEquipmentList((EEquipmentType)m_selectedTab)
            //         .Select(item => new InventoryFlexItemData(item, item == m_selectedEquipment))
            //         .ToList();
            //
            // m_flex.Draw(itemDatas, OnClickFlexItem);
        }

        void UpdateSelectedEquipmentView()
        {
            if (selectedMechPart == null)
            {
                m_selectedEquipmentIcon.enabled = false;
                m_selectedEquipmentIcon.sprite = null;

                m_selectedEquipmentName.text = "";

                m_selectedEquipmentDescription.text = $"<style=\"WarningPrimaryColor\">장비 선택 X.</style>";
            }
            else
            {
                m_selectedEquipmentIcon.enabled = true;
                m_selectedEquipmentIcon.sprite = selectedMechPart.Icon;

                m_selectedEquipmentName.text = selectedMechPart.DisplayName;

                if (selectedMechPart.Owner != null)
                    m_selectedEquipmentDescription.text = $"<style=\"NoticePrimaryColor\">{selectedMechPart.Owner.Value.DisplayName} 장착 중</style>\n";
                else
                    m_selectedEquipmentDescription.text = "";
                m_selectedEquipmentDescription.text += selectedMechPart.Description;
            }
        }
    }
}