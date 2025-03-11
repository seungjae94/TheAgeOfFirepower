using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using System.Linq;

namespace Mathlife.ProjectL.Gameplay
{
    public class EquipmentChangeModal : Presenter
    {
        public const float k_fadeTime = 0.25f;

        [Inject] PartyPage m_partyPage;
        [Inject] InventoryState inventoryState;

        [SerializeField] CanvasGroup m_canvasGroup;
        [SerializeField] Button m_okButton;
        [SerializeField] Button m_cancelButton;
        [SerializeField] TMP_Text m_title;
        [SerializeField] Image m_currentEquipmentIcon;
        [SerializeField] TMP_Text m_currentEquipmentName;
        [SerializeField] TMP_Text m_currentEquipmentDescription;
        [SerializeField] Image m_selectedEquipmentIcon;
        [SerializeField] TMP_Text m_selectedEquipmentName;
        [SerializeField] TMP_Text m_selectedEquipmentDescription;
        [SerializeField] InventoryFlex m_flex;
        [SerializeField] Button m_unequipButton;

        EEquipmentType m_slotType = EEquipmentType.Barrel;

        EquipmentModel m_selectedEquipment = null;

        // �ʱ�ȭ

        protected override void SubscribeUserInteractions()
        {
            m_okButton.OnClickAsObservable()
                .Subscribe(OnClickOKButton)
                .AddTo(gameObject);

            m_cancelButton.OnClickAsObservable()
                .Subscribe(OnClickCancelButton)
                .AddTo(gameObject);

            m_unequipButton.OnClickAsObservable()
                .Subscribe(OnClickUnequipButton)
                .AddTo(gameObject);
        }

        protected override void InitializeView()
        {
            m_canvasGroup.Hide();
        }

        // ���� ��ȣ�ۿ�
        async void OnClickOKButton(Unit _)
        {
            m_partyPage.GetSelectedCharacter()
                .Equip(m_slotType, m_selectedEquipment);

            await m_canvasGroup.Hide(k_fadeTime);
        }

        async void OnClickCancelButton(Unit _)
        {
            await m_canvasGroup.Hide(k_fadeTime);
        }

        void OnClickUnequipButton(Unit _)
        {
            m_selectedEquipment = null;

            UpdateSelectedEquipmentView();
            UpdateFlex();
        }

        public async UniTask Show(EEquipmentType slotType)
        {
            m_slotType = slotType;

            EquipmentModel currentEquipment = m_partyPage.GetSelectedCharacter().GetEquipment(slotType);
            m_selectedEquipment = currentEquipment;

            UpdateCurrentEquipmentView(currentEquipment);
            UpdateSelectedEquipmentView();
            UpdateFlex();

            await m_canvasGroup.Show(k_fadeTime);
        }

        public async UniTask Hide()
        {
            await m_canvasGroup.Hide(k_fadeTime);
        }

        void OnClickFlexItem(EquipmentModel equipment)
        {
            m_selectedEquipment = equipment;
            UpdateSelectedEquipmentView();
            UpdateFlex();
        }

        // �� ������Ʈ
        public void UpdateCurrentEquipmentView(EquipmentModel currentEquipment)
        {
            string equipTypeString = EquipmentTypeToString(m_slotType);
            m_title.text = $"{equipTypeString} ��ü";

            if (currentEquipment == null)
            {
                m_currentEquipmentIcon.enabled = false;
                m_currentEquipmentIcon.sprite = null;
                m_currentEquipmentName.text = "";
                m_currentEquipmentDescription.text = $"<style=\"WarningPrimaryColor\">{equipTypeString}�� �����ϰ� ���� �ʽ��ϴ�.</style>";
            }
            else
            {
                m_currentEquipmentIcon.enabled = true;
                m_currentEquipmentIcon.sprite = currentEquipment.icon;
                m_currentEquipmentName.text = currentEquipment.displayName;
                m_currentEquipmentDescription.text = $"<style=\"NoticePrimaryColor\">{currentEquipment.owner.displayName} ���� ��</style>\n";
                m_currentEquipmentDescription.text += currentEquipment.description;
            }
        }

        void UpdateSelectedEquipmentView()
        {
            string equipTypeString = EquipmentTypeToString(m_slotType);

            if (m_selectedEquipment == null)
            {
                m_selectedEquipmentIcon.enabled = false;
                m_selectedEquipmentIcon.sprite = null;
                m_selectedEquipmentName.text = "";
                m_selectedEquipmentDescription.text = $"<style=\"WarningPrimaryColor\">{equipTypeString}�� �������� �ʽ��ϴ�.</style>";
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

        void UpdateFlex()
        {
            var itemDatas = inventoryState
                    .GetSortedEquipmentList(m_slotType)
                    .Select(item => new InventoryFlexItemData(item, item == m_selectedEquipment))
                    .ToList();

            m_flex.Draw(itemDatas, OnClickFlexItem);
        }

        string EquipmentTypeToString(EEquipmentType type)
        {
            switch (type)
            {
                case EEquipmentType.Barrel:
                    return "����";
                case EEquipmentType.Armor:
                    return "��";
                case EEquipmentType.Engine:
                    return "��Ƽ��Ʈ";
            }

            return "";
        }
    }
}
