using System;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    /// <summary>
    /// 메모리 상에 존재하는 싱글톤
    /// </summary>
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        private static readonly string s_typeName = typeof(T).Name;
        private static T s_inst;

        public static T Inst => s_inst ??= new T();
    }
}