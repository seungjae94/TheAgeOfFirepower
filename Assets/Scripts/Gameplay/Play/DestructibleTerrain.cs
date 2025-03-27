using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class DestructibleTerrain : MonoBehaviour
    {
        [SerializeField]
        private Texture2D texture;
        
        private TerrainData terrainData;
        private QuadTree quadTree;

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (quadTree != null)
            {
                quadTree.DrawGizmos();
            }
        }
        
        [Button(ButtonSizes.Gigantic, Name = "Generate Terrain")]
        public void GenerateTerrain()
        {
            terrainData = new TerrainData(texture);
            quadTree = new QuadTree(terrainData);
            EditorUtility.SetDirty(this);
        }
        #endif
    }
}