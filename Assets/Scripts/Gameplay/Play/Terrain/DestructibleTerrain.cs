using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mathlife.ProjectL.Gameplay.Play
{
    [RequireComponent(typeof(MeshFilter))]
    public class DestructibleTerrain : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter meshFilter;

        [SerializeField]
        private Texture2D texture;

        [SerializeField]
        private float pixelsPerUnit = 64f;

        private TerrainData terrainData;
        private QuadTree quadTree;
        
#if UNITY_EDITOR
        [ShowInInspector]
        private bool drawGizmos = false;
        
        [ReadOnly]
        [ShowInInspector]
        private int quadNodeCount = 0;

        private void OnDrawGizmosSelected()
        {
            if (quadTree != null && drawGizmos)
            {
                quadTree.DrawGizmos(texture, pixelsPerUnit);
            }
        }

        [Button(ButtonSizes.Large, Name = "Generate Terrain")]
        public void GenerateTerrain()
        {
            terrainData = new TerrainData(texture);
            quadTree = new QuadTree(terrainData);
            quadNodeCount = quadTree.Count();
            CreateMesh();
            EditorUtility.SetDirty(this);
        }

        private void CreateMesh()
        {
            MeshCreator creator = new();
            
            foreach (Quad quad in quadTree)
            {
                if (quad.HasChildren() == true)
                    continue;

                if (terrainData.IsFilled(quad) == false)
                    continue;
                
                VertexData[] vertexes = QuadToVertexes(quad);
                creator.AddTriangle(vertexes[0], vertexes[1], vertexes[2]);
                creator.AddTriangle(vertexes[0], vertexes[2], vertexes[3]);
            }
            
            meshFilter.mesh = creator.Create();
        }

        // 1 2
        // 0 3
        private VertexData[] QuadToVertexes(Quad quad)
        {
            VertexData[] vertexArray = new VertexData[4]
            {
                TexCoordToVertex(quad.xMin, quad.yMin),
                TexCoordToVertex(quad.xMin, quad.yMin + quad.height),
                TexCoordToVertex(quad.xMin + quad.width, quad.yMin + quad.height),
                TexCoordToVertex(quad.xMin + quad.width, quad.yMin),
            };
            
            return vertexArray;
        }

        private VertexData TexCoordToVertex(int x, int y)
        {
            float u = x / (float) texture.width;
            float v = y / (float) texture.height;
            
            return new VertexData()
            {
                position = new Vector3(x - (texture.width / 2f), y - (texture.height / 2f), 0) / pixelsPerUnit,
                uv = new Vector2(u, v),
                normal = transform.forward
            };
        }
#endif
    }
}