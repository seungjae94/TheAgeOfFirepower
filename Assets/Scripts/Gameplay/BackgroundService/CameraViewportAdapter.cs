using System;
using Mathlife.ProjectL.Gameplay.UI;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class CameraViewportAdapter : MonoBehaviour
    {
        private const float k_minScreenAspect = 16f / 9f;
        private const float k_maxScreenAspect = 21f / 9f;

        private Camera mainCamera;
        private float prevScreenAspect = -1f;

        [SerializeField] 

        #region Unity Event Functions

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void Start()
        {
            Adapt();
        }

        private void Update()
        {
            float screenAspect = (float)Screen.width / Screen.height;

            if (Mathf.Approximately(prevScreenAspect, screenAspect))
                return;

            Adapt();
        }

        #endregion

        private void Adapt()
        {
            float screenAspect = (float)Screen.width / Screen.height;

            //string deviceInfo = $"[CameraViewportAdapter]\n" +
            //                    $"해상도: {Screen.width}x{Screen.height}, 가로세로비: {screenAspect}\n" +
            //                    $"안전 영역: {Screen.safeArea.width}x{Screen.safeArea.height}";
            //Debug.Log(deviceInfo);

            if (screenAspect < k_minScreenAspect)
                AdaptWithLetterBox(screenAspect);
            else if (screenAspect > k_maxScreenAspect)
                AdaptWithPillarBox(screenAspect);
            else
                AdaptToFullScreen(screenAspect);

            // 스크린 aspect 저장
            prevScreenAspect = screenAspect;

            // Safe Area 적용
            ApplySafeArea();
        }

        // 스크린 aspect가 16:9 미만일 경우, 위 아래에 레터 박스를 그려서 카메라 aspect를 16:9로 고정한다.
        private void AdaptWithLetterBox(float screenAspect)
        {
            mainCamera.aspect = k_minScreenAspect;

            float rectHeight = screenAspect / k_minScreenAspect;
            float rectY = (1f - rectHeight) / 2f;
            mainCamera.rect = new Rect(0f, rectY, 1f, rectHeight);
        }

        // 스크린 aspect가 16:9 이상 21:9 이하일 경우, 스크린 aspect를 카메라 aspect로 사용한다. 
        private void AdaptToFullScreen(float screenAspect)
        {
            mainCamera.aspect = screenAspect;
            mainCamera.rect = new Rect(0, 0, 1, 1);
        }

        // 스크린 aspect가 21:9를 초과할 경우, 양 옆에 필러 박스를 그려서 카메라 aspect를 21:9로 고정한다.
        private void AdaptWithPillarBox(float screenAspect)
        {
            mainCamera.aspect = k_maxScreenAspect;

            float rectWidth = k_maxScreenAspect / screenAspect;
            float rectX = (1f - rectWidth) / 2f;
            mainCamera.rect = new Rect(rectX, 0f, rectWidth, 1f);
        }

        public void ApplySafeArea()
        {
            SafeAreaApplier[] safeMargins = GameObject.FindObjectsByType<SafeAreaApplier>(FindObjectsSortMode.None);
            
            foreach (var safeMargin in safeMargins)
            {
                safeMargin.Apply();
            }
        }
    }
}