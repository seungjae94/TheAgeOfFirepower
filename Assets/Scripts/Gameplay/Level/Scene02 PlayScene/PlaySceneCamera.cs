using System;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using Mathlife.ProjectL.Gameplay.Play;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class PlaySceneCamera : MonoSingleton<PlaySceneCamera>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Scene;

        private const float TRACK_CAMERA_SPEED = 50f;
        private const float VERTICAL_OFFSET = 0.75f;
        
        // Alias
        private float HalfHeight => camera.orthographicSize;
        private float HalfWidth => camera.aspect * HalfHeight;
        
        // Field
        private new Camera camera;
        private Transform trackingTarget;

        protected override void OnRegistered()
        {
            camera = GetComponent<Camera>();
        }

        public void SetTracking(Transform target)
        {
            trackingTarget = target;
        }

        private void Update()
        {
            // TODO: 드래깅 하면 트래킹 풀리도록 처리
        }
        
        private void LateUpdate()
        {
            if (trackingTarget == null)
                return;
            
            Vector3 trackingPosition = trackingTarget.position + Vector3.up * VERTICAL_OFFSET;
            
            // Clamp
            float targetX = Mathf.Clamp(trackingPosition.x, HalfWidth, DestructibleTerrain.Inst.MapWidth - HalfWidth);
            float targetY = Mathf.Clamp(trackingPosition.y, HalfHeight, DestructibleTerrain.Inst.MapHeight + 10f - HalfWidth);
            Vector3 targetPosition = new Vector3(targetX, targetY, transform.position.z);
            
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * TRACK_CAMERA_SPEED);
        }
    }
}