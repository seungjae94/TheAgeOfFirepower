using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public partial class QuadTree : IEnumerable<QuadNode>
    {
        private TerrainData terrainData;
        private readonly QuadNode root;

        public QuadTree(TerrainData terrainData)
        {
            this.terrainData = terrainData;
            root = new QuadNode(0, 0, terrainData.width, terrainData.height);

            Stack<QuadNode> stack = new();
            stack.Push(root);

            while (stack.TryPop(out QuadNode node))
            {
                if (terrainData.IsQuadNodeUniform(node))
                    continue;

                QuadNode[] children = node.Divide();

                foreach (QuadNode child in children)
                {
                    if (child.IsDivisible())
                    {
                        stack.Push(child);
                    }
                }
            }
        }

        public IEnumerator<QuadNode> GetEnumerator()
        {
            return new QuadNodeEnumerator(root);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

#if UNITY_EDITOR
        public void DrawGizmos()
        {
            //DrawQuadNode(root, Color.green);
            foreach (QuadNode node in this)
            {
                Color color = terrainData.IsFilled(node) ? Color.green : Color.red;
                DrawQuadNode(node, color);
            }
        }
        
        private void DrawQuadNode(QuadNode node, Color color)
        {
            Vector3 center = new Vector3(node.xMin + (node.width / 2f), node.yMin + (node.height / 2f));
            Vector3 size = new Vector3(node.width, node.height);
            
            Gizmos.color = color;
            Gizmos.DrawWireCube(center, size);
            
            color.a = 0.4f;
            Gizmos.color = color;
            Gizmos.DrawCube(center, size);
        }
#endif
    }
}