using System.Collections.Generic;
using UnityEngine;

namespace Mathlife.ProjectL.Utils
{
    /// <summary>
    /// p1과 p2를 통과하는 곡선 선분
    /// </summary>
    public class CatmullRomSegment
    {
        private readonly bool clockWiseOriented;

        private readonly Vector2 p0;
        private readonly Vector2 p1;
        private readonly Vector2 p2;
        private readonly Vector2 p3;

        private readonly float t0;
        private readonly float t1;
        private readonly float t2;
        private readonly float t3;

        public float ArcLength => (p2 - p1).magnitude;

        public CatmullRomSegment(bool clockWiseOriented, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            this.clockWiseOriented = clockWiseOriented;

            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;

            t0 = 0;
            t1 = GetNextParameter(t0, this.p0, this.p1);
            t2 = GetNextParameter(t1, this.p1, this.p2);
            t3 = GetNextParameter(t2, this.p2, this.p3);
        }

        public Vector2 GetPoint(float t)
        {
            t = DenormalizeParameter(t);

            Vector2 a1 = p0 * (t1 - t) / (t1 - t0) + p1 * (t - t0) / (t1 - t0);
            Vector2 a2 = p1 * (t2 - t) / (t2 - t1) + p2 * (t - t1) / (t2 - t1);
            Vector2 a3 = p2 * (t3 - t) / (t3 - t2) + p3 * (t - t2) / (t3 - t2);

            Vector2 b1 = a1 * (t2 - t) / (t2 - t0) + a2 * (t - t0) / (t2 - t0);
            Vector2 b2 = a2 * (t3 - t) / (t3 - t1) + a3 * (t - t1) / (t3 - t1);

            return b1 * (t2 - t) / (t2 - t1) + b2 * (t - t1) / (t2 - t1);
        }

        public Vector2 GetTangent(float t)
        {
            t = DenormalizeParameter(t);

            float dt = 0.01f;

            if (t + dt > t2)
            {
                return (GetPoint(t) - GetPoint(t - dt)).normalized;
            }

            return (GetPoint(t + dt) - GetPoint(t)).normalized;
        }

        public Vector2 GetNormal(float t)
        {
            Vector2 tangent = GetTangent(t);
            
            if (clockWiseOriented)
            {
                // tangent를 반시계 방향으로 90도 회전하면 노말  
                return new Vector2(-tangent.y, tangent.x).normalized;
            }
            else
            {
                // tangent를 시계 방향으로 90도 회전하면 노말
                return new Vector2(tangent.y, -tangent.x).normalized;
            }
        }

        private float DenormalizeParameter(float t)
        {
            float clamped = Mathf.Clamp(t, 0f, 1f);
            return Mathf.Lerp(t1, t2, clamped);
        }

        /// <param name="alpha">{0: uniform, 0.5: Centripetal, 1: Chordal}</param>
        private static float GetNextParameter(float t, Vector2 p0, Vector2 p1, float alpha = 0.5f)
        {
            return Mathf.Pow((p1 - p0).sqrMagnitude, alpha / 2f) + t;
        }

//         private void Sampling()
//         {
//             arcLengthSamples.Add(0);
//             Vector2 prev = GetPointAt(0);
//
// #if UNITY_EDITOR
//             debugLineRenderer.Clear();
// #endif
//
//             for (int i = 1; i < sampleCount; i++)
//             {
//                 float t = t2 * i / (sampleCount - 1);
//                 Vector2 cur = GetPointAt(t);
//
// #if UNITY_EDITOR
//                 debugLineRenderer.DrawLine(prev, cur, Color.magenta);
// #endif
//
//                 arcLengthSamples.Add(arcLengthSamples[^1] + Vector2.Distance(prev, cur));
//                 prev = cur;
//             }
//         }

// #if UNITY_EDITOR
//         public static IDebugLineRenderer debugLineRenderer;
// #endif
    }
}