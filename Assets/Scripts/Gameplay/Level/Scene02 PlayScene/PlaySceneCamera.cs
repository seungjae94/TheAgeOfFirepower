using System;
using Cysharp.Threading.Tasks;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using Mathlife.ProjectL.Gameplay.Play;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay
{
    public class PlaySceneCamera : MonoSingleton<PlaySceneCamera>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Scene;

        private const float TRACK_CAMERA_SPEED = 50f;
        private const float VERTICAL_OFFSET = 0.25f;
        private const float DRAG_SPEED = 1f;

        // Alias
        private float HalfHeight => camera.orthographicSize;
        private float HalfWidth => camera.aspect * HalfHeight;

        // Component
        [SerializeField]
        private Graphic dragInputCaptureGraphic;

        [SerializeField]
        private RectTransform hudTrans;

        // Field
        private new Camera camera;
        private Transform trackingTarget;

        private bool isDragging;
        private Vector3 mousePositionOnDragStart;
        private Vector3 cameraPositionOnDragStart;
        private Vector3 dragOffset;

        private float canvasHeight = 0f;

        protected override void OnRegistered()
        {
            camera = GetComponent<Camera>();

            dragInputCaptureGraphic
                .OnPointerDownAsObservable()
                .Subscribe(OnPointerDown)
                .AddTo(gameObject);

            var canvasTrans = hudTrans.root as RectTransform;
            canvasHeight = canvasTrans?.rect.height ?? 1080;
        }

        public void SetTracking(Transform target)
        {
            trackingTarget = target;
        }

        private void OnPointerDown(PointerEventData ev)
        {
            DragStart();
        }

        private void DragStart()
        {
            isDragging = true;
            mousePositionOnDragStart = Input.mousePosition;
            cameraPositionOnDragStart = transform.position;
            trackingTarget = null;
        }

        private void Update()
        {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            bool mouseButtonUp = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended;
#else
            bool mouseButtonUp = Input.GetMouseButtonUp(0);
#endif

            if (isDragging)
            {
                if (mouseButtonUp)
                {
                    isDragging = false;
                    return;
                }

                //Vector3 dragOffsetRaw = camera.ScreenToViewportPoint(mousePositionOnDragStart - Input.mousePosition);
                Vector3 dragOffsetRaw = camera.ScreenToWorldPoint(mousePositionOnDragStart) -
                                        camera.ScreenToWorldPoint(Input.mousePosition);
                dragOffset = new Vector3(dragOffsetRaw.x * DRAG_SPEED, dragOffsetRaw.y * DRAG_SPEED);
                return;
            }

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            bool mouseButtonDown = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
#else
            bool mouseButtonDown = Input.GetMouseButtonDown(0);
#endif

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            bool isPointerOverGameObject = Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#else
            bool isPointerOverGameObject = EventSystem.current.IsPointerOverGameObject();
#endif
            
            if (mouseButtonDown && isPointerOverGameObject == false)
            {
                DragStart();
            }
        }

        private void LateUpdate()
        {
            if (isDragging)
            {
                MoveTowards(cameraPositionOnDragStart + dragOffset);
                return;
            }

            if (trackingTarget == null)
                return;

            Vector3 trackingPosition = trackingTarget.position + Vector3.up * VERTICAL_OFFSET;
            MoveTowards(trackingPosition);
        }

        private void MoveTowards(Vector3 position)
        {
            float hudPixelHeight = hudTrans.sizeDelta.y;
            float hudHeight = hudPixelHeight / canvasHeight * 2f * HalfHeight;

            // Clamp
            float targetX = Mathf.Clamp(position.x, HalfWidth, DestructibleTerrain.Inst.MapWidth - HalfWidth);
            float targetY = Mathf.Clamp(position.y, HalfHeight - hudHeight,
                DestructibleTerrain.Inst.MapHeight + 10f - HalfWidth);
            Vector3 targetPosition = new Vector3(targetX, targetY, transform.position.z);

            // Move towards
            transform.position =
                Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * TRACK_CAMERA_SPEED);
        }
    }
}