using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Gameplay.AppScope
{
    public class Cameras : MonoSingleton<Cameras>
    {
        [field: SerializeField] public Camera MainCamera { get; private set; }
    }
}