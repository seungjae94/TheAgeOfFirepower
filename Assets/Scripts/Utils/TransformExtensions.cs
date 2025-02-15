using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Utils
{
    public static class TransformExtensions
    {
        public static T FindRecursive<T>(this Transform transform)
        {
            Queue<Transform> queue = new();
            queue.Enqueue(transform);

            while (queue.Count > 0)
            {
                Transform node = queue.Dequeue();
                T component = node.GetComponent<T>();

                if (component != null)
                    return component;

                foreach (Transform child in node)
                    queue.Enqueue(child);
            }

            return default;
        }

        public static T FindRecursiveByName<T>(this Transform transform, string n)
        {
            Queue<Transform> queue = new();
            queue.Enqueue(transform);

            while (queue.Count > 0)
            {
                Transform node = queue.Dequeue();

                if (node.name == n)
                    return node.GetComponent<T>();

                foreach (Transform child in node)
                    queue.Enqueue(child);
            }

            return default;
        }

        public static List<T> FindAllRecursive<T>(this Transform transform)
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
    }
}
