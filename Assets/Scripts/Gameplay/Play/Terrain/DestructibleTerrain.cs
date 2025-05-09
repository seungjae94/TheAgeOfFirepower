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
    public enum SlideResult
    {
        Success,
        ShortSpline,
        WrongSpline,
    }
    
    public class DestructibleTerrain : MonoSingleton<DestructibleTerrain>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Scene;

        // Dependency
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
        public float PixelsPerUnit { get; private set; } = 64f;

        private Vector2Int chunkCount = new Vector2Int(1, 1);
        private Vector2Int lastChunkSize = new Vector2Int(0, 0);

        private QuadTreeChunk[,] chunks;

        public float MapWidth => originalTexture.width / PixelsPerUnit;
        public float MapHeight => originalTexture.height / PixelsPerUnit;
        
        public async UniTask GenerateTerrain(Sprite sprite, IProgress<float> progress)
        {
            terrainLayer = LayerMask.NameToLayer("Terrain");

            // 텍스쳐 데이터 저장
            originalTexture = sprite.texture;
            PixelsPerUnit = sprite.pixelsPerUnit;

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
                    instance.transform.localPosition = new Vector3((float)x * chunkSize.x / PixelsPerUnit,
                        (float)y * chunkSize.y / PixelsPerUnit, 0f);

                    QuadTreeChunk chunk = instance.GetComponent<QuadTreeChunk>();
                    chunk.Init(chunkTexture, PixelsPerUnit);

                    chunks[x, y] = chunk;
                }

                if (x % 2 == 0)
                {
                    await UniTask.NextFrame();
                    progress.Report((float)(x + 1) / chunkCount.x);
                }
            }
            
            Destroy(chunkPrefab);
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

        // 스냅
        public bool VerticalSnapToSurface(Vector2 position, out Vector2 surfacePosition, bool restricted = false)
        {
            return SnapToSurface(position, Vector2.up, out surfacePosition, restricted);
        }

        public bool SnapToSurface(Vector2 position, Vector2 snapDirection, out Vector2 surfacePosition, bool restricted = false)
        {
            if (OnSurface(position))
            {
                surfacePosition = position;
                return true;
            }

            // width or height 중 더 큰 방향으로 최소 1 픽셀은 움직이도록 수정
            snapDirection.Normalize();
            //Vector2 displacement = snapDirection / Mathf.Max(Mathf.Abs(snapDirection.x), Mathf.Abs(snapDirection.y));
            Vector2 displacement = snapDirection;
            displacement /= PixelsPerUnit;

            int k = 1;
            while (restricted == false || k < 100)
            {
                Vector2 testPosition0 = position + k * displacement;
                Vector2 testPosition1 = position - k * displacement;

                if (OnSurface(testPosition0))
                {
                    surfacePosition = testPosition0;
                    return true;
                }
                
                if (OnSurface(testPosition1))
                {
                    surfacePosition = testPosition1;
                    return true;
                }
                
                if (InTerrain(testPosition0) == false && InTerrain(testPosition1) == false)
                {
                    break;
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
        public SlideResult Slide(Vector2 startPosition, float translation, out Vector2 endPosition, out Vector2 normal,
            out Vector2 tangent)
        {
            endPosition = Vector3.zero;
            normal = Vector3.zero;
            tangent = Vector3.zero;
            
            // surface에 있는지 테스트
            if (false == OnSurface(startPosition))
                VerticalSnapToSurface(startPosition, out startPosition);

            bool clockWise = translation > 0f;
            int contourLength = Mathf.RoundToInt(Mathf.Abs(translation) * PixelsPerUnit) + 1;
            contourLength = Mathf.Clamp(contourLength, minContourLength, contourLength);

            // 시작 위치 찾기
            Vector2Int startOffset = WorldPositionToTexCoord(startPosition);

            // 컨투어 계산
            List<Vector2Int> contour = MyMathf.MooreNeighbor(startOffset, clockWise, contourLength, GetTexel);

            List<Vector2> worldContour = contour
                .Select(posPx => new Vector2(posPx.x + 0.5f, posPx.y + 0.5f) / PixelsPerUnit)
                .ToList();

            // 스플라인 보간
            int n = worldContour.Count;

            if (n < contourLength)
                return SlideResult.ShortSpline;

            CatmullRomSpline spline = new(clockWise, worldContour);
            
            #if UNITY_EDITOR
            if (drawSpline)
                spline.DrawSpline(DebugLineRenderer.Inst);
            #endif
            
            bool splineResult = spline.GetPoint(Mathf.Abs(translation), out endPosition, out normal, out tangent);

            if (false == splineResult)
                return SlideResult.WrongSpline;
            
            var tempEndPosition = endPosition;
            
            SnapToSurface(endPosition, normal, out endPosition);

            if (Vector2.Distance(endPosition, startPosition) > 1f)
            {
                MyDebug.Log("순간 이동 in Slide");
            }
            
            return SlideResult.Success;
        }

        public bool ExtractNormalTangent(Vector2 startPosition, out Vector2 normal, out Vector2 tangent)
        {
            normal = Vector2.zero;
            tangent = Vector2.zero;
            
            if (false == OnSurface(startPosition))
                return false;
            
            Vector2Int startTexCoord = WorldPositionToTexCoord(startPosition);
            
            // 컨투어 계산
            List<Vector2Int> contourCW = MyMathf.MooreNeighbor(startTexCoord, true, CatmullRomSpline.MODULO + 1, GetTexel);
            List<Vector2Int> contourCCW = MyMathf.MooreNeighbor(startTexCoord, false, CatmullRomSpline.MODULO + 1, GetTexel);
            contourCCW.Reverse();
            contourCCW.RemoveAt(contourCCW.Count - 1);
            contourCCW.AddRange(contourCW);
            
            List<Vector2> worldContour = contourCCW
                .Select(posPx => new Vector2(posPx.x + 0.5f, posPx.y + 0.5f) / PixelsPerUnit)
                .ToList();
            
            CatmullRomSpline spline = new(true, worldContour);
            bool splineResult = spline.GetCenterPoint(out Vector2 point, out normal, out tangent);

            if (false == splineResult)
                return false;
            
            return true;
        }

        
        // Terrain Data
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
        
        private Vector2Int WorldPositionToTexCoord(Vector2 worldPosition)
        {
            Vector2 texCoordFloat = (worldPosition - (Vector2)transform.position) * PixelsPerUnit;
            return Vector2Int.FloorToInt(texCoordFloat);
        }

        private Vector3 TexCoordToWorldPosition(Vector2Int texCoord)
        {
            Vector3 localPosition = new Vector2(texCoord.x + 0.5f, texCoord.y + 0.5f) / PixelsPerUnit;
            return localPosition + transform.position;
        }

        public bool InFairArea(Vector2 worldPosition)
        {
            Vector2 position = worldPosition - (Vector2) transform.position;
            
            Camera mainCamera = Cameras.Inst.MainCamera;
            float vSize = mainCamera.orthographicSize;
            float hSize = vSize * mainCamera.aspect;
            return position.y >= -vSize && position.x >= -hSize &&  position.x <= originalTexture.width / PixelsPerUnit + hSize;
        }

        public bool IsBoundary(Vector2 worldPosition)
        {
            Vector2Int texCoord = WorldPositionToTexCoord(worldPosition);
            return texCoord.x == 0 || texCoord.x == originalTexture.width - 1;
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
            if (InTerrain(worldPosition) == false)
                return false;
            
            Vector2Int texCoord = WorldPositionToTexCoord(worldPosition);
            return GetTexel(texCoord.x, texCoord.y);
        }

        public bool OnSurface(Vector2 worldPosition)
        {
            if (InTerrain(worldPosition) == false)
                return false;
            
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

                    if (InTerrain(x, y) == false || GetTexel(x, y) == false)
                        return true;
                }
            }

            return false;
        }
    }
}