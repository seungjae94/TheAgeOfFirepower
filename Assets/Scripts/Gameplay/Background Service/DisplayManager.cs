using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using Mathlife.ProjectL.Gameplay.UI;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public struct ResolutionOption
    {
        public readonly int width;
        public readonly int height;
        public readonly bool fullScreen;

        public ResolutionOption(int width, int height, bool fullScreen)
        {
            this.width = width;
            this.height = height;
            this.fullScreen = fullScreen;
        }

        public override string ToString()
        {
            return $"(resolution = {width} x {height}, fullScreen = {fullScreen})";
        }
    }

    public class DisplayManager : MonoSingleton<DisplayManager>
    {
        // 상수
        public static readonly ResolutionOption[] resolutionOptions = new ResolutionOption[3]
        {
            new ResolutionOption(0, 0, true),
            new ResolutionOption(1920, 1080, false),
            new ResolutionOption(1280, 720, false),
        };

        private const float MIN_SCREEN_ASPECT = 16f / 9f;
        private const float MAX_SCREEN_ASPECT = 21f / 9f;

        // 필드
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.App;
        private Camera mainCamera;

        [SerializeField]
        private Camera clearCamera;

        [SerializeField]
        private Camera loadingScreenCamera;

        public static readonly Subject<int> displayChanged = new();

        public async UniTask Adapt()
        {
            clearCamera.gameObject.SetActive(true);

            mainCamera = Camera.main;

            // 모바일 vs PC
            int resolutionOptionIndex = GameState.Inst.gameSettingState.resolutionOptionIndex.Value;
            if (Application.isMobilePlatform)
            {
                // 해상도 옵션을 무시하고 높이를 1080으로 고정
                float deviceAspect = (float)Display.main.systemWidth / Display.main.systemHeight;
                Screen.SetResolution(Mathf.RoundToInt(deviceAspect * Constants.VIEWPORT_HEIGHT),
                    Constants.VIEWPORT_HEIGHT, FullScreenMode.ExclusiveFullScreen);
            }
            else
            {
                ResolutionOption resolutionOption = resolutionOptions[resolutionOptionIndex];

                if (resolutionOption.fullScreen)
                {
                    Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight,
                        FullScreenMode.ExclusiveFullScreen);
                }
                else
                {
                    Screen.SetResolution(resolutionOption.width, resolutionOption.height, FullScreenMode.Windowed);
                }
            }

            float screenAspect = (float)Screen.width / (float)Screen.height;
            switch (screenAspect)
            {
                case < MIN_SCREEN_ASPECT:
                    AdaptWithLetterBox(screenAspect);
                    break;
                case > MAX_SCREEN_ASPECT:
                    AdaptWithPillarBox(screenAspect);
                    break;
                default:
                    AdaptToFullScreen(screenAspect);
                    break;
            }

            await UniTask.NextFrame();

            int viewportWidth = Mathf.RoundToInt(Screen.width * mainCamera.rect.width);
            int viewportHeight = Mathf.RoundToInt(Screen.height * mainCamera.rect.height);

            Debug.Log($"Viewport = {viewportWidth}x{viewportHeight}");

            var mainCanvas = (MonoBehaviour)GameManager.Inst.CurrentCanvas;
            CanvasScaler canvasScaler = mainCanvas.GetComponent<CanvasScaler>();
            canvasScaler.referenceResolution = new Vector2((float)Constants.VIEWPORT_HEIGHT * viewportWidth / viewportHeight,
                Constants.VIEWPORT_HEIGHT);
            canvasScaler.scaleFactor = 1f;
            //mainCanvas.GetComponent<Canvas>().scaleFactor = 1f;

            await UniTask.NextFrame();

            // Safe Area 적용
            ApplySafeArea();

            displayChanged.OnNext(resolutionOptionIndex);
            clearCamera.gameObject.SetActive(false);
        }

        // 스크린 aspect가 16:9 미만일 경우, 위 아래에 레터 박스를 그려서 카메라 aspect를 16:9로 고정한다.
        private void AdaptWithLetterBox(float screenAspect)
        {
            mainCamera.aspect = MIN_SCREEN_ASPECT;
            loadingScreenCamera.aspect = MIN_SCREEN_ASPECT;

            float rectHeight = screenAspect / MIN_SCREEN_ASPECT;
            float rectY = (1f - rectHeight) / 2f;
            mainCamera.rect = new Rect(0f, rectY, 1f, rectHeight);
            loadingScreenCamera.rect = new Rect(0f, rectY, 1f, rectHeight);

            int viewportWidth = Screen.width;
            int viewportHeight = Mathf.RoundToInt(Screen.height * rectHeight);

            if (mainCamera.TryGetComponent<PixelPerfectCamera>(out var pixelPerfectCamera))
            {
                pixelPerfectCamera.cropFrame = PixelPerfectCamera.CropFrame.Letterbox;
                pixelPerfectCamera.refResolutionX = viewportWidth;
                pixelPerfectCamera.refResolutionY = viewportHeight;
                pixelPerfectCamera.assetsPPU = viewportHeight / 10;
            }
        }

        // 스크린 aspect가 16:9 이상 21:9 이하일 경우, 스크린 aspect를 카메라 aspect로 사용한다. 
        private void AdaptToFullScreen(float screenAspect)
        {
            mainCamera.aspect = screenAspect;
            loadingScreenCamera.aspect = screenAspect;
            mainCamera.rect = new Rect(0, 0, 1, 1);
            loadingScreenCamera.rect = new Rect(0, 0, 1, 1);

            int viewportWidth = Screen.width;
            int viewportHeight = Screen.height;

            if (mainCamera.TryGetComponent<PixelPerfectCamera>(out var pixelPerfectCamera))
            {
                pixelPerfectCamera.cropFrame = PixelPerfectCamera.CropFrame.None;
                pixelPerfectCamera.refResolutionX = viewportWidth;
                pixelPerfectCamera.refResolutionY = viewportHeight;
                pixelPerfectCamera.assetsPPU = viewportHeight / 10;
            }
        }

        // 스크린 aspect가 21:9를 초과할 경우, 양 옆에 필러 박스를 그려서 카메라 aspect를 21:9로 고정한다.
        private void AdaptWithPillarBox(float screenAspect)
        {
            mainCamera.aspect = MAX_SCREEN_ASPECT;
            loadingScreenCamera.aspect = MAX_SCREEN_ASPECT;

            float rectWidth = MAX_SCREEN_ASPECT / screenAspect;
            float rectX = (1f - rectWidth) / 2f;
            mainCamera.rect = new Rect(rectX, 0f, rectWidth, 1f);
            loadingScreenCamera.rect = new Rect(rectX, 0f, rectWidth, 1f);

            int viewportWidth = Mathf.RoundToInt(Screen.width * rectWidth);
            int viewportHeight = Screen.height;

            if (mainCamera.TryGetComponent<PixelPerfectCamera>(out var pixelPerfectCamera))
            {
                pixelPerfectCamera.cropFrame = PixelPerfectCamera.CropFrame.Pillarbox;
                pixelPerfectCamera.refResolutionX = viewportWidth;
                pixelPerfectCamera.refResolutionY = viewportHeight;
                pixelPerfectCamera.assetsPPU = viewportHeight / 10;
            }
        }

        private void ApplySafeArea()
        {
            SafeAreaApplier[] safeMargins =
                GameObject.FindObjectsByType<SafeAreaApplier>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var safeMargin in safeMargins)
            {
                safeMargin.Setup();
                safeMargin.Apply();
            }
        }
    }
}