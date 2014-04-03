﻿using System;

namespace ObjectDetect
{
    public struct Rectangle
    {
        public FixedPoint X, Y, W, H;

        public int Left { get { return (int)Math.Ceiling(X); } set { X = value; } }
        public int Top { get { return (int)Math.Ceiling(Y); } set { Y = value; } }
        public int Width { get { return (int)Math.Floor(W); } set { W = value; } }
        public int Height { get { return (int)Math.Floor(H); } set { H = value; } }

        public Rectangle(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }
        public Rectangle(double x, double y, double w, double h)
        {
            X = (FixedPoint)x;
            Y = (FixedPoint)y;
            W = (FixedPoint)w;
            H = (FixedPoint)h;
        }
    }
}
