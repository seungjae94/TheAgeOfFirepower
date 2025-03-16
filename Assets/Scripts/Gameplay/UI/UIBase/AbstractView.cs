using System;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public abstract class AbstractView : MonoBehaviour, IInitializable
    {
        public virtual void Initialize()
        {
        }
        
        /// <summary>Presenter가 Open할 때 호출</summary> 
        public virtual void Draw()
        {
        }

        /// <summary>Presenter가 Close할 때 호출</summary>
        public virtual void Clear()
        {
        }
    }
}