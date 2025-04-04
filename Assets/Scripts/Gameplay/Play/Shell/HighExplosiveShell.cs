using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class HighExplosiveShell : MonoBehaviour, IShell
    {
        private static int terrainLayerIndex = -1;
        private static int battlerLayer = -1;
        
        private ShellGameData shellGameData;

        private SpriteRenderer spriteRenderer;
        
        private Rigidbody2D rigidbody2D;
        
        private ParticleSystem particleSystem;

        // Field
        private bool toBeDestroyed = false;
        
        public void Init(ShellGameData shellGameData)
        {
            this.shellGameData = shellGameData;
            
            if (terrainLayerIndex < 0)
                terrainLayerIndex = LayerMask.NameToLayer("Terrain");
            
            if (battlerLayer < 0)
                battlerLayer = LayerMask.GetMask("Battler");
        }

        public void Fire(Vector2 velocity)
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (rigidbody2D == null)
                rigidbody2D = GetComponent<Rigidbody2D>();
            
            if (particleSystem == null)
                particleSystem = GetComponentInChildren<ParticleSystem>();
            
            rigidbody2D.linearVelocity = velocity;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (toBeDestroyed)
                return;
            
            if (other.gameObject.layer != terrainLayerIndex)
            {
                return;
            }

            Vector2 contactPoint = other.contacts[0].point;
            DestructibleTerrain.Inst.Paint(contactPoint, Shape.Circle((int)shellGameData.explosionRadius), Color.clear);

            toBeDestroyed = true;
            spriteRenderer.enabled = false;
            rigidbody2D.simulated = false;
            particleSystem.Play();

            float radius = shellGameData.explosionRadius / 64f;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(contactPoint, radius, battlerLayer);

            foreach (var collider in colliders)
            {
                collider.GetComponent<ArtyController>()?.Damage(10);
            }
            
            DestroyOnParticleDead().Forget();
        }

        private async UniTaskVoid DestroyOnParticleDead()
        {
            await UniTask.WaitWhile(particleSystem.IsAlive);
            
            Destroy(gameObject);
        }
        
        
    }
}