using System;

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

        public static implicit operator double(FixedPoint val)
        {
            return val._value / (double)(1 << BinaryPlacesAfterPoint);
        }

        public static implicit operator FixedPoint(int val)
        {
            return new FixedPoint((long)val << BinaryPlacesAfterPoint);
        }

        public static explicit operator FixedPoint(double val)
        {
            return new FixedPoint(checked((long)(val * (1 << BinaryPlacesAfterPoint))));
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

        public override string ToString()
        {
            return ((double)this).ToString("F4");
        }

        public static FixedPoint Max(FixedPoint left, FixedPoint right)
        {
            return left > right ? left : right;
        }

        public static FixedPoint Min(FixedPoint left, FixedPoint right)
        {
            return left > right ? right : left;
        }
    }
}
