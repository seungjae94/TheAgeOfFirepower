using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public class CameraArea : MonoBehaviour, IUIArea
    {
        async void OnEnable()
        {
            await UniTask.WaitUntil(() => CanvasUpdateRegistry.IsRebuildingGraphics() == false);
            await UniTask.SwitchToMainThread();
            ApplyArea();
        }

        public void ApplyArea()
        {
            if (CanvasUpdateRegistry.IsRebuildingGraphics())
            {
                Debug.LogWarning("[CameraArea] CanvasUpdateRegistry.RebuildingGraphics is true. Stop applying ApplyArea.");
                return;
            }

            RectTransform rectTransform = GetComponent<RectTransform>();
            CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();

            Resolution resolution = Screen.currentResolution;

            if (resolution.width <= 0 || resolution.height <= 0)
                return;

            Vector2 canvasRefResolution = canvasScaler.referenceResolution;
            float scaleFactor = Mathf.Min(resolution.width / canvasRefResolution.x, resolution.height / canvasRefResolution.y);

            // Calculate rect of viewport w.r.t. scaled screen coordinate.
            Rect renderZone = new Rect(
                Camera.main.rect.x * resolution.width / scaleFactor,
                Camera.main.rect.y * resolution.height / scaleFactor,
                Camera.main.rect.width * resolution.width / scaleFactor,
                Camera.main.rect.height * resolution.height / scaleFactor
            );

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