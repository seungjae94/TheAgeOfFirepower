using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public interface IShell
    {
        bool ShouldBeDestroyed { get; }
        void Init(ArtyController firer);
        void Fire(Vector2 velocity);
    }
}