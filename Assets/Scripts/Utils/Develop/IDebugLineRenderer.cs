using UnityEngine;

namespace Mathlife.ProjectL.Utils
{
    public interface IDebugLineRenderer
    {
        public void Clear();
        public void DrawLine(Vector2 start, Vector2 end, Color color, float lineWidth = 0.01f);
        public void DrawLine(Vector3 start, Vector3 end, Color color, float lineWidth = 0.01f);
    }
}