using System;
using System.Collections.Generic;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using Mathlife.ProjectL.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI.Extensions;

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

        // 렌더링
        public void Paint(Vector3 worldPosition, Shape shape, Color paintColor)
        {
            Vector2Int offset = WorldPositionToOffset(worldPosition);
            foreach (Column column in shape.columns)
            {
                foreach (Range range in column.ranges)
                {
                    PaintRange(offset.x + shape.Offset.x, offset.y + shape.Offset.y, range, paintColor);
                }

                ++offset.x;
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
                    texelYMax = Mathf.Clamp((offsetY + range.yMax) % chunkSize.y, 0, chunk.Height - 1);
                }

                chunk.Paint(texelX, texelYMin, texelYMax - texelYMin + 1, paintColor);
            }
        }

        // 물리

        public void ProjectDownToSurface(Vector3 position, out Vector3 surfacePosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down);
            surfacePosition = hit.point;
        }

        /// <summary>
        /// 표면을 따라 이동한 결과를 반환한다.
        /// </summary>
        /// <param name="startPosition">시작 위치 (표면 위에 있거나 표면 근처에 있는 위치)</param>
        /// <param name="slideAmount">표면을 따라 이동할 거리 (방향 포함)</param>
        /// <param name="endPosition">[out] 도착 위치</param>
        /// <param name="normal">[out] 도착 위치에서의 노말</param>
        public void Slide(Vector3 startPosition, float slideAmount, out Vector3 endPosition, out Vector3 normal)
        {
            ProjectDownToSurface(startPosition, out Vector3 startSurfacePosition);

            int slideCount = Mathf.RoundToInt(slideAmount * pixelsPerUnit);

            // 시작 위치 찾기
            Vector2Int startOffset = WorldPositionToOffset(startSurfacePosition);

            // 컨투어 계산
            List<Vector2Int> contour = MyMathf.MooreNeighbor(startOffset, slideCount, GetTexel);
            //DebugDrawBoundary(contour);
            
            // 도착지점 계산
            Vector2Int endPositionPx = contour[^1];
            endPosition = new Vector3(endPositionPx.x + 0.5f, endPositionPx.y + 0.5f) / pixelsPerUnit;

            // TODO: Catmull-Rom 보간을 통한 노말 계산
            normal = new Vector3();
        }

        private bool GetTexel(int x, int y)
        {
            int indexX = MyMathf.FloorDiv(x, chunkSize.x);
            int indexY = MyMathf.FloorDiv(y, chunkSize.y);

            if (indexX < 0 || indexX >= chunks.GetLength(0) || indexY < 0 || indexY >= chunks.GetLength(1))
                return false;

            return chunks[indexX, indexY].GetTexel(x % chunkSize.x, y % chunkSize.y);
        }

        // 유틸
        private Vector2Int WorldPositionToOffset(Vector3 worldPosition)
        {
            Vector3 localPositionPx = (worldPosition - transform.position) * pixelsPerUnit;
            return new Vector2Int((int)localPositionPx.x, (int)localPositionPx.y);
        }

        private Vector3 OffsetToWorldPosition(Vector2Int offset)
        {
            Vector3 localPositionPx = new Vector3(offset.x, offset.y);
            return (localPositionPx / pixelsPerUnit) + transform.position;
        }

        private void DebugDrawBoundary(List<Vector2Int> boundary)
        {
#if UNITY_EDITOR
            foreach (var boundaryPoint in boundary)
            {
                Vector3 center = OffsetToWorldPosition(boundaryPoint);
                float hw = 0.5f / pixelsPerUnit;
                Debug.DrawLine(center + new Vector3(-hw, hw), center + new Vector3(hw, hw), Color.magenta);
                Debug.DrawLine(center + new Vector3(-hw, hw), center + new Vector3(hw, -hw), Color.magenta);
                Debug.DrawLine(center + new Vector3(hw, hw), center + new Vector3(hw, -hw), Color.magenta);
                Debug.DrawLine(center + new Vector3(-hw, -hw), center + new Vector3(hw, -hw), Color.magenta);
            }
#endif
        }
    }
}