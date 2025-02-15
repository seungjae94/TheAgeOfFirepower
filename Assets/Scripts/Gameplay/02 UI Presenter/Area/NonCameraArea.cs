using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public enum ECameraAreaDirection
    {
        Left, Right, Top, Bottom
    }

    public class NonCameraArea : MonoBehaviour, IUIArea
    {
        [SerializeField] ECameraAreaDirection m_direction;

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
                Debug.LogWarning("[NonCameraArea] CanvasUpdateRegistry.RebuildingGraphics is true. Stop applying ApplyArea.");
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
            Rect screenArea = new Rect(
                0.0f,
                0.0f,
                resolution.width / scaleFactor,
                resolution.height / scaleFactor
            );

            Rect cameraArea = new Rect(
                Camera.main.rect.x * resolution.width / scaleFactor,
                Camera.main.rect.y * resolution.height / scaleFactor,
                Camera.main.rect.width * resolution.width / scaleFactor,
                Camera.main.rect.height * resolution.height / scaleFactor
            );

            switch (m_direction)
            {
                case ECameraAreaDirection.Left:
                    ApplyArea_Left(rectTransform, cameraArea, screenArea);
                    break;
                case ECameraAreaDirection.Right:
                    ApplyArea_Right(rectTransform, cameraArea, screenArea);
                    break;
                case ECameraAreaDirection.Top:
                    ApplyArea_Top(rectTransform, cameraArea, screenArea);
                    break;
                case ECameraAreaDirection.Bottom:
                    ApplyArea_Bottom(rectTransform, cameraArea, screenArea);
                    break;
                default:
                    break;
            }
        }

        void ApplyArea_Left(RectTransform trans, Rect cameraArea, Rect screenArea)
        {
            // Set RectTransform of the ui render zone element.
            // Have to change the coordinate system to normalized coordinate.
            trans.anchorMin = new Vector2(0.0f, 0.0f);
            trans.anchorMax = new Vector2(0.0f, 0.0f);
            trans.pivot = new Vector2(0.0f, 0.0f);
            trans.anchoredPosition = new Vector2(0.0f, 0.0f);
            trans.sizeDelta = new Vector2(cameraArea.x, cameraArea.height);
        }

        void ApplyArea_Right(RectTransform trans, Rect cameraArea, Rect screenArea)
        {
            // Set RectTransform of the ui render zone element.
            // Have to change the coordinate system to normalized coordinate.
            trans.anchorMin = new Vector2(0.0f, 0.0f);
            trans.anchorMax = new Vector2(0.0f, 0.0f);
            trans.pivot = new Vector2(0.0f, 0.0f);
            trans.anchoredPosition = new Vector2(cameraArea.xMax, 0.0f);
            trans.sizeDelta = new Vector2(screenArea.width - cameraArea.xMax, cameraArea.height);
        }

        void ApplyArea_Top(RectTransform trans, Rect cameraArea, Rect screenArea)
        {
            // Set RectTransform of the ui render zone element.
            // Have to change the coordinate system to normalized coordinate.
            trans.anchorMin = new Vector2(0.0f, 0.0f);
            trans.anchorMax = new Vector2(0.0f, 0.0f);
            trans.pivot = new Vector2(0.0f, 0.0f);
            trans.anchoredPosition = new Vector2(0.0f, cameraArea.yMax);
            trans.sizeDelta = new Vector2(cameraArea.width, screenArea.height - cameraArea.yMax);
        }

        void ApplyArea_Bottom(RectTransform trans, Rect cameraArea, Rect screenArea)
        {
            // Set RectTransform of the ui render zone element.
            // Have to change the coordinate system to normalized coordinate.
            trans.anchorMin = new Vector2(0.0f, 0.0f);
            trans.anchorMax = new Vector2(0.0f, 0.0f);
            trans.pivot = new Vector2(0.0f, 0.0f);
            trans.anchoredPosition = new Vector2(0.0f, 0.0f);
            trans.sizeDelta = new Vector2(cameraArea.width, cameraArea.y);
        }
    }
}