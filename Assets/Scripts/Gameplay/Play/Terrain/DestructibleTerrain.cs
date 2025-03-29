using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class DestructibleTerrain : MonoBehaviour
    {
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

        // Info: 테스트용
        private void Awake()
        {
            GenerateTerrain();
        }
        
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
        
        public void Paint( /*Vector3 position, Shape shape, EPaintType paintType*/)
        {
        }

#if UNITY_EDITOR
        // [ShowInInspector]
        // private bool drawGizmos = false;
        //
        // [ReadOnly]
        // [ShowInInspector]
        // private int quadNodeCount = 0;
        //
        // [SerializeField]
        // private new Camera camera;
        //
        // private Vector3 mousePosition = Vector3.zero;
        // private float circleRadius = 0.2f;
        //
        // private void OnDrawGizmosSelected()
        // {
        //     if (quadTree != null && drawGizmos)
        //     {
        //         quadTree.DrawGizmos(texture, pixelsPerUnit);
        //     }
        // }
        //
        // private void Awake()
        // {
        //     GenerateTerrain();
        // }
        //
        // private void Update()
        // {
        //     if (Input.GetMouseButton(0))
        //     {
        //         Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 5f));
        //         DestroyArea(mousePosition);
        //     }
        // }
        //
        // private void DestroyArea(Vector3 position)
        // {
        //     // 일단은 맵이 0, 0에 있다고 가정
        //     int texelCenterX = (int)(position.x * pixelsPerUnit + texture.width / 2f);
        //     int texelCenterY = (int)(position.y * pixelsPerUnit + texture.height / 2f);
        //     int texelRadius = (int)(circleRadius * pixelsPerUnit);
        //
        //     terrainData.DestroyCircle(texelCenterX, texelCenterY, texelRadius);
        //
        //     quadTree = new QuadTree(terrainData);
        //     quadNodeCount = quadTree.Count();
        //     CreateMesh();
        // }
        //
        // private void GenerateTerrain()
        // {
        //     terrainData = new TerrainData(texture);
        //     quadTree = new QuadTree(terrainData);
        //     quadNodeCount = quadTree.Count();
        //     CreateMesh();
        // }
        //
        // [Button(ButtonSizes.Large, Name = "Generate Terrain")]
        // public void GenerateTerrainEditor()
        // {
        //     GenerateTerrain();
        //     EditorUtility.SetDirty(this);
        // }
        //
        // private void CreateMesh()
        // {
        //     MeshCreator creator = new();
        //
        //     foreach (Quad quad in quadTree)
        //     {
        //         if (quad.HasChildren() == true)
        //             continue;
        //
        //         if (terrainData.IsFilled(quad) == false)
        //             continue;
        //
        //         VertexData[] vertexes = QuadToVertexes(quad);
        //         creator.AddTriangle(vertexes[0], vertexes[1], vertexes[2]);
        //         creator.AddTriangle(vertexes[0], vertexes[2], vertexes[3]);
        //     }
        //
        //     meshFilter.mesh = creator.Create();
        // }
        //
        // // 1 2
        // // 0 3
        // private VertexData[] QuadToVertexes(Quad quad)
        // {
        //     VertexData[] vertexArray = new VertexData[4]
        //     {
        //         TexCoordToVertex(quad.xMin, quad.yMin),
        //         TexCoordToVertex(quad.xMin, quad.yMin + quad.height),
        //         TexCoordToVertex(quad.xMin + quad.width, quad.yMin + quad.height),
        //         TexCoordToVertex(quad.xMin + quad.width, quad.yMin),
        //     };
        //
        //     return vertexArray;
        // }
        //
        // private VertexData TexCoordToVertex(int x, int y)
        // {
        //     float u = x / (float)texture.width;
        //     float v = y / (float)texture.height;
        //
        //     return new VertexData()
        //     {
        //         position = new Vector3(x - (texture.width / 2f), y - (texture.height / 2f), 0) / pixelsPerUnit,
        //         uv = new Vector2(u, v),
        //         normal = transform.forward
        //     };
        // }
#endif
    }
}