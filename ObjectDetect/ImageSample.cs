using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdaBoost;

namespace ObjectDetect
{
    public class ImageSample : ISample
    {
        public readonly string fileName;
        public readonly int windowIndex;
        public readonly SlidingWindow windowManager;
        public rectangle location { get { return windowManager.getRectangle(windowIndex); } }
        public int scale { get { return windowManager.getScale(windowIndex); } }
        public int pixelCount { get { return windowManager.getOffsetStepsPerWindow(); } }

        public ImageSample(string fileName, int windowIndex, SlidingWindow windowManager)
        {
            this.fileName = fileName;
            this.windowIndex = windowIndex;
            this.windowManager = windowManager;
        }

        public fixed_point getZoomLevelAtScale(int scale)
        {
            return windowManager.getZoomLevelAtScale(scale);
        }

        public bool fileEquals(ImageSample other)
        {
            return fileName.Equals(other.fileName);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ImageSample)) return false;
            var other = (ImageSample)obj;

            return location.Equals(other.location) && fileName.Equals(other.fileName);
        }

        public override int GetHashCode()
        {
            return location.GetHashCode() ^ fileName.GetHashCode();
        }

        public override string ToString()
        {
            //"ImageSample C:\path\to\file.jpg [x,y] wxh"
            return "ImageSample " + fileName + " [" + location.x + "," + location.y + "] " + location.w + "x" + location.h;
        }
    }
}
