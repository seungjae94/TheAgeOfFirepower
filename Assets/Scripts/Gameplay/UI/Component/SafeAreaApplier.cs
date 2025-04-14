using System;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    [Flags]
    public enum AnchorFlag
    {
        None = 0,
        Zero = 1,
        One = 2
    }

    [Serializable]
    public class Margin
    {
        public float left;
        public float right;
        public float bottom;
        public float top;


        public float this[int index]
        {
            get
            {
                return index switch
                {
                    0 => left,
                    1 => right,
                    2 => bottom,
                    3 => top,
                    _ => throw new System.IndexOutOfRangeException("Invalid index: " + index)
                };
            }
            set
            {
                switch (index)
                {
                    case 0: left = value; break;
                    case 1: right = value; break;
                    case 2: bottom = value; break;
                    case 3: top = value; break;
                    default: throw new System.IndexOutOfRangeException("Invalid index: " + index);
                }
            }
        }
    }

    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaApplier : MonoBehaviour
    {
        [SerializeField]
        private Margin minimalOffset;

        private RectTransform rectTransform;
        private RectTransform canvasRectTransform;
        private AnchorFlag horAnchorFlag;
        private AnchorFlag verAnchorFlag;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }

        public void Apply()
        {
            // 원본 사이즈 계산
            Vector2 originalSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
            
            // 스크린 경계와 오프셋 박스 사이의 마진을 계산한다.
            Margin screenToOffsetBoxMargin = new()
            {
                left = (Screen.width - canvasRectTransform.rect.width) / 2 + minimalOffset.left,
                right = (Screen.width - canvasRectTransform.rect.width) / 2 + minimalOffset.right,
                top = (Screen.height - canvasRectTransform.rect.height) / 2 + minimalOffset.top,
                bottom = (Screen.height - canvasRectTransform.rect.height) / 2 + minimalOffset.bottom
            };

            // 스크린 경계와 안전 영역의 사이의 마진을 계산한다.
            Margin screenToSafeAreaMargin = new()
            {
                left = Screen.safeArea.xMin,
                right = Screen.width - Screen.safeArea.xMax,
                top = Screen.height - Screen.safeArea.yMax,
                bottom = Screen.safeArea.yMin,
            };

            // 앵커 분석
            if (Mathf.Approximately(rectTransform.anchorMin.x, 0f))
                horAnchorFlag |= AnchorFlag.Zero;
            if (Mathf.Approximately(rectTransform.anchorMax.x, 1f))
                horAnchorFlag |= AnchorFlag.One;
            if (Mathf.Approximately(rectTransform.anchorMin.y, 0f))
                verAnchorFlag |= AnchorFlag.Zero;
            if (Mathf.Approximately(rectTransform.anchorMax.y, 1f))
                verAnchorFlag |= AnchorFlag.One;

            // 실제로 적용할 컨텐츠 마진 계산
            Margin contentsMargin = new();

            if (horAnchorFlag.HasFlag(AnchorFlag.Zero))
            {
                CalcContentsMargin(ref contentsMargin, 0, screenToOffsetBoxMargin, screenToSafeAreaMargin);
            }

            if (horAnchorFlag.HasFlag(AnchorFlag.One))
            {
                CalcContentsMargin(ref contentsMargin, 1, screenToOffsetBoxMargin, screenToSafeAreaMargin);
            }

            if (verAnchorFlag.HasFlag(AnchorFlag.Zero))
            {
                CalcContentsMargin(ref contentsMargin, 2, screenToOffsetBoxMargin, screenToSafeAreaMargin);
            }

            if (verAnchorFlag.HasFlag(AnchorFlag.One))
            {
                CalcContentsMargin(ref contentsMargin, 3, screenToOffsetBoxMargin, screenToSafeAreaMargin);
            }
            
            // 컨텐츠 마진 적용
            rectTransform.offsetMin = new Vector2(contentsMargin.left, contentsMargin.bottom);
            rectTransform.offsetMax = new Vector2(-contentsMargin.right, -contentsMargin.top);

            // 타겟 사이즈 계산
            // - 앵커가 없거나 한 쪽에만 있을 경우 원본 크기 유지
            // - 앵커가 양쪽에 있을 경우 앵커와 오프셋을 기준으로 크기 설정
            Vector2 anchorBoxSize = (rectTransform.anchorMax - rectTransform.anchorMin);
            anchorBoxSize.x *= canvasRectTransform.rect.width;
            anchorBoxSize.y *= canvasRectTransform.rect.height;

            Vector2 targetSize = originalSize; 
            if (horAnchorFlag.HasFlag(AnchorFlag.Zero) && horAnchorFlag.HasFlag(AnchorFlag.One))
            {
                targetSize.x = canvasRectTransform.rect.width - contentsMargin.left - contentsMargin.right;
            }
            if (verAnchorFlag.HasFlag(AnchorFlag.Zero) && verAnchorFlag.HasFlag(AnchorFlag.One))
            {
                targetSize.y = canvasRectTransform.rect.height - contentsMargin.bottom - contentsMargin.top;
            }

            // 타겟 사이즈 적용
            rectTransform.sizeDelta = targetSize - anchorBoxSize;
        }

        private void CalcContentsMargin(ref Margin contentsMargin, int varIndex, Margin screenToOffsetBoxMargin, Margin screenToSafeAreaMargin)
        {
            // 안전 영역을 적용할 필요 없는 경우
            float offset = minimalOffset[varIndex];

            // 안전 영역을 적용해야 하는 경우
            if (screenToOffsetBoxMargin[varIndex] < screenToSafeAreaMargin[varIndex])
            {
                offset += screenToSafeAreaMargin[varIndex] - screenToOffsetBoxMargin[varIndex];
            }

            contentsMargin[varIndex] = offset;
        }
        
        public void Setup()
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();

            if (canvasRectTransform == null)
                canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }
    }
}