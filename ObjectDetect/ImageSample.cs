using AdaBoost;

namespace ObjectDetect
{
    public class ImageSample : ISample
    {
        internal readonly string FileName;
        internal readonly int WindowIndex;
        internal readonly SlidingWindow WindowManager;
        internal Rectangle Location { get { return WindowManager.GetRectangle(WindowIndex); } }
        public int Scale { get { return WindowManager.GetScale(WindowIndex); } }
        public int PixelCount { get { return WindowManager.OffsetStepsPerWindow; } }

        internal ImageSample(string fileName, int windowIndex, SlidingWindow windowManager)
        {
            FileName = fileName;
            WindowIndex = windowIndex;
            WindowManager = windowManager;
        }

        public FixedPoint GetZoomLevelAtScale(int scale)
        {
            return WindowManager.GetZoomLevelAtScale(scale);
        }

        public bool FileEquals(ImageSample other)
        {
            return other != null && FileName.Equals(other.FileName);
        }

        public override bool Equals(object obj)
        {
            for (var other = obj as ImageSample; other != null; )
            {
                return Location.Equals(other.Location) && FileName.Equals(other.FileName);
            }
            return false;
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
