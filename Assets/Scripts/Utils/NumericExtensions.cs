using System;
using UnityEngine;

namespace Mathlife.ProjectL.Utils
{
    public static class NumericExtensions
    {
        // float
        public static int Round(this float v)
        {
            return (int)Math.Round(v, 0);
        }

        public static int AwayFromZero(this float v)
        {
            return (v > 0) ? (int)Math.Ceiling(v) : (int)Math.Floor(v);
        }

        // Vector2Int
        public static Vector3 ToVector3(this Vector2Int v)
        {
            return new Vector3(v.x, v.y, 0.0f);
        }

        public static bool IsZero(this Vector2Int v)
        {
            return v == Vector2Int.zero;
        }

        // Vector2
        public static Vector2Int ToDirection(this Vector2 v)
        {
            if (v.x > 0) return Vector2Int.right;
            if (v.x < 0) return Vector2Int.left;
            if (v.y > 0) return Vector2Int.up;
            if (v.y < 0) return Vector2Int.down;
            return Vector2Int.zero;
        }

        public static Vector2Int AwayFromZero(this Vector2 v)
        {
            return new Vector2Int(v.x.AwayFromZero(), v.y.AwayFromZero());
        }

        public static Vector2Int ToVector2Int(this Vector2 v)
        {
            return new Vector2Int(v.x.Round(), v.y.Round());
        }

        // Vector3
        public static Vector2Int PositionToCoord(this Vector3 v)
        {
            return v.ToVector2Int();
        }

        public static Vector2Int ToVector2Int(this Vector3 v)
        {
            return new Vector2Int(v.x.Round(), v.y.Round());
        }
    }
}
