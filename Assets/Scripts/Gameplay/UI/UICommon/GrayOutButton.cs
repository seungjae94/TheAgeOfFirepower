using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class GrayOutButton : Button
    {
        [SerializeField]
        private List<Graphic> targetGraphics = new();

        [SerializeField]
        [MinMaxSlider(0f, 1f)]
        private float saturation;
        
        private readonly List<Color> originalColors = new();

        protected override void Awake()
        {
            base.Awake();

            transition = Selectable.Transition.None;
        }
        
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            ApplyGrayOut(state);
        }
        
        private void ApplyGrayOut(SelectionState state)
        {
            if (originalColors.Count < targetGraphics.Count)
            {
                originalColors.Clear();
                foreach (var graphic in targetGraphics)
                {
                    originalColors.Add(graphic.color);
                }
            }
            
            for (int i = 0; i < targetGraphics.Count; i++)
            {
                if (targetGraphics[i] == null)
                {
                    MyDebug.LogError("Some of targetGraphic is null.");
                    return;
                }

                switch (state)
                {
                    case SelectionState.Normal:
                    case SelectionState.Highlighted:
                    case SelectionState.Selected:
                        targetGraphics[i].color = originalColors[i];
                        break;
                    case SelectionState.Pressed:
                    case SelectionState.Disabled:
                        targetGraphics[i].color = originalColors[i] * saturation;
                        break;
                }
            }
        }
    }
}