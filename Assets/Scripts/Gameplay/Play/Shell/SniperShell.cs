using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    // 저격탄
    public class SniperShell : ShellBase
    {
        private const float DAMAGE_VARIANT = 2f; // +200%p
        private const float MAX_DAMAGE_DISTANCE = 15f;
        
        // Field
        private bool firstTouch = false;

        private Vector2 prevPoint = Vector2.zero;
        private float flyingDistance = 0f; 
            

        // Method
        public override void Fire(Vector2 velocity)
        {
            base.Fire(velocity);
            prevPoint = transform.position;
        }
        
        protected override float CalculateDamage()
        {
            flyingDistance = Mathf.Clamp(flyingDistance, 0f, MAX_DAMAGE_DISTANCE);
            float shellDamage = shellGameData.damage + 100f * DAMAGE_VARIANT * flyingDistance / MAX_DAMAGE_DISTANCE;
            return firer.Model.GetAtk() * shellDamage / 100f;
        }

        // Event Func
        private void Update()
        {
            if (ShouldBeDestroyed)
                return;
            
            flyingDistance += Vector2.Distance(prevPoint, transform.position);
            prevPoint = transform.position;
            
            if (false == DestructibleTerrain.Inst.InFairArea(transform.position))
            {
                ShouldBeDestroyed = true;
            }
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
            
            firstTouch = true;
            HideBody();
            partSysExplosion.Play();
            DamageBattlersInRange(other);
            
            WaitForExplosionParticleSystem()
                .ContinueWith(() => ShouldBeDestroyed = true)
                .Forget();
        }
    }
}