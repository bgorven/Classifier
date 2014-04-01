using System;
using System.Collections.Generic;

namespace AdaBoost
{
    internal struct KahanSum
    {
        /// <summary>
        /// The nominal value of this object.
        /// </summary>
        public float S;

        /// <summary>
        /// The compensation, such that s - c would be a better approximation of the sum of all values accumulated
        /// in s than s itself is.
        /// </summary>
        public float C;

        public static KahanSum Sum(IEnumerable<float> vals)
        {
            KahanSum sum = new KahanSum();

            foreach (float val in vals)
            {
                sum.Add(val);
            }

            return sum;
        }

        internal void Add<T>(IEnumerable<T> vals, Func<T, float> function)
        {
            foreach (T val in vals)
            {
                Add(function(val));
            }
        }

        internal void Add(IEnumerable<float> vals)
        {
            foreach (float val in vals)
            {
                Add(val);
            }
        }

        internal void Add(float val)
        {
            if (float.IsInfinity(S))
            {
                return;
            }
            else if (float.IsInfinity(val))
            {
                S = val;
            }
            else
            {
                float v = val - C;
                float t = S + v;
                C = (t - S) - v;
                S = t;
            }
        }
    }
}
