using Mathlife.ProjectL.Gameplay.ObjectBase;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class FireGuideArrowRenderer : MonoBehaviour
    {
        // 기본 길이 1, 최대 길이 2.5
        private const float DEFAULT_LENGTH = 1f;
        private const float MAX_GAP = 1.5f;
        
        private SpriteRenderer spriteRenderer;
        
        // Field
        public void Setup()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void On()
        {
            spriteRenderer.enabled = true;
        }
        
        public void Off()
        {
            spriteRenderer.enabled = false;
        }
        
        public void SetAngle(float angle)
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
        
        /// <param name="power">int [1, 100]</param>
        public void SetPower(int power)
        {
            float length = DEFAULT_LENGTH + MAX_GAP * power / 100;
            spriteRenderer.size = new Vector2(length, spriteRenderer.size.y);
        }

        public Vector2 GetVelocity()
        {
            float power = (spriteRenderer.size.x - DEFAULT_LENGTH) / MAX_GAP;
            return transform.right * (0.1f + 0.9f * power);
        }
    }
}