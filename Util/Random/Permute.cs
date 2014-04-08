using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Random
{
    public static class Permute
    {
        public static int Int32(int val)
        {
            const long prime = int.MaxValue;
            long temp = val > 0 ? val : -val;

            //Don't want 0 to map to itself
            //Don't want the sequence 0, 1, 4, 9, 25... to exist in the output
            temp = ((temp >> 16) & 0xffff) + ((~temp & 0xffff) << 16);

            if (temp < prime)
            {
                var residue = temp*temp%prime;
                temp = temp*2 < prime ? residue : prime - residue;
            }
            return (int)(val > 0 ? temp : -temp);
        }
    }
}
