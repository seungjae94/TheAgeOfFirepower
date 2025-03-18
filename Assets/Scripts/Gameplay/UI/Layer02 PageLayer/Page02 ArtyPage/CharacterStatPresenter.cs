// using TMPro;
// using UniRx;
// using UnityEngine;
//
// namespace Mathlife.ProjectL.Gameplay.UI
// {
//     public class CharacterStatPresenter : Presenter
//     {
//         CharacterDetailPage m_characterDetailPage; 
//
//         [SerializeField] TMP_Text m_maxHpText;
//         [SerializeField] TMP_Text m_atkText;
//         [SerializeField] TMP_Text m_defText;
//         [SerializeField] TMP_Text m_spdText;
//
//         // Fields
//         CharacterModel m_character;
//         CompositeDisposable m_characterDataSubscriptions = new();
//
//         // Methods
//
//         void OnDestroy()
//         {
//             m_characterDataSubscriptions.Dispose();
//         }
//
//         protected override void SubscribeDataChange()
//         {
//             m_characterDetailPage.characterRx
//                 .Subscribe(OnSelectedCharacterChange)
//                 .AddTo(gameObject);
//         }
//
//         protected override void InitializeView()
//         {
//             UpdateView();
//         }
//
//         void OnSelectedCharacterChange(CharacterModel character)
//         {
//             // Subscribe selected character stat change events.
//             m_characterDataSubscriptions.Clear();
//
//             character?
//                 .ObserveEveryValueChanged(character => character.GetMaxHp())
//                 .Subscribe(v => m_maxHpText.text = v.ToString())
//                 .AddTo(m_characterDataSubscriptions);
//             
//
//             character?
//                 .ObserveEveryValueChanged(character => character.GetSpd())
//                 .Subscribe(v => m_spdText.text = v.ToString())
//                 .AddTo(m_characterDataSubscriptions);
//
//             character?
//                 .ObserveEveryValueChanged(character => character.GetAtk())
//                 .Subscribe(v => m_atkText.text = v.ToString())
//                 .AddTo(m_characterDataSubscriptions);
//
//             character?
//                 .ObserveEveryValueChanged(character => character.GetDef())
//                 .Subscribe(v => m_defText.text = v.ToString())
//                 .AddTo(m_characterDataSubscriptions);
//             
//             UpdateView();
//         }
//
//         void UpdateView()
//         {
//             if (m_character == null)
//                 return;
//
//             m_maxHpText.text = m_character.GetMaxHp().ToString();
//             m_atkText.text = m_character.GetAtk().ToString();
//             m_defText.text = m_character.GetDef().ToString();
//             m_spdText.text = m_character.GetSpd().ToString();
//         }
//     }
// }
