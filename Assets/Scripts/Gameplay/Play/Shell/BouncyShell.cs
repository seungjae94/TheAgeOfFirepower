using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    // 고탄성탄
    public class BouncyShell : ShellBase
    {
        private const int BOUNCE_COUNT = 1;
        private const float BOUNCINESS = 0.6f;

        // Field
        private int touchCount = 0;
        private Collider2D collider;
        private Vector2 velocity;

        // Method
        public override void Init(ArtyController firer)
        {
            base.Init(firer);
            collider = GetComponent<Collider2D>();
        }

        // Event Func
        private void FixedUpdate()
        {
            // Capture velocity
            velocity = rgbShellBody.linearVelocity;
        }

        private void LateUpdate()
        {
            if (ShouldBeDestroyed || touchCount == BOUNCE_COUNT + 1)
            {
                return;
            }

            if (false == DestructibleTerrain.Inst.InFairArea(transform.position))
            {
                ShouldBeDestroyed = true;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (touchCount == BOUNCE_COUNT + 1 || collider.enabled == false)
                return;

            // 지형과 충돌할 때만 처리
            if (false == IsCollisionWithTerrain(other))
            {
                return;
            }

            touchCount++;
            if (touchCount == BOUNCE_COUNT + 1)
            {
                OnFinalTouch(other);
                return;
            }

            // 바운스
            Vector2 contactPoint = GetContantPoint(other);
            DestructibleTerrain.Inst.SnapToSurface(contactPoint, velocity.normalized, out contactPoint);
            bool extract =
                DestructibleTerrain.Inst.ExtractNormalTangent(contactPoint, out Vector2 normal, out Vector2 tangent);

            if (false == extract)
            {
                Debug.LogError($"contact at {contactPoint * 100f}, Failed to extract normal.");
            }

            Vector2 afterVelocity = Vector2.Reflect(velocity, normal);
            rgbShellBody.linearVelocity = BOUNCINESS * afterVelocity;

            DebugDrawCollision(contactPoint, normal, afterVelocity);

            // 이번 프레임 동안 콜리전 무시
            StartCoroutine(DisableColliderForFrame());
        }

        private IEnumerator DisableColliderForFrame()
        {
            collider.enabled = false;

            yield return new WaitForSeconds(0.05f);
            //yield return new WaitForEndOfFrame();

            collider.enabled = true;
        }

        private void OnFinalTouch(Collision2D other)
        {
            HideBody();
            partSysExplosion.Play();
            DestructTerrain(other);
            DamageBattlersInRange(other);

            WaitForExplosionParticleSystem()
                .ContinueWith(() => ShouldBeDestroyed = true)
                .Forget();
        }

        private void DebugDrawCollision(Vector2 contactPoint, Vector2 normal, Vector2 afterVelocity)
        {
#if UNITY_EDITOR
            float radius = 0.1f;
            Debug.DrawLine(contactPoint, contactPoint + Vector2.left * radius, Color.white, 1000f);
            Debug.DrawLine(contactPoint, contactPoint + Vector2.right * radius, Color.white, 1000f);
            Debug.DrawLine(contactPoint, contactPoint + Vector2.up * radius, Color.white, 1000f);
            Debug.DrawLine(contactPoint, contactPoint + Vector2.down * radius, Color.white, 1000f);

            Debug.DrawLine(contactPoint, contactPoint + normal, Color.green, 1000f);
            Debug.DrawLine(contactPoint, contactPoint + velocity, Color.magenta, 1000f);
            Debug.DrawLine(contactPoint, contactPoint + afterVelocity, Color.blue, 1000f);
#endif
        }
    }
}