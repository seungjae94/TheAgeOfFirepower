using Mathlife.ProjectL.Gameplay.ObjectBase;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class DebugSettings : MonoSingleton<DebugSettings>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.App;

        [SerializeField]
        [LabelText("세이브 파일 사용")]
        [LabelWidth(150)]
        private bool useSaveFileIfAvailable;
        
        public bool UseSaveFileIfAvailable
        {
            get
            {
#if UNITY_EDITOR
                return useSaveFileIfAvailable;
#else
                return true;
#endif
            }
        }
    }
}