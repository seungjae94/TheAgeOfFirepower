using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public interface IPresenter
    {
        public void Open();
        public void Close();
    }

    public class WidgetNotRegisteredException : Exception
    {
        public WidgetNotRegisteredException(Type widgetType) 
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
        
        private void Awake()
        {
            // 씬이 로딩될 때 등록
            s_widgets.Add(GetType(), this);
        }
        
        public static TWidget Find<TWidget>() where TWidget : Presenter
        {
            Type widgetType = typeof(TWidget);
    
            if (s_widgets.TryGetValue(widgetType, out Presenter widget))
            {
                return widget as TWidget;
            }
    
            throw new WidgetNotRegisteredException(widgetType);
        }

        public virtual void Open()
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }
    }
}