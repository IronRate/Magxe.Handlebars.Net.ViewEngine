﻿using System.Collections.Generic;

namespace Magxe.Handlebars
{
    internal static class EnumerableExtensions
    {
        public static bool IsOneOf<TSource, TExpected>(this IEnumerable<TSource> source)
            where TExpected : TSource
        {
            var enumerator = source.GetEnumerator();
            enumerator.MoveNext();
            return (enumerator.Current is TExpected) && (enumerator.MoveNext() == false);
        }

        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }
    }
}
