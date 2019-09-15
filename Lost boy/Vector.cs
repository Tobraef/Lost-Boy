using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost_boy
{
    public struct Vector
    {
        public int X
        {
            get;
            set;
        }

        public int Y
        {
            get;
            set;
        }

        public Vector(int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.X + v2.X, v2.Y + v1.Y);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector operator *(Vector v1, int val)
        {
            return new Vector(v1.X * val, v1.Y * val);
        }

        public static Vector operator /(Vector v1, int val)
        {
            return new Vector(v1.X / val, v1.Y / val);
        }

        public double Length
        {
            get { return Math.Sqrt((X * X + Y * Y)); }
        }

        public double DistanceFrom(Vector v)
        {
            return Math.Sqrt(Math.Pow(X - v.X, 2) + Math.Pow(Y - v.Y, 2));
        }

        public void FitToLength(int length)
        {
            int y = Y;
            Y = (int)Math.Sqrt(length / (((double)(X * X) / (double)(Y * Y)) + 1));
            X = (int)((double)X / (double)y * Y);
        }

        public static implicit operator System.Drawing.Point(Vector v)
        {
            return new System.Drawing.Point(v.X, v.Y);
        }

        public override string ToString()
        {
            return "X = " + X + " Y = " + Y;
        }
    }
}