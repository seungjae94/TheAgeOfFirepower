using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public class CharacterBasicInfoPresenter : Presenter
    {
        // View
        Image m_portrait;
        TMP_Text m_nameText;
        TMP_Text m_levelText;
        Slider m_expSlider;
        TMP_Text m_currentLevelExpText;
        TMP_Text m_needExpText;

        // Fields
        CharacterModel m_character;
        CompositeDisposable m_characterDataSubscriptions = new();

        // Methods
        void Awake()
        {
            m_portrait = transform.FindRecursiveByName<Image>("Portrait");
            m_nameText = transform.FindRecursiveByName<TMP_Text>("Name Text");
            m_levelText = transform.FindRecursiveByName<TMP_Text>("Level Text");
            m_expSlider = transform.FindRecursiveByName<Slider>("Exp Slider");
            m_currentLevelExpText = transform.FindRecursiveByName<TMP_Text>("Current Level Exp Text");
            m_needExpText = transform.FindRecursiveByName<TMP_Text>("Need Exp Text");
        }

        void OnDestroy()
        {
            m_characterDataSubscriptions.Dispose();
        }

        public void Initialize()
        {
            m_worldSceneManager.GetPage<PartyPage>()
                .SubscribeSelectedCharacterChangeEvent(OnSelectedCharacterChanged)
                .AddTo(gameObject);
        }

        void OnSelectedCharacterChanged(CharacterModel character)
        {
            m_character = character;

            // Subscribe selected character basic info change events.
            m_characterDataSubscriptions.Clear();

            character?
                .SubscribeLevelChangeEvent(v => m_levelText.text = v.ToString())
                .AddTo(m_characterDataSubscriptions);

            character?
                .SubscribeTotalExpChangeEvent(OnTotalExpChange)
                .AddTo(m_characterDataSubscriptions);

            UpdateView();
        }

        void OnTotalExpChange(long totalExp)
        {
            m_expSlider.value = (float) m_character.currentLevelExp / m_character.needExp;
            m_currentLevelExpText.text = m_character.currentLevelExp.ToString();
            m_needExpText.text = m_character.needExp.ToString();
        }

        new void UpdateView()
        {
            if (m_character == null)
            {
                m_portrait.sprite = null;
                m_nameText.text = "캐릭터 지정 X";
                return;
            }

            m_portrait.sprite = m_character.portrait;
            m_nameText.text = m_character.displayName;
        }
    }
}
