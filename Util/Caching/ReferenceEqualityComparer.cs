using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Caching
{
    public sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        private static readonly ReferenceEqualityComparer _instance = new ReferenceEqualityComparer();
        public static ReferenceEqualityComparer Default { get { return _instance; } }
        private ReferenceEqualityComparer() { }

        bool IEqualityComparer<object>.Equals(object left, object right)
        {
            return ReferenceEquals(left, right);
        }

        int IEqualityComparer<object>.GetHashCode(object source)
        {
            return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(source);
        }
    }
}
