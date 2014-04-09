using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Random
{
    public class PermutedSequence : IEnumerator<int>
    {
        private readonly int _prime;
        private readonly int _start;
        private int _current;
        public PermutedSequence(int maxVal) : this(0, maxVal) { }

        public PermutedSequence(int startVal, int maxVal)
        {
            if (startVal < 0 || startVal >= maxVal) throw new ArgumentOutOfRangeException();
            _start = startVal - 1;
            _current = startVal - 1;
            _prime = maxVal - maxVal%4 + 3;
            while (!Prime.IsPrime(_prime)) _prime += 4;
        }

        public int Permute(int val)
        {
            if (val < 0 || val >= _prime) throw new ArgumentOutOfRangeException();

            //Don't want 0 to map to itself
            var temp = ((long) val + _prime/2)%_prime;

            var residue = (int)(temp*temp%_prime);
            return temp*2 < _prime ? residue : _prime - residue;
        }

        public void Dispose() { }

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
        public static bool IsPrime(long val)
        {
            if(val == 1) return false;
            if(val == 2 || val == 3) return true;
            if((val & 1) == 0) return false;
            if((val + 1)%6 == 0 && (val - 1)%6 == 0) return false;
            var sqrt = (long)Math.Sqrt(val) + 1;
            while (sqrt*sqrt <= val) sqrt++;
            for (long i = 3; i < sqrt; i += 2)
            {
                if (val%i == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
