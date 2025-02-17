using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Mathlife.ProjectL.Gameplay
{
    public class CharacterStatPresenter : Presenter
    {
        enum ArtifactSlot
        {
            Destruction,
            Protection,
            Strategy,
            Contraction
        }

        // View
        TMP_Text m_maxHpText;
        TMP_Text m_maxEnergyText;
        TMP_Text m_energyRecoveryText;
        TMP_Text m_atkText;
        TMP_Text m_defText;
        TMP_Text m_magText;
        TMP_Text m_spdText;

        // Fields
        CharacterModel m_character;
        CompositeDisposable m_characterDataSubscriptions = new();

        // Methods
        void Awake()
        {
            m_maxHpText = transform.FindRecursiveByName<TMP_Text>("Max HP Text");
            m_maxEnergyText = transform.FindRecursiveByName<TMP_Text>("Max Energy Text");
            m_energyRecoveryText = transform.FindRecursiveByName<TMP_Text>("Energy Recovery Text");
            m_spdText = transform.FindRecursiveByName<TMP_Text>("Spd Text");
            m_atkText = transform.FindRecursiveByName<TMP_Text>("Atk Text");
            m_defText = transform.FindRecursiveByName<TMP_Text>("Def Text");
            m_magText = transform.FindRecursiveByName<TMP_Text>("Mag Text");
        }

        void OnDestroy()
        {
            m_characterDataSubscriptions.Dispose();
        }

        public void Initialize()
        {
            m_worldSceneManager.GetPage<TeamPage>()
                .SubscribeSelectedCharacterChangeEvent(OnSelectedCharacterChanged)
                .AddTo(gameObject);
        }

        void OnSelectedCharacterChanged(CharacterModel character)
        {
            m_character = character;

            // Subscribe selected character stat change events.
            m_characterDataSubscriptions.Clear();

            character?
                .ObserveEveryValueChanged(character => character.GetMaxHp())
                .Subscribe(v => m_maxHpText.text = v.ToString())
                .AddTo(m_characterDataSubscriptions);

            character?
                .ObserveEveryValueChanged(character => character.GetMaxEnergy())
                .Subscribe(v => m_maxEnergyText.text = v.ToString())
                .AddTo(m_characterDataSubscriptions);


            character?
                .ObserveEveryValueChanged(character => character.GetEnergyRecovery())
                .Subscribe(v => m_energyRecoveryText.text = v.ToString())
                .AddTo(m_characterDataSubscriptions);

            character?
                .ObserveEveryValueChanged(character => character.GetSpd())
                .Subscribe(v => m_spdText.text = v.ToString())
                .AddTo(m_characterDataSubscriptions);

            character?
                .ObserveEveryValueChanged(character => character.GetAtk())
                .Subscribe(v => m_atkText.text = v.ToString())
                .AddTo(m_characterDataSubscriptions);

            character?
                .ObserveEveryValueChanged(character => character.GetDef())
                .Subscribe(v => m_defText.text = v.ToString())
                .AddTo(m_characterDataSubscriptions);

            character?
                .ObserveEveryValueChanged(character => character.GetMag())
                .Subscribe(v => m_magText.text = v.ToString())
                .AddTo(m_characterDataSubscriptions);

            UpdateView();
        }

        new void UpdateView()
        {
            if (m_character == null)
                return;

            m_maxHpText.text = m_character.GetMaxHp().ToString();
            m_maxEnergyText.text = m_character.GetMaxEnergy().ToString();
            m_energyRecoveryText.text = m_character.GetEnergyRecovery().ToString();
            m_atkText.text = m_character.GetAtk().ToString();
            m_defText.text = m_character.GetDef().ToString();
            m_magText.text = m_character.GetMag().ToString();
            m_spdText.text = m_character.GetSpd().ToString();
        }
    }
}
