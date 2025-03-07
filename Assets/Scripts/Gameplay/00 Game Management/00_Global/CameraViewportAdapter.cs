using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class CameraViewportAdapter : MonoBehaviour
    {
        private const float MinScreenAspect = 16f / 9f;
        private const float MaxScreenAspect = 21f / 9f;

        [SerializeField] 
        private Camera targetCamera;
        
        public void AdaptDisplay(int screenWidth, int screenHeight)
        {
            float screenAspect = (float)screenWidth / (float)screenHeight;

            string deviceInfo = $"[CameraViewportAdapter]\n" +
                                $"해상도: {screenWidth}x{screenHeight}, 가로세로비: {screenAspect}\n" +
                                $"안전 영역: {Screen.safeArea.width}x{Screen.safeArea.height}";
            Debug.Log(deviceInfo);

            // 스크린 클리어
            GL.Clear(true, true, Color.blue);
            
            if (screenAspect < MinScreenAspect)
                AdaptWithLetterBox(screenAspect);
            else if (screenAspect > MaxScreenAspect)
                AdaptWithPillarBox(screenAspect);
            else
                AdaptToCurrentScreen(screenAspect);
        }

        // 스크린 aspect가 16:9 미만일 경우, 위 아래에 레터 박스를 그려서 카메라 aspect를 16:9로 고정한다.
        void AdaptWithLetterBox(float screenAspect)
        {
            Debug.Log($"목표 aspect: {MinScreenAspect}, 스크린 aspect: {screenAspect}");
            targetCamera.aspect = MinScreenAspect;

            float rectHeight = screenAspect / MinScreenAspect;
            float rectY = (1f - rectHeight) / 2f;
            targetCamera.rect = new Rect(0f, rectY, 1f, rectHeight);
            Debug.Log($"rect: {targetCamera.rect}");
        }

        // 스크린 aspect가 16:9 이상 21:9 이하일 경우, 스크린 aspect를 카메라 aspect로 사용한다. 
        void AdaptToCurrentScreen(float screenAspect)
        {
            Debug.Log($"목표 aspect: 스크린 aspect");
            targetCamera.aspect = screenAspect;
            targetCamera.rect = new Rect(0, 0, 1, 1);
        }

        // 스크린 aspect가 21:9를 초과할 경우, 양 옆에 필러 박스를 그려서 카메라 aspect를 21:9로 고정한다.
        void AdaptWithPillarBox(float screenAspect)
        {
            Debug.Log($"목표 aspect: 21:9");
            targetCamera.aspect = MaxScreenAspect;

            float rectWidth = MaxScreenAspect / screenAspect;
            float rectX = (1f - rectWidth) / 2f;
            targetCamera.rect = new Rect(rectX, 0f, rectWidth, 1f);
            Debug.Log($"rect: {targetCamera.rect}");
        }

        void Start()
        {
            AdaptDisplay(Screen.width, Screen.height);
        }
    }
}