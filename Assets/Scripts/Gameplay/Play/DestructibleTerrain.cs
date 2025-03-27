using System.Linq;
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
        [ReadOnly]
        [ShowInInspector]
        private int quadNodeCount = 0;
        
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
            quadNodeCount = quadTree.Count();
            EditorUtility.SetDirty(this);
        }
        #endif
    }
}