using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Arithmetic
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Kahan")]
    public struct KahanSum
    {
        /// <summary>
        /// The nominal value of this object.
        /// </summary>
        public float Value { get; private set; }

        /// <summary>
        /// The compensation, such that s - c would be a better approximation of the sum of all values accumulated
        /// in s than s itself is.
        /// </summary>
        public float Compensation { get; private set; }

        public static KahanSum Sum(IEnumerable<float> values)
        {
            if (values == null) throw new ArgumentNullException("values");
            var sum = new KahanSum();

            foreach (var val in values)
            {
                sum.Add(val);
            }

            return sum;
        }

        public void Add<T>(IEnumerable<T> values, Func<T, float> function)
        {
            if (values == null) throw new ArgumentNullException("values");
            if (function == null) throw new ArgumentNullException("function");
            foreach (var val in values)
            {
                Add(function(val));
            }
        }

        public void Add(IEnumerable<float> values)
        {
            if (values == null) throw new ArgumentNullException("values");
            foreach (var val in values)
            {
                Add(val);
            }
        }

        public void Add(float value)
        {
            if (float.IsInfinity(Value))
            {
                return;
            }
            if (float.IsInfinity(value))
            {
                Value = value;
                return;
            }

            var v = value - Compensation;
            var t = Value + v;
            Compensation = (t - Value) - v;
            Value = t;
        }

        public override string ToString()
        {
            return ((double)Value + Compensation).ToString(CultureInfo.CurrentCulture);
        }

        public string ToString(IFormatProvider provider)
        {
            return ((double)Value + Compensation).ToString(provider);
        }

        public override int GetHashCode()
        {
            return Compensation.GetHashCode() ^ Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is KahanSum) return this == (KahanSum)obj;
            var val = (double)obj;
            var thisVal = (double)Value - Compensation;
            return Value - thisVal == Compensation && thisVal == val;
        }

        public static bool operator ==(KahanSum left, KahanSum right)
        {
            return left.Value == right.Value && left.Compensation == right.Compensation;
        }

        public static bool operator !=(KahanSum left, KahanSum right)
        {
            return left.Value != right.Value && left.Compensation != right.Compensation;
        }
    }
}
