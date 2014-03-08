using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdaBoost;

namespace ObjectDetect
{
    public struct ImageSample : ISample
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

        private bool filesProbablyEqual(ImageSample other)
        {
            var retval = System.IO.File.Exists(file.AbsolutePath) && System.IO.File.Exists(other.file.AbsolutePath);
            retval &= System.IO.Path.GetFileName(file.AbsolutePath).Equals(System.IO.Path.GetFileName(other.file.AbsolutePath));
            retval &= System.IO.File.GetCreationTime(file.AbsolutePath).Equals(System.IO.File.GetCreationTime(other.file.AbsolutePath));
            return retval;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ImageSample)) return false;
            var other = (ImageSample)obj;

            return x == other.x && y == other.y && w == other.w && h == other.h && filesProbablyEqual(other);
        }

        public override int GetHashCode()
        {
            return x ^ y ^ w ^ h ^ System.IO.Path.GetFileName(file.AbsolutePath).GetHashCode();
        }

        public override string ToString()
        {
            //"ImageSample C:\path\to\file.jpg [x,y] wxh"
            return "ImageSample " + file.AbsolutePath + " [" + x + "," + y + "] " + w + "x" + h;
        }
    }
}
