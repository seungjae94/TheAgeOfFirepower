using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    [RequireComponent(typeof(RectTransform))]   
    public class SafeMargin : MonoBehaviour
    {
        [SerializeField] 
        private float left;
        
        [SerializeField] 
        private float right;
        
        [SerializeField] 
        private float bottom;
        
        public void Apply()
        {
            
        }
    }
}