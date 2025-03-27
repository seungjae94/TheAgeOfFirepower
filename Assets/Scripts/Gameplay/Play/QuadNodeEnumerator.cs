using System.Collections;
using System.Collections.Generic;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class QuadNodeEnumerator : IEnumerator<QuadNode>
    {
        Stack<QuadNode> stack = new();
        private QuadNode root;

        public QuadNode Current { get; private set; }

        object IEnumerator.Current => Current;

        public QuadNodeEnumerator(QuadNode root)
        {
            this.root = root;
            stack.Push(root);
        }
        
        public bool MoveNext()
        {
            if (stack.TryPop(out QuadNode node))
            {
                Current = node;
                if (node.TryGetChildren(out QuadNode[] children))
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