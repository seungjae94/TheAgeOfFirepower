using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class AnimationButton : MonoBehaviour
    {
        [SerializeField]
        private List<Graphic> graphics = new();
        
        [SerializeField]
        private List<Color> colors = new();
        
        private void Start()
        {
            foreach (var graphic in graphics)
            {
                colors.Add(graphic.color);
            }
        }
        
        
        private void EnableAnimCallback()
        {
            for (int i = 0; i < graphics.Count; ++i)
            {
                graphics[i].color = colors[i];
            }
        }
        
        private void DisableAnimCallback()
        {
            for (int i = 0; i < graphics.Count; ++i)
            {
                graphics[i].color = colors[i] * 0.75f;
            }
        }
    }
}