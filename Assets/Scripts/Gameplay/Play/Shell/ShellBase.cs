using Cysharp.Threading.Tasks;
using UnityEngine;

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
        protected ArtyModel firer;
        protected ShellGameData shellGameData;
        
        // Override
        public bool ShouldBeDestroyed { get; protected set; }
        
        public virtual void Init(ArtyModel firer, ShellGameData shellGameData)
        {
            this.firer = firer;
            this.shellGameData = shellGameData;

            if (terrainLayerIndex == -1)
            {
                terrainLayerIndex = LayerMask.NameToLayer("Terrain");
            }

            if (battlerLayer == -1)
            {
                battlerLayer = LayerMask.GetMask("Battler");
            }
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
            Vector2 contactPoint = GetContantPoint(collision);
            DestructibleTerrain.Inst.Paint(contactPoint, Shape.Circle((int)shellGameData.explosionRadius), Color.clear);
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
                collider.transform.root.GetComponent<ArtyController>()?.Damage(firer.GetAtk() * shellGameData.damage / 100f);
            }
        }
        
        protected async UniTask WaitForExplosionParticleSystem()
        {
            await UniTask.WaitWhile(partSysExplosion.IsAlive);
        }
    }
}