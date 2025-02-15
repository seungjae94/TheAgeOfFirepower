using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Utils;
using System;
using TMPro;
using UniRx;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public class ArtifactChangeModal : Presenter
    {
        [Inject] CharacterRepository m_characterRepository;

        CanvasGroup m_canvasGroup;
        Button m_okButton;
        Button m_cancelButton;

        TMP_Text m_title;

        Image m_currentArtifactIcon;
        TMP_Text m_currentArtifactName;
        TMP_Text m_currentArtifactDescription;

        Image m_selectedArtifactIcon;
        TMP_Text m_selectedArtifactName;
        TMP_Text m_selectedArtifactDescription;

        [SerializeField] ArtifactChangeModalScrollViewPresenter m_scrollViewPresenter;
        [SerializeField] Button m_unequipButton; 

        ReactiveProperty<EEquipmentType> m_slotType = new(EEquipmentType.Weapon);
        public EEquipmentType slotType { get => m_slotType.Value; set => m_slotType.Value = value; }
        public IDisposable SubscribeSlotTypeChangeEvent(Action<EEquipmentType> action)
        {
            return m_slotType.Subscribe(action);
        }

        ReactiveProperty<EquipmentModel> m_selectedArtifact = new(null);
        public EquipmentModel selectedEquipment { get => m_selectedArtifact.Value; set => m_selectedArtifact.Value = value; }
        public IDisposable SubscribeSelectedArtifactChangeEvent(Action<EquipmentModel> action)
        {
            return m_selectedArtifact.Subscribe(action);
        }

        const float k_fadeTime = 0.25f;

        void Awake()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
            m_okButton = transform.FindRecursiveByName<Button>("OK Button");
            m_cancelButton = transform.FindRecursiveByName<Button>("Cancel Button");

            m_title = transform.FindRecursiveByName<TMP_Text>("Title Text");

            m_currentArtifactIcon = transform.FindRecursiveByName<RectTransform>("Current Artifact")
                .FindRecursiveByName<Image>("Icon Image");
            m_currentArtifactName = transform.FindRecursiveByName<RectTransform>("Current Artifact")
                .FindRecursiveByName<TMP_Text>("Artifact Name");
            m_currentArtifactDescription = transform.FindRecursiveByName<RectTransform>("Current Artifact")
                .FindRecursiveByName<TMP_Text>("Artifact Description");

            m_selectedArtifactIcon = transform.FindRecursiveByName<RectTransform>("Selected Artifact")
                .FindRecursiveByName<Image>("Icon Image");
            m_selectedArtifactName = transform.FindRecursiveByName<RectTransform>("Selected Artifact")
                .FindRecursiveByName<TMP_Text>("Artifact Name");
            m_selectedArtifactDescription = transform.FindRecursiveByName<RectTransform>("Selected Artifact")
                .FindRecursiveByName<TMP_Text>("Artifact Description");
        }

        public void Initialize()
        {
            m_okButton.OnClickAsObservable()
                .Subscribe(OnClickOKButton)
                .AddTo(gameObject);

            m_cancelButton.OnClickAsObservable()
                .Subscribe(OnClickCancelButton)
                .AddTo(gameObject);

            m_unequipButton.OnClickAsObservable()
                .Subscribe(_ => selectedEquipment = null)
                .AddTo(gameObject);

            SubscribeSelectedArtifactChangeEvent(OnSelect)
                .AddTo(gameObject);

            InitializeChildren();

            m_canvasGroup.Hide();
        }

        void InitializeChildren()
        {
            m_scrollViewPresenter.Initialize();
        }

        async void OnClickOKButton(Unit _)
        {
            m_worldSceneManager.GetPage<TeamPage>()
                .selectedCharacter.Equip(slotType, selectedEquipment);

            await m_canvasGroup.Hide(k_fadeTime);
        }

        async void OnClickCancelButton(Unit _)
        {
           await m_canvasGroup.Hide(k_fadeTime);
        }

        public async UniTask Show()
        {
            EquipmentModel equipment = m_worldSceneManager.GetPage<TeamPage>().selectedCharacter.GetEquipment(slotType);

            switch (slotType)
            {
                case EEquipmentType.Weapon:
                    m_title.text = "무기 교체";
                    break;
                case EEquipmentType.Armor:
                    m_title.text = "방어구 교체";
                    break;
                case EEquipmentType.Artifact:
                    m_title.text = "아티팩트 교체";
                    break;
            }

            if (equipment == null)
            {
                m_currentArtifactIcon.enabled = false;
                m_currentArtifactIcon.sprite = null;
                m_currentArtifactName.text = "";
                m_currentArtifactDescription.text = "<style=\"WarningPrimaryColor\">아티팩트를 장착하고 있지 않습니다.</style>";
            }
            else
            {
                m_currentArtifactIcon.enabled = true;
                m_currentArtifactIcon.sprite = equipment.icon;
                m_currentArtifactName.text = equipment.displayName;
                m_currentArtifactDescription.text = $"<style=\"NoticePrimaryColor\">{equipment.owner.displayName} 장착 중</style>\n";
                m_currentArtifactDescription.text += equipment.description;
            }

            selectedEquipment = equipment;

            m_scrollViewPresenter.UpdateView();

            await m_canvasGroup.Show(k_fadeTime);
        }

        void OnSelect(EquipmentModel equipment)
        {
            if (equipment == null)
            {
                m_selectedArtifactIcon.enabled = false;
                m_selectedArtifactIcon.sprite = null;
                m_selectedArtifactName.text = "";
                m_selectedArtifactDescription.text = "<style=\"WarningPrimaryColor\">아티팩트를 장착하지 않습니다.</style>";
            }
            else
            {
                m_selectedArtifactIcon.enabled = true;
                m_selectedArtifactIcon.sprite = equipment.icon;
                m_selectedArtifactName.text = equipment.displayName;

                if (equipment.owner != null)
                    m_selectedArtifactDescription.text = $"<style=\"NoticePrimaryColor\">{equipment.owner.displayName} 장착 중</style>\n";
                else
                    m_selectedArtifactDescription.text = "";

                m_selectedArtifactDescription.text += equipment.description;
            }
        }

        public bool IsSelected(EquipmentModel equipment)
        {
            return selectedEquipment == equipment;
        }
    }
}
