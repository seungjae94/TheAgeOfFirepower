using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    // 산탄 (캐니스터탄)
    public class CanisterShell : ShellBase
    {
        private const int CHILDREN_COUNT = 4;
        
        // Field
        private bool firstTouch = false;
        private bool shouldBeDestroyedInternal = false;
        
        private bool isChild = false;
        private List<CanisterShell> children = null;

        // Method
        public override void Init(ArtyController firer)
        {
            base.Init(firer);

            if (isChild)
                return;

            children = new ();
            
            for (int i = 0; i < CHILDREN_COUNT; i++)
            {
                GameObject shellGameObject = Instantiate(gameObject);
                shellGameObject.transform.position = transform.position; // TODO: 없애도 되나?

                CanisterShell shell = shellGameObject.GetComponent<CanisterShell>();
                shell.isChild = true;
                shell.Init(firer); // TODO: 없애도 되나?
                
                children.Add(shell);
            }
        }

        public override void Fire(Vector2 velocity)
        {
            base.Fire(velocity);

            if (isChild)
                return;

            float[] angles = new float[CHILDREN_COUNT] { -10, -5, 5, 10 };
            for (int i = 0; i < CHILDREN_COUNT; i++)
            {
                children[i].Fire(Quaternion.Euler(0, 0, angles[i]) * velocity);
            }
        }

        // Event Func
        private void LateUpdate()
        {
            if (false == isChild && false == ShouldBeDestroyed)
            {
                ShouldBeDestroyed = shouldBeDestroyedInternal && children.All(child => child.shouldBeDestroyedInternal);

                if (ShouldBeDestroyed)
                {
                    foreach (var child in children)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
            
            if (false == DestructibleTerrain.Inst.InFairArea(transform.position))
            {
                shouldBeDestroyedInternal = true;
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
                .ContinueWith(() => shouldBeDestroyedInternal = true)
                .Forget();
        }
    }
}