using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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

        [SerializeField]
        private int minContourLength = 12;

#if UNITY_EDITOR
        [SerializeField]
        private bool drawSpline = true;
#endif

        // Const
        private int terrainLayer = -1;

        // Field
        private QuadTree quadTree;

        private Texture2D originalTexture;
        private float pixelsPerUnit = 64f;

        private Vector2Int chunkCount = new Vector2Int(1, 1);
        private Vector2Int lastChunkSize = new Vector2Int(0, 0);

        private QuadTreeChunk[,] chunks;

        public void GenerateTerrain()
        {
            terrainLayer = LayerMask.NameToLayer("Terrain");

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
                    instance.layer = terrainLayer;
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
            Vector2Int offset = WorldPositionToTexCoord(worldPosition);
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
            if (offsetX < 0 || offsetX >= originalTexture.width)
                return;

            int yMin = Mathf.Max(offsetY + range.yMin, 0);
            int yMax = Mathf.Min(offsetY + range.yMax, originalTexture.height - 1);

            int chunkIndexX = MyMathf.FloorDiv(offsetX, chunkSize.x);
            int chunkIndexYMin = MyMathf.FloorDiv(yMin, chunkSize.y);
            int chunkIndexYMax = MyMathf.FloorDiv(yMax, chunkSize.y);

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

                int texelYMin = (chunkIndexY == chunkIndexYMin) 
                    ? (yMin % chunkSize.y) 
                    : 0;

                int texelYMax = (chunkIndexY == chunkIndexYMax) 
                    ? Mathf.Clamp(yMax % chunkSize.y, 0, chunk.Height - 1)
                    : (chunk.Height - 1);

                chunk.Paint(texelX, texelYMin, texelYMax - texelYMin + 1, paintColor);
            }
        }

        // 물리
        public bool ProjectDownToSurface(Vector2 position, out Vector2 surfacePosition)
        {
            return ProjectToSurface(position, Vector2.up, out surfacePosition);
        }

        public bool ProjectToSurface(Vector2 position, Vector2 projDirection, out Vector2 surfacePosition)
        {
            if (OnSurface(position))
            {
                surfacePosition = position;
                return true;
            }

            // width or height 중 더 큰 방향으로 최소 1 픽셀은 움직이도록 수정
            projDirection.Normalize();
            Vector2 displacement = projDirection / Mathf.Max(Mathf.Abs(projDirection.x), Mathf.Abs(projDirection.y));
            displacement /= pixelsPerUnit;

            if (InGround(position) == false)
                displacement = -displacement;

            int k = 1;
            while (true)
            {
                Vector2 testPosition = position + k * displacement;

                if (InTerrain(testPosition) == false)
                {
                    break;
                }

                if (OnSurface(testPosition))
                {
                    surfacePosition = testPosition;
                    //Debug.Log($"{testPosition.x:F6} {testPosition.y:F6}");
                    return true;
                }

                ++k;
            }

            surfacePosition = position;
            return false;
        }

        /// <summary>
        /// 표면을 따라 이동한 결과를 반환한다.
        /// </summary>
        /// <param name="startPosition">시작 위치 (표면 위에 있거나 표면 근처에 있는 위치)</param>
        /// <param name="translation">표면을 따라 이동할 거리 (방향 포함)</param>
        /// <param name="endPosition">[out] 도착 위치</param>
        /// <param name="normal">[out] 도착 위치에서의 노말</param>
        public bool Slide(Vector2 startPosition, float translation, out Vector2 endPosition, out Vector2 normal,
            out Vector2 tangent)
        {
            // surface에 있는지 테스트
            if (false == OnSurface(startPosition))
                ProjectDownToSurface(startPosition, out startPosition);

            bool clockWise = translation > 0f;
            int contourLength = Mathf.RoundToInt(Mathf.Abs(translation) * pixelsPerUnit) + 1;
            contourLength = Mathf.Clamp(contourLength, minContourLength, contourLength);

            // 시작 위치 찾기
            Vector2Int startOffset = WorldPositionToTexCoord(startPosition);

            // 컨투어 계산
            List<Vector2Int> contour = MyMathf.MooreNeighbor(startOffset, clockWise, contourLength, GetTexel);

            List<Vector2> worldContour = contour
                .Select(posPx => new Vector2(posPx.x + 0.5f, posPx.y + 0.5f) / pixelsPerUnit)
                .ToList();

            // 스플라인 보간
            int n = worldContour.Count;

            if (n < contourLength)
            {
                endPosition = Vector3.zero;
                normal = Vector3.zero;
                tangent = Vector3.zero;
                return false;
            }

            CatmullRomSpline spline = new(clockWise, worldContour);
#if UNITY_EDITOR
            if (drawSpline)
                spline.DrawSpline(DebugLineRenderer.Inst);
#endif

            spline.GetPoint(Mathf.Abs(translation), out endPosition, out normal, out tangent);
            ProjectToSurface(endPosition, normal, out endPosition);
            return true;
        }

        private bool GetTexel(Vector2Int texCoord)
        {
            return GetTexel(texCoord.x, texCoord.y);
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
        private Vector2Int WorldPositionToTexCoord(Vector2 worldPosition)
        {
            Vector2 texCoordFloat = (worldPosition - (Vector2)transform.position) * pixelsPerUnit;
            return Vector2Int.FloorToInt(texCoordFloat);
        }

        private Vector3 TexCoordToWorldPosition(Vector2Int texCoord)
        {
            Vector3 localPosition = new Vector2(texCoord.x + 0.5f, texCoord.y + 0.5f) / pixelsPerUnit;
            return localPosition + transform.position;
        }

        public bool InTerrain(Vector2 worldPosition)
        {
            return InTerrain(WorldPositionToTexCoord(worldPosition));
        }

        public bool InTerrain(Vector2Int texCoord)
        {
            return InTerrain(texCoord.x, texCoord.y);
        }

        public bool InTerrain(int x, int y)
        {
            return x >= 0 && x < originalTexture.width && y >= 0 && y < originalTexture.height;
        }

        public bool InGround(Vector2 worldPosition)
        {
            Vector2Int texCoord = WorldPositionToTexCoord(worldPosition);
            return GetTexel(texCoord.x, texCoord.y);
        }

        public bool OnSurface(Vector2 worldPosition)
        {
            Vector2Int texCoord = WorldPositionToTexCoord(worldPosition);

            if (GetTexel(texCoord.x, texCoord.y) == false)
                return false;

            // 근처 8방향에 Air가 하나라도 있는지 확인
            for (int dx = -1; dx <= 1; ++dx)
            {
                for (int dy = -1; dy <= 1; ++dy)
                {
                    int x = texCoord.x + dx;
                    int y = texCoord.y + dy;

                    if (InTerrain(x, y) == false)
                        continue;

                    if (GetTexel(x, y) == false)
                        return true;
                }
            }

            return false;
        }
    }
}