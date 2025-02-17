using Sirenix.OdinInspector;
using System;
using System.Diagnostics;
using UnityEngine;

namespace Mathlife.ProjectL.Utils
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class SpaceOnlyAttribute : PropertyAttribute
    {
        public readonly float height;

        public SpaceOnlyAttribute()
        {
            height = 8f;
        }

        public SpaceOnlyAttribute(float height)
        {
            this.height = height;
        }
    }
}
