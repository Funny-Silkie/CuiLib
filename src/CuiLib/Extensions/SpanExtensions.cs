using System;
using System.Runtime.CompilerServices;

namespace CuiLib.Extensions
{
    /// <summary>
    /// <see cref="Span{T}"/>，<see cref="ReadOnlySpan{T}"/>の拡張を表します。
    /// </summary>
    public static class SpanExtensions
    {
        /// <summary>
        /// 指定したインデックスの値を取得します。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="span">使用する<see cref="Span{T}"/>のインスタンス</param>
        /// <param name="index">インデックス</param>
        /// <returns><paramref name="index"/>に対応する値。存在しない場合は既定値</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? GetOrDefault<T>(this Span<T> span, int index) => GetOrDefault((ReadOnlySpan<T>)span, index);

        /// <summary>
        /// 指定したインデックスの値を取得します。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="span">使用する<see cref="ReadOnlySpan{T}"/>のインスタンス</param>
        /// <param name="index">インデックス</param>
        /// <returns><paramref name="index"/>に対応する値。存在しない場合は既定値</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? GetOrDefault<T>(this ReadOnlySpan<T> span, int index)
        {
            return GetOrDefault(span, index, default!);
        }

        /// <summary>
        /// 指定したインデックスの値を取得します。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="span">使用する<see cref="Span{T}"/>のインスタンス</param>
        /// <param name="index">インデックス</param>
        /// <param name="defaultValue">既定値</param>
        /// <returns><paramref name="index"/>に対応する値。存在しない場合は<paramref name="defaultValue"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrDefault<T>(this Span<T> span, int index, T defaultValue) => GetOrDefault((ReadOnlySpan<T>)span, index, defaultValue);

        /// <summary>
        /// 指定したインデックスの値を取得します。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="span">使用する<see cref="ReadOnlySpan{T}"/>のインスタンス</param>
        /// <param name="index">インデックス</param>
        /// <param name="defaultValue">既定値</param>
        /// <returns><paramref name="index"/>に対応する値。存在しない場合は<paramref name="defaultValue"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrDefault<T>(this ReadOnlySpan<T> span, int index, T defaultValue)
        {
            if ((uint)index >= (uint)span.Length) return defaultValue;
            return span[index];
        }

        /// <summary>
        /// 指定した範囲の値を取得します。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="span">使用する<see cref="Span{T}"/>のインスタンス</param>
        /// <param name="index">インデックス</param>
        /// <param name="count">範囲</param>
        /// <returns><paramref name="index"/>と<paramref name="count"/>に対応する範囲。存在しない場合は<see cref="Span{T}.Empty"/></returns>
        public static Span<T> SliceOrDefault<T>(this Span<T> span, int index, int count)
        {
            return SliceOrDefault(span, index, count, []);
        }

        /// <summary>
        /// 指定した範囲の値を取得します。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="span">使用する<see cref="ReadOnlySpan{T}"/>のインスタンス</param>
        /// <param name="index">インデックス</param>
        /// <param name="count">範囲</param>
        /// <returns><paramref name="index"/>と<paramref name="count"/>に対応する範囲。存在しない場合は<see cref="ReadOnlySpan{T}.Empty"/></returns>
        public static ReadOnlySpan<T> SliceOrDefault<T>(this ReadOnlySpan<T> span, int index, int count)
        {
            return SliceOrDefault(span, index, count, []);
        }

        /// <summary>
        /// 指定した範囲の値を取得します。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="span">使用する<see cref="Span{T}"/>のインスタンス</param>
        /// <param name="index">インデックス</param>
        /// <param name="count">範囲</param>
        /// <param name="defaultValue">既定値</param>
        /// <returns><paramref name="index"/>と<paramref name="count"/>に対応する範囲。存在しない場合は<paramref name="defaultValue"/></returns>
        public static Span<T> SliceOrDefault<T>(this Span<T> span, int index, int count, Span<T> defaultValue)
        {
            if (count == 0) return [];
            if (index < 0) index = 0;
            if (index + count > span.Length) count = span.Length - index;
            if (count <= 0) return defaultValue;
            return span.Slice(index, count);
        }

        /// <summary>
        /// 指定した範囲の値を取得します。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="span">使用する<see cref="ReadOnlySpan{T}"/>のインスタンス</param>
        /// <param name="index">インデックス</param>
        /// <param name="count">範囲</param>
        /// <param name="defaultValue">既定値</param>
        /// <returns><paramref name="index"/>と<paramref name="count"/>に対応する範囲。存在しない場合は<paramref name="defaultValue"/></returns>
        public static ReadOnlySpan<T> SliceOrDefault<T>(this ReadOnlySpan<T> span, int index, int count, ReadOnlySpan<T> defaultValue)
        {
            if (count == 0) return [];
            if (index < 0) index = 0;
            if (index + count > span.Length) count = span.Length - index;
            if (count <= 0) return defaultValue;
            return span.Slice(index, count);
        }

        /// <summary>
        /// 指定した範囲の値を取得します。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="span">使用する<see cref="Span{T}"/>のインスタンス</param>
        /// <param name="start">開始インデックス</param>
        /// <returns><paramref name="start"/>以降の範囲。存在しない場合は<see cref="Span{T}.Empty"/></returns>
        public static Span<T> SliceOrDefault<T>(this Span<T> span, int start)
        {
            return SliceOrDefault(span, start, []);
        }

        /// <summary>
        /// 指定した範囲の値を取得します。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="span">使用する<see cref="ReadOnlySpan{T}"/>のインスタンス</param>
        /// <param name="start">開始インデックス</param>
        /// <returns><paramref name="start"/>以降の範囲。存在しない場合は<see cref="ReadOnlySpan{T}.Empty"/></returns>
        public static ReadOnlySpan<T> SliceOrDefault<T>(this ReadOnlySpan<T> span, int start)
        {
            return SliceOrDefault(span, start, []);
        }

        /// <summary>
        /// 指定した範囲の値を取得します。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="span">使用する<see cref="Span{T}"/>のインスタンス</param>
        /// <param name="start">開始インデックス</param>
        /// <param name="defaultRange">既定値</param>
        /// <returns><paramref name="start"/>以降の範囲。存在しない場合は<paramref name="defaultRange"/></returns>
        public static Span<T> SliceOrDefault<T>(this Span<T> span, int start, Span<T> defaultRange)
        {
            if (start < 0) start = 0;
            if (start >= span.Length) return defaultRange;
            return span[start..];
        }

        /// <summary>
        /// 指定した範囲の値を取得します。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="span">使用する<see cref="ReadOnlySpan{T}"/>のインスタンス</param>
        /// <param name="start">開始インデックス</param>
        /// <param name="defaultRange">既定値</param>
        /// <returns><paramref name="start"/>以降の範囲。存在しない場合は<paramref name="defaultRange"/></returns>
        public static ReadOnlySpan<T> SliceOrDefault<T>(this ReadOnlySpan<T> span, int start, ReadOnlySpan<T> defaultRange)
        {
            if (start < 0) start = 0;
            if (start >= span.Length) return defaultRange;
            return span[start..];
        }
    }
}
