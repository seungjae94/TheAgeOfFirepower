using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Mathlife.ProjectL.Gameplay.Gameplay.RenderPass
{
    public class ClearScreenRenderPass : ScriptableRenderPass
    {
        private new readonly Color clearColor;
        
        public ClearScreenRenderPass(Color clearColor)
        {
            this.clearColor = clearColor;
            renderPassEvent = RenderPassEvent.BeforeRenderingPrePasses;
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("Clear Screen");
            cmd.ClearRenderTarget(true, true, clearColor);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}