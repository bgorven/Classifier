using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetect
{
    public struct rectangle
    {
        public fixed_point x, y, w, h;

        public int Left { get { return (int)Math.Ceiling(x); } set { x = value; } }
        public int Top { get { return (int)Math.Ceiling(y); } set { y = value; } }
        public int Width { get { return (int)Math.Floor(w); } set { w = value; } }
        public int Height { get { return (int)Math.Floor(h); } set { h = value; } }

        public rectangle(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }
        public rectangle(double x, double y, double w, double h)
        {
            this.x = (fixed_point)x;
            this.y = (fixed_point)y;
            this.w = (fixed_point)w;
            this.h = (fixed_point)h;
        }
    }
}
