using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetect
{
    public class SlidingWindow
    {
        readonly int w;
        readonly int h;
        readonly int szMin, szMax, szExp, offsetExp;

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
            this.w = w;
            this.h = h;
            this.szMin = startSize;
            this.szMax = endSize;
            this.szExp = stepSize;
            this.offsetExp = offsetStepSize;
        }

        public int getOffsetStepsPerWindow()
        {
            return 1 << offsetExp;
        }

        public int getNearestWindow(int x, int y, int width, int height)
        {
            //precise center point
            var centerX = x + (fixed_point)width / 2;
            var centerY = y + (fixed_point)height / 2;

            int index_base = 0, next_index_base = 0;
            foreach (var winSz in winSizes())
            {
                index_base = next_index_base;
                next_index_base = checked(index_base + getNumWindows(winSz));

                var offset = winSz >> offsetExp;

                int xIndex = clamp(getIndex(centerX, winSz, offset), 0, getNumWindows(winSz, this.w) - 1);
                int yIndex = clamp(getIndex(centerY, winSz, offset), 0, getNumWindows(winSz, this.h) - 1);
                if (checkContains(xIndex * offset, winSz, x, x + width) && checkContains(yIndex * offset, winSz, y, y + height))
                {
                    return checked(index_base + yIndex * getNumWindows(winSz, this.w) + xIndex);
                }
            }
            return -1;
        }

        public bool getWindowDimensions(int index, out double x, out double y, out double w, out double h)
        {
            try
            {
                var rect = getRectangle(index);
                x = rect.x;
                y = rect.y;
                w = rect.w;
                h = rect.h;
                return w < szMax && h < szMax;
            }
            catch (ArgumentOutOfRangeException)
            {
                x = y = w = h = 0;
                return false;
            }
        }
        public rectangle getRectangle(int index)
        {
            if (index < 0) throw new ArgumentOutOfRangeException();
            int index_range = 0;
            foreach (var winSz in winSizes())
            {
                int page_index = index - index_range;
                index_range += getNumWindows(winSz);
                if (index_range <= index) continue;

                int row_width = getNumWindows(winSz, this.w);

                int xIndex = page_index % row_width;
                int yIndex = page_index / row_width;

                var offset = winSz >> offsetExp;

                var x = offset * xIndex;
                var y = offset * yIndex;
                var w = winSz;
                var h = winSz;
                return new rectangle(x, y, w, h);
            }
            throw new ArgumentOutOfRangeException();
        }

        private fixed_point winSize(int size)
        {
            var sz = winSizes().GetEnumerator();
            for (int i = 0; i < size; i++)
            {
                sz.MoveNext();
            }
            return sz.Current;
        }

        //C# doesn't allow to set floating point modes, but we require absolute repeatability...
        private IEnumerable<fixed_point> winSizes()
        {
            fixed_point sz = szMin;
            while (sz < Math.Min(w,h))
            {
                yield return sz;
                int multiplier = (1 << szExp) + 1;
                //int divisor = 1 << stepExp;
                sz = (sz * multiplier) >> szExp;
            }
        }

        private int getNumWindows(fixed_point winSz)
        {
            return getNumWindows(winSz, w) * getNumWindows(winSz, h);
        }

        private int getNumWindows(fixed_point winSz, int length)
        {
            //double availableWidth = (length - winSz);
            //double offset = (winSz / Math.Pow(2, offsetExp));
            //return checked((int)Math.Floor(availableWidth / offset) + 1);

            if (length < winSz) throw new ArgumentOutOfRangeException();
            return ((length / winSz - 1) << offsetExp).Floor() + 1;
        }

        private int clamp(int val, int lowBound, int highBound)
        {
            if (val < lowBound) return lowBound;
            if (val > highBound) return highBound;
            return val;
        }

        private static int getIndex(fixed_point center, fixed_point winSz, fixed_point offset)
        {
            //Find ideal coordinate
            var coord = center - winSz / 2;
            //Snap to grid
            return (coord / offset).Round();
        }

        //Note that this is the opposite of normal bounds checking: the region between lowBound and highBound
        //must fit entirely within the region from location to location+width.
        private bool checkContains(fixed_point location, fixed_point winSz, int lowBound, int highBound)
        {
            if (location > lowBound) return false;
            if (location + winSz < highBound) return false;
            return true;
        }

        public override int GetHashCode()
        {
 	         return w ^ h ^ szMin ^ szMax ^ szExp ^ offsetExp;
        }

        internal int getScale(int index)
        {
            if (index < 0) throw new ArgumentOutOfRangeException();
            int scale = 0, index_range = 0;
            foreach (var winSz in winSizes())
            {
                index_range += getNumWindows(winSz);
                if (index_range > index) return scale;
                scale++;
            }
            throw new ArgumentOutOfRangeException();
        }

        internal fixed_point getZoomLevelAtScale(int scale)
        {
            return winSizes().ElementAt(scale) >> offsetExp;
        }
    }
}
