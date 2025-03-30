using System;
using UnityEngine;

namespace Mathlife.ProjectL.Utils
{
    public struct double2
    {
        public double x;
        public double y;

        public double2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double2(Vector2 v)
        {
            this.x = v.x;
            this.y = v.y;
        }
        
        public double sqrMagnitude => x * x + y * y;

        public static implicit operator Vector2(double2 d)
        {
            return new Vector2((float)d.x, (float)d.y);
        }
        
        public override string ToString()
        {
            return String.Format("({0:F6}, {1:F6})", x, y);
        }

        public static double2 operator +(double2 lhs, double2 rhs)
        {
            lhs.x += rhs.x;
            lhs.y += rhs.y;
            return lhs;
        }
        
        public static double2 operator -(double2 lhs, double2 rhs)
        {
            lhs.x -= rhs.x;
            lhs.y -= rhs.y;
            return lhs;
        }
        
        public static double2 operator *(double2 lhs, double rhs)
        {
            lhs.x *= rhs;
            lhs.y *= rhs;
            return lhs;
        }
        
        public static double2 operator /(double2 lhs, double rhs)
        {
            lhs.x /= rhs;
            lhs.y /= rhs;
            return lhs;
        }
    }
}