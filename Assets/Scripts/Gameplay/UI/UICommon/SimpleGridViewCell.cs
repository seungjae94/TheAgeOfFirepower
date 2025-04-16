using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public abstract class SimpleGridViewCell<TItemData, TContext>
        : FancyGridViewCell<TItemData, TContext>
        where TContext : SimpleGridViewContext, new()
    {
        [SerializeField]
        protected Button cellButton;

        public override void Initialize()
        {
            base.Initialize();

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