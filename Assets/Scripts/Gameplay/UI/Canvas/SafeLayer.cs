using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public class SafeLayer : MonoBehaviour
    {
        private Canvas canvas;
        
        void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
        }
        
        void OnEnable()
        {
            ApplyArea();
        }

        public void ApplyArea()
        {
            if (CanvasUpdateRegistry.IsRebuildingGraphics())
            {
                Debug.LogWarning("[SafeLayer] CanvasUpdateRegistry.RebuildingGraphics is true. Stop applying ApplyArea.");
                return;
            }

            RenderMode renderMode = canvas.renderMode;
            bool isScreenSpaceCamera = (renderMode == RenderMode.ScreenSpaceCamera);

            if (isScreenSpaceCamera)
                ApplyArea_ScreenSpaceCamera();
            else
                ApplyArea_ScreenSpaceOverlay();
        }

        private void ApplyArea_ScreenSpaceCamera()
        {
            if (CanvasUpdateRegistry.IsRebuildingGraphics())
            {
                Debug.LogWarning("[SafeArea] CanvasUpdateRegistry.RebuildingGraphics is true. Stop applying ApplyArea.");
                return;
            }

            RectTransform rectTransform = GetComponent<RectTransform>();
            CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();

            Resolution resolution = Screen.currentResolution;

            if (resolution.width <= 0 || resolution.height <= 0)
                return;

            Rect safeArea = Screen.safeArea;

            if (safeArea.width <= 0 || safeArea.height <= 0)
                return;

            Rect cameraRect = new Rect(
                Camera.main.rect.x * resolution.width,
                Camera.main.rect.y * resolution.height,
                Camera.main.rect.width * resolution.width,
                Camera.main.rect.height * resolution.height
            );

            Vector2 canvasRefResolution = canvasScaler.referenceResolution;
            float xScaleFactor = canvasRefResolution.x / cameraRect.width;  // 카메라를 캔버스 크기만큼 확장하려면 x 방향으로 몇 배 늘려야 하는가?
            float yScaleFactor = canvasRefResolution.y / cameraRect.height; // 카메라를 캔버스 높이만큼 확장하려면 y 방향으로 몇 배 늘려야 하는가?
            float scaleFactor = Mathf.Max(xScaleFactor, yScaleFactor);      // ScreenMatchMode가 Expand이므로 캔버스를 최대한 확장한다.

            Rect renderZone = new Rect();
            renderZone.x = Mathf.Max(safeArea.x - cameraRect.x, 0) * scaleFactor;
            renderZone.y = Mathf.Max(safeArea.y - cameraRect.y, 0) * scaleFactor;
            renderZone.xMax = (Mathf.Min(cameraRect.xMax, safeArea.xMax) - cameraRect.x) * scaleFactor;
            renderZone.yMax = (Mathf.Min(cameraRect.yMax, safeArea.yMax) - cameraRect.y) * scaleFactor;

            // Set RectTransform of the ui render zone element.
            // Have to change the coordinate system to normalized coordinate.
            rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
            rectTransform.anchorMax = new Vector2(0.0f, 0.0f);
            rectTransform.pivot = new Vector2(0.0f, 0.0f);
            rectTransform.sizeDelta = renderZone.max - renderZone.min;
            rectTransform.anchoredPosition = new Vector2(renderZone.x, renderZone.y);
        }

        private void ApplyArea_ScreenSpaceOverlay()
        {
            if (CanvasUpdateRegistry.IsRebuildingGraphics())
            {
                Debug.LogWarning("[SafeArea] CanvasUpdateRegistry.RebuildingGraphics is true. Stop applying ApplyArea.");
                return;
            }

            RectTransform rectTransform = GetComponent<RectTransform>();
            CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();

            Resolution resolution = Screen.currentResolution;
            Rect safeArea = Screen.safeArea;

            Vector2 canvasRefResolution = canvasScaler.referenceResolution;
            float scaleFactor = Mathf.Min(resolution.width / canvasRefResolution.x, resolution.height / canvasRefResolution.y);

            // Calculate rect of safe area w.r.t. scaled screen coordinate.
            Rect scaledSafeArea = new Rect(
                safeArea.x / scaleFactor,
                safeArea.y / scaleFactor,
                safeArea.width / scaleFactor,
                safeArea.height / scaleFactor
            );

            // Calculate rect of viewport w.r.t. scaled screen coordinate.
            Rect scaledCameraArea = new Rect(
                Camera.main.rect.x * resolution.width / scaleFactor,
                Camera.main.rect.y * resolution.height / scaleFactor,
                Camera.main.rect.width * resolution.width / scaleFactor,
                Camera.main.rect.height * resolution.height / scaleFactor
            );

            // Calculate rect of ui render zone w.r.t. scaled screen coordinate.
            Rect renderZone = new Rect();
            renderZone.x = Mathf.Max(scaledSafeArea.x - scaledCameraArea.x, 0);
            renderZone.y = Mathf.Max(scaledSafeArea.y - scaledCameraArea.y, 0);

            float screenZoneX = renderZone.x + scaledCameraArea.x;
            float screenZoneY = renderZone.y + scaledCameraArea.y;
            float screenZoneXMax = scaledCameraArea.xMax - Mathf.Max(scaledCameraArea.xMax - scaledSafeArea.xMax, 0);
            float screenZoneYMax = scaledCameraArea.yMax - Mathf.Max(scaledCameraArea.yMax - scaledSafeArea.yMax, 0);
            renderZone.width = screenZoneXMax - screenZoneX;
            renderZone.height = screenZoneYMax - screenZoneY;

            // Set RectTransform of the ui render zone element.
            // Have to change the coordinate system to normalized coordinate.
            rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
            rectTransform.anchorMax = new Vector2(0.0f, 0.0f);
            rectTransform.pivot = new Vector2(0.0f, 0.0f);
            rectTransform.sizeDelta = renderZone.max - renderZone.min;
            rectTransform.anchoredPosition = new Vector2(renderZone.x, renderZone.y);
        }
    }
}