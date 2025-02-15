using Mathlife.ProjectL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class Page : Presenter
    {
        CanvasGroup canvasGroup;

        public abstract EPageId pageId { get; }

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public abstract void Initialize();
        protected abstract void InitializeChildren();

        public void Open()
        {
            canvasGroup.Show();
        }

        public void Close()
        {
            canvasGroup.Hide();
        }
    }
}
