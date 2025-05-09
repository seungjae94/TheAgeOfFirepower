using System;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public abstract class AbstractView : MonoBehaviour, ISceneBehaviour
    {
        public bool IsClear { get; private set; }
        
        public virtual void OnSceneInitialize()
        {
        }

        public virtual void OnSceneClear()
        {
            
        }

        /// <summary>Presenter가 Open할 때 호출</summary> 
        public virtual void Draw()
        {
            IsClear = false;
        }

        /// <summary>Presenter가 Close할 때 호출</summary>
        public virtual void Clear()
        {
            IsClear = true;
        }
    }
}