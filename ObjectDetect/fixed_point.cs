using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetect
{
    public struct fixed_point
    {
        private const int BINARY_PLACES_AFTER_POINT = 16;
        private readonly long value;

        private fixed_point(long value)
        {
            this.value = value;
        }

        public static implicit operator double(fixed_point val)
        {
            return val.value / (double)(1 << BINARY_PLACES_AFTER_POINT);
        }

        public static implicit operator fixed_point(int val)
        {
            return new fixed_point((long)val << BINARY_PLACES_AFTER_POINT);
        }

        public static explicit operator fixed_point(double val)
        {
            return new fixed_point(checked((long)(val * (1 << BINARY_PLACES_AFTER_POINT))));
        }

        public static fixed_point operator /(fixed_point number, fixed_point divisor)
        {
            return new fixed_point(checked((number.value << BINARY_PLACES_AFTER_POINT) / divisor.value));
        }

        public static fixed_point operator -(fixed_point number, fixed_point subtrahend)
        {
            return new fixed_point(checked(number.value - subtrahend.value));
        }

        public static fixed_point operator +(fixed_point number, fixed_point addend)
        {
            return new fixed_point(checked(number.value + addend.value));
        }

        public static fixed_point operator *(fixed_point number, fixed_point factor)
        {
            return new fixed_point(checked((number.value * factor.value) >> BINARY_PLACES_AFTER_POINT));
        }

        public static fixed_point operator <<(fixed_point number, int shift)
        {
            return new fixed_point(checked(number.value << shift));
        }

        public static fixed_point operator >>(fixed_point number, int shift)
        {
            return new fixed_point(checked(number.value >> shift));
        }

        public static bool operator <(fixed_point number, fixed_point other)
        {
            return number.value < other.value;
        }

        public static bool operator >(fixed_point number, fixed_point other)
        {
            return number.value > other.value;
        }

        public int Ceiling()
        {
            return checked((int)-(-value >> BINARY_PLACES_AFTER_POINT));
        }

        public int Floor()
        {
            return checked((int)value >> BINARY_PLACES_AFTER_POINT);
        }

        public int Round()
        {
            return (int)Math.Round((double)this, MidpointRounding.AwayFromZero);
        }

        public override string ToString()
        {
            return ((double)this).ToString("F4");
        }
    }
}
