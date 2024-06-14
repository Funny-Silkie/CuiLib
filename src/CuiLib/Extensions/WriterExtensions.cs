using CuiLib.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace CuiLib.Extensions
{
    /// <summary>
    /// <see cref="TextWriter"/>の拡張を表します。
    /// </summary>
    public static class WriterExtensions
    {
        /// <summary>
        /// 文字列を結合して出力します。
        /// </summary>
        /// <typeparam name="T">シーケンスの要素の型</typeparam>
        /// <param name="writer">使用する<see cref="TextWriter"/>のインスタンス</param>
        /// <param name="separator">区切り文字</param>
        /// <param name="values">結合するシーケンス</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>または<paramref name="values"/>がnull</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        public static void WriteJoin<T>(this TextWriter writer, char separator, IEnumerable<T> values)
        {
            ReadOnlySpan<char> separatorSpan;
#if NETSTANDARD2_1_OR_GREATER || NET
            separatorSpan = MemoryMarshal.CreateReadOnlySpan(ref separator, 1);
#else
            unsafe
            {
                separatorSpan = new ReadOnlySpan<char>(&separator, 1);
            }
#endif
            WriteJoinPrivate(writer, separatorSpan, values);
        }

        /// <summary>
        /// 文字列を結合して出力します。
        /// </summary>
        /// <typeparam name="T">シーケンスの要素の型</typeparam>
        /// <param name="writer">使用する<see cref="TextWriter"/>のインスタンス</param>
        /// <param name="separator">区切り文字列</param>
        /// <param name="values">結合するシーケンス</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>または<paramref name="values"/>がnull</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        public static void WriteJoin<T>(this TextWriter writer, string? separator, IEnumerable<T> values)
        {
            WriteJoinPrivate(writer, separator.AsSpan(), values);
        }

        /// <summary>
        /// 文字列を結合して出力します。
        /// </summary>
        /// <typeparam name="T">シーケンスの要素の型</typeparam>
        /// <param name="writer">使用する<see cref="TextWriter"/>のインスタンス</param>
        /// <param name="separator">区切り文字列</param>
        /// <param name="values">結合するシーケンス</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>または<paramref name="values"/>がnull</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        private static void WriteJoinPrivate<T>(TextWriter writer, ReadOnlySpan<char> separator, IEnumerable<T> values)
        {
            ThrowHelpers.ThrowIfNull(writer);
            ThrowHelpers.ThrowIfNull(values);

            if (values is string?[] _array)
            {
                WriteJoinPrivate(writer, separator, _array.AsSpan());
                return;
            }
#if NET6_0_OR_GREATER
            if (values is List<string?> _list)
            {
                WriteJoinPrivate(writer, separator, CollectionsMarshal.AsSpan(_list));
                return;
            }
#endif

            using IEnumerator<T> enumerator = values.GetEnumerator();
            if (!enumerator.MoveNext()) return;
            writer.Write(enumerator.Current?.ToString());

            while (enumerator.MoveNext())
            {
                writer.Write(separator);
                writer.Write(enumerator.Current?.ToString());
            }
        }

        /// <summary>
        /// 文字列を結合して出力します。
        /// </summary>s
        /// <param name="writer">使用する<see cref="TextWriter"/>のインスタンス</param>
        /// <param name="separator">区切り文字</param>
        /// <param name="values">結合する文字列一覧</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>または<paramref name="values"/>がnull</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        public static void WriteJoin(this TextWriter writer, char separator, params string?[] values)
        {
            ThrowHelpers.ThrowIfNull(values);

            ReadOnlySpan<char> separatorSpan;
#if NETSTANDARD2_1_OR_GREATER || NET
            separatorSpan = MemoryMarshal.CreateReadOnlySpan(ref separator, 1);
#else
            unsafe
            {
                separatorSpan = new ReadOnlySpan<char>(&separator, 1);
            }
#endif
            WriteJoinPrivate(writer, separatorSpan, values.AsSpan());
        }

        /// <summary>
        /// 文字列を結合して出力します。
        /// </summary>s
        /// <param name="writer">使用する<see cref="TextWriter"/>のインスタンス</param>
        /// <param name="separator">区切り文字列</param>
        /// <param name="values">結合する文字列一覧</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>または<paramref name="values"/>がnull</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        public static void WriteJoin(this TextWriter writer, string? separator, params string?[] values)
        {
            ThrowHelpers.ThrowIfNull(values);

            WriteJoinPrivate(writer, separator.AsSpan(), values.AsSpan());
        }

        /// <summary>
        /// 文字列を結合して出力します。
        /// </summary>s
        /// <param name="writer">使用する<see cref="TextWriter"/>のインスタンス</param>
        /// <param name="separator">区切り文字列</param>
        /// <param name="values">結合する文字列一覧</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>がnull</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        private static void WriteJoinPrivate(TextWriter writer, ReadOnlySpan<char> separator, ReadOnlySpan<string?> values)
        {
            ThrowHelpers.ThrowIfNull(writer);

            if (values.Length == 0) return;

            writer.Write(values[0]);
            for (int i = 1; i < values.Length; i++)
            {
                writer.Write(separator);
                writer.Write(values[i]);
            }
        }

        /// <summary>
        /// 文字列を結合して出力します。
        /// </summary>s
        /// <param name="writer">使用する<see cref="TextWriter"/>のインスタンス</param>
        /// <param name="separator">区切り文字</param>
        /// <param name="values">結合するオブジェクト一覧</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>または<paramref name="values"/>がnull</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        public static void WriteJoin(this TextWriter writer, char separator, params object?[] values)
        {
            ThrowHelpers.ThrowIfNull(values);

            ReadOnlySpan<char> separatorSpan;
#if NETSTANDARD2_1_OR_GREATER || NET
            separatorSpan = MemoryMarshal.CreateReadOnlySpan(ref separator, 1);
#else
            unsafe
            {
                separatorSpan = new ReadOnlySpan<char>(&separator, 1);
            }
#endif
            WriteJoinPrivate(writer, separatorSpan, values.AsSpan());
        }

        /// <summary>
        /// 文字列を結合して出力します。
        /// </summary>s
        /// <param name="writer">使用する<see cref="TextWriter"/>のインスタンス</param>
        /// <param name="separator">区切り文字列</param>
        /// <param name="values">結合するオブジェクト一覧</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>または<paramref name="values"/>がnull</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        public static void WriteJoin(this TextWriter writer, string? separator, params object?[] values)
        {
            ThrowHelpers.ThrowIfNull(values);

            WriteJoinPrivate(writer, separator.AsSpan(), values.AsSpan());
        }

        /// <summary>
        /// 文字列を結合して出力します。
        /// </summary>s
        /// <param name="writer">使用する<see cref="TextWriter"/>のインスタンス</param>
        /// <param name="separator">区切り文字列</param>
        /// <param name="values">結合するオブジェクト一覧</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>がnull</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        private static void WriteJoinPrivate(TextWriter writer, ReadOnlySpan<char> separator, ReadOnlySpan<object?> values)
        {
            ThrowHelpers.ThrowIfNull(writer);

            if (values.Length == 0) return;

            writer.Write(values[0]);
            for (int i = 1; i < values.Length; i++)
            {
                writer.Write(separator);
                writer.Write(values[i]?.ToString());
            }
        }
    }
}
