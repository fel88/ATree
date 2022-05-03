using System;

namespace ATree
{
    public static class Helpers
    {
        public static bool IntersectSegments(Vector2d p0, Vector2d p1, Vector2d q0, Vector2d q1, ref Vector2d c0)
        {
            double ux = p1.X - p0.X;
            double uy = p1.Y - p0.Y;
            double vx = q1.X - q0.X;
            double vy = q1.Y - q0.Y;
            double wx = p0.X - q0.X;
            double wy = p0.Y - q0.Y;

            double d = (ux * vy - uy * vx);
            double s = (vx * wy - vy * wx) / d;

            // Intersection point
            c0.X = p0.X + s * ux;
            c0.Y = p0.Y + s * uy;
            if (!IsPointInsideSegment(p0, p1, c0)) return false;
            if (!IsPointInsideSegment(q0, q1, c0)) return false;
            return true;
        }
        public static bool IsPointOnLine(Vector2d start, Vector2d end, Vector2d pnt, double epsilon = 10e-6f)
        {
            float tolerance = 10e-6f;
            var d1 = pnt - start;
            if (d1.Length < tolerance) return true;
            if ((end - start).Length < tolerance) throw new Exception("degenerated line");
            d1 = d1.Normalized();
            var p2 = (end - start).Normalized();
            var crs = Vector2d.CrossLen(d1, p2);
            return Math.Abs(crs) < epsilon;
        }
        public static bool IsPointInsideSegment(Vector2d start, Vector2d end, Vector2d pnt, double epsilon = 10e-6f)
        {
            if (!IsPointOnLine(start, end, pnt, epsilon)) return false;
            var diff1 = (pnt - start).Length + (pnt - end).Length;
            return Math.Abs(diff1 - (end - start).Length) < epsilon;
        }
    }
}

