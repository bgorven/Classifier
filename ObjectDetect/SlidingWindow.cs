using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectDetect
{
    public class SlidingWindow
    {
        readonly int _w;
        readonly int _h;
        readonly int _szMin, _szMax, _szExp, _offsetExp;

        /// <summary>
        /// Each rectangular region within an image is given a unique integer index.
        /// 
        /// Each successive window is offset by (window_width / step_size^2).
        /// </summary>
        /// <param name="w">width of the entire image</param>
        /// <param name="h">height of the image</param>
        /// <param name="startSize">Minimum object size, in pixels</param>
        /// <param name="endSize">Largest window will be slightly larger than the maximum object size</param>
        /// <param name="stepSize">each level of window is (2^stepsize+1)/(2^stepsize) times larger than the previous</param>
        /// <param name="offsetStepSize">each window will overlap horizontally and vertically with 2^offsetStepSize other windows in each row or column.<br/>
        /// offset steps should not be smaller than pixels, thus if the startSize is 64, for instance, offsetStepSize param should not be less than 6.</param>
        public SlidingWindow(int w, int h, int startSize, int endSize, int stepSize, int offsetStepSize)
        {
            this._w = w;
            this._h = h;
            this._szMin = startSize;
            this._szMax = endSize;
            this._szExp = stepSize;
            this._offsetExp = offsetStepSize;
        }

        public int OffsetStepsPerWindow
        {
            get { return 1 << _offsetExp; }
        }

        public int GetNearestWindow(int x, int y, int width, int height)
        {
            //precise center point
            var centerX = x + (FixedPoint)width / 2;
            var centerY = y + (FixedPoint)height / 2;

            int nextIndexBase = 0;
            foreach (var winSz in WinSizes())
            {
                int indexBase = nextIndexBase;
                nextIndexBase = checked(indexBase + GetNumWindows(winSz));

                var offset = winSz >> _offsetExp;

                int xIndex = Clamp(GetIndex(centerX, winSz, offset), 0, GetNumWindows(winSz, this._w) - 1);
                int yIndex = Clamp(GetIndex(centerY, winSz, offset), 0, GetNumWindows(winSz, this._h) - 1);
                if (CheckContains(xIndex * offset, winSz, x, x + width) && CheckContains(yIndex * offset, winSz, y, y + height))
                {
                    return checked(indexBase + yIndex * GetNumWindows(winSz, this._w) + xIndex);
                }
            }
            return -1;
        }

        public int GetNearestWindow(Rectangle loc)
        {
            //precise center point
            var centerX = loc.X + loc.W / 2;
            var centerY = loc.Y + loc.H / 2;

            int nextIndexBase = 0;
            foreach (var winSz in WinSizes())
            {
                int indexBase = nextIndexBase;
                nextIndexBase = checked(indexBase + GetNumWindows(winSz));

                var offset = winSz >> _offsetExp;

                int xIndex = Clamp(GetIndex(centerX, winSz, offset), 0, GetNumWindows(winSz, this._w) - 1);
                int yIndex = Clamp(GetIndex(centerY, winSz, offset), 0, GetNumWindows(winSz, this._h) - 1);
                if (CheckContains(xIndex * offset, winSz, loc.X, loc.X + loc.W) && CheckContains(yIndex * offset, winSz, loc.Y, loc.Y + loc.H))
                {
                    return checked(indexBase + yIndex * GetNumWindows(winSz, this._w) + xIndex);
                }
            }
            return -1;
        }

        public bool GetWindowDimensions(int index, out double x, out double y, out double w, out double h)
        {
            try
            {
                var rect = GetRectangle(index);
                x = rect.X;
                y = rect.Y;
                w = rect.W;
                h = rect.H;
                return w < _szMax && h < _szMax;
            }
            catch (ArgumentOutOfRangeException)
            {
                x = y = w = h = 0;
                return false;
            }
        }
        public Rectangle GetRectangle(int index)
        {
            if (index < 0) throw new ArgumentOutOfRangeException();
            int indexRange = 0;
            foreach (var winSz in WinSizes())
            {
                int pageIndex = index - indexRange;
                indexRange += GetNumWindows(winSz);
                if (indexRange <= index) continue;

                int rowWidth = GetNumWindows(winSz, this._w);

                int xIndex = pageIndex % rowWidth;
                int yIndex = pageIndex / rowWidth;

                var offset = winSz >> _offsetExp;

                var x = offset * xIndex;
                var y = offset * yIndex;
                var w = winSz;
                var h = winSz;
                return new Rectangle(x, y, w, h);
            }
            throw new ArgumentOutOfRangeException();
        }

        private FixedPoint WinSize(int size)
        {
            var sz = WinSizes().GetEnumerator();
            for (int i = 0; i < size; i++)
            {
                sz.MoveNext();
            }
            return sz.Current;
        }

        //C# doesn't allow to set floating point modes, but we require absolute repeatability...
        private IEnumerable<FixedPoint> WinSizes()
        {
            FixedPoint sz = _szMin;
            while (sz < Math.Min(_w,_h))
            {
                yield return sz;
                int multiplier = (1 << _szExp) + 1;
                //int divisor = 1 << stepExp;
                sz = (sz * multiplier) >> _szExp;
            }
        }

        private int GetNumWindows(FixedPoint winSz)
        {
            return GetNumWindows(winSz, _w) * GetNumWindows(winSz, _h);
        }

        private int GetNumWindows(FixedPoint winSz, int length)
        {
            //double availableWidth = (length - winSz);
            //double offset = (winSz / Math.Pow(2, offsetExp));
            //return checked((int)Math.Floor(availableWidth / offset) + 1);

            if (length < winSz) throw new ArgumentOutOfRangeException();
            return ((length / winSz - 1) << _offsetExp).Floor() + 1;
        }

        private static int Clamp(int val, int lowBound, int highBound)
        {
            if (val < lowBound) return lowBound;
            if (val > highBound) return highBound;
            return val;
        }

        private static int GetIndex(FixedPoint center, FixedPoint winSz, FixedPoint offset)
        {
            //Find ideal coordinate
            var coord = center - winSz / 2;
            //Snap to grid
            return (coord / offset).Round();
        }

        //Note that this is the opposite of normal bounds checking: the region between lowBound and highBound
        //must fit entirely within the region from location to location+width.
        private static bool CheckContains(FixedPoint location, FixedPoint winSz, FixedPoint lowBound, FixedPoint highBound)
        {
            if (location > lowBound) return false;
            if (location + winSz < highBound) return false;
            return true;
        }

        public override int GetHashCode()
        {
 	         return _w ^ _h ^ _szMin ^ _szMax ^ _szExp ^ _offsetExp;
        }

        internal int GetScale(int index)
        {
            if (index < 0) throw new ArgumentOutOfRangeException();
            int scale = 0, indexRange = 0;
            foreach (var winSz in WinSizes())
            {
                indexRange += GetNumWindows(winSz);
                if (indexRange > index) return scale;
                scale++;
            }
            throw new ArgumentOutOfRangeException();
        }

        internal FixedPoint GetZoomLevelAtScale(int scale)
        {
            return WinSizes().ElementAt(scale) >> _offsetExp;
        }
    }
}
