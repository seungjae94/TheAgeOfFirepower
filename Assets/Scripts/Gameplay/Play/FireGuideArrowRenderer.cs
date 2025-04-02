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
        public float Angle { get; private set; } // Angle from Vector2.right
        public int Power { get; private set; }
        
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
        
        /// <param name="angle">int degree</param>
        public void SetAngle(float angle)
        {
            Angle = angle;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        
        /// <param name="power">int [1, 100]</param>
        public void SetPower(int power)
        {
            Power = power;

            float length = DEFAULT_LENGTH + MAX_GAP * power / 100;
            spriteRenderer.size = new Vector2(length, spriteRenderer.size.y);
        }
    }
}