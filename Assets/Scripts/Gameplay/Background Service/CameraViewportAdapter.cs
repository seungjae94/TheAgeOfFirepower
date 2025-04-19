using System;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.UI;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class CameraViewportAdapter : MonoBehaviour
    {
        private const float MIN_SCREEN_ASPECT = 16f / 9f;
        private const float MAX_SCREEN_ASPECT = 21f / 9f;

        [SerializeField]
        private Canvas targetCanvas;

        private Camera targetCamera;
        private float prevScreenAspect = -1f;

        private void Awake()
        {
            targetCamera = GetComponent<Camera>();
        }

        private void Start()
        {
            Adapt((float)Display.main.systemWidth / Display.main.systemHeight).Forget();
        }

        private void Update()
        {
            float screenAspect = (float)Display.main.systemWidth / Display.main.systemHeight;

            if (Mathf.Approximately(prevScreenAspect, screenAspect))
                return;
            
            Adapt((float)Screen.width / Screen.height).Forget();
        }

        public async UniTaskVoid Adapt(float screenAspect)
        {
            if (screenAspect < MIN_SCREEN_ASPECT)
                AdaptWithLetterBox(screenAspect);
            else if (screenAspect > MAX_SCREEN_ASPECT)
                AdaptWithPillarBox(screenAspect);
            else
                AdaptToFullScreen(screenAspect);

            // 스크린 aspect 저장
            prevScreenAspect = screenAspect;

            //await UniTask.WaitForEndOfFrame(this);
            await UniTask.NextFrame();
            
            // Safe Area 적용
            if (targetCamera.CompareTag("MainCamera"))
            {
                ApplySafeArea();
            }
        }

        // 스크린 aspect가 16:9 미만일 경우, 위 아래에 레터 박스를 그려서 카메라 aspect를 16:9로 고정한다.
        private void AdaptWithLetterBox(float screenAspect)
        {
            targetCamera.aspect = MIN_SCREEN_ASPECT;

            float rectHeight = screenAspect / MIN_SCREEN_ASPECT;
            float rectY = (1f - rectHeight) / 2f;
            targetCamera.rect = new Rect(0f, rectY, 1f, rectHeight);
        }

        // 스크린 aspect가 16:9 이상 21:9 이하일 경우, 스크린 aspect를 카메라 aspect로 사용한다. 
        private void AdaptToFullScreen(float screenAspect)
        {
            targetCamera.aspect = screenAspect;
            targetCamera.rect = new Rect(0, 0, 1, 1);
        }

        // 스크린 aspect가 21:9를 초과할 경우, 양 옆에 필러 박스를 그려서 카메라 aspect를 21:9로 고정한다.
        private void AdaptWithPillarBox(float screenAspect)
        {
            targetCamera.aspect = MAX_SCREEN_ASPECT;

            float rectWidth = MAX_SCREEN_ASPECT / screenAspect;
            float rectX = (1f - rectWidth) / 2f;
            targetCamera.rect = new Rect(rectX, 0f, rectWidth, 1f);
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
        
        public void Setup()
        {
            targetCamera = GetComponent<Camera>();
        }
    }
}