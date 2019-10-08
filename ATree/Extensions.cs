using System;
using System.Drawing;

namespace ATree
{
    public static class Extensions
    {
        public static float DistTo(this PointF p,PointF v)
        {
            return (float)Math.Sqrt(Math.Pow(p.X - v.X, 2) + Math.Pow(p.Y - v.Y, 2));
        }
    }
}

