using System;
using System.Collections.Generic;
using System.IO;

namespace CuiLib.Internal
{
    /// <summary>
    /// バージョン互換を保つための拡張のクラスです。
    /// </summary>
    internal static class VersionBufferExtensions
    {
#if NETSTANDARD2_0

        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
        {
            key = pair.Key;
            value = pair.Value;
        }

        public static bool Remove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue removed)
        {
            if (!dictionary.TryGetValue(key, out removed)) return false;
            dictionary.Remove(key);
            return true;
        }

        public static string[] Split(this string self, string separator)
        {
            return self.Split([separator], StringSplitOptions.None);
        }

        public static string[] Split(this string self, string separator, StringSplitOptions options)
        {
            return self.Split([separator], options);
        }

        public static bool StartsWith(this string self, char value) => self[0] == value;

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) => new HashSet<T>(source);

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer) => new HashSet<T>(source, comparer);

        public static void Write(this TextWriter writer, ReadOnlySpan<char> value)
        {
            writer.Write(value.ToString());
        }

#endif
    }
}
