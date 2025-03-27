using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class MeshCreator
    {
        private readonly List<Vector3> vertices = new();
        private readonly List<int> indexes = new();
        private readonly List<Vector2> uvs = new();
        private readonly List<Vector3> normals = new();
        
        public Mesh Create()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = indexes.ToArray();
            mesh.normals = normals.ToArray();
            mesh.uv = uvs.ToArray();
            return mesh;
        }
        
        public void AddTriangle(VertexData vertexA, VertexData vertexB, VertexData vertexC)
        {
            Vector3 normal = ComputeNormal(vertexA, vertexB, vertexC);
        
            vertexA.normal = normal;
            vertexB.normal = normal;
            vertexC.normal = normal;
            
            int indexA = AddVertex(vertexA);
            int indexB = AddVertex(vertexB);
            int indexC = AddVertex(vertexC);
        
            indexes.Add(indexA);
            indexes.Add(indexB);
            indexes.Add(indexC);
        }

        private int AddVertex(VertexData vertex)
        {
            vertices.Add(vertex.position);
            uvs.Add(vertex.uv);
            normals.Add(vertex.normal);
            return vertices.Count - 1;
        }
        
        private static Vector3 ComputeNormal(VertexData vertexA, VertexData vertexB, VertexData vertexC)
        {
            Vector3 leftEdge = vertexB.position - vertexA.position;
            Vector3 rightEdge = vertexC.position - vertexA.position;

            Vector3 normal = Vector3.Cross(leftEdge, rightEdge);

            return normal.normalized;
        }
    }
}