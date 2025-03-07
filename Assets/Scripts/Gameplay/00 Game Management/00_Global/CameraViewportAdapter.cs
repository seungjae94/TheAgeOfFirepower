using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Mathlife.ProjectL.Gameplay
{
    public class CameraViewportAdapter : MonoBehaviour
    {
        private const float MinScreenAspect = 16f / 9f;
        private const float MaxScreenAspect = 21f / 9f;

        private float prevScreenAspect = -1f;
        private bool shouldClearScreen = false;

        [SerializeField] private Camera targetCamera;

        private void OnPreRender()
        {
            targetCamera.clearFlags = CameraClearFlags.Color;
        }

        private void AdaptDisplay()
        {
            float screenAspect = (float)Screen.width / (float)Screen.height;
            
            string deviceInfo = $"[CameraViewportAdapter]\n" +
                                $"해상도: {Screen.width}x{Screen.height}, 가로세로비: {screenAspect}\n" +
                                $"안전 영역: {Screen.safeArea.width}x{Screen.safeArea.height}";
            Debug.Log(deviceInfo);

            if (screenAspect < MinScreenAspect)
                AdaptWithLetterBox(screenAspect);
            else if (screenAspect > MaxScreenAspect)
                AdaptWithPillarBox(screenAspect);
            else
                AdaptToCurrentScreen(screenAspect);

            // 스크린 aspect 저장
            prevScreenAspect = screenAspect;
            
            // 스크린 클리어 예약
            shouldClearScreen = true;
        }

        // 스크린 aspect가 16:9 미만일 경우, 위 아래에 레터 박스를 그려서 카메라 aspect를 16:9로 고정한다.
        void AdaptWithLetterBox(float screenAspect)
        {
            targetCamera.aspect = MinScreenAspect;

            float rectHeight = screenAspect / MinScreenAspect;
            float rectY = (1f - rectHeight) / 2f;
            targetCamera.rect = new Rect(0f, rectY, 1f, rectHeight);
        }

        // 스크린 aspect가 16:9 이상 21:9 이하일 경우, 스크린 aspect를 카메라 aspect로 사용한다. 
        void AdaptToCurrentScreen(float screenAspect)
        {
            targetCamera.aspect = screenAspect;
            targetCamera.rect = new Rect(0, 0, 1, 1);
        }

        // 스크린 aspect가 21:9를 초과할 경우, 양 옆에 필러 박스를 그려서 카메라 aspect를 21:9로 고정한다.
        void AdaptWithPillarBox(float screenAspect)
        {
            targetCamera.aspect = MaxScreenAspect;

            float rectWidth = MaxScreenAspect / screenAspect;
            float rectX = (1f - rectWidth) / 2f;
            targetCamera.rect = new Rect(rectX, 0f, rectWidth, 1f);
        }

        void Start()
        {
            AdaptDisplay();
        }

        void Update()
        {
            float screenAspect = (float)Screen.width / (float)Screen.height;

            if (Mathf.Approximately(prevScreenAspect, screenAspect))
                return;

            AdaptDisplay();
        }
    }
}