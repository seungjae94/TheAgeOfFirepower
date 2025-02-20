using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class InventoryPage : Page
    {
        public override EPageId pageId => EPageId.InventoryPage;

        [Inject] MainSceneManager m_mainSceneManager;
        [Inject] InventoryRepository m_inventoryRepository;

        // 뒤로 가기 기능
        [SerializeField] Button m_navBackButton;

        // 인게임 재화 표시 기능
        [SerializeField] IngameCurrencyBar m_ingameCurrencyBar;

        // 탭 선택 기능
        [SerializeField] Transform m_tabMenuBar;
        List<TabMenu> m_tabMenus = new();
        int m_selectedTab = 0;

        // 그리드 표시 기능
        [SerializeField] InventoryFlex m_flex;

        // 선택한 장비 정보
        [SerializeField] Image m_selectedEquipmentIcon;
        [SerializeField] TMP_Text m_selectedEquipmentName;
        [SerializeField] TMP_Text m_selectedEquipmentDescription;
        EquipmentModel m_selectedEquipment = null;

        // 초기화
        protected override void Awake()
        {
            base.Awake();

            foreach (Transform tabMenuTrans in m_tabMenuBar)
            {
                m_tabMenus.Add(tabMenuTrans.GetComponent<TabMenu>());
            }
        }

        protected override void InitializeChildren()
        {
            m_tabMenus[0].Initialize(0, OnSelectTabMenu);
            m_tabMenus[0].Select();

            for (int i = 1; i < m_tabMenus.Count; ++i)
            {
                m_tabMenus[i].Initialize(i, OnSelectTabMenu);
                m_tabMenus[i].Default();
            }

            m_ingameCurrencyBar.Initialize();
        }

        protected override void SubscribeUserInteractions()
        {
            m_navBackButton.OnClickAsObservable()
                .Subscribe(_ => m_mainSceneManager.NavigateBack())
                .AddTo(gameObject);
        }

        // 유저 상호작용
        public override void Open()
        {
            base.Open();

            OnSelectTabMenu(0);
            OnClickFlexItem(null);
        }

        void OnSelectTabMenu(int index)
        {
            m_tabMenus[m_selectedTab].Default();
            m_tabMenus[index].Select();
            
            m_selectedTab = index;
            m_selectedEquipment = null;

            UpdateFlex();
            UpdateSelectedEquipmentView();
        }

        void OnClickFlexItem(EquipmentModel equipment)
        {
            m_selectedEquipment = equipment;

            UpdateSelectedEquipmentView();
            UpdateFlex();
        }

        // 뷰 업데이트
        void UpdateFlex()
        {
            var itemDatas = m_inventoryRepository
                    .GetSortedEquipmentList((EEquipmentType)m_selectedTab)
                    .Select(item => new InventoryFlexItemData(item, item == m_selectedEquipment))
                    .ToList();

            m_flex.Draw(itemDatas, OnClickFlexItem);
        }

        void UpdateSelectedEquipmentView()
        {
            if (m_selectedEquipment == null)
            {
                m_selectedEquipmentIcon.enabled = false;
                m_selectedEquipmentIcon.sprite = null;

                m_selectedEquipmentName.text = "";

                m_selectedEquipmentDescription.text = $"<style=\"WarningPrimaryColor\">아이템을 선택하지 않았습니다.</style>";
            }
            else
            {
                m_selectedEquipmentIcon.enabled = true;
                m_selectedEquipmentIcon.sprite = m_selectedEquipment.icon;

                m_selectedEquipmentName.text = m_selectedEquipment.displayName;

                if (m_selectedEquipment.owner != null)
                    m_selectedEquipmentDescription.text = $"<style=\"NoticePrimaryColor\">{m_selectedEquipment.owner.displayName} 장착 중</style>\n";
                else
                    m_selectedEquipmentDescription.text = "";
                m_selectedEquipmentDescription.text += m_selectedEquipment.description;
            }
        }
    }
}