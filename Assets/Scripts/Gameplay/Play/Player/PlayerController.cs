using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float speed = 5f;
        
        private void Update()
        {
            float axis = Input.GetAxisRaw("Horizontal");
            if (axis == 0f)
                return;

            float slideAmount = axis * speed * Time.deltaTime;
            
            DestructibleTerrain.Inst.Slide(transform.position, slideAmount, out Vector3 endPosition, out Vector3 normal);

            transform.position = endPosition;
        }
    }
}