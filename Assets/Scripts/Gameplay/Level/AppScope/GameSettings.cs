using Mathlife.ProjectL.Gameplay.ObjectBase;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class GameSettings : MonoSingleton<GameSettings>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.App;

        [field: SerializeField]
        public bool UseSaveFileIfAvailable { get; private set; }
    }
}