using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace CuiLib.Internal
{
    /// <summary>
    /// バージョン互換を保つための拡張のクラスです。
    /// </summary>
    internal static class VersionBufferExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string CreateString<TState>(int length, TState state, SpanAction<char, TState> action)
        {
#if NETSTANDARD2_1 || NET
            return string.Create(length, state, action);
#else
            ThrowHelpers.ThrowIfNull(action);
            ThrowHelpers.ThrowIfNegative(length);
            if (length == 0) return string.Empty;

            var result = new string('\0', length);
            unsafe
            {
                fixed (char* ptr = result)
                {
                    var span = new Span<char>(ptr, length);
                    action.Invoke(span, state);
                }
            }
            return result;
#endif
        }

#if NETSTANDARD

        public static bool TryGetNonEnumeratedCount<T>(this IEnumerable<T> source, out int count)
        {
            if (source is ICollection<T> gc)
            {
                count = gc.Count;
                return true;
            }
            if (source is IReadOnlyCollection<T> grc)
            {
                count = grc.Count;
                return true;
            }
            if (source is ICollection c)
            {
                count = c.Count;
                return true;
            }

            count = 0;
            return false;
        }

#endif

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

#if NETSTANDARD2_0

    internal delegate void SpanAction<T, in TArg>(Span<T> span, TArg arg);

    internal delegate void ReadOnlySpanAction<T, in TArg>(ReadOnlySpan<T> span, TArg arg);

#endif
}
