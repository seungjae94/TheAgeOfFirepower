using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private float speed = 5f;

        private void Update()
        {
            float axis = Input.GetAxisRaw("Horizontal");
            if (axis == 0f)
                return;

            float slideAmount = axis * speed * Time.deltaTime;

            bool slideResult = DestructibleTerrain.Inst.Slide(transform.position, slideAmount, out Vector3 endPosition, out Vector3 normal,
                out Vector3 tangent);

            if (false == slideResult)
            {
                Vector3 nextPosition = transform.position + slideAmount * Vector3.right;

                if (DestructibleTerrain.Inst.OnGround(nextPosition))
                {
                    nextPosition = DestructibleTerrain.Inst.PushOutToSurface(nextPosition, Vector3.up); 
                }
                // TODO: 중력 처리

                transform.position = nextPosition;
                
                return;
            }
            
            transform.position = endPosition;

            float angle;
            if (axis > 0f)
            {
                spriteRenderer.flipX = false;
                angle = Vector3.SignedAngle(Vector3.right, tangent, Vector3.forward);
                
            }
            else
            {
                spriteRenderer.flipX = true;
                angle = Vector3.SignedAngle(Vector3.left, tangent, Vector3.forward);
            }
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            
            // Draw Normal
            DebugLineRenderer.Inst.DrawLine(endPosition, endPosition + tangent, Color.red, 0.1f);
            DebugLineRenderer.Inst.DrawLine(endPosition, endPosition + normal, Color.green, 0.1f);
        }
    }
}