using System;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    [Flags]
    public enum AnchorFlag
    {
        None = 0,
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8
    }
    
    [Serializable]
    public class Margin
    {
        public float left;

        public float right;

        public float top;

        public float bottom;
    }

    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaApplier : MonoBehaviour
    {
        [SerializeField] private Margin minimalOffset;
        
        private RectTransform rectTransform;
        private RectTransform canvasRectTransform;
        private AnchorFlag anchorFlag;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }

        public void Apply()
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            
            if (canvasRectTransform == null)
                canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;
            
            // 스크린 경계와 오프셋 경계 사이의 마진을 계산한다.
            Margin margin0 = new()
            {
                left = (Screen.width - canvasRectTransform.rect.width) / 2 + minimalOffset.left,
                right = (Screen.width - canvasRectTransform.rect.width) / 2 + minimalOffset.right,
                top = (Screen.height - canvasRectTransform.rect.height) / 2 + minimalOffset.top,
                bottom = (Screen.height - canvasRectTransform.rect.height) / 2 + minimalOffset.bottom
            };

            // 스크린 경계와 안전 영역의 사이의 마진을 계산한다.
            Margin margin1 = new()
            {
                left = Screen.safeArea.xMin,
                right = Screen.width - Screen.safeArea.xMax,
                top = Screen.height - Screen.safeArea.yMax,
                bottom = Screen.safeArea.yMin,
            };
            
            // 앵커 분석
            anchorFlag = AnchorFlag.None;
            if (Mathf.Approximately(rectTransform.anchorMin.x, 0f))
                anchorFlag |= AnchorFlag.Left;
            if (Mathf.Approximately(rectTransform.anchorMax.x, 1f))
                anchorFlag |= AnchorFlag.Right;
            if (Mathf.Approximately(rectTransform.anchorMax.y, 1f))
                anchorFlag |= AnchorFlag.Top;
            if (Mathf.Approximately(rectTransform.anchorMin.y, 0f))
                anchorFlag |= AnchorFlag.Bottom;

            // 피벗 세팅
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            if (anchorFlag.HasFlag(AnchorFlag.Left) && !anchorFlag.HasFlag(AnchorFlag.Right))
            {
                rectTransform.pivot = new Vector2(0f, 0.5f);
            }
            else if (!anchorFlag.HasFlag(AnchorFlag.Left) && anchorFlag.HasFlag(AnchorFlag.Right))
            {
                rectTransform.pivot = new Vector2(1f, 0.5f);
            }
            if (anchorFlag.HasFlag(AnchorFlag.Bottom) && !anchorFlag.HasFlag(AnchorFlag.Top))
            {
                rectTransform.pivot = new Vector2(rectTransform.pivot.x, 0f);
            }
            else if (!anchorFlag.HasFlag(AnchorFlag.Bottom) && anchorFlag.HasFlag(AnchorFlag.Top))
            {
                rectTransform.pivot = new Vector2(rectTransform.pivot.x, 1f);
            }
            
            // 왼쪽 앵커
            if (anchorFlag.HasFlag(AnchorFlag.Left))
            {
                // 안전 영역을 적용할 필요 없는 경우
                float offset = minimalOffset.left;
                
                // 안전 영역을 적용해야 하는 경우
                if (margin0.left < margin1.left)
                {
                    offset += margin1.left - margin0.left;
                }
                
                rectTransform.offsetMin = new Vector2(offset, rectTransform.offsetMin.y);
            }
            
            // 오른쪽 앵커
            if (anchorFlag.HasFlag(AnchorFlag.Right))
            {
                // 안전 영역을 적용할 필요 없는 경우
                float offset = minimalOffset.right;
                
                // 안전 영역을 적용해야 하는 경우
                if (margin0.right < margin1.right)
                {
                    offset += margin1.right - margin0.right;
                }
                
                rectTransform.offsetMax = new Vector2(-offset, rectTransform.offsetMax.y);
            }
            
            // 위쪽 앵커
            if (anchorFlag.HasFlag(AnchorFlag.Top))
            {
                // 안전 영역을 적용할 필요 없는 경우
                float offset = minimalOffset.top;
                
                // 안전 영역을 적용해야 하는 경우
                if (margin0.top < margin1.top)
                {
                    offset += margin1.top - margin0.top;
                }
                
                rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -offset);
            }
            
            // 아래쪽 앵커
            if (anchorFlag.HasFlag(AnchorFlag.Bottom))
            {
                // 안전 영역을 적용할 필요 없는 경우
                float offset = minimalOffset.bottom;
                
                // 안전 영역을 적용해야 하는 경우
                if (margin0.bottom < margin1.bottom)
                {
                    offset += margin1.bottom - margin0.bottom;
                }
                
                rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, offset);
            }
            
            // 사이즈
            Vector2 targetSize = new Vector2(width, height);
            Vector2 anchorBoxSize = (rectTransform.anchorMax - rectTransform.anchorMin); 
            anchorBoxSize.x *= canvasRectTransform.rect.width;
            anchorBoxSize.y *= canvasRectTransform.rect.height;
            rectTransform.sizeDelta = targetSize - anchorBoxSize;
        }
    }
}