using System;

namespace Utilities.Collections
{
    public static class Tuple
    {
        public static Tuple<T1> Unpack<T1>(this Tuple<T1> source, out T1 item1)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source.Item1;
            return source;
        }

        public static Tuple<T1, T2> Unpack<T1, T2>(this Tuple<T1, T2> source, out T1 item1, out T2 item2)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source.Item1;
            item2 = source.Item2;
            return source;
        }

        public static Tuple<T1, T2, T3> Unpack<T1, T2, T3>(this Tuple<T1, T2, T3> source, out T1 item1, out T2 item2,
            out T3 item3)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source.Item1;
            item2 = source.Item2;
            item3 = source.Item3;
            return source;
        }

        public static Tuple<T1, T2, T3, T4> Unpack<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> source, out T1 item1,
            out T2 item2, out T3 item3, out T4 item4)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source.Item1;
            item2 = source.Item2;
            item3 = source.Item3;
            item4 = source.Item4;
            return source;
        }

        public static Tuple<T1, T2, T3, T4, T5> Unpack<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> source,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source.Item1;
            item2 = source.Item2;
            item3 = source.Item3;
            item4 = source.Item4;
            item5 = source.Item5;
            return source;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6> Unpack<T1, T2, T3, T4, T5, T6>(
            this Tuple<T1, T2, T3, T4, T5, T6> source, out T1 item1, out T2 item2, out T3 item3, out T4 item4,
            out T5 item5, out T6 item6)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source.Item1;
            item2 = source.Item2;
            item3 = source.Item3;
            item4 = source.Item4;
            item5 = source.Item5;
            item6 = source.Item6;
            return source;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7> Unpack<T1, T2, T3, T4, T5, T6, T7>(
            this Tuple<T1, T2, T3, T4, T5, T6, T7> source, out T1 item1, out T2 item2, out T3 item3, out T4 item4,
            out T5 item5, out T6 item6, out T7 item7)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source.Item1;
            item2 = source.Item2;
            item3 = source.Item3;
            item4 = source.Item4;
            item5 = source.Item5;
            item6 = source.Item6;
            item7 = source.Item7;
            return source;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7, T8> Unpack<T1, T2, T3, T4, T5, T6, T7, T8>(
            this Tuple<T1, T2, T3, T4, T5, T6, T7, T8> source, out T1 item1, out T2 item2, out T3 item3, out T4 item4,
            out T5 item5, out T6 item6, out T7 item7, out T8 item8)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source.Item1;
            item2 = source.Item2;
            item3 = source.Item3;
            item4 = source.Item4;
            item5 = source.Item5;
            item6 = source.Item6;
            item7 = source.Item7;
            item8 = source.Rest;
            return source;
        }
    }
}
