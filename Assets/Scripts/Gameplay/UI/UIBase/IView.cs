using System;
using Mathlife.ProjectL.Gameplay.ObjectBase;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public interface IView : IInitializable
    {
        /// <summary>Presenter가 Open할 때 호출</summary> 
        void Draw();
        
        /// <summary>Presenter가 Close할 때 호출</summary>
        void Clear();
    }
}