using Unity.Mathematics.Geometry;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class TerrainData
    {
        private readonly float alphaThreshold = float.Epsilon;
        
        private readonly bool[,] texels;
        public readonly int width;
        public readonly int height;

        public TerrainData(Texture2D texture)
        {
            width = texture.width;
            height = texture.height;
            texels = new bool[width, height];

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    texels[x,y] = texture.GetPixel(x, y).a > alphaThreshold;
                }
            }
        }

        public bool IsFilled(Quad node)
        {
            return texels[node.xMin, node.yMin];
        }
        
        public bool IsQuadUniform(Quad node)
        {
            bool value = texels[node.xMin, node.yMin];
            for (int x = node.xMin; x < node.xMin + node.width; ++x)
            {
                for (int y = node.yMin; y < node.yMin + node.height; ++y)
                {
                    if (texels[x, y] != value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void DestroyCircle(int centerX, int centerY, int radius)
        {
            int xOrigin = Mathf.Max(0, centerX - radius);
            int xEnd = Mathf.Min(width - 1, centerX + radius);
            int yOrigin = Mathf.Max(0, centerY - radius);
            int yEnd = Mathf.Min(height - 1, centerY + radius);
            
            for (int x = xOrigin; x < xEnd; x++)
            {
                for (int y = yOrigin; y < yEnd; y++)
                {
                    int xDelta = x - centerX;
                    int yDelta = y - centerY;
                    float sqrDistance = (xDelta * xDelta) + (yDelta*yDelta);

                    if (sqrDistance < radius * radius)
                    {
                        texels[x, y] = false;
                    }
                }
            }
        }
    }
}