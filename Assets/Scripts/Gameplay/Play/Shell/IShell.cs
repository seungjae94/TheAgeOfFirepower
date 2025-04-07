using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public interface IShell
    {
        bool ShouldBeDestroyed { get; }
        void Init(ArtyModel firer, ShellGameData shellGameData);
        void Fire(Vector2 velocity);
    }
}