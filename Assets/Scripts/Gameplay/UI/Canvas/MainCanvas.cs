using System.Collections.Generic;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public enum ECanvasLayer
    {
        HUD,
        Background,
        Page,
        Overlay,
        Popup,
        Screen // Fade Screen
    }

    public interface IMainCanvas
    {
        public CanvasLayer GetLayer(ECanvasLayer layer);

        public void DeactivateAllPresenters(bool ignoreBackground = true);
    }

    public class MainCanvas<TMainCanvas> : MonoSingleton<TMainCanvas>, IMainCanvas
        where TMainCanvas : MainCanvas<TMainCanvas>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Scene;
        private readonly List<CanvasLayer> canvasLayers = new();

        protected override void OnRegistered()
        {
            base.OnRegistered();

            foreach (var layer in gameObject.GetComponentsInChildren<CanvasLayer>())
            {
                canvasLayers.Add(layer);
            }
        }

        public CanvasLayer GetLayer(ECanvasLayer layer)
        {
            return canvasLayers[(int)layer];
        }

        public void DeactivateAllPresenters(bool ignoreBackground = true)
        {
            for (int i = 0; i < canvasLayers.Count; ++i)
            {
                if (ignoreBackground && i == (int)ECanvasLayer.Background)
                    continue;
                
                canvasLayers[i].DeactivateAllPresenters();
            }
        }
    }
}