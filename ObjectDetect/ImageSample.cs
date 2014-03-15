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
        public readonly Uri file;
        public readonly rectangle location;

        public ImageSample(Uri file, rectangle location)
        {
            this.file = file;
            this.location = location;
        }

        public ImageSample(Uri file, int left, int top, int width, int height)
        {
            this.file = file;
            this.location = new rectangle(left, top, width, height);
        }

        public bool fileProbablyEquals(ImageSample other)
        {
            return System.IO.Path.GetFileName(file.AbsolutePath).Equals(System.IO.Path.GetFileName(other.file.AbsolutePath)) && 
                System.IO.File.Exists(file.AbsolutePath) && System.IO.File.Exists(other.file.AbsolutePath) && 
                System.IO.File.GetCreationTime(file.AbsolutePath).Equals(System.IO.File.GetCreationTime(other.file.AbsolutePath));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ImageSample)) return false;
            var other = (ImageSample)obj;

            return location.Equals(other.location) && System.IO.Path.GetFileName(file.AbsolutePath).Equals(System.IO.Path.GetFileName(other.file.AbsolutePath));
        }

        public override int GetHashCode()
        {
            return location.GetHashCode() ^ System.IO.Path.GetFileName(file.AbsolutePath).GetHashCode();
        }

        public override string ToString()
        {
            //"ImageSample C:\path\to\file.jpg [x,y] wxh"
            return "ImageSample " + file.AbsolutePath + " [" + location.x + "," + location.y + "] " + location.w + "x" + location.h;
        }
    }
}
