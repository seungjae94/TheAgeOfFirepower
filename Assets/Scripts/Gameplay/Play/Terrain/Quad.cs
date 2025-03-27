using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class Quad
    {
        public readonly int xMin;
        public readonly int yMin;
        public readonly int width;
        public readonly int height;

        private Quad[] children;

        public Quad(int xMin, int yMin, int width, int height)
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

        public Quad[] Divide()
        {
            int halfWidth = width / 2;
            int halfHeight = height / 2;

            if (width == 1)
            {
                children = new[]
                {
                    new Quad(xMin, yMin, 1, halfHeight),
                    new Quad(xMin, yMin + halfHeight, 1, width - halfHeight)
                };
            }
            else if (height == 1)
            {
                children = new[]
                {
                    new Quad(xMin, yMin, halfWidth, 1),
                    new Quad(xMin + halfWidth, yMin, width - halfWidth, 1),
                };
            }
            else
            {
                children = new[]
                {
                    new Quad(xMin, yMin, halfWidth, halfHeight),
                    new Quad(xMin + halfWidth, yMin, width - halfWidth, halfHeight),
                    new Quad(xMin, yMin + halfHeight, halfWidth, height - halfHeight),
                    new Quad(xMin + halfWidth, yMin + halfHeight, width - halfWidth, height - halfHeight)
                };
            }

            return children;
        }

        public bool HasChildren()
        {
            return children is { Length: > 0 };
        }

        public bool TryGetChildren(out Quad[] children)
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