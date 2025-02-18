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

    interface IFlexItemFactory<FlexItemData, FlexItemAction, FlexItemView>
        where FlexItemView : Presenter<FlexItemData, FlexItemAction>
    {
        FlexItemView Create(Transform parent, FlexItemData flexItemData, FlexItemAction flexItemAction);
    }

    interface IFlexItemFactory<FlexItemData, FlexItemAction0, FlexItemAction1, FlexItemView>
        where FlexItemView : Presenter<FlexItemData, FlexItemAction0, FlexItemAction1>
    {
        FlexItemView Create(Transform parent, FlexItemData flexItemData, FlexItemAction0 flexItemAction0, FlexItemAction1 flexItemAction1);
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

    abstract class AbstractFlex<FlexItemData, FlexItemAction, FlexItemView> : AbstractFlexBase<FlexItemData, FlexItemView>
        where FlexItemView : Presenter<FlexItemData, FlexItemAction>
    {
        [Inject] protected IFlexItemFactory<FlexItemData, FlexItemAction, FlexItemView> m_factory;

        public virtual void Draw(List<FlexItemData> itemDatas, FlexItemAction itemAction)
        {
            ClearItemViews();

            foreach (FlexItemData itemData in itemDatas)
            {
                FlexItemView itemView = m_factory.Create(transform, itemData, itemAction);
                m_itemViews.Add(itemView);
            }
        }
    }

    abstract class AbstractFlex<FlexItemData, FlexItemAction0, FlexItemAction1, FlexItemView> : AbstractFlexBase<FlexItemData, FlexItemView>
        where FlexItemView : Presenter<FlexItemData, FlexItemAction0, FlexItemAction1>
    {
        [Inject] protected IFlexItemFactory<FlexItemData, FlexItemAction0, FlexItemAction1, FlexItemView> m_factory;

        public virtual void Draw(List<FlexItemData> itemDatas, FlexItemAction0 itemAction0, FlexItemAction1 itemAction1)
        {
            ClearItemViews();

            foreach (FlexItemData itemData in itemDatas)
            {
                FlexItemView itemView = m_factory.Create(transform, itemData, itemAction0, itemAction1);
                m_itemViews.Add(itemView);
            }
        }
    }
}