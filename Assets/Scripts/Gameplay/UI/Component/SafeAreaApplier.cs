using System;
using UnityEngine;
using UnityEngine.Rendering;

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
        
        public override string ToString()
        {
            return $"(L={left}, R={right}, B={bottom}, T={top})";
        }
    }

    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaApplier : MonoBehaviour
    {
        [SerializeField]
        private Margin minimalOffset;

        private Camera mainCamera;
        private RectTransform rectTransform;
        private RectTransform canvasRectTransform;
        private AnchorFlag horAnchorFlag;
        private AnchorFlag verAnchorFlag;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            mainCamera = Camera.main;
        }

        public void Apply()
        {
            // 원본 사이즈 계산
            Vector2 originalSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
            
            // 계산의 편의를 위해 캔버스 높이가 스크린 높이와 같다고 가정한다.
            // 다음 순서로 계산한다:
            // 1. SafeArea가 뷰포트를 얼마나 침범하는지 계산한다. 이를 safeAreaMargin이라고 하자.
            // 2. safeAreaMargin는 스크린 크기를 기준으로 계산한 값이다. 실제 캔버스 높이에 맞게 보정해준다. 이를 scaledOffset이라고 하자.
            // 2. minimalOffset은 캔버스 높이가 1080이라고 가정하고 지정한 값이다. 실제 캔버스 높이에 맞게 보정해준다. 이를 scaledOffset이라고 하자.
            // 3. safeAreaMargin과 scaledOffset을 비교한다. left, right, top, bottom 각각에 대해 더 큰 값을 취한 마진을 maxMargin이라고 하자.
            // 4. maxMargin을 적용해준다.
            // 5. 컨텐츠 사이즈를 갱신해준다.
            
            // 1. safeAreaMargin 계산
            Margin safeAreaMargin = new()
            {
                left = Mathf.Max(Screen.safeArea.xMin - (Screen.width * mainCamera.rect.xMin), 0),
                right = Mathf.Max((Screen.width * mainCamera.rect.xMax) - Screen.safeArea.xMax , 0),
                bottom = Mathf.Max(Screen.safeArea.yMin - (Screen.height * mainCamera.rect.yMin), 0),
                top = Mathf.Max((Screen.height * mainCamera.rect.yMax) - Screen.safeArea.yMax, 0),
            };
            
            // 2. scaledOffset 계산
            float canvasScalingFactor = Constants.VIEWPORT_HEIGHT / canvasRectTransform.rect.height;
            Margin scaledOffset = new()
            {
                left = minimalOffset.left / canvasScalingFactor,
                right = minimalOffset.right / canvasScalingFactor,
                bottom = minimalOffset.bottom / canvasScalingFactor,
                top = minimalOffset.top / canvasScalingFactor
            };
            
            // 3. maxMargin 계산
            Margin maxMargin = new()
            {
                left = Mathf.Max(safeAreaMargin.left, scaledOffset.left),
                right = Mathf.Max(safeAreaMargin.right, scaledOffset.right),
                top = Mathf.Max(safeAreaMargin.top, scaledOffset.top),
                bottom = Mathf.Max(safeAreaMargin.bottom, scaledOffset.bottom)
            };
            
            //MyDebug.Log($"스케일링 팩터: {canvasScalingFactor}");
            //MyDebug.Log($"스크린 크기: {Screen.width} x {Screen.height}");
            //MyDebug.Log($"캔버스 크기: {canvasRectTransform.rect.width} x {canvasRectTransform.rect.height}");
            //MyDebug.Log($"MinimalOffset: {minimalOffset}");
            //MyDebug.Log($"실제 적용할 마진: {maxMargin}");
            
            // 컨텐츠 마진 적용
            rectTransform.offsetMin = new Vector2(maxMargin.left, maxMargin.bottom);
            rectTransform.offsetMax = new Vector2(-maxMargin.right, -maxMargin.top);

            // 타겟 사이즈 계산
            // - 앵커가 없거나 한 쪽에만 있을 경우 원본 크기 유지
            // - 앵커가 양쪽에 있을 경우 앵커와 오프셋을 기준으로 크기 설정
            Vector2 anchorBoxSize = (rectTransform.anchorMax - rectTransform.anchorMin);
            anchorBoxSize.x *= canvasRectTransform.rect.width;
            anchorBoxSize.y *= canvasRectTransform.rect.height;
            
            // 앵커 분석
            if (Mathf.Approximately(rectTransform.anchorMin.x, 0f))
                horAnchorFlag |= AnchorFlag.Zero;
            if (Mathf.Approximately(rectTransform.anchorMax.x, 1f))
                horAnchorFlag |= AnchorFlag.One;
            if (Mathf.Approximately(rectTransform.anchorMin.y, 0f))
                verAnchorFlag |= AnchorFlag.Zero;
            if (Mathf.Approximately(rectTransform.anchorMax.y, 1f))
                verAnchorFlag |= AnchorFlag.One;

            Vector2 targetSize = originalSize; 
            if (horAnchorFlag.HasFlag(AnchorFlag.Zero) && horAnchorFlag.HasFlag(AnchorFlag.One))
            {
                targetSize.x = canvasRectTransform.rect.width - maxMargin.left - maxMargin.right;
            }
            if (verAnchorFlag.HasFlag(AnchorFlag.Zero) && verAnchorFlag.HasFlag(AnchorFlag.One))
            {
                targetSize.y = canvasRectTransform.rect.height - maxMargin.bottom - maxMargin.top;
            }

            // 타겟 사이즈 적용
            rectTransform.sizeDelta = targetSize - anchorBoxSize;
        }
        
        public void Setup()
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();

            if (canvasRectTransform == null)
                canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            
            if (mainCamera == null)
                mainCamera = Camera.main;
        }
    }
}