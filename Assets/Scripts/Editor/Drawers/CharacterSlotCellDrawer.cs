using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using global::Sirenix.OdinInspector.Editor.Drawers;
using global::Sirenix.OdinInspector;
using global::Sirenix.Utilities.Editor;
using global::Sirenix.Utilities;
using UnityEngine;
using Mathlife.ProjectL.Gameplay;
using DG.DemiEditor;

namespace Mathlife.ProjectL.Editor
{
    internal sealed class CharacterSlotCellDrawer<TArray> : TwoDimensionalArrayDrawer<TArray, CharacterState> where TArray : System.Collections.IList
    {
        protected override TableMatrixAttribute GetDefaultTableMatrixAttributeSettings()
        {
            return new TableMatrixAttribute()
            {
                SquareCells = true,
                HideColumnIndices = true,
                HideRowIndices = true,
                ResizableColumns = false
            };
        }

        protected override CharacterState DrawElement(Rect rect, CharacterState value)
        {
            var id = DragAndDropUtilities.GetDragAndDropId(rect);

            if (value.character != null)
            {
                DragAndDropUtilities.DrawDropZone(rect, value.character.portrait, null, id); // Draws the drop-zone using the icon.
            }
            else
            {
                DragAndDropUtilities.DrawDropZone(rect, null, null, id); // Draws the drop-zone using the icon.
            }

            if (value.character != null)
            {
                // Draw Character level and exp
                var levelRect = rect.Padding(2, 2, 2, 18).AlignBottom(16);
                var expRect = rect.Padding(2).AlignBottom(16);

                SirenixEditorGUI.DrawRoundRect(levelRect.AlignLeft(30), Color.black, 2);
                GUI.Label(levelRect.AlignLeft(30), "Lv");
                value.level = EditorGUI.IntField(levelRect.Padding(32, 0, 0, 0), Mathf.Max(1, value.level));

                SirenixEditorGUI.DrawRoundRect(expRect.AlignLeft(30), Color.black, 2);
                GUI.Label(expRect.AlignLeft(30), "Exp");
                value.currentLevelExp = EditorGUI.LongField(expRect.Padding(32, 0, 0, 0), Math.Max(0L, value.currentLevelExp));
            }

            value = DragAndDropUtilities.DropZone(rect, value, id);                                             // Drop zone for CharacterSlot structs.
            value.character = DragAndDropUtilities.DropZone<CharacterSO>(rect, value.character, id);     // Drop zone for CharacterDataAsset types.
            value = DragAndDropUtilities.DragZone(rect, value, true, true, id);                             // Enables dragging of the CharacterSlot

            return value;
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            base.DrawPropertyLayout(label);

            // Draws a drop-zone where we can destroy items.
            var rect = GUILayoutUtility.GetRect(0, 40).Padding(2);
            var id = DragAndDropUtilities.GetDragAndDropId(rect);
            DragAndDropUtilities.DrawDropZone(rect, null as UnityEngine.Object, null, id);
            DragAndDropUtilities.DropZone<CharacterState>(rect, new CharacterState(), false, id);
        }
    }
}
