using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class EquipmentChangeModal : PopupPresenter
    {
        public const float k_fadeTime = 0.25f;

        // Alias
        BatteryPage BatteryPage => Find<BatteryPage>();
        InventoryState InventoryState => GameState.Inst.InventoryState;

        // View
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
        //[SerializeField] InventoryFlex m_flex;
        [SerializeField] Button m_unequipButton;

        EMechPartType m_slotType = EMechPartType.Barrel;

        MechPartModel selectedMechPart = null;

        // �ʱ�ȭ

        protected void SubscribeUserInteractions()
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

        protected void InitializeView()
        {
            m_canvasGroup.Hide();
        }

        // ���� ��ȣ�ۿ�
        async void OnClickOKButton(Unit _)
        {
            BatteryPage.SelectedArty
                .Equip(m_slotType, selectedMechPart);

            await m_canvasGroup.Hide(k_fadeTime);
        }

        async void OnClickCancelButton(Unit _)
        {
            await m_canvasGroup.Hide(k_fadeTime);
        }

        void OnClickUnequipButton(Unit _)
        {
            selectedMechPart = null;

            UpdateSelectedEquipmentView();
            UpdateFlex();
        }

        public async UniTask Show(EMechPartType slotType)
        {
            m_slotType = slotType;

            MechPartModel currentMechPart = BatteryPage.SelectedArty.GetEquipment(slotType);
            selectedMechPart = currentMechPart;

            UpdateCurrentEquipmentView(currentMechPart);
            UpdateSelectedEquipmentView();
            UpdateFlex();

            await m_canvasGroup.Show(k_fadeTime);
        }

        public async UniTask Hide()
        {
            await m_canvasGroup.Hide(k_fadeTime);
        }

        void OnClickFlexItem(MechPartModel mechPart)
        {
            selectedMechPart = mechPart;
            UpdateSelectedEquipmentView();
            UpdateFlex();
        }

        // �� ������Ʈ
        public void UpdateCurrentEquipmentView(MechPartModel currentMechPart)
        {
            string equipTypeString = EquipmentTypeToString(m_slotType);
            m_title.text = $"{equipTypeString} ��ü";

            if (currentMechPart == null)
            {
                m_currentEquipmentIcon.enabled = false;
                m_currentEquipmentIcon.sprite = null;
                m_currentEquipmentName.text = "";
                m_currentEquipmentDescription.text = $"<style=\"WarningPrimaryColor\">{equipTypeString}�� �����ϰ� ���� �ʽ��ϴ�.</style>";
            }
            else
            {
                m_currentEquipmentIcon.enabled = true;
                m_currentEquipmentIcon.sprite = currentMechPart.Icon;
                m_currentEquipmentName.text = currentMechPart.DisplayName;
                m_currentEquipmentDescription.text = $"<style=\"NoticePrimaryColor\">{currentMechPart.Owner.Value.DisplayName} ���� ��</style>\n";
                m_currentEquipmentDescription.text += currentMechPart.Description;
            }
        }

        void UpdateSelectedEquipmentView()
        {
            string equipTypeString = EquipmentTypeToString(m_slotType);

            if (selectedMechPart == null)
            {
                m_selectedEquipmentIcon.enabled = false;
                m_selectedEquipmentIcon.sprite = null;
                m_selectedEquipmentName.text = "";
                m_selectedEquipmentDescription.text = $"<style=\"WarningPrimaryColor\">{equipTypeString}�� �������� �ʽ��ϴ�.</style>";
            }
            else
            {
                m_selectedEquipmentIcon.enabled = true;
                m_selectedEquipmentIcon.sprite = selectedMechPart.Icon;
                m_selectedEquipmentName.text = selectedMechPart.DisplayName;

                if (selectedMechPart.Owner != null)
                    m_selectedEquipmentDescription.text = $"<style=\"NoticePrimaryColor\">{selectedMechPart.Owner.Value.DisplayName} ���� ��</style>\n";
                else
                    m_selectedEquipmentDescription.text = "";

                m_selectedEquipmentDescription.text += selectedMechPart.Description;
            }
        }

        void UpdateFlex()
        {
            // var itemDatas = inventoryState
            //         .GetSortedEquipmentList(m_slotType)
            //         .Select(item => new InventoryFlexItemData(item, item == m_selectedEquipment))
            //         .ToList();
            //
            // m_flex.Draw(itemDatas, OnClickFlexItem);
        }

        string EquipmentTypeToString(EMechPartType type)
        {
            switch (type)
            {
                case EMechPartType.Barrel:
                    return "����";
                case EMechPartType.Armor:
                    return "��";
                case EMechPartType.Engine:
                    return "��Ƽ��Ʈ";
            }

            return "";
        }
    }
}
