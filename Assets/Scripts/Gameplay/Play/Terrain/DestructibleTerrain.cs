using System.Linq;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class DestructibleTerrain : MonoSingleton<DestructibleTerrain>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Scene;
        
        // Dependency
        [SerializeField]
        private Sprite sprite;

        [SerializeField]
        [MinValue(1f)]
        private Vector2Int chunkSize = new Vector2Int(64, 64);

        // Field
        private QuadTree quadTree;

        private Texture2D originalTexture;
        private float pixelsPerUnit = 64f;

        private Vector2Int chunkCount = new Vector2Int(1, 1);
        private Vector2Int lastChunkSize = new Vector2Int(0, 0);

        private QuadTreeChunk[,] chunks;
        
        public void GenerateTerrain()
        {
            // 텍스쳐 데이터 저장
            originalTexture = sprite.texture;
            pixelsPerUnit = sprite.pixelsPerUnit;

            // 청크 데이터 저장
            chunkCount.x = (originalTexture.width + chunkSize.x - 1) / chunkSize.x;
            chunkCount.y = (originalTexture.height + chunkSize.y - 1) / chunkSize.y;
            lastChunkSize.x = (originalTexture.width % chunkSize.x == 0)
                ? chunkSize.x
                : (originalTexture.width % chunkSize.x);
            lastChunkSize.y = (originalTexture.height % chunkSize.y == 0)
                ? chunkSize.y
                : (originalTexture.height % chunkSize.y);
            chunks = new QuadTreeChunk[chunkCount.x, chunkCount.y];

            // 프리팹 생성
            GameObject chunkPrefab = new GameObject();
            chunkPrefab.AddComponent<SpriteRenderer>();
            chunkPrefab.AddComponent<QuadTreeChunk>();

            for (int x = 0; x < chunkCount.x; ++x)
            {
                for (int y = 0; y < chunkCount.y; ++y)
                {
                    int sx = (x < chunkCount.x - 1) ? chunkSize.x : lastChunkSize.x;
                    int sy = (y < chunkCount.y - 1) ? chunkSize.y : lastChunkSize.y;

                    Color[] pixels = originalTexture.GetPixels(x * chunkSize.x, y * chunkSize.y, sx, sy);

                    Texture2D chunkTexture = new Texture2D(sx, sy);
                    chunkTexture.filterMode = originalTexture.filterMode;
                    chunkTexture.SetPixels(0, 0, sx, sy, pixels);
                    chunkTexture.Apply();

                    GameObject instance = Instantiate(chunkPrefab, transform);
                    instance.name = $"Chunk{x * chunkCount.y + y}";
                    instance.transform.localPosition = new Vector3((float)x * chunkSize.x / pixelsPerUnit,
                        (float)y * chunkSize.y / pixelsPerUnit, 0f);

                    QuadTreeChunk chunk = instance.GetComponent<QuadTreeChunk>();
                    chunk.Init(chunkTexture, pixelsPerUnit);

                    chunks[x, y] = chunk;
                }
            }
        }
        
        public void Paint(Vector3 worldPosition, Shape shape, Color paintColor)
        {
            
        }
    }
}