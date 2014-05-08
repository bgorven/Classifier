using System;
using System.Globalization;

namespace ObjectDetect
{
    public struct FixedPoint
    {
        private const int BinaryPlacesAfterPoint = 16;
        private readonly long _value;

        private FixedPoint(long value)
        {
            _value = value;
        }

        public static implicit operator double(FixedPoint value)
        {
            return value._value / (double)(1 << BinaryPlacesAfterPoint);
        }

        public static implicit operator FixedPoint(int value)
        {
            return new FixedPoint((long)value << BinaryPlacesAfterPoint);
        }

        public static explicit operator FixedPoint(double value)
        {
            return new FixedPoint(checked((long)(value * (1 << BinaryPlacesAfterPoint))));
        }

        public static bool operator ==(FixedPoint left, FixedPoint right)
        {
            return left._value == right._value;
        }

        public static bool operator !=(FixedPoint left, FixedPoint right)
        {
            return left._value != right._value;
        }

        public override bool Equals(object obj)
        {
            return (obj is FixedPoint) && _value == ((FixedPoint)obj)._value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static FixedPoint operator /(FixedPoint number, FixedPoint divisor)
        {
            return new FixedPoint(checked((number._value << BinaryPlacesAfterPoint) / divisor._value));
        }

        public static FixedPoint operator -(FixedPoint number, FixedPoint subtrahend)
        {
            return new FixedPoint(checked(number._value - subtrahend._value));
        }

        public static FixedPoint operator +(FixedPoint number, FixedPoint addend)
        {
            return new FixedPoint(checked(number._value + addend._value));
        }

        public static FixedPoint operator *(FixedPoint number, FixedPoint factor)
        {
            return new FixedPoint(checked((number._value * factor._value) >> BinaryPlacesAfterPoint));
        }

        public static FixedPoint operator <<(FixedPoint number, int shift)
        {
            return new FixedPoint(checked(number._value << shift));
        }

        public static FixedPoint operator >>(FixedPoint number, int shift)
        {
            return new FixedPoint(checked(number._value >> shift));
        }

        public static bool operator <(FixedPoint number, FixedPoint other)
        {
            return number._value < other._value;
        }

        public static bool operator >(FixedPoint number, FixedPoint other)
        {
            return number._value > other._value;
        }

        public int Ceiling()
        {
            return checked((int)-(-_value >> BinaryPlacesAfterPoint));
        }

        public int Floor()
        {
            return checked((int)_value >> BinaryPlacesAfterPoint);
        }

        public int Round()
        {
            return (int)Math.Round(this, MidpointRounding.AwayFromZero);
        }

        public static FixedPoint Max(FixedPoint left, FixedPoint right)
        {
            return left > right ? left : right;
        }

        public static FixedPoint Min(FixedPoint left, FixedPoint right)
        {
            return left > right ? right : left;
        }

        public override string ToString()
        {
            const long fracMask = ~(~0 << BinaryPlacesAfterPoint);
            var frac = 1.0 / (1 << BinaryPlacesAfterPoint);
            var decimalPlaces = 0;
            // 2^-16 = 0.0000152587890625 => 6 decimal places
            while (frac < 10)
            {
                frac *= 10;
                decimalPlaces++;
            }
            return (_value >> BinaryPlacesAfterPoint).ToString(CultureInfo.InvariantCulture) + "." +
                   ((long)Math.Round((_value & fracMask) * frac))
                       .ToString(CultureInfo.InvariantCulture)
                       .PadLeft(decimalPlaces, '0');
        }

        public string ToString(string format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return ((double)this).ToString(format, provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return CultureInfo.InvariantCulture.Equals(provider) ? ToString() : ToString("G", provider);
        }
    }
}
