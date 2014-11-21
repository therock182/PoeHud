using System;

namespace PoeHUD.Framework
{
    public struct Vec2
    {
        public static readonly Vec2 Empty = new Vec2(0, 0);
        public int X;
        public int Y;

        public Vec2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public float Dist(Vec2 other)
        {
            float num = other.X - X;
            float num2 = other.Y - Y;
            return (float) Math.Sqrt(num*num + num2*num2);
        }

        public override int GetHashCode()
        {
            return X + Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vec2)
            {
                var vec = (Vec2) obj;
                return vec.X == X && vec.Y == Y;
            }
            return false;
        }

        public override string ToString()
        {
            return string.Concat(new object[]
            {
                "[",X,", ",Y,"]"});
        }

        public static bool operator ==(Vec2 left, Vec2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vec2 left, Vec2 right)
        {
            return !left.Equals(right);
        }

        public static Vec2 operator +(Vec2 left, Vec2 right)
        {
            return new Vec2(left.X + right.X, left.Y + right.Y);
        }

        public static Vec2 operator -(Vec2 left, Vec2 right)
        {
            return new Vec2(left.X - right.X, left.Y - right.Y);
        }

        public static Vec2 operator *(Vec2 left, float right)
        {
            return new Vec2((int) (left.X*right), (int) (left.Y*right));
        }

        public static Vec2 operator /(Vec2 left, float right)
        {
            return new Vec2((int) (left.X/right), (int) (left.Y/right));
        }


        public double GetPolarCoordinates(out double phi)
        {
            double distance = Math.Sqrt(X*X + Y*Y);
            phi = Math.Acos(X/distance);
            if (Y < 0)
                phi = 2*Math.PI - phi;
            return distance;
        }
    }
}