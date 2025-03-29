using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class Shape
    {
        public readonly List<Column> columns = new();
        public Vector2Int Offset { get; private set; }

        private Shape()
        {
        }

        public void AddColumn(Column column)
        {
            columns.Add(column);
        }
        
        public static Shape Circle(int radius)
        {
            Shape shape = new Shape();
            shape.Offset = new Vector2Int(-radius, -radius);
            
            for (int x = 0; x < 2 * radius + 1; ++x)
            {
                Column column = new Column();
                shape.AddColumn(column);
                
                // (x-r)^2 + (y-r)^2 <= r^2
                // r - sqrt(r^2 - (x-r)^2) <= y <= r + sqrt(r^2 - (x-r)^2)
                int dr = (int) Mathf.Sqrt((float)(radius * radius - (x - radius) * (x - radius)));
                Range range = new Range(radius - dr, radius + dr);
                column.AddRange(range);
            }
                
            return shape;
        }
    }
}