using Mathlife.ProjectL.Gameplay.ObjectBase;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Gameplay.AppScope
{
    public class Cameras : MonoSingleton<Cameras>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Scene;
        
        [field: SerializeField] public Camera MainCamera { get; private set; }
    }
}