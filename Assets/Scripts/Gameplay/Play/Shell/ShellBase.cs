using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public abstract class ShellBase : MonoBehaviour, IShell
    {
        protected static int terrainLayerIndex = -1;
        protected static int battlerLayer = -1;
     
        [SerializeField]
        protected SpriteRenderer spriteRenderer;
        
        [SerializeField]
        protected Rigidbody2D rgbShellBody;
        
        [SerializeField]
        protected ParticleSystem partSysExplosion;
        
        // Field
        protected Vector2 capturedVelocity;
        protected ArtyController firer;
        protected ShellGameData shellGameData;
        
        // Override
        public bool ShouldBeDestroyed { get; protected set; }
        
        public virtual void Init(ArtyController firer)
        {
            this.firer = firer;
            this.shellGameData = firer.Model.Shell;

            if (terrainLayerIndex == -1)
            {
                terrainLayerIndex = LayerMask.NameToLayer("Terrain");
            }

            if (battlerLayer == -1)
            {
                battlerLayer = LayerMask.GetMask("Battler");
            }
        }
        
        // Event
        private void FixedUpdate()
        {
            // Capture velocity
            capturedVelocity = rgbShellBody.linearVelocity;
        }

        public virtual void Fire(Vector2 velocity)
        {
            rgbShellBody.linearVelocity = velocity;
        }
        
        // Util
        protected Vector2 GetContantPoint(Collision2D collision)
        {
            return collision.contacts[0].point;
        }
        
        protected void DestructTerrain(Collision2D collision)
        {
            AudioManager.Inst.PlaySE(ESoundEffectId.Explosion);
            
            Vector2 contactPoint = GetContantPoint(collision);
            Vector2 direction = capturedVelocity.normalized;
            DestructibleTerrain.Inst.SnapToSurface(contactPoint, direction, out Vector2 surfacePosition);
            
            // 땅을 너무 깊게 파면 화포가 빠져나오지 못하는 문제가 발생한다.
            Vector2 circleCenter = surfacePosition - shellGameData.explosionRadius * 0.25f * direction / DestructibleTerrain.Inst.PixelsPerUnit;
            DestructibleTerrain.Inst.Paint(circleCenter, Shape.Circle((int)shellGameData.explosionRadius), Color.clear);
        }

        protected bool IsCollisionWithTerrain(Collision2D collision)
        {
            return collision.gameObject.layer == terrainLayerIndex;
        }

        protected void HideBody()
        {
            spriteRenderer.enabled = false;
            rgbShellBody.simulated = false;
        }

        protected void DamageBattlersInRange(Collision2D collision)
        {
            float radius = shellGameData.explosionRadius / DestructibleTerrain.Inst.PixelsPerUnit;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(GetContantPoint(collision), radius, battlerLayer);

            foreach (var collider in colliders)
            {
                collider.transform.root.GetComponent<ArtyController>()?.Damage(CalculateDamage());
            }
        }

        protected virtual float CalculateDamage()
        {
            return firer.Model.GetAtk() * shellGameData.damage / 100f;
        }
        
        protected async UniTask WaitForExplosionParticleSystem()
        {
            await UniTask.WaitWhile(partSysExplosion.IsAlive);
        }
    }
}