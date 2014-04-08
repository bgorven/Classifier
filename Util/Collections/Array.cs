using System;

namespace Util.Collections
{
    public static class Array
    {
        public static ArraySegment<T> Unpack<T>(this T[] This, out T item1)
        {
            item1 = This[0];
            return This.Length > 1
                ? new ArraySegment<T>(This, 1, This.Length - 1)
                : new ArraySegment<T>(This, This.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this T[] This, out T item1, out T item2)
        {
            item1 = This[0];
            item2 = This[1];
            return This.Length > 2
                ? new ArraySegment<T>(This, 2, This.Length - 2)
                : new ArraySegment<T>(This, This.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this T[] This, out T item1, out T item2, out T item3)
        {
            item1 = This[0];
            item2 = This[1];
            item3 = This[2];
            return This.Length > 3
                ? new ArraySegment<T>(This, 3, This.Length - 3)
                : new ArraySegment<T>(This, This.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this T[] This, out T item1, out T item2, out T item3, out T item4)
        {
            item1 = This[0];
            item2 = This[1];
            item3 = This[2];
            item4 = This[3];
            return This.Length > 4
                ? new ArraySegment<T>(This, 4, This.Length - 4)
                : new ArraySegment<T>(This, This.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this T[] This, out T item1, out T item2, out T item3, out T item4,
            out T item5)
        {
            item1 = This[0];
            item2 = This[1];
            item3 = This[2];
            item4 = This[3];
            item5 = This[4];
            return This.Length > 5
                ? new ArraySegment<T>(This, 5, This.Length - 5)
                : new ArraySegment<T>(This, This.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this T[] This, out T item1, out T item2, out T item3, out T item4,
            out T item5, out T item6)
        {
            item1 = This[0];
            item2 = This[1];
            item3 = This[2];
            item4 = This[3];
            item5 = This[4];
            item6 = This[5];
            return This.Length > 6
                ? new ArraySegment<T>(This, 6, This.Length - 6)
                : new ArraySegment<T>(This, This.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this T[] This, out T item1, out T item2, out T item3, out T item4,
            out T item5, out T item6, out T item7)
        {
            item1 = This[0];
            item2 = This[1];
            item3 = This[2];
            item4 = This[3];
            item5 = This[4];
            item6 = This[5];
            item7 = This[6];
            return This.Length > 7
                ? new ArraySegment<T>(This, 7, This.Length - 7)
                : new ArraySegment<T>(This, This.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this T[] This, out T item1, out T item2, out T item3, out T item4,
            out T item5, out T item6, out T item7, out T item8)
        {
            item1 = This[0];
            item2 = This[1];
            item3 = This[2];
            item4 = This[3];
            item5 = This[4];
            item6 = This[5];
            item7 = This[6];
            item8 = This[7];
            return This.Length > 8
                ? new ArraySegment<T>(This, 8, This.Length - 8)
                : new ArraySegment<T>(This, This.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this T[] This, out TResult item1, Func<T, TResult> transform)
        {
            item1 = transform(This[0]);
            return This.Length > 1
                ? new ArraySegment<T>(This, 1, This.Length - 1)
                : new ArraySegment<T>(This, This.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this T[] This, out TResult item1, out TResult item2, Func<T, TResult> transform)
        {
            item1 = transform(This[0]);
            item2 = transform(This[1]);
            return This.Length > 2
                ? new ArraySegment<T>(This, 2, This.Length - 2)
                : new ArraySegment<T>(This, This.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this T[] This, out TResult item1, out TResult item2, out TResult item3, Func<T, TResult> transform)
        {
            item1 = transform(This[0]);
            item2 = transform(This[1]);
            item3 = transform(This[2]);
            return This.Length > 3
                ? new ArraySegment<T>(This, 3, This.Length - 3)
                : new ArraySegment<T>(This, This.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this T[] This, out TResult item1, out TResult item2, out TResult item3, out TResult item4, Func<T, TResult> transform)
        {
            item1 = transform(This[0]);
            item2 = transform(This[1]);
            item3 = transform(This[2]);
            item4 = transform(This[3]);
            return This.Length > 4
                ? new ArraySegment<T>(This, 4, This.Length - 4)
                : new ArraySegment<T>(This, This.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this T[] This, out TResult item1, out TResult item2, out TResult item3, out TResult item4,
            out TResult item5, Func<T, TResult> transform)
        {
            item1 = transform(This[0]);
            item2 = transform(This[1]);
            item3 = transform(This[2]);
            item4 = transform(This[3]);
            item5 = transform(This[4]);
            return This.Length > 5
                ? new ArraySegment<T>(This, 5, This.Length - 5)
                : new ArraySegment<T>(This, This.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this T[] This, out TResult item1, out TResult item2, out TResult item3, out TResult item4,
            out TResult item5, out TResult item6, Func<T, TResult> transform)
        {
            item1 = transform(This[0]);
            item2 = transform(This[1]);
            item3 = transform(This[2]);
            item4 = transform(This[3]);
            item5 = transform(This[4]);
            item6 = transform(This[5]);
            return This.Length > 6
                ? new ArraySegment<T>(This, 6, This.Length - 6)
                : new ArraySegment<T>(This, This.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this T[] This, out TResult item1, out TResult item2, out TResult item3, out TResult item4,
            out TResult item5, out TResult item6, out TResult item7, Func<T, TResult> transform)
        {
            item1 = transform(This[0]);
            item2 = transform(This[1]);
            item3 = transform(This[2]);
            item4 = transform(This[3]);
            item5 = transform(This[4]);
            item6 = transform(This[5]);
            item7 = transform(This[6]);
            return This.Length > 7
                ? new ArraySegment<T>(This, 7, This.Length - 7)
                : new ArraySegment<T>(This, This.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this T[] This, out TResult item1, out TResult item2, out TResult item3, out TResult item4,
            out TResult item5, out TResult item6, out TResult item7, out TResult item8, Func<T, TResult> transform)
        {
            item1 = transform(This[0]);
            item2 = transform(This[1]);
            item3 = transform(This[2]);
            item4 = transform(This[3]);
            item5 = transform(This[4]);
            item6 = transform(This[5]);
            item7 = transform(This[6]);
            item8 = transform(This[7]);
            return This.Length > 8
                ? new ArraySegment<T>(This, 8, This.Length - 8)
                : new ArraySegment<T>(This, This.Length - 1, 0);
        }
    }
}
