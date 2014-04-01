using AdaBoost;

namespace ObjectDetect
{
    public class ImageSample : ISample
    {
        public readonly string FileName;
        public readonly int WindowIndex;
        public readonly SlidingWindow WindowManager;
        public Rectangle Location { get { return WindowManager.GetRectangle(WindowIndex); } }
        public int Scale { get { return WindowManager.GetScale(WindowIndex); } }
        public int PixelCount { get { return WindowManager.OffsetStepsPerWindow; } }

        public ImageSample(string fileName, int windowIndex, SlidingWindow windowManager)
        {
            this.FileName = fileName;
            this.WindowIndex = windowIndex;
            this.WindowManager = windowManager;
        }

        public FixedPoint GetZoomLevelAtScale(int scale)
        {
            return WindowManager.GetZoomLevelAtScale(scale);
        }

        public bool FileEquals(ImageSample other)
        {
            return FileName.Equals(other.FileName);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ImageSample)) return false;
            var other = (ImageSample)obj;

            return Location.Equals(other.Location) && FileName.Equals(other.FileName);
        }

        public override int GetHashCode()
        {
            return Location.GetHashCode() ^ FileName.GetHashCode();
        }

        public override string ToString()
        {
            //"ImageSample C:\path\to\file.jpg [x,y] wxh"
            return "ImageSample " + FileName + " [" + Location.X + "," + Location.Y + "] " + Location.W + "x" + Location.H;
        }
    }
}
