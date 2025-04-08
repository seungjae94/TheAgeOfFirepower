using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class DamageText : MonoBehaviour
    {
        // Alias
        private RectTransform rectTransform => (RectTransform)transform;
        
        // Setting
        [SerializeField]
        private TextMeshPro textMesh;
        
        [SerializeField]
        private Material healTextMaterial;
        
        // Field
        private ArtyController owner;

        private float localY = 2f;
        
        public void Setup(ArtyController owner, int damage, bool isHeal)
        {
            textMesh.text = damage.ToString();
            
            if (isHeal)
                textMesh.fontMaterial = healTextMaterial;
            
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