using System;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class TerrainData
    {
        private readonly float alphaThreshold = Single.Epsilon;
        
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

        public bool IsFilled(QuadNode node)
        {
            return texels[node.xMin, node.yMin];
        }
        
        public bool IsQuadNodeUniform(QuadNode node)
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
        
        #if UNITY_EDITOR
        // public void DrawGizmos(float ppu)
        // {
        //     if (ppu <= 0)
        //         throw new ArgumentException("PPU(Pixel Per Unit) should be positive.");
        //
        //     if (texels == null)
        //         throw new NullReferenceException("TerrainData isn't initialized.");
        //
        //     for (int x = 0; x < width; ++x)
        //     {
        //         for (int y = 0; y < height; ++y)
        //         {
        //             Vector3 position = new Vector3(x, y, 0) / ppu;
        //             Gizmos.color = texels[x, y] ? Color.green : Color.red;
        //             Gizmos.DrawSphere(position, 0.5f / ppu);
        //         }
        //     }
        // }
        #endif
    }
}