using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public interface IShell
    {
        void Init(ShellGameData shellGameData);
        void Fire(Vector2 velocity);
    }
}