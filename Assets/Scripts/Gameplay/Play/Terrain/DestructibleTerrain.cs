using Mathlife.ProjectL.Gameplay.ObjectBase;
using Mathlife.ProjectL.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI.Extensions;

#if UNITY_EDITOR
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
            Vector3 localPixelPosition = (worldPosition - transform.position) * pixelsPerUnit;

            int offsetX = (int) localPixelPosition.x;
            int offsetY = (int) localPixelPosition.y;
            foreach (Column column in shape.columns)
            {
                foreach (Range range in column.ranges)
                {
                    PaintRange(offsetX + shape.Offset.x, offsetY + shape.Offset.y, range, paintColor);                    
                }

                ++offsetX;
            }
        }

        private void PaintRange(int offsetX, int offsetY, Range range, Color paintColor)
        {
            int chunkIndexX = MyMathf.FloorDiv(offsetX, chunkSize.x);
            int chunkIndexYMin = MyMathf.FloorDiv(offsetY + range.yMin, chunkSize.y);
            int chunkIndexYMax = MyMathf.FloorDiv(offsetY + range.yMax, chunkSize.y);
            
            if (chunkIndexX < 0 || chunkIndexX >= chunks.GetLength(0))
                return;

            if (chunkIndexYMax < 0 || chunkIndexYMin >= chunks.GetLength(1))
                return;

            chunkIndexYMin.Clamp(0, chunks.GetLength(1) - 1);
            chunkIndexYMax.Clamp(0, chunks.GetLength(1) - 1);
            
            int texelX = offsetX % chunkSize.x;

            for (int chunkIndexY = chunkIndexYMin; chunkIndexY <= chunkIndexYMax; ++chunkIndexY)
            {
                QuadTreeChunk chunk = chunks[chunkIndexX, chunkIndexY];

                int texelYMin = 0;
                if (chunkIndexY == chunkIndexYMin)
                {
                    texelYMin = (offsetY + range.yMin) % chunkSize.y;
                }

                int texelYMax = chunk.Height - 1;
                if (chunkIndexY == chunkIndexYMax)
                {
                    texelYMax  = Mathf.Clamp((offsetY + range.yMax) % chunkSize.y, 0, chunk.Height - 1);
                }

                chunk.Paint(texelX, texelYMin, texelYMax - texelYMin + 1, paintColor);
            }
        }
    }
}