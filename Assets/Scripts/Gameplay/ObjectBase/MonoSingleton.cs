using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.ObjectBase
{
    public enum SingletonLifeTime
    {
        App,
        Scene,
    }
    
    /// <summary>
    /// 씬에 배치되어 있는 정적 싱글톤.
    /// </summary>
    /// <remarks>
    /// 모노 싱글톤은 Awake 대신 OnRegistered 함수를 사용해서 초기화한다.
    /// </remarks>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected abstract SingletonLifeTime LifeTime { get; }

        private static readonly string s_typeName = typeof(T).Name;

        private static T s_inst;


        // FUTURE: 동적 싱글톤을 지원하게 될 경우 lock, OnApplicationQuit을 고려해야 한다.
        public static T Inst
        {
            get
            {
                if (s_inst == null)
                    Debug.LogWarning($"[{s_typeName}] Singleton instance doesn't exist in the current scene.");
                return s_inst;
            }
        }

        private void Awake()
        {
            if (s_inst == null)
            {
                s_inst = this as T;
                OnRegistered();

                if (LifeTime == SingletonLifeTime.App)
                    DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.Log($"[{s_typeName}] Singleton instance already exist. You have multiple instances of the type.");
                Destroy(gameObject);
            }
        }
        
        protected virtual void OnRegistered()
        {
        }
    }
}