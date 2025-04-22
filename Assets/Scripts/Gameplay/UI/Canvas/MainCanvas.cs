using System.Collections.Generic;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public enum ECanvasLayer
    {
        HUD,
        Page,
        Overlay,
        Popup
    }

    public interface IMainCanvas
    {
        public CanvasLayer GetLayer(ECanvasLayer layerType);

        public void DeactivateAllPresenters();
    }

    public class MainCanvas : MonoSingleton<MainCanvas>, IMainCanvas
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Scene;
        private readonly Dictionary<ECanvasLayer, CanvasLayer> canvasLayers = new();

        protected override void OnRegistered()
        {
            base.OnRegistered();

            foreach (var layer in gameObject.GetComponentsInChildren<CanvasLayer>())
            {
                canvasLayers.Add(layer.LayerType, layer);
            }
        }

        public CanvasLayer GetLayer(ECanvasLayer layerType)
        {
            canvasLayers.TryGetValue(layerType, out var canvasLayer);
            return canvasLayer;
        }

        public void DeactivateAllPresenters()
        {
            foreach (var layer in canvasLayers.Values)
            {
                layer.DeactivateAllPresenters();
            }
        }
    }
}