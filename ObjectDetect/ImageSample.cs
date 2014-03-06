using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdaBoost;

namespace ObjectDetect
{
    struct ImageSample : ISample
    {
        public Uri file;

        private int x;
        public int Left { get { return x; } set { x = value; } }

        private int y;
        public int Top { get { return y; } set { y = value; } }

        private int w;
        public int Width { get { return w; } set { w = value; } }

        private int h;
        public int Height { get { return h; } set { h = value; } }

        public ImageSample(Uri file, int left, int top, int width, int height)
        {
            this.file = file;
            this.x = left;
            this.y = top;
            this.w = width;
            this.h = height;
        }
    }
}
