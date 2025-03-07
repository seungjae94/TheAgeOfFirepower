using System;
using Mathlife.ProjectL.Gameplay.Gameplay.RenderPass;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Mathlife.ProjectL.Gameplay.Rendering
{
    public class ClearScreenRendererFeature : ScriptableRendererFeature
    {
        [SerializeField] private Color clearColor = Color.black;

        private ClearScreenRenderPass renderPass;
        
        public override void Create()
        {
            renderPass =  new ClearScreenRenderPass(clearColor);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            
            
            if (renderingData.cameraData.camera.CompareTag("MainCamera"))
            {
                renderer.EnqueuePass(renderPass);
            }
        }
    }
}