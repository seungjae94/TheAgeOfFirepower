using System;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class ArtyPageNoArtySelectedException : Exception
    {
        public ArtyPageNoArtySelectedException(string msg) : base(msg)
        {
        }
    }
}