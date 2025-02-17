using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay
{
    public class PageNavigator
    {
        Dictionary<EPageId, Page> m_pages = new();
        Stack<Page> m_stack = new();

        public T GetPage<T>() where T : Page
        {
            foreach (var (id, page) in m_pages)
            {
                if (page is T)
                {
                    return (T) m_pages[id];
                }
            }

            Debug.LogError($"Failed to find page of type {typeof(T).Name}.");
            return null;
        }

        public void AddPages(Page[] pages)
        {
            foreach (var page in pages)
            {
                m_pages.Add(page.pageId, page);
            }

            foreach (var page in pages)
            {
                page.Initialize();
            }
        }

        public virtual void Navigate(EPageId pageId)
        {
            if (m_stack.Any())
                m_stack.Peek().Close();

            Page page = m_pages[pageId];
            m_stack.Push(page);
            page.Open();
        }

        public virtual void Back()
        {
            Page page = m_stack.Pop();
            page.Close();

            if (m_stack.Any())
                m_stack.Peek().Open();
        }

        public virtual void Home()
        {
            while (m_stack.Any())
            {
                Back();
            }

            Navigate(EPageId.HomePage);
        }

        public bool IsHome()
        {
            return m_stack.Count == 1;
        }
    }
}
