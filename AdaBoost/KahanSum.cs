using System;
using System.Collections.Generic;

namespace AdaBoost
{
    internal struct KahanSum
    {
        /// <summary>
        /// The nominal value of this object.
        /// </summary>
        public float s;

        /// <summary>
        /// The compensation, such that s - c would be a better approximation of the sum of all values accumulated
        /// in s than s itself is.
        /// </summary>
        public float c;

        public static KahanSum sum(IEnumerable<float> vals)
        {
            KahanSum sum = new KahanSum();

            foreach (float val in vals)
            {
                sum.add(val);
            }

            return sum;
        }

        internal void add<T>(IEnumerable<T> vals, Func<T, float> function)
        {
            foreach (T val in vals)
            {
                add(function(val));
            }
        }

        internal void add(IEnumerable<float> vals)
        {
            foreach (float val in vals)
            {
                add(val);
            }
        }

        internal void add(float val)
        {
            if (float.IsInfinity(s))
            {
                return;
            }
            else if (float.IsInfinity(val))
            {
                s = val;
            }
            else
            {
                float v = val - c;
                float t = s + v;
                c = (t - s) - v;
                s = t;
            }
        }
    }
}
