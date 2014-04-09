using System;

namespace ObjectDetect
{
    internal struct Rectangle
    {
        internal FixedPoint X, Y, W, H;

        internal int Left { get { return (int)Math.Ceiling(X); } set { X = value; } }
        internal int Top { get { return (int)Math.Ceiling(Y); } set { Y = value; } }
        internal int Width { get { return (int)Math.Floor(W); } set { W = value; } }
        internal int Height { get { return (int)Math.Floor(H); } set { H = value; } }

        internal Rectangle(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }
        internal Rectangle(double x, double y, double w, double h)
        {
            X = (FixedPoint)x;
            Y = (FixedPoint)y;
            W = (FixedPoint)w;
            H = (FixedPoint)h;
        }

        internal bool Overlaps(Rectangle r)
        {
            return ((X <= r.X && r.X < X + W) || (r.X <= X && X < r.X + r.W)) &&
                   ((Y <= r.Y && r.Y < Y + H) || (r.Y <= Y && Y < r.Y + r.H));
        }
    }
}
