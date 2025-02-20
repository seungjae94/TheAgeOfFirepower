using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Mathlife.ProjectL.Gameplay
{
    interface IFlexItemFactory<FlexItemData, FlexItemView>
        where FlexItemView : Presenter<FlexItemData>
    {
        FlexItemView Create(Transform parent, FlexItemData flexItemData);
    }

    interface IFlexItemFactory<FlexItemData, FlexItemActions, FlexItemView>
        where FlexItemView : Presenter<FlexItemData, FlexItemActions>
    {
        FlexItemView Create(Transform parent, FlexItemData flexItemData, FlexItemActions flexItemAction);
    }

    abstract class AbstractFlexBase<FlexItemData, FlexItemView> : Presenter
        where FlexItemView : PresenterBase
    {
        protected List<FlexItemView> m_itemViews = new();

        protected void ClearItemViews()
        {
            foreach (FlexItemView itemView in m_itemViews)
            {
                Destroy(itemView.gameObject);
            }
            m_itemViews.Clear();
        }
    }

    abstract class AbstractFlex<FlexItemData, FlexItemView> : AbstractFlexBase<FlexItemData, FlexItemView>
        where FlexItemView : Presenter<FlexItemData>
    {
        [Inject] protected IFlexItemFactory<FlexItemData, FlexItemView> m_factory;

        protected virtual void Draw(List<FlexItemData> itemDatas)
        {
            ClearItemViews();

            foreach (FlexItemData itemData in itemDatas)
            {
                FlexItemView itemView = m_factory.Create(transform, itemData);
                m_itemViews.Add(itemView);
            }
        }
    }

    abstract class AbstractFlex<FlexItemData, FlexItemActions, FlexItemView> : AbstractFlexBase<FlexItemData, FlexItemView>
        where FlexItemView : Presenter<FlexItemData, FlexItemActions>
    {
        [Inject] protected IFlexItemFactory<FlexItemData, FlexItemActions, FlexItemView> m_factory;

        public virtual void Draw(List<FlexItemData> itemDatas, FlexItemActions itemAction)
        {
            ClearItemViews();

            foreach (FlexItemData itemData in itemDatas)
            {
                FlexItemView itemView = m_factory.Create(transform, itemData, itemAction);
                m_itemViews.Add(itemView);
            }
        }
    }
}