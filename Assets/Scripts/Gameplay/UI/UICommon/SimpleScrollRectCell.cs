using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public abstract class SimpleScrollRectCell<TItemData, TContext> : FancyScrollRectCell<TItemData, TContext>
        where TContext : SimpleScrollRectContext, new()
    {
        [SerializeField]
        protected Button cellButton;

        public override void Initialize()
        {
            base.Initialize();

            // Not Selectable
            if (cellButton == null)
                return;
            
            cellButton.OnClickAsObservable()
                .Subscribe(OnClick)
                .AddTo(gameObject);
        }

        protected virtual void OnClick(Unit _)
        {
            Context.onCellClickRx.OnNext(Index);
        }
    }
}