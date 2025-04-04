using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public interface IShell
    {
        void Init(ArtyModel firer, ShellGameData shellGameData);
        void Fire(Vector2 velocity);

        GameObject GetGameObject();
    }
}