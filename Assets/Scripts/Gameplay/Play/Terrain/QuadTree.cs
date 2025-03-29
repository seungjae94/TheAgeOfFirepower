using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class QuadTree : IEnumerable<Quad>
    {
        private readonly bool[,] texels;
        private readonly float pixelsPerUnit; 
        
        private readonly Quad root;

        public QuadTree(Sprite sprite, float alphaThreshold = float.Epsilon)
        {
            // 텍스쳐 데이터 저장
            Texture2D texture = sprite.texture;
            texels = new bool[texture.width, texture.height];
            pixelsPerUnit = sprite.pixelsPerUnit;
            
            for (int x = 0; x < texture.width; ++x)
            {
                for (int y = 0; y < texture.height; ++y)
                {
                    texels[x,y] = texture.GetPixel(x, y).a > alphaThreshold;
                }
            }
            
            // 쿼드 트리 생성 (BFS)
            root = new Quad(0, 0, texture.width, texture.height);

            Stack<Quad> stack = new();
            stack.Push(root);

            while (stack.TryPop(out Quad quad))
            {
                if (IsQuadUniform(quad))
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

        public bool IsQuadUniform(Quad quad)
        {
            bool value = texels[quad.xMin, quad.yMin];
            for (int x = quad.xMin; x < quad.xMin + quad.width; ++x)
            {
                for (int y = quad.yMin; y < quad.yMin + quad.height; ++y)
                {
                    if (texels[x, y] != value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool IsQuadFilled(Quad quad)
        {
            return texels[quad.xMin, quad.yMin];
        }

#if UNITY_EDITOR
        public void DrawGizmos()
        {
            foreach (Quad quad in this)
            {
                if (IsQuadUniform(quad))
                {
                    Color color = IsQuadFilled(quad) ? Color.blue : Color.red;
                    DrawQuad(quad, color);
                }
            }
        }
        
        private void DrawQuad(Quad quad, Color color)
        {
            int width = texels.GetLength(0);
            int height = texels.GetLength(1);
            
            Vector3 center = new Vector3(quad.xMin + (quad.width / 2f) - (width / 2f), quad.yMin + (quad.height / 2f) - (height / 2f), 0) / pixelsPerUnit;
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