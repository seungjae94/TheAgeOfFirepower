using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Utils
{
    public static class TransformExtensions
    {
        public static T FindRecursive<T>(this Transform transform) where T : Component
        {
            Queue<Transform> queue = new();
            queue.Enqueue(transform);

            while (queue.Count > 0)
            {
                Transform node = queue.Dequeue();
                if (node.TryGetComponent<T>(out var component) == true)
                    return component;

                foreach (Transform child in node)
                    queue.Enqueue(child);
            }

            return null;
        }

        public static T FindRecursiveByName<T>(this Transform transform, string n) where T : Component
        {
            Queue<Transform> queue = new();
            queue.Enqueue(transform);

            while (queue.Count > 0)
            {
                Transform node = queue.Dequeue();

                if (node.name == n)
                {
                    if (node.TryGetComponent<T>(out var component) == true)
                        return component;
                    else
                        throw new System.Exception($"[TrasnformExtensions] GameObject {n} exists but {typeof(T).Name} component not found.");
                }

                foreach (Transform child in node)
                    queue.Enqueue(child);
            }

            return null;
        }

        public static List<T> FindAllRecursive<T>(this Transform transform) where T : Component
        {
            List<T> list = new List<T>();

            Queue<Transform> queue = new();
            queue.Enqueue(transform);

            while (queue.Count > 0)
            {
                Transform node = queue.Dequeue();
                T component = node.GetComponent<T>();

                if (component != null)
                    list.Add(component);

                foreach (Transform child in node)
                    queue.Enqueue(child);
            }

            return list;
        }

        public static Rect GetGlobalRect(this RectTransform rectTransform)
        {
            var rect = rectTransform.rect;
            Vector2 offset = rectTransform.localPosition;
            Transform parent = rectTransform.parent;
            while (parent.GetComponent<Canvas>() == null || !parent.GetComponent<Canvas>().isRootCanvas)
            {
                offset += (Vector2)parent.localPosition;
                parent = parent.parent;
            }
            rect.x += offset.x;
            rect.y += offset.y;
            return rect;
        }
    }
}
