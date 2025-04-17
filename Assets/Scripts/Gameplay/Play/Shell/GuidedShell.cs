using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    // 유도탄
    public class GuidedShell : ShellBase
    {
        private const float GUIDE_RANGE = 2.5f;
        private const float MAX_GUIDED_SPEED = 10f;
        private const float INITIAL_GUIDED_SPEED = 5f;
        private const float ACC_DURATION = 0.8f;

        [SerializeField]
        private GameObject guideMarkerPrefab;

        [SerializeField]
        private AudioClip detectionSound;
        
        // Field
        private Vector2 onDetectDirection;
        
        private bool targetFound = false;
        private Transform target = null;
        private GameObject marker = null;
        private bool firstTouch = false;
        private float accTimer = 0f;

        private AudioSource borrowedAudioSource;

        // Event Func
        private void Update()
        {
            if (ShouldBeDestroyed)
            {
                return;
            }

            if (false == DestructibleTerrain.Inst.InFairArea(transform.position))
            {
                ShouldBeDestroyed = true;
                Destroy(marker);
                return;
            }

            if (targetFound)
            {
                accTimer += Time.deltaTime;
                float speed = Mathf.Lerp(INITIAL_GUIDED_SPEED, MAX_GUIDED_SPEED, accTimer / ACC_DURATION);
                
                Vector2 targetDirection = (target.position - transform.position).normalized;
                Vector2 finalDirection = Vector2.Lerp(onDetectDirection, targetDirection, accTimer / ACC_DURATION);
                
                rgbShellBody.linearVelocity = finalDirection * speed;
                return;
            }

            Collider2D battlerCollider = Physics2D.OverlapCircle(transform.position, GUIDE_RANGE, battlerLayer);

            if (battlerCollider == null)
                return;

            ArtyController detected = battlerCollider.GetComponentInParent<ArtyController>();
            if (detected.IsPlayer == firer.IsPlayer)
            {
                return;
            }

            // 감지 성공
            borrowedAudioSource = AudioManager.Inst.BorrowAudioSource();
            borrowedAudioSource.PlayOneShot(detectionSound);
            
            targetFound = true;
            target = battlerCollider.transform;
            rgbShellBody.gravityScale = 0f;
            onDetectDirection = rgbShellBody.linearVelocity.normalized;

            marker = Instantiate(guideMarkerPrefab);
            marker.transform.position = battlerCollider.bounds.center;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (firstTouch)
                return;

            // 지형과 충돌할 때만 처리
            if (false == IsCollisionWithTerrain(other))
            {
                return;
            }

            AudioManager.Inst.ReturnAudioSource(borrowedAudioSource);
            
            firstTouch = true;
            HideBody();
            partSysExplosion.Play();
            DestructTerrain(other);
            DamageBattlersInRange(other);
            Destroy(marker);

            WaitForExplosionParticleSystem()
                .ContinueWith(() => ShouldBeDestroyed = true)
                .Forget();
        }

        private void OnDestroy()
        {
            if (marker)
                Destroy(marker);
        }
    }
}