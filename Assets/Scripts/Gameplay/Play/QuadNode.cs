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
            return width > 1 && height > 1;
        }

        public QuadNode[] Divide()
        {
            int halfWidth = width / 2;
            int halfHeight = height / 2;
            children = new[]
            {
                new QuadNode(xMin, yMin, halfWidth, halfHeight),
                new QuadNode(xMin + halfWidth, yMin, halfWidth, halfHeight),
                new QuadNode(xMin, yMin + halfHeight, halfWidth, halfHeight),
                new QuadNode(xMin + halfWidth, yMin + halfHeight, halfWidth, halfHeight)
            };
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