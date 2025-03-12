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

        [Inject] LobbySceneGameMode lobbySceneGameMode;
        [Inject] InventoryState inventoryState;

        // �ڷ� ���� ���
        [SerializeField] Button m_navBackButton;

        // �ΰ��� ��ȭ ǥ�� ���
        [SerializeField] IngameCurrencyBar m_ingameCurrencyBar;

        // �� ���� ���
        [SerializeField] Transform m_tabMenuBar;
        List<TabMenu> m_tabMenus = new();
        int m_selectedTab = 0;

        // �׸��� ǥ�� ���
        [SerializeField] InventoryFlex m_flex;

        // ������ ��� ����
        [SerializeField] Image m_selectedEquipmentIcon;
        [SerializeField] TMP_Text m_selectedEquipmentName;
        [SerializeField] TMP_Text m_selectedEquipmentDescription;
        EquipmentModel m_selectedEquipment = null;

        // �ʱ�ȭ
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
                .Subscribe(_ => lobbySceneGameMode.NavigateBack())
                .AddTo(gameObject);
        }

        // ���� ��ȣ�ۿ�
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

        // �� ������Ʈ
        void UpdateFlex()
        {
            var itemDatas = inventoryState
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

                m_selectedEquipmentDescription.text = $"<style=\"WarningPrimaryColor\">�������� �������� �ʾҽ��ϴ�.</style>";
            }
            else
            {
                m_selectedEquipmentIcon.enabled = true;
                m_selectedEquipmentIcon.sprite = m_selectedEquipment.icon;

                m_selectedEquipmentName.text = m_selectedEquipment.displayName;

                if (m_selectedEquipment.owner != null)
                    m_selectedEquipmentDescription.text = $"<style=\"NoticePrimaryColor\">{m_selectedEquipment.owner.displayName} ���� ��</style>\n";
                else
                    m_selectedEquipmentDescription.text = "";
                m_selectedEquipmentDescription.text += m_selectedEquipment.description;
            }
        }
    }
}