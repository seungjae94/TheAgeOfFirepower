using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class InventoryPage : Page
    {
        public override EPageId pageId => EPageId.InventoryPage;

        [Inject] InventoryRepository m_inventoryRepository;

        // �ڷ� ���� ���
        [SerializeField] Button m_backButton;

        // �� ���� ���
        [SerializeField] Transform m_tabMenuBar;
        List<TabMenu> m_tabMenus = new();
        int m_selectedTab = 0;

        // �׸��� ǥ�� ���
        [SerializeField] EquipmentSlotGridView m_gridView;

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

        public override void Initialize()
        {
            m_backButton.OnClickAsObservable()
                .Subscribe(OnClickBackButton)
                .AddTo(gameObject);

            InitializeChildren();
            Close();
        }

        protected override void InitializeChildren()
        {
            InitializeTabMenu();
        }

        void InitializeTabMenu()
        {
            m_tabMenus[0].BindSelectAction(0, OnSelectTabMenu);
            m_tabMenus[0].Select();

            for (int i = 1; i < m_tabMenus.Count; ++i)
            {
                m_tabMenus[i].BindSelectAction(i, OnSelectTabMenu);
                m_tabMenus[i].Default();
            }
        }

        // ���� ��ȣ�ۿ�
        public override void Open()
        {
            base.Open();

            UpdateGridView();
            UpdateSelectedEquipmentView();
        }

        void OnClickBackButton(Unit _)
        {
            m_worldSceneManager.NavigateBack();
        }

        void OnSelectTabMenu(int index)
        {
            m_tabMenus[m_selectedTab].Default();
            m_tabMenus[index].Select();
            
            m_selectedTab = index;
            m_selectedEquipment = null;

            UpdateGridView();
            UpdateSelectedEquipmentView();
        }

        void OnClickSlot(EquipmentModel equipment)
        {
            m_selectedEquipment = equipment;

            UpdateSelectedEquipmentView();
            UpdateGridView();
        }

        // �� ������Ʈ
        void UpdateGridView()
        {
            m_gridView.Render(
                m_inventoryRepository.GetSortedEquipmentList((EEquipmentType) m_selectedTab),
                EquipmentSlotViewFactoryMethod);
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

        // ��ƿ��Ƽ
        EquipmentSlotView EquipmentSlotViewFactoryMethod(Transform parent, EquipmentModel equipment)
        {
            EquipmentSlotView slot = m_gameDataDB.Instantiate<EquipmentSlotView>(EPrefabId.EquipmentSlot, parent);
            slot.Render(equipment, equipment == m_selectedEquipment, OnClickSlot);
            return slot;
        }
    }
}