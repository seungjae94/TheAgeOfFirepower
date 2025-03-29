using System.Collections.Generic;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class Column
    {
        public List<Range> ranges = new();

        public void AddRange(Range range)
        {
            ranges.Add(range);
        }
    }
}