using System;

namespace Util.Collections
{
    static class Tuple
    {
        public static Tuple<T1> Unpack<T1>(this Tuple<T1> This, out T1 item1)
        {
            item1 = This.Item1;
            return This;
        }

        public static Tuple<T1, T2> Unpack<T1, T2>(this Tuple<T1, T2> This, out T1 item1, out T2 item2)
        {
            item1 = This.Item1;
            item2 = This.Item2;
            return This;
        }

        public static Tuple<T1, T2, T3> Unpack<T1, T2, T3>(this Tuple<T1, T2, T3> This, out T1 item1, out T2 item2,
            out T3 item3)
        {
            item1 = This.Item1;
            item2 = This.Item2;
            item3 = This.Item3;
            return This;
        }

        public static Tuple<T1, T2, T3, T4> Unpack<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> This, out T1 item1,
            out T2 item2, out T3 item3, out T4 item4)
        {
            item1 = This.Item1;
            item2 = This.Item2;
            item3 = This.Item3;
            item4 = This.Item4;
            return This;
        }

        public static Tuple<T1, T2, T3, T4, T5> Unpack<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> This,
            out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5)
        {
            item1 = This.Item1;
            item2 = This.Item2;
            item3 = This.Item3;
            item4 = This.Item4;
            item5 = This.Item5;
            return This;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6> Unpack<T1, T2, T3, T4, T5, T6>(
            this Tuple<T1, T2, T3, T4, T5, T6> This, out T1 item1, out T2 item2, out T3 item3, out T4 item4,
            out T5 item5, out T6 item6)
        {
            item1 = This.Item1;
            item2 = This.Item2;
            item3 = This.Item3;
            item4 = This.Item4;
            item5 = This.Item5;
            item6 = This.Item6;
            return This;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7> Unpack<T1, T2, T3, T4, T5, T6, T7>(
            this Tuple<T1, T2, T3, T4, T5, T6, T7> This, out T1 item1, out T2 item2, out T3 item3, out T4 item4,
            out T5 item5, out T6 item6, out T7 item7)
        {
            item1 = This.Item1;
            item2 = This.Item2;
            item3 = This.Item3;
            item4 = This.Item4;
            item5 = This.Item5;
            item6 = This.Item6;
            item7 = This.Item7;
            return This;
        }

        public static Tuple<T1, T2, T3, T4, T5, T6, T7, T8> Unpack<T1, T2, T3, T4, T5, T6, T7, T8>(
            this Tuple<T1, T2, T3, T4, T5, T6, T7, T8> This, out T1 item1, out T2 item2, out T3 item3, out T4 item4,
            out T5 item5, out T6 item6, out T7 item7, out T8 item8)
        {
            item1 = This.Item1;
            item2 = This.Item2;
            item3 = This.Item3;
            item4 = This.Item4;
            item5 = This.Item5;
            item6 = This.Item6;
            item7 = This.Item7;
            item8 = This.Rest;
            return This;
        }
    }
}
