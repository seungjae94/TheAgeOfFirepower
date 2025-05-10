using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class QuadTreeChunk : MonoBehaviour
    {
        public int Width => spriteRenderer?.sprite?.texture?.width ?? 0;
        public int Height => spriteRenderer?.sprite?.texture?.height ?? 0;
        
        private SpriteRenderer spriteRenderer;
        private QuadTree quadTree;

        /// <summary>
        /// 한 프레임에 여러 번 Paint를 할 수 있기 때문에 Paint를 호출할 때는 dirty 체크만 하고
        /// Update 타이밍에 콜라이더를 업데이트한다. 
        /// </summary>
        private bool dirty = false;
        
        public void Init(Texture2D texture, float pixelsPerUnit)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero,
                pixelsPerUnit);
            
            RefreshColliders();
        }

        public void Paint(int texelX, int texelY, int height, Color paintColor)
        {
            if (texelX < 0 || texelX >= Width || texelY < 0 || height < 0 || texelY + height > Height)
                throw new IndexOutOfRangeException("Given texels are out of bounds.");
            
            Texture2D texture = spriteRenderer.sprite.texture;

            Color[] colors = new Color[height];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = paintColor;
            
            texture.SetPixels(texelX, texelY, 1, height, colors);
            texture.Apply();

            dirty = true;
        }

        private void RefreshColliders()
        {
            float pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;

            // 콜라이더 Off
            List<BoxCollider2D> colliders = gameObject.GetComponents<BoxCollider2D>().ToList();
            foreach (var boxCollider2D in colliders)
            {
                boxCollider2D.enabled = false;
            }

            // 쿼드 트리 재생성
            quadTree = new QuadTree(spriteRenderer.sprite);

            foreach (Quad quad in quadTree)
            {
                if (quadTree.IsQuadUniform(quad) == false || quadTree.IsQuadFilled(quad) == false)
                    continue;

                Vector2 targetOffset = quad.Offset / pixelsPerUnit;
                Vector2 targetSize = quad.Size / pixelsPerUnit;
                
                BoxCollider2D foundCollider = colliders.Find(boxCollider2D =>
                    boxCollider2D.offset == targetOffset
                    && boxCollider2D.size == targetSize);
                
                if (foundCollider != null)
                {
                    foundCollider.enabled = true;
                    continue;
                }

                BoxCollider2D newCollider = gameObject.AddComponent<BoxCollider2D>();
                newCollider.offset = targetOffset;
                newCollider.size = targetSize;
            }

            // 불필요한 콜라이더 제거
            foreach (var boxCollider2D in colliders)
            {
                if (boxCollider2D.enabled == false)
                    Destroy(boxCollider2D);
            }
        }

        private void Update()
        {
            if (dirty)
            {
                RefreshColliders();
                dirty = false;
            }
        }

        public bool GetTexel(int x, int y)
        {
            return spriteRenderer.sprite.texture.GetPixel(x, y).a > float.Epsilon;
        }

        private void OnDestroy()
        {
            if (spriteRenderer?.sprite?.texture != null)
            {
                Destroy(spriteRenderer.sprite.texture);
            }
            
            if (spriteRenderer?.sprite != null)
            {
                Destroy(spriteRenderer.sprite);
            }
        }
    }
}