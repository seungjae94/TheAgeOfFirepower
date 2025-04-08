using System;
using System.Collections.Generic;
using Mathlife.ProjectL.Gameplay.Play;
using Mathlife.ProjectL.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class ItemHUD : Presenter, IInteractable
    {
        // Component
        [SerializeField]
        private List<BattleItemButton> buttons;

        public override void Activate()
        {
            base.Activate();
            
            foreach (var button in buttons)
            {
                button.Setup();
            }
        }
        
        public void Enable()
        {
            foreach (var button in buttons)
            {
                button.Enable();
            }
        }

        public void Disable()
        {
            foreach (var button in buttons)
            {
                button.Disable();
            }
        }
    }
}