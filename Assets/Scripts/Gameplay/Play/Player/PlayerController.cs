using UnityEngine;

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

            DestructibleTerrain.Inst.Slide(transform.position, slideAmount, out Vector3 endPosition, out Vector3 normal,
                out Vector3 tangent);
            transform.position = DestructibleTerrain.Inst.ProjectUpToSurface(endPosition);

            spriteRenderer.flipX = tangent.x < 0f;

            float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, (tangent.x < 0f) ? angle + 180f : angle));
        }
    }
}