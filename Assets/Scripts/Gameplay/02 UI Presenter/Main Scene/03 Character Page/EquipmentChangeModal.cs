using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class EquipmentChangeModal : Presenter
    {
        public const float k_fadeTime = 0.25f;

        [SerializeField] PartyPage m_partyPage;
        //[Inject] MainSceneManager m_mainSceneManager;
        [Inject] InventoryRepository m_inventoryRepository;

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
        [SerializeField] InventorySlotFlex m_slotGridView;
        [SerializeField] Button m_unequipButton;

        EEquipmentType m_slotType = EEquipmentType.Weapon;

        EquipmentModel m_selectedEquipment = null;

        // 초기화
        protected override void SubscribeDataChange()
        {
        }

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

        // 유저 상호작용
        async void OnClickOKButton(Unit _)
        {
            m_partyPage.selectedCharacter.GetState()
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
            //UpdateGridView();
        }

        public async UniTask Show(EEquipmentType slotType)
        {
            m_slotType = slotType;

            EquipmentModel currentEquipment = m_partyPage.selectedCharacter.GetState().GetEquipment(slotType);
            m_selectedEquipment = currentEquipment;

            UpdateCurrentEquipmentView(currentEquipment);
            UpdateSelectedEquipmentView();
            //UpdateGridView();

            await m_canvasGroup.Show(k_fadeTime);
        }

        public async UniTask Hide()
        {
            await m_canvasGroup.Hide(k_fadeTime);
        }

        void OnClickSlot(EquipmentModel equipment)
        {
            m_selectedEquipment = equipment;
            UpdateSelectedEquipmentView();
            //UpdateGridView();
        }

        // 뷰 업데이트
        public void UpdateCurrentEquipmentView(EquipmentModel currentEquipment)
        {
            string equipTypeString = EquipmentTypeToString(m_slotType);
            m_title.text = $"{equipTypeString} 교체";

            if (currentEquipment == null)
            {
                m_currentEquipmentIcon.enabled = false;
                m_currentEquipmentIcon.sprite = null;
                m_currentEquipmentName.text = "";
                m_currentEquipmentDescription.text = $"<style=\"WarningPrimaryColor\">{equipTypeString}를 장착하고 있지 않습니다.</style>";
            }
            else
            {
                m_currentEquipmentIcon.enabled = true;
                m_currentEquipmentIcon.sprite = currentEquipment.icon;
                m_currentEquipmentName.text = currentEquipment.displayName;
                m_currentEquipmentDescription.text = $"<style=\"NoticePrimaryColor\">{currentEquipment.owner.displayName} 장착 중</style>\n";
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
                m_selectedEquipmentDescription.text = $"<style=\"WarningPrimaryColor\">{equipTypeString}를 장착하지 않습니다.</style>";
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

        //void UpdateGridView()
        //{
        //    m_slotGridView.Render(
        //        m_inventoryRepository.GetSortedEquipmentList(m_slotType),
        //        EquipmentSlotViewFactoryMethod);
        //}

        //// 유틸리티
        //EquipmentSlotView EquipmentSlotViewFactoryMethod(Transform parent, EquipmentModel equipment)
        //{
        //    EquipmentSlotView slot = m_gameDataDB.Instantiate<EquipmentSlotView>(EPrefabId.EquipmentSlot, parent);
        //    slot.Render(equipment, equipment == m_selectedEquipment, OnClickSlot);
        //    return slot;
        //}

        string EquipmentTypeToString(EEquipmentType type)
        {
            switch (type)
            {
                case EEquipmentType.Weapon:
                    return "무기";
                case EEquipmentType.Armor:
                    return "방어구";
                case EEquipmentType.Artifact:
                    return "아티팩트";
            }

            return "";
        }
    }
}
