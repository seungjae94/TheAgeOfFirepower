using Mathlife.ProjectL.Gameplay;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Mathlife.ProjectL.Editor
{
    //internal class CharacterSlotDrawer : OdinValueDrawer<CharacterSlot>
    //{
    //    InspectorProperty character;
    //    InspectorProperty level;
    //    InspectorProperty currentLevelExp;

    //    protected override void Initialize()
    //    {
    //        character = Property.Children[nameof(character)];
    //        level = Property.Children[nameof(level)];
    //        currentLevelExp = Property.Children[nameof(currentLevelExp)];

    //    }

    //    protected override void DrawPropertyLayout(GUIContent label)
    //    {
    //        Rect rect = EditorGUILayout.GetControlRect();

    //        //if (label != null)
    //        //    rect = EditorGUI.PrefixLabel(rect, label);

    //        character.ValueEntry.WeakSmartValue
    //            = SirenixEditorFields.PreviewObjectField(
    //                rect.Split(0, 2), (CharacterDataAsset)character.ValueEntry.WeakSmartValue,
    //                dragOnly: false, allowMove: false, allowSwap: true);
    //        level.ValueEntry.WeakSmartValue
    //            = SirenixEditorFields.IntField(
    //                rect.Split(1, 2).SplitVertical(0, 2),
    //                (int)level.ValueEntry.WeakSmartValue);
    //        currentLevelExp.ValueEntry.WeakSmartValue
    //            = SirenixEditorFields.LongField(
    //                rect.Split(1, 2).SplitVertical(1, 2),
    //                (long)currentLevelExp.ValueEntry.WeakSmartValue);
    //    }
    //}
}
