using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public partial class QuadTree : IEnumerable<Quad>
    {
        private TerrainData terrainData;
        private readonly Quad root;

        public QuadTree(TerrainData terrainData)
        {
            this.terrainData = terrainData;
            root = new Quad(0, 0, terrainData.width, terrainData.height);

            Stack<Quad> stack = new();
            stack.Push(root);

            while (stack.TryPop(out Quad quad))
            {
                if (terrainData.IsQuadUniform(quad))
                    continue;

                Quad[] children = quad.Divide();

                foreach (Quad child in children)
                {
                    if (child.IsDivisible())
                    {
                        stack.Push(child);
                    }
                }
            }
        }

        public IEnumerator<Quad> GetEnumerator()
        {
            return new QuadEnumerator(root);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

#if UNITY_EDITOR
        public void DrawGizmos(Texture2D texture, float pixelsPerUnit)
        {
            foreach (Quad quad in this)
            {
                if (terrainData.IsQuadUniform(quad))
                {
                    Color color = terrainData.IsFilled(quad) ? Color.blue : Color.red;
                    DrawQuad(quad, color, texture, pixelsPerUnit);
                }
            }
        }
        
        private void DrawQuad(Quad quad, Color color, Texture2D texture, float pixelsPerUnit)
        {
            Vector3 center = new Vector3(quad.xMin + (quad.width / 2f) - (texture.width / 2f), quad.yMin + (quad.height / 2f) - (texture.height / 2f), 0) / pixelsPerUnit;
            Vector3 size = new Vector3(quad.width, quad.height, 0) / pixelsPerUnit;
            
            Gizmos.color = color;
            Gizmos.DrawWireCube(center, size);
            
            color.a = 0.4f;
            Gizmos.color = color;
            Gizmos.DrawCube(center, size);
        }
#endif
    }
}