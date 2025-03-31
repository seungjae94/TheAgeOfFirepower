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

#if UNITY_EDITOR
        [SerializeField]
        private bool drawTangentNormal = false;
#endif

        // Field
        private bool ready = false;
        private bool onGround = true;

        public void Ready()
        {
            ready = true;
        }

        private void Update()
        {
            if (ready == false)
                return;

            if (MovabilityTest() == false)
                return;

            float axis = Input.GetAxisRaw("Horizontal");
            if (axis == 0f)
                return;

            float slideAmount = axis * speed * Time.deltaTime;

            bool slideResult = DestructibleTerrain.Inst.Slide(transform.position, slideAmount, out Vector2 endPosition,
                out Vector2 normal,
                out Vector2 tangent);

            if (false == slideResult)
            {
                Vector2 nextPosition = transform.position + slideAmount * Vector3.right;

                if (DestructibleTerrain.Inst.OnGround(nextPosition))
                {
                     DestructibleTerrain.Inst.ProjectToSurface(nextPosition, Vector3.up, out nextPosition);
                }
                // TODO: 중력 처리

                transform.position = nextPosition;
                Debug.Log("slide failed.");

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

#if UNITY_EDITOR
            DrawTangentNormal(endPosition, tangent, normal);
#endif
        }

        /// <summary>
        /// 이동이 가능한지 체크
        /// </summary>
        /// <returns>
        /// 다음 단계를 수행할 수 있는지 여부
        /// </returns>
        private bool MovabilityTest()
        {
            bool thisFrameOnGround = DestructibleTerrain.Inst.OnGround(transform.position);

            // 임시로 가짜 중력 적용
            if (thisFrameOnGround == false)
            {
                DestructibleTerrain.Inst.ProjectDownToSurface(transform.position, out Vector2 surfacePosition);
                transform.position = surfacePosition;
            }

            return true;
        }

#if UNITY_EDITOR
        private void DrawTangentNormal(Vector3 position, Vector3 tangent, Vector3 normal)
        {
            if (drawTangentNormal == false)
                return;

            DebugLineRenderer.Inst.DrawLine(position, position + tangent, Color.red, 0.01f);
            DebugLineRenderer.Inst.DrawLine(position, position + normal, Color.green, 0.01f);
        }
#endif
    }
}