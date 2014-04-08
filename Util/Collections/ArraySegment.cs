using System;

namespace Util.Collections
{
    public static class ArraySegment
    {
        public static ArraySegment<T> Unpack<T>(this ArraySegment<T> This, out T item1)
        {
            item1 = This.Array[This.Offset + 0];
            return This.Count > 1
                ? new ArraySegment<T>(This.Array, This.Offset + 1, This.Count - 1)
                : new ArraySegment<T>(This.Array, This.Offset + This.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this ArraySegment<T> This, out T item1, out T item2)
        {
            item1 = This.Array[This.Offset + 0];
            item2 = This.Array[This.Offset + 1];
            return This.Count > 2
                ? new ArraySegment<T>(This.Array, This.Offset + 2, This.Count - 2)
                : new ArraySegment<T>(This.Array, This.Offset + This.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this ArraySegment<T> This, out T item1, out T item2, out T item3)
        {
            item1 = This.Array[This.Offset + 0];
            item2 = This.Array[This.Offset + 1];
            item3 = This.Array[This.Offset + 2];
            return This.Count > 3
                ? new ArraySegment<T>(This.Array, This.Offset + 3, This.Count - 3)
                : new ArraySegment<T>(This.Array, This.Offset + This.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this ArraySegment<T> This, out T item1, out T item2, out T item3, out T item4)
        {
            item1 = This.Array[This.Offset + 0];
            item2 = This.Array[This.Offset + 1];
            item3 = This.Array[This.Offset + 2];
            item4 = This.Array[This.Offset + 3];
            return This.Count > 4
                ? new ArraySegment<T>(This.Array, This.Offset + 4, This.Count - 4)
                : new ArraySegment<T>(This.Array, This.Offset + This.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this ArraySegment<T> This, out T item1, out T item2, out T item3, out T item4,
            out T item5)
        {
            item1 = This.Array[This.Offset + 0];
            item2 = This.Array[This.Offset + 1];
            item3 = This.Array[This.Offset + 2];
            item4 = This.Array[This.Offset + 3];
            item5 = This.Array[This.Offset + 4];
            return This.Count > 5
                ? new ArraySegment<T>(This.Array, This.Offset + 5, This.Count - 5)
                : new ArraySegment<T>(This.Array, This.Offset + This.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this ArraySegment<T> This, out T item1, out T item2, out T item3, out T item4,
            out T item5, out T item6)
        {
            item1 = This.Array[This.Offset + 0];
            item2 = This.Array[This.Offset + 1];
            item3 = This.Array[This.Offset + 2];
            item4 = This.Array[This.Offset + 3];
            item5 = This.Array[This.Offset + 4];
            item6 = This.Array[This.Offset + 5];
            return This.Count > 6
                ? new ArraySegment<T>(This.Array, This.Offset + 6, This.Count - 6)
                : new ArraySegment<T>(This.Array, This.Offset + This.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this ArraySegment<T> This, out T item1, out T item2, out T item3, out T item4,
            out T item5, out T item6, out T item7)
        {
            item1 = This.Array[This.Offset + 0];
            item2 = This.Array[This.Offset + 1];
            item3 = This.Array[This.Offset + 2];
            item4 = This.Array[This.Offset + 3];
            item5 = This.Array[This.Offset + 4];
            item6 = This.Array[This.Offset + 5];
            item7 = This.Array[This.Offset + 6];
            return This.Count > 7
                ? new ArraySegment<T>(This.Array, This.Offset + 7, This.Count - 7)
                : new ArraySegment<T>(This.Array, This.Offset + This.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this ArraySegment<T> This, out T item1, out T item2, out T item3, out T item4,
            out T item5, out T item6, out T item7, out T item8)
        {
            item1 = This.Array[This.Offset + 0];
            item2 = This.Array[This.Offset + 1];
            item3 = This.Array[This.Offset + 2];
            item4 = This.Array[This.Offset + 3];
            item5 = This.Array[This.Offset + 4];
            item6 = This.Array[This.Offset + 5];
            item7 = This.Array[This.Offset + 6];
            item8 = This.Array[This.Offset + 7];
            return This.Count > 8
                ? new ArraySegment<T>(This.Array, This.Offset + 8, This.Count - 8)
                : new ArraySegment<T>(This.Array, This.Offset + This.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this ArraySegment<T> This, out TResult item1, Func<T, TResult> transform)
        {
            item1 = transform(This.Array[This.Offset + 0]);
            return This.Count > 1
                ? new ArraySegment<T>(This.Array, This.Offset + 1, This.Count - 1)
                : new ArraySegment<T>(This.Array, This.Offset + This.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this ArraySegment<T> This, out TResult item1, out TResult item2, Func<T, TResult> transform)
        {
            item1 = transform(This.Array[This.Offset + 0]);
            item2 = transform(This.Array[This.Offset + 1]);
            return This.Count > 2
                ? new ArraySegment<T>(This.Array, This.Offset + 2, This.Count - 2)
                : new ArraySegment<T>(This.Array, This.Offset + This.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this ArraySegment<T> This, out TResult item1, out TResult item2, out TResult item3, Func<T, TResult> transform)
        {
            item1 = transform(This.Array[This.Offset + 0]);
            item2 = transform(This.Array[This.Offset + 1]);
            item3 = transform(This.Array[This.Offset + 2]);
            return This.Count > 3
                ? new ArraySegment<T>(This.Array, This.Offset + 3, This.Count - 3)
                : new ArraySegment<T>(This.Array, This.Offset + This.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this ArraySegment<T> This, out TResult item1, out TResult item2, out TResult item3, out TResult item4, Func<T, TResult> transform)
        {
            item1 = transform(This.Array[This.Offset + 0]);
            item2 = transform(This.Array[This.Offset + 1]);
            item3 = transform(This.Array[This.Offset + 2]);
            item4 = transform(This.Array[This.Offset + 3]);
            return This.Count > 4
                ? new ArraySegment<T>(This.Array, This.Offset + 4, This.Count - 4)
                : new ArraySegment<T>(This.Array, This.Offset + This.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this ArraySegment<T> This, out TResult item1, out TResult item2, out TResult item3, out TResult item4,
            out TResult item5, Func<T, TResult> transform)
        {
            item1 = transform(This.Array[This.Offset + 0]);
            item2 = transform(This.Array[This.Offset + 1]);
            item3 = transform(This.Array[This.Offset + 2]);
            item4 = transform(This.Array[This.Offset + 3]);
            item5 = transform(This.Array[This.Offset + 4]);
            return This.Count > 5
                ? new ArraySegment<T>(This.Array, This.Offset + 5, This.Count - 5)
                : new ArraySegment<T>(This.Array, This.Offset + This.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this ArraySegment<T> This, out TResult item1, out TResult item2, out TResult item3, out TResult item4,
            out TResult item5, out TResult item6, Func<T, TResult> transform)
        {
            item1 = transform(This.Array[This.Offset + 0]);
            item2 = transform(This.Array[This.Offset + 1]);
            item3 = transform(This.Array[This.Offset + 2]);
            item4 = transform(This.Array[This.Offset + 3]);
            item5 = transform(This.Array[This.Offset + 4]);
            item6 = transform(This.Array[This.Offset + 5]);
            return This.Count > 6
                ? new ArraySegment<T>(This.Array, This.Offset + 6, This.Count - 6)
                : new ArraySegment<T>(This.Array, This.Offset + This.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this ArraySegment<T> This, out TResult item1, out TResult item2, out TResult item3, out TResult item4,
            out TResult item5, out TResult item6, out TResult item7, Func<T, TResult> transform)
        {
            item1 = transform(This.Array[This.Offset + 0]);
            item2 = transform(This.Array[This.Offset + 1]);
            item3 = transform(This.Array[This.Offset + 2]);
            item4 = transform(This.Array[This.Offset + 3]);
            item5 = transform(This.Array[This.Offset + 4]);
            item6 = transform(This.Array[This.Offset + 5]);
            item7 = transform(This.Array[This.Offset + 6]);
            return This.Count > 7
                ? new ArraySegment<T>(This.Array, This.Offset + 7, This.Count - 7)
                : new ArraySegment<T>(This.Array, This.Offset + This.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this ArraySegment<T> This, out TResult item1, out TResult item2, out TResult item3, out TResult item4,
            out TResult item5, out TResult item6, out TResult item7, out TResult item8, Func<T, TResult> transform)
        {
            item1 = transform(This.Array[This.Offset + 0]);
            item2 = transform(This.Array[This.Offset + 1]);
            item3 = transform(This.Array[This.Offset + 2]);
            item4 = transform(This.Array[This.Offset + 3]);
            item5 = transform(This.Array[This.Offset + 4]);
            item6 = transform(This.Array[This.Offset + 5]);
            item7 = transform(This.Array[This.Offset + 6]);
            item8 = transform(This.Array[This.Offset + 7]);
            return This.Count > 8
                ? new ArraySegment<T>(This.Array, This.Offset + 8, This.Count - 8)
                : new ArraySegment<T>(This.Array, This.Offset + This.Count - 1, 0);
        }
    }
}