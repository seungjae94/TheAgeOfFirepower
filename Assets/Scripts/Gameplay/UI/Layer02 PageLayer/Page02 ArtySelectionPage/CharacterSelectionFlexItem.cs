// using System;
// using TMPro;
// using UniRx;
// using UniRx.Triggers;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace Mathlife.ProjectL.Gameplay
// {
//     class CharacterSelectionFlexItemFactory
//     : IFlexItemFactory<CharacterModel, Action<CharacterModel>, CharacterSelectionFlexItem>
//     {
//         GameDataLoader gameDataLoader;
//
//         public CharacterSelectionFlexItem Create(Transform parent, CharacterModel character, Action<CharacterModel> onClick)
//         {
//             CharacterSelectionFlexItem flexItem
//                 = gameDataLoader.Instantiate<CharacterSelectionFlexItem>(EPrefabId.CharacterSelectionFlexItem, parent);
//
//             flexItem.Initialize(character, onClick);
//
//             //m_container.Inject(flexItem);
//
//             return flexItem;
//         }
//     }
//
//
//     [RequireComponent(typeof(ObservablePointerClickTrigger))]
//     class CharacterSelectionFlexItem : Presenter<CharacterModel, Action<CharacterModel>>
//     {
//         ObservablePointerClickTrigger m_clickTrigger;
//         [SerializeField] Image m_portraitImage;
//         [SerializeField] TMP_Text m_levelText;
//         [SerializeField] TMP_Text m_nameText;
//
//         CharacterModel character;
//         Action<CharacterModel> m_onClick;
//
//         void Awake()
//         {
//             m_clickTrigger = GetComponent<ObservablePointerClickTrigger>(); 
//         }
//
//         protected override void Store(CharacterModel character, Action<CharacterModel> onClick)
//         {
//             this.character = character;
//             m_onClick = onClick;
//         }
//
//         protected override void SubscribeDataChange()
//         {
//             character.levelRx
//                 .Subscribe(UpdateLevelText)
//                 .AddTo(gameObject);
//         }
//
//         protected override void SubscribeUserInteractions()
//         {
//             m_clickTrigger.OnPointerClickAsObservable()
//                 .Subscribe(ev => m_onClick(character))
//                 .AddTo(gameObject);
//         }
//
//         protected override void InitializeView()
//         {
//             m_portraitImage.sprite = character.Sprite;
//             m_nameText.text = character.displayName;
//             UpdateLevelText(character.levelRx.Value);
//         }
//
//         // 뷰 업데이트
//         void UpdateLevelText(int level)
//         {
//             m_levelText.text = character.levelRx.ToString();
//         }
//     }
// }
