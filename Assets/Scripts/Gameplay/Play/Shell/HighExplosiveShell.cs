using System;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class HighExplosiveShell : MonoBehaviour, IShell
    {
        private static int terrainLayer = -1;
        
        private ShellGameData shellGameData;

        private Rigidbody2D rigidbody2D;

        // Field
        private bool toBeDestroyed = false;
        
        public void Init(ShellGameData shellGameData)
        {
            this.shellGameData = shellGameData;
            
            if (terrainLayer < 0)
                terrainLayer = LayerMask.NameToLayer("Terrain");
        }

        public void Fire(Vector2 velocity)
        {
            if (rigidbody2D == null)
                rigidbody2D = GetComponent<Rigidbody2D>();
            
            rigidbody2D.linearVelocity = velocity;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (toBeDestroyed)
                return;
            
            if (other.gameObject.layer != terrainLayer)
            {
                return;
            }
            
            DestructibleTerrain.Inst.Paint(other.contacts[0].point, 
                Shape.Circle((int)shellGameData.explosionRadius), Color.clear);

            toBeDestroyed = true;
            Destroy(gameObject);
        }
    }
}