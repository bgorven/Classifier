using System;

namespace Utilities.Collections
{
    public static class Array
    {
        public static ArraySegment<T> Unpack<T>(this T[] source, out T item1)
        {
            if (source == null) throw new ArgumentNullException("source");
            item1 = source[0];
            return source.Length > 1
                ? new ArraySegment<T>(source, 1, source.Length - 1)
                : new ArraySegment<T>(source, source.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this T[] source, out T item1, out T item2)
        {
            if (source == null) throw new ArgumentNullException("source");
            item1 = source[0];
            item2 = source[1];
            return source.Length > 2
                ? new ArraySegment<T>(source, 2, source.Length - 2)
                : new ArraySegment<T>(source, source.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this T[] source, out T item1, out T item2, out T item3)
        {
            if (source == null) throw new ArgumentNullException("source");
            item1 = source[0];
            item2 = source[1];
            item3 = source[2];
            return source.Length > 3
                ? new ArraySegment<T>(source, 3, source.Length - 3)
                : new ArraySegment<T>(source, source.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this T[] source, out T item1, out T item2, out T item3, out T item4)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source[0];
            item2 = source[1];
            item3 = source[2];
            item4 = source[3];
            return source.Length > 4
                ? new ArraySegment<T>(source, 4, source.Length - 4)
                : new ArraySegment<T>(source, source.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this T[] source, out T item1, out T item2, out T item3, out T item4,
            out T item5)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source[0];
            item2 = source[1];
            item3 = source[2];
            item4 = source[3];
            item5 = source[4];
            return source.Length > 5
                ? new ArraySegment<T>(source, 5, source.Length - 5)
                : new ArraySegment<T>(source, source.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this T[] source, out T item1, out T item2, out T item3, out T item4,
            out T item5, out T item6)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source[0];
            item2 = source[1];
            item3 = source[2];
            item4 = source[3];
            item5 = source[4];
            item6 = source[5];
            return source.Length > 6
                ? new ArraySegment<T>(source, 6, source.Length - 6)
                : new ArraySegment<T>(source, source.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this T[] source, out T item1, out T item2, out T item3, out T item4,
            out T item5, out T item6, out T item7)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source[0];
            item2 = source[1];
            item3 = source[2];
            item4 = source[3];
            item5 = source[4];
            item6 = source[5];
            item7 = source[6];
            return source.Length > 7
                ? new ArraySegment<T>(source, 7, source.Length - 7)
                : new ArraySegment<T>(source, source.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T>(this T[] source, out T item1, out T item2, out T item3, out T item4,
            out T item5, out T item6, out T item7, out T item8)
        {
            if (source == null) throw new ArgumentNullException("source");
			item1 = source[0];
            item2 = source[1];
            item3 = source[2];
            item4 = source[3];
            item5 = source[4];
            item6 = source[5];
            item7 = source[6];
            item8 = source[7];
            return source.Length > 8
                ? new ArraySegment<T>(source, 8, source.Length - 8)
                : new ArraySegment<T>(source, source.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this T[] source, out TResult item1, Func<T, TResult> transform)
        {
            if (source == null) throw new ArgumentNullException("source");
			if (transform == null) throw new ArgumentNullException("transform");
			item1 = transform(source[0]);
            return source.Length > 1
                ? new ArraySegment<T>(source, 1, source.Length - 1)
                : new ArraySegment<T>(source, source.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this T[] source, out TResult item1, out TResult item2, Func<T, TResult> transform)
        {
            if (source == null) throw new ArgumentNullException("source");
			if (transform == null) throw new ArgumentNullException("transform");
			item1 = transform(source[0]);
            item2 = transform(source[1]);
            return source.Length > 2
                ? new ArraySegment<T>(source, 2, source.Length - 2)
                : new ArraySegment<T>(source, source.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this T[] source, out TResult item1, out TResult item2, out TResult item3, Func<T, TResult> transform)
        {
            if (source == null) throw new ArgumentNullException("source");
			if (transform == null) throw new ArgumentNullException("transform");
			item1 = transform(source[0]);
            item2 = transform(source[1]);
            item3 = transform(source[2]);
            return source.Length > 3
                ? new ArraySegment<T>(source, 3, source.Length - 3)
                : new ArraySegment<T>(source, source.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this T[] source, out TResult item1, out TResult item2, out TResult item3, out TResult item4, Func<T, TResult> transform)
        {
            if (source == null) throw new ArgumentNullException("source");
			if (transform == null) throw new ArgumentNullException("transform");
			item1 = transform(source[0]);
            item2 = transform(source[1]);
            item3 = transform(source[2]);
            item4 = transform(source[3]);
            return source.Length > 4
                ? new ArraySegment<T>(source, 4, source.Length - 4)
                : new ArraySegment<T>(source, source.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this T[] source, out TResult item1, out TResult item2, out TResult item3, out TResult item4,
            out TResult item5, Func<T, TResult> transform)
        {
            if (source == null) throw new ArgumentNullException("source");
			if (transform == null) throw new ArgumentNullException("transform");
			item1 = transform(source[0]);
            item2 = transform(source[1]);
            item3 = transform(source[2]);
            item4 = transform(source[3]);
            item5 = transform(source[4]);
            return source.Length > 5
                ? new ArraySegment<T>(source, 5, source.Length - 5)
                : new ArraySegment<T>(source, source.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this T[] source, out TResult item1, out TResult item2, out TResult item3, out TResult item4,
            out TResult item5, out TResult item6, Func<T, TResult> transform)
        {
            if (source == null) throw new ArgumentNullException("source");
			if (transform == null) throw new ArgumentNullException("transform");
			item1 = transform(source[0]);
            item2 = transform(source[1]);
            item3 = transform(source[2]);
            item4 = transform(source[3]);
            item5 = transform(source[4]);
            item6 = transform(source[5]);
            return source.Length > 6
                ? new ArraySegment<T>(source, 6, source.Length - 6)
                : new ArraySegment<T>(source, source.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this T[] source, out TResult item1, out TResult item2, out TResult item3, out TResult item4,
            out TResult item5, out TResult item6, out TResult item7, Func<T, TResult> transform)
        {
            if (source == null) throw new ArgumentNullException("source");
			if (transform == null) throw new ArgumentNullException("transform");
			item1 = transform(source[0]);
            item2 = transform(source[1]);
            item3 = transform(source[2]);
            item4 = transform(source[3]);
            item5 = transform(source[4]);
            item6 = transform(source[5]);
            item7 = transform(source[6]);
            return source.Length > 7
                ? new ArraySegment<T>(source, 7, source.Length - 7)
                : new ArraySegment<T>(source, source.Length - 1, 0);
        }

        public static ArraySegment<T> Unpack<T, TResult>(this T[] source, out TResult item1, out TResult item2, out TResult item3, out TResult item4,
            out TResult item5, out TResult item6, out TResult item7, out TResult item8, Func<T, TResult> transform)
        {
            if (source == null) throw new ArgumentNullException("source");
			if (transform == null) throw new ArgumentNullException("transform");
			item1 = transform(source[0]);
            item2 = transform(source[1]);
            item3 = transform(source[2]);
            item4 = transform(source[3]);
            item5 = transform(source[4]);
            item6 = transform(source[5]);
            item7 = transform(source[6]);
            item8 = transform(source[7]);
            return source.Length > 8
                ? new ArraySegment<T>(source, 8, source.Length - 8)
                : new ArraySegment<T>(source, source.Length - 1, 0);
        }
    }
}
