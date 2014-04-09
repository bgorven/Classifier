using System;
using System.Collections;
using System.Collections.Generic;

namespace Utilities.Random
{
    public class PermutedSequence : IEnumerator<int>
    {
        private readonly int _prime;
        private readonly int _start;
        private int _current;

        public PermutedSequence(int maxValue) : this(0, maxValue) { }

        public PermutedSequence(int startValue, int maxValue)
        {
            if (startValue < 0 || startValue >= maxValue)
                throw new ArgumentOutOfRangeException(startValue > 0
                    ? "startValue should be positive."
                    : "maxValue should be greater than startValue");
            _start = startValue - 1;
            _current = startValue - 1;
            _prime = maxValue - maxValue%4 + 3;
            while (!Prime.IsPrime(_prime)) _prime += 4;
        }

        public int Permute(int value)
        {
            if (value < 0 || value >= _prime) throw new ArgumentOutOfRangeException("value should be between 0 and " + _prime);

            //Don't want 0 to map to itself
            var temp = ((long) value + _prime/2)%_prime;

            var residue = (int)(temp*temp%_prime);
            return temp*2 < _prime ? residue : _prime - residue;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanManaged) { }

        public bool MoveNext()
        {
            _current++;
            return _current < _prime;
        }

        public void Reset()
        {
            _current = _start;
        }

        public int Current { get { return Permute(Permute(_current)); } }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }

    public static class Prime
    {
        /// <summary>
        /// Taken from http://stackoverflow.com/a/2945445/2063518
        /// </summary>
        public static bool IsPrime(long value)
        {
            if(value == 1) return false;
            if(value == 2 || value == 3) return true;
            if((value & 1) == 0) return false;
            if((value + 1)%6 == 0 && (value - 1)%6 == 0) return false;
            var sqrt = (long)Math.Sqrt(value) + 1;
            while (sqrt*sqrt <= value) sqrt++;
            for (long i = 3; i < sqrt; i += 2)
            {
                if (value%i == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
