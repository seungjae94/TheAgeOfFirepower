using System;
using UnityEngine;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    public abstract class PresenterBase : MonoBehaviour
    {
        protected virtual void SubscribeDataChange() { }
        protected virtual void SubscribeUserInteractions() { }
        protected virtual void InitializeView() { }

        protected virtual void InitializeChildren() { }
    }

    public abstract class Presenter : PresenterBase
    {
        public virtual void Initialize()
        {
            SubscribeDataChange();
            SubscribeUserInteractions();
            InitializeView();
            InitializeChildren();
        }
    }

    public abstract class Presenter<T> : PresenterBase
    {
        public virtual void Initialize(T param)
        {
            Store(param);
            SubscribeDataChange();
            SubscribeUserInteractions();
            InitializeView();
            InitializeChildren();
        }

        protected abstract void Store(T param);
        protected virtual void Rebind(T param) { }
    }

    public abstract class Presenter<T0, T1> : PresenterBase
    {
        public virtual void Initialize(T0 param0, T1 param1)
        {
            Store(param0, param1);
            SubscribeDataChange();
            SubscribeUserInteractions();
            InitializeView();
            InitializeChildren();
        }

        protected abstract void Store(T0 param0, T1 param1);

        protected virtual void Rebind(T0 param0, T1 param1) { }
    }

    public abstract class Presenter<T0, T1, T2> : PresenterBase
    {
        public virtual void Initialize(T0 param0, T1 param1, T2 param2)
        {
            Store(param0, param1, param2);
            SubscribeDataChange();
            SubscribeUserInteractions();
            InitializeView();
            InitializeChildren();
        }

        protected abstract void Store(T0 param0, T1 param1, T2 param2);

        protected virtual void Rebind(T0 param0, T1 param1, T2 param2) { }
    }
}
