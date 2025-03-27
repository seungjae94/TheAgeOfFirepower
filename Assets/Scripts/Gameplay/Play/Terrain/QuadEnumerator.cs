using System.Collections;
using System.Collections.Generic;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class QuadEnumerator : IEnumerator<Quad>
    {
        Stack<Quad> stack = new();
        private Quad root;

        public Quad Current { get; private set; }

        object IEnumerator.Current => Current;

        public QuadEnumerator(Quad root)
        {
            this.root = root;
            stack.Push(root);
        }
        
        public bool MoveNext()
        {
            if (stack.TryPop(out Quad quad))
            {
                Current = quad;
                if (quad.TryGetChildren(out Quad[] children))
                {
                    foreach (var child in children)
                    {
                        stack.Push(child);
                    }
                }
                return true;
            }

            return false;
        }

        public void Reset()
        {
            stack.Clear();
            Current = null;
            stack.Push(root);
        }


        public void Dispose()
        {
            stack.Clear();
        }
    }
}