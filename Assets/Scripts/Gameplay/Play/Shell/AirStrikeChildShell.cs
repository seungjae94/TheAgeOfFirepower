using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class AirStrikeChildShell : ShellBase
    {
        // Field
        private bool firstTouch = false;

        // Event Func
        private void LateUpdate()
        {
            if (ShouldBeDestroyed)
                return;
            
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
            DestructTerrain(other);
            DamageBattlersInRange(other);

            WaitForExplosionParticleSystem()
                .ContinueWith(() => ShouldBeDestroyed = true)
                .Forget();
        }
    }
}