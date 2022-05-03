using System;

namespace ATree
{
    public struct Vector2d
    {        
        public Vector2d(double x, double y)
        {
            X = x;
            Y = y;
        }
        public double X;
        public double Y;
        public static double CrossLen(Vector2d v1, Vector2d v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X;
        }
        
        public static Vector2d operator -(Vector2d left, Vector2d right)
        {
            return new Vector2d(left.X - right.X, left.Y - right.Y);
        }
        public Vector2d Normalized()
        {
            return new Vector2d(X / Length, Y / Length);
        }
        public double Length
        {
            get => Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
        }
    }
}

