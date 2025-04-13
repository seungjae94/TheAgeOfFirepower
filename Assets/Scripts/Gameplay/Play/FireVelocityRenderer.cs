using Mathlife.ProjectL.Gameplay.ObjectBase;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    [RequireComponent(typeof(LineRenderer))]
    public class FireVelocityRenderer : MonoBehaviour
    {
        // 기본 길이 0.5, 최대 길이 2
        private const float DEFAULT_LENGTH = 0.5f;
        private const float MAX_GAP = 1.5f;
        
        private LineRenderer lineRenderer;
        
        // Field
        public void Setup()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            Off();
        }

        public void On()
        {
            lineRenderer.enabled = true;
        }
        
        public void Off()
        {
            lineRenderer.enabled = false;
        }
        
        public void Draw(bool fromRight, float angle, int power)
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, fromRight ? angle : 180f - angle);
            
            float t = (power - 10) / 90f;
            float width = Mathf.Lerp(DEFAULT_LENGTH, DEFAULT_LENGTH + MAX_GAP, t);
            lineRenderer.SetPosition(1, Vector3.right * width);
        }
    }
}