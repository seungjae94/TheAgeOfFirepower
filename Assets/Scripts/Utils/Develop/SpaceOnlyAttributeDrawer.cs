#if UNITY_EDITOR

using UnityEngine;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Mathlife.ProjectL.Utils
{
    [DrawerPriority(2.0, 0.0, 0.0)]
    public sealed class SpaceOnlyAttributeDrawer : OdinAttributeDrawer<SpaceOnlyAttribute>
    {
        private bool drawSpace;

        protected override void Initialize()
        {
            if (base.Property.Parent == null)
            {
                drawSpace = true;
            }
            else if (base.Property.Parent.ChildResolver is ICollectionResolver)
            {
                drawSpace = false;
            }
            else
            {
                drawSpace = true;
            }
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (drawSpace)
            {
                SpaceOnlyAttribute spaceAttribute = base.Attribute;
                if (spaceAttribute.height == 0f)
                {
                    EditorGUILayout.Space();
                }
                else
                {
                    GUILayout.Space(spaceAttribute.height);
                }
            }
        }
    }
}

#endif