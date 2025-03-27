using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class QuadNode
    {
        public readonly int xMin;
        public readonly int yMin;
        public readonly int width;
        public readonly int height;

        private QuadNode[] children;

        public QuadNode(int xMin, int yMin, int width, int height)
        {
            this.xMin = xMin;
            this.yMin = yMin;
            this.width = width;
            this.height = height;
        }

        public bool IsDivisible()
        {
            return width > 1 || height > 1;
        }

        public QuadNode[] Divide()
        {
            int halfWidth = width / 2;
            int halfHeight = height / 2;

            if (width == 1)
            {
                children = new[]
                {
                    new QuadNode(xMin, yMin, 1, halfHeight),
                    new QuadNode(xMin, yMin + halfHeight, 1, width - halfHeight)
                };
            }
            else if (height == 1)
            {
                children = new[]
                {
                    new QuadNode(xMin, yMin, halfWidth, 1),
                    new QuadNode(xMin + halfWidth, yMin, width - halfWidth, 1),
                };
            }
            else
            {
                children = new[]
                {
                    new QuadNode(xMin, yMin, halfWidth, halfHeight),
                    new QuadNode(xMin + halfWidth, yMin, width - halfWidth, halfHeight),
                    new QuadNode(xMin, yMin + halfHeight, halfWidth, height - halfHeight),
                    new QuadNode(xMin + halfWidth, yMin + halfHeight, width - halfWidth, height - halfHeight)
                };
            }

            return children;
        }

        public bool TryGetChildren(out QuadNode[] children)
        {
            if (this.children != null)
            {
                children = this.children;
                return true;
            }

            children = null;
            return false;
        }
    }
}