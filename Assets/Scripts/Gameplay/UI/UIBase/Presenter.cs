using System;
using System.Collections.Generic;
using Mathlife.ProjectL.Gameplay.ObjectBase;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public interface IPresenter : ISceneBehaviour
    {
        public void Activate();
        public void Deactivate();
    }

    public class PresenterNotRegisteredException : Exception
    {
        public PresenterNotRegisteredException(Type widgetType) 
            : base($"Widget of type {widgetType.Name} is not registered.")
        {
        }
    }
    
    public abstract class Presenter : MonoBehaviour, IPresenter 
    {
        private static readonly Dictionary<Type, Presenter> s_widgets = new();
    
        //public virtual ECanvasLayer CanvasLayer { get; }
        
        // Alias
        protected RectTransform rectTransform => (transform as RectTransform);

        public static void Clear()
        {
            s_widgets.Clear();
        }
        
        public static bool Has<TWidget>() where TWidget : Presenter
        {
            return s_widgets.ContainsKey(typeof(TWidget));
        }
        
        public static TWidget Find<TWidget>() where TWidget : Presenter
        {
            Type widgetType = typeof(TWidget);
    
            if (s_widgets.TryGetValue(widgetType, out Presenter widget))
            {
                return widget as TWidget;
            }
    
            throw new PresenterNotRegisteredException(widgetType);
        }

        public static Presenter Find(Type widgetType)
        {
            if (s_widgets.TryGetValue(widgetType, out Presenter widget))
            {
                return widget;
            }
            
            throw new PresenterNotRegisteredException(widgetType);
        }
        
        public virtual void OnSceneInitialize()
        {
            s_widgets.Add(GetType(), this);
        }

        public virtual void OnSceneClear()
        {
        }

        public virtual void Activate()
        {
            gameObject.SetActive(true);
        }

        public virtual void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}