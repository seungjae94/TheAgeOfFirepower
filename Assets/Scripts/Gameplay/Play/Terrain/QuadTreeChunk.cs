using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class QuadTreeChunk : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private QuadTree quadTree;
        
        public void Init(Texture2D texture, float pixelsPerUnit)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero,
                pixelsPerUnit);
            RefreshColliders();
        }

        public void Paint()
        {
            // TODO: refresh texture
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
    }
}