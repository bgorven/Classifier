﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.Collections
{
    public static class Enumerable
    {
        /// <summary>
        /// Iterates the IEnumerable immediately.
        /// </summary>
        /// <param name="collection">The sequence to iterate.</param>
        public static void Apply<T>(this IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            foreach (var element in collection) { }
        }

        /// <summary>
        /// Iterates the IEnumerable immediately, executing the specified action for each element.
        /// </summary>
        /// <param name="collection">The sequence to iterate.</param>
        /// <param name="action">An action to be executed for each element.</param>
        public static void Apply<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (action == null) throw new ArgumentNullException("action");
            foreach (var element in collection)
            {
                action(element);
            }
        }

        /// <summary>
        /// Applies the specified action to a merged sequence.
        /// </summary>
        /// <param name="first">The first collection to merge.</param>
        /// <param name="second">The second collection to merge.</param>
        /// <param name="action">The action to be applied to elements of each sequence.</param>
        public static void Zip<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second,
            Action<T1, T2> action)
        {
            if (first == null || second == null) throw new ArgumentNullException(first == null? "first" : "second");
            if (action == null) throw new ArgumentNullException("action");
            first.Zip(second, (f, s) =>
            {
                action(f, s);
                return false;
            }).Apply();
        }
    }
}
