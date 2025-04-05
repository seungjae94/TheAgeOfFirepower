using TMPro;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class DamageText : MonoBehaviour
    {
        // Alias
        private RectTransform rectTransform => (RectTransform)transform;
        
        
        // Field
        [SerializeField]
        private TextMeshPro text;

        private ArtyController owner;

        private float localY = 2f;
        
        public void Setup(ArtyController owner, int damage)
        {
            text.text = damage.ToString();
            this.owner = owner;
            rectTransform.anchoredPosition = owner.transform.position + localY * Vector3.up;
        }

        private void Update()
        {
            localY += 1f * Time.deltaTime;
            rectTransform.anchoredPosition = owner.transform.position + localY * Vector3.up;
        }
        
        private void DestroyEventCallback()
        {
            Destroy(gameObject);
        }
    }
}