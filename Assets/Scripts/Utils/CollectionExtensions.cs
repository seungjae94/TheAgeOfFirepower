using System;
using System.Collections.Generic;

namespace Mathlife.ProjectL.Utils
{
    public static class CollectionExtensions
    {
        public static void Swap<T>(this IList<T> list, int from, int to)
        {
            if (from < 0 || from >= list.Count || to < 0 || to >= list.Count)
                throw new IndexOutOfRangeException();

            T tmp = list[from];
            list[from] = list[to];
            list[to] = tmp;
        }
    }
}
