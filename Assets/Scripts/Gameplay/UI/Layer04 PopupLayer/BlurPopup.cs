// Inspired by https://gist.github.com/jimfleming/0f1ecf5ac4a5d6f6a9a8

using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class BlurPopup : PopupPresenter
    {
        private static readonly int s_blurSizeId = Shader.PropertyToID("_BlurSize");

#if UNITY_EDITOR
        [SerializeField]
        private bool saveRenderTextureAsFile = false;
#endif

        [SerializeField]
        private RawImage outputImage;

        [SerializeField]
        private Color tint = Color.white;

        [SerializeField]
        private float blurSize = 2.0f;

        [SerializeField]
        private int passes = 4;

        private Material blurMaterial;
        private RenderTexture resultRenderTexture;

        private enum ECoroutineState
        {
            Ready,
            Working,
            Finished
        }

        private ECoroutineState coroutineState = ECoroutineState.Ready;
        private readonly WaitForEndOfFrame waitForEndOfFrame = new();

        public override void OnSceneInitialize()
        {
            base.OnSceneInitialize();

            outputImage.material = new Material(Shader.Find("UI/Default"));
            outputImage.color = Color.clear;

            Shader shader = Shader.Find("Hidden/Blur");
            blurMaterial = new Material(shader);
            blurMaterial.SetFloat(s_blurSizeId, blurSize);
            
            int width = (int)(Screen.width * Cameras.Inst.MainCamera.rect.width);
            int height = (int)(Screen.height * Cameras.Inst.MainCamera.rect.height);
            resultRenderTexture = new RenderTexture(width, height, 0);

            DisplayManager.displayChanged
                .DistinctUntilChanged()
                .Subscribe(OnDisplayChanged)
                .AddTo(gameObject);
        }

        private void OnDisplayChanged(int optionIndex)
        {
            int width = (int)(Screen.width * Cameras.Inst.MainCamera.rect.width);
            int height = (int)(Screen.height * Cameras.Inst.MainCamera.rect.height);
            resultRenderTexture = new RenderTexture(width, height, 0);
        }

        public override async UniTask OpenWithAnimation()
        {
            await base.OpenWithAnimation();

            coroutineState = ECoroutineState.Working;
            StartCoroutine(GetBlurredScreenTexture());
            await UniTask.WaitUntil(this, This => This.coroutineState == ECoroutineState.Finished);
            StopCoroutine(GetBlurredScreenTexture());
        }

        public override async UniTask CloseWithAnimation()
        {
            await base.CloseWithAnimation();
            outputImage.color = Color.clear;
        }

        private IEnumerator GetBlurredScreenTexture()
        {
            RenderTexture activeRenderTexture = RenderTexture.active;

            yield return waitForEndOfFrame;

            // 스샷은 EOF에만 찍을 수 있다.
            ScreenCapture.CaptureScreenshotIntoRenderTexture(resultRenderTexture);

            RenderTexture tempA = RenderTexture.GetTemporary(resultRenderTexture.width, resultRenderTexture.height);
            RenderTexture tempB = RenderTexture.GetTemporary(resultRenderTexture.width, resultRenderTexture.height);

            Graphics.Blit(resultRenderTexture, tempA, blurMaterial, 0);
            Graphics.Blit(tempA, tempB, blurMaterial, 1);

            for (int i = 1; i < passes; i++)
            {
                Graphics.Blit(tempB, tempA, blurMaterial, 0);
                Graphics.Blit(tempA, tempB, blurMaterial, 1);
            }

            // Save To RT
            if (SystemInfo.graphicsUVStartsAtTop)
                Graphics.Blit(tempB, resultRenderTexture, new Vector2(1, -1), new Vector2(0, 1));
            else
                Graphics.Blit(tempB, resultRenderTexture);

#if UNITY_EDITOR
            if (saveRenderTextureAsFile)
                SaveRenderTexture(resultRenderTexture, "BlurredScreen");
#endif

            outputImage.texture = resultRenderTexture;
            outputImage.color = tint;
            outputImage.SetAllDirty();

            RenderTexture.ReleaseTemporary(tempA);
            RenderTexture.ReleaseTemporary(tempB);

            RenderTexture.active = activeRenderTexture;

            coroutineState = ECoroutineState.Finished;
        }

#if UNITY_EDITOR
        // 렌더 텍스처 저장
        private void SaveRenderTexture(RenderTexture rt, string fileName)
        {
            Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();
            SaveTexture2D(tex, fileName);
            RenderTexture.active = null;
            Destroy(tex);
        }

        // 텍스처 2D 저장
        private void SaveTexture2D(Texture2D tex, string fileName)
        {
            byte[] bytes = tex.EncodeToPNG();
            string path = $"{Application.dataPath}/Debug/{fileName}.png";
            System.IO.File.WriteAllBytes(path, bytes);
            MyDebug.Log($"[Debug] Texture saved to {path}");
        }
#endif

        public override void OnSceneClear()
        {
            if (resultRenderTexture)
            {
                Destroy(resultRenderTexture);
                resultRenderTexture = null;
            }

            if (blurMaterial)
            {
                Destroy(blurMaterial);
                blurMaterial = null;
            }

            if (outputImage.material)
            {
                Destroy(outputImage.material);
            }
        }
    }
}