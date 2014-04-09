using System;

namespace Utilities.Collections
{
    public static class ArraySegment
    {
        public static ArraySegment<T> Unpack<T>(this ArraySegment<T> source, out T item1)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source.Array[source.Offset + 0];
            return source.Count > 1
                ? new ArraySegment<T>(source.Array, source.Offset + 1, source.Count - 1)
                : new ArraySegment<T>(source.Array, source.Offset + source.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this ArraySegment<T> source, out T item1, out T item2)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source.Array[source.Offset + 0];
            item2 = source.Array[source.Offset + 1];
            return source.Count > 2
                ? new ArraySegment<T>(source.Array, source.Offset + 2, source.Count - 2)
                : new ArraySegment<T>(source.Array, source.Offset + source.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this ArraySegment<T> source, out T item1, out T item2, out T item3)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source.Array[source.Offset + 0];
            item2 = source.Array[source.Offset + 1];
            item3 = source.Array[source.Offset + 2];
            return source.Count > 3
                ? new ArraySegment<T>(source.Array, source.Offset + 3, source.Count - 3)
                : new ArraySegment<T>(source.Array, source.Offset + source.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this ArraySegment<T> source, out T item1, out T item2, out T item3, out T item4)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source.Array[source.Offset + 0];
            item2 = source.Array[source.Offset + 1];
            item3 = source.Array[source.Offset + 2];
            item4 = source.Array[source.Offset + 3];
            return source.Count > 4
                ? new ArraySegment<T>(source.Array, source.Offset + 4, source.Count - 4)
                : new ArraySegment<T>(source.Array, source.Offset + source.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this ArraySegment<T> source, out T item1, out T item2, out T item3, out T item4,
            out T item5)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source.Array[source.Offset + 0];
            item2 = source.Array[source.Offset + 1];
            item3 = source.Array[source.Offset + 2];
            item4 = source.Array[source.Offset + 3];
            item5 = source.Array[source.Offset + 4];
            return source.Count > 5
                ? new ArraySegment<T>(source.Array, source.Offset + 5, source.Count - 5)
                : new ArraySegment<T>(source.Array, source.Offset + source.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this ArraySegment<T> source, out T item1, out T item2, out T item3, out T item4,
            out T item5, out T item6)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source.Array[source.Offset + 0];
            item2 = source.Array[source.Offset + 1];
            item3 = source.Array[source.Offset + 2];
            item4 = source.Array[source.Offset + 3];
            item5 = source.Array[source.Offset + 4];
            item6 = source.Array[source.Offset + 5];
            return source.Count > 6
                ? new ArraySegment<T>(source.Array, source.Offset + 6, source.Count - 6)
                : new ArraySegment<T>(source.Array, source.Offset + source.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this ArraySegment<T> source, out T item1, out T item2, out T item3, out T item4,
            out T item5, out T item6, out T item7)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source.Array[source.Offset + 0];
            item2 = source.Array[source.Offset + 1];
            item3 = source.Array[source.Offset + 2];
            item4 = source.Array[source.Offset + 3];
            item5 = source.Array[source.Offset + 4];
            item6 = source.Array[source.Offset + 5];
            item7 = source.Array[source.Offset + 6];
            return source.Count > 7
                ? new ArraySegment<T>(source.Array, source.Offset + 7, source.Count - 7)
                : new ArraySegment<T>(source.Array, source.Offset + source.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this ArraySegment<T> source, out T item1, out T item2, out T item3, out T item4,
            out T item5, out T item6, out T item7, out T item8)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source.Array[source.Offset + 0];
            item2 = source.Array[source.Offset + 1];
            item3 = source.Array[source.Offset + 2];
            item4 = source.Array[source.Offset + 3];
            item5 = source.Array[source.Offset + 4];
            item6 = source.Array[source.Offset + 5];
            item7 = source.Array[source.Offset + 6];
            item8 = source.Array[source.Offset + 7];
            return source.Count > 8
                ? new ArraySegment<T>(source.Array, source.Offset + 8, source.Count - 8)
                : new ArraySegment<T>(source.Array, source.Offset + source.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this ArraySegment<T> source, out TResult item1, Func<T, TResult> transform)
        {
            if (source == null) throw new ArgumentNullException("source");
			if (transform == null) throw new ArgumentNullException("transform");
			item1 = transform(source.Array[source.Offset + 0]);
            return source.Count > 1
                ? new ArraySegment<T>(source.Array, source.Offset + 1, source.Count - 1)
                : new ArraySegment<T>(source.Array, source.Offset + source.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this ArraySegment<T> source, out TResult item1, out TResult item2, Func<T, TResult> transform)
        {
            if (source == null) throw new ArgumentNullException("source");
			if (transform == null) throw new ArgumentNullException("transform");
			item1 = transform(source.Array[source.Offset + 0]);
            item2 = transform(source.Array[source.Offset + 1]);
            return source.Count > 2
                ? new ArraySegment<T>(source.Array, source.Offset + 2, source.Count - 2)
                : new ArraySegment<T>(source.Array, source.Offset + source.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this ArraySegment<T> source, out TResult item1, out TResult item2, out TResult item3, Func<T, TResult> transform)
        {
            if (source == null) throw new ArgumentNullException("source");
			if (transform == null) throw new ArgumentNullException("transform");
			item1 = transform(source.Array[source.Offset + 0]);
            item2 = transform(source.Array[source.Offset + 1]);
            item3 = transform(source.Array[source.Offset + 2]);
            return source.Count > 3
                ? new ArraySegment<T>(source.Array, source.Offset + 3, source.Count - 3)
                : new ArraySegment<T>(source.Array, source.Offset + source.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this ArraySegment<T> source, out TResult item1, out TResult item2, out TResult item3, out TResult item4, Func<T, TResult> transform)
        {
            if (source == null) throw new ArgumentNullException("source");
			if (transform == null) throw new ArgumentNullException("transform");
			item1 = transform(source.Array[source.Offset + 0]);
            item2 = transform(source.Array[source.Offset + 1]);
            item3 = transform(source.Array[source.Offset + 2]);
            item4 = transform(source.Array[source.Offset + 3]);
            return source.Count > 4
                ? new ArraySegment<T>(source.Array, source.Offset + 4, source.Count - 4)
                : new ArraySegment<T>(source.Array, source.Offset + source.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this ArraySegment<T> source, out TResult item1, out TResult item2, out TResult item3, out TResult item4,
            out TResult item5, Func<T, TResult> transform)
        {
            if (source == null) throw new ArgumentNullException("source");
			if (transform == null) throw new ArgumentNullException("transform");
			item1 = transform(source.Array[source.Offset + 0]);
            item2 = transform(source.Array[source.Offset + 1]);
            item3 = transform(source.Array[source.Offset + 2]);
            item4 = transform(source.Array[source.Offset + 3]);
            item5 = transform(source.Array[source.Offset + 4]);
            return source.Count > 5
                ? new ArraySegment<T>(source.Array, source.Offset + 5, source.Count - 5)
                : new ArraySegment<T>(source.Array, source.Offset + source.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this ArraySegment<T> source, out TResult item1, out TResult item2, out TResult item3, out TResult item4,
            out TResult item5, out TResult item6, Func<T, TResult> transform)
        {
            if (source == null) throw new ArgumentNullException("source");
			if (transform == null) throw new ArgumentNullException("transform");
			item1 = transform(source.Array[source.Offset + 0]);
            item2 = transform(source.Array[source.Offset + 1]);
            item3 = transform(source.Array[source.Offset + 2]);
            item4 = transform(source.Array[source.Offset + 3]);
            item5 = transform(source.Array[source.Offset + 4]);
            item6 = transform(source.Array[source.Offset + 5]);
            return source.Count > 6
                ? new ArraySegment<T>(source.Array, source.Offset + 6, source.Count - 6)
                : new ArraySegment<T>(source.Array, source.Offset + source.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this ArraySegment<T> source, out TResult item1, out TResult item2, out TResult item3, out TResult item4,
            out TResult item5, out TResult item6, out TResult item7, Func<T, TResult> transform)
        {
            if (source == null) throw new ArgumentNullException("source");
			if (transform == null) throw new ArgumentNullException("transform");
			item1 = transform(source.Array[source.Offset + 0]);
            item2 = transform(source.Array[source.Offset + 1]);
            item3 = transform(source.Array[source.Offset + 2]);
            item4 = transform(source.Array[source.Offset + 3]);
            item5 = transform(source.Array[source.Offset + 4]);
            item6 = transform(source.Array[source.Offset + 5]);
            item7 = transform(source.Array[source.Offset + 6]);
            return source.Count > 7
                ? new ArraySegment<T>(source.Array, source.Offset + 7, source.Count - 7)
                : new ArraySegment<T>(source.Array, source.Offset + source.Count - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this ArraySegment<T> source, out TResult item1, out TResult item2, out TResult item3, out TResult item4,
            out TResult item5, out TResult item6, out TResult item7, out TResult item8, Func<T, TResult> transform)
        {
            if (source == null) throw new ArgumentNullException("source");
			if (transform == null) throw new ArgumentNullException("transform");
			item1 = transform(source.Array[source.Offset + 0]);
            item2 = transform(source.Array[source.Offset + 1]);
            item3 = transform(source.Array[source.Offset + 2]);
            item4 = transform(source.Array[source.Offset + 3]);
            item5 = transform(source.Array[source.Offset + 4]);
            item6 = transform(source.Array[source.Offset + 5]);
            item7 = transform(source.Array[source.Offset + 6]);
            item8 = transform(source.Array[source.Offset + 7]);
            return source.Count > 8
                ? new ArraySegment<T>(source.Array, source.Offset + 8, source.Count - 8)
                : new ArraySegment<T>(source.Array, source.Offset + source.Count - 1, 0);
        }
    }
}