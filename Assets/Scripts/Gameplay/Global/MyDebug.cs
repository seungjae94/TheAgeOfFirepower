using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public static class MyDebug
    {
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Log(object message)
        {
            Debug.Log(message);
        }
        
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogError(object message)
        {
            Debug.LogError(message);
        }
        
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogWarning(object message)
        {
            Debug.LogError(message);
        }
    }
}