using System.Collections.Generic;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using Mathlife.ProjectL.Utils;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class DebugLineRenderer : MonoSingleton<DebugLineRenderer>, IDebugLineRenderer
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Scene;
        
        private readonly List<LineRenderer> lineRenderers = new();
        private int drawIndex = 0;

        protected override void OnRegistered()
        {
            GameObject prefab = new GameObject();
            LineRenderer prefabLineRenderer = prefab.AddComponent<LineRenderer>();
            prefabLineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));
            prefabLineRenderer.alignment = LineAlignment.TransformZ;
            
            for (int i = 0; i < 300; ++i)
            {
                GameObject go = Instantiate(prefab, transform);
                lineRenderers.Add(go.GetComponent<LineRenderer>());
            }
        }

        public void Clear()
        {
            foreach (var lineRenderer in lineRenderers)
            {
                lineRenderer.enabled = false;
            }
        }

        public void DrawLine(Vector2 start, Vector2 end, Color color, float lineWidth)
        {
            DrawLine(new Vector3(start.x, start.y, 0), new Vector3(end.x, end.y, 0), color, lineWidth);
        }
        
        public void DrawLine(Vector3 start, Vector3 end, Color color, float lineWidth)
        {
            lineRenderers[drawIndex].enabled = true;
            
            lineRenderers[drawIndex].startColor = color;
            lineRenderers[drawIndex].endColor = color;
            
            lineRenderers[drawIndex].startWidth = lineWidth;
            lineRenderers[drawIndex].endWidth = lineWidth;

            lineRenderers[drawIndex].positionCount = 2;
            lineRenderers[drawIndex].SetPosition(0, start);
            lineRenderers[drawIndex].SetPosition(1, end);
            
            drawIndex = (drawIndex + 1) % lineRenderers.Count;
        }
    }
}