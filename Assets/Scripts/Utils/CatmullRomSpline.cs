using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mathlife.ProjectL.Utils
{
    /// <summary>
    /// Centripetal Catmull–Rom Spline -> Uniform
    /// </summary>
    /// <seealso href="https://en.wikipedia.org/wiki/Centripetal_Catmull%E2%80%93Rom_spline"/>
    public class CatmullRomSpline
    {
        public const int MODULO = 12;
        
        private readonly bool clockWiseOriented;

        private readonly List<CatmullRomSegment> segments = new();
        private readonly List<float> accArcLengths = new();

        public CatmullRomSpline(bool clockWiseOriented, List<Vector2> points)
        {
            this.clockWiseOriented = clockWiseOriented;

            var controlPoints = points.Where((v, index) => index % MODULO == 0).ToList();
            controlPoints.Insert(0, 2 * controlPoints[0] - controlPoints[1]);
            controlPoints.Add(2 * controlPoints[^1] - controlPoints[^2]);

            for (int i = 0; i + 3 < controlPoints.Count; ++i)
            {
                CatmullRomSegment segment = new(clockWiseOriented,
                    controlPoints[i], controlPoints[i + 1], controlPoints[i + 2], controlPoints[i + 3]);
                segments.Add(segment);

                if (i == 0)
                    accArcLengths.Add(segment.ArcLength);
                else
                    accArcLengths.Add(accArcLengths[i - 1] + segment.ArcLength);
            }
        }

        public void GetPoint(float length, out Vector2 point, out Vector2 normal, out Vector2 tangent)
        {
            if (length <= 0)
            {
                point = segments[0].GetPoint(0f);
                normal = segments[0].GetNormal(0f);
                tangent = segments[0].GetTangent(0f);
                return;
            }

            if (length >= accArcLengths[^1])
            {
                point = segments[^1].GetPoint(1f);
                normal = segments[^1].GetNormal(1f);
                tangent = segments[^1].GetTangent(1f);
                return;
            }

            // acc = [0.02, 0.04, 0.06]
            // length = 0.01 -> index = 0 (acc[index] > length) 
            // length = 0.03 -> index = 1 (acc[index] > length)
            // length = 0.05 -> index = 2 (acc[index] > length)

            int index = accArcLengths.FindIndex(acc => length < acc);

            float accPrev = (index > 0) ? accArcLengths[index - 1] : 0;
            float accNext = accArcLengths[index];
            float t = (length - accPrev) / (accNext - accPrev);

            CatmullRomSegment segment = segments[index];
            point = segment.GetPoint(t);
            normal = segment.GetNormal(t);
            tangent = segment.GetTangent(t);
        }

#if UNITY_EDITOR
        public void DrawSpline(IDebugLineRenderer lineRenderer)
        {
            lineRenderer.Clear();

            foreach (var segment in segments)
            {
                lineRenderer.DrawLine(segment.GetPoint(0), segment.GetPoint(1), Color.magenta);
            }
        }
#endif
    }
}