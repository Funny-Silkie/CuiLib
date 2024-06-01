using System;

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
        /// <param name="span">使用する<see cref="ReadOnlySpan{T}"/>のインスタンス</param>
        /// <param name="index">インデックス</param>
        /// <returns><paramref name="index"/>に対応する値。存在しない場合は既定値</returns>
        public static T? GetOrDefault<T>(this ReadOnlySpan<T> span, int index)
        {
            return GetOrDefault(span, index, default!);
        }

        /// <summary>
        /// 指定したインデックスの値を取得します。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="span">使用する<see cref="ReadOnlySpan{T}"/>のインスタンス</param>
        /// <param name="index">インデックス</param>
        /// <param name="defaultValue">既定値</param>
        /// <returns><paramref name="index"/>に対応する値。存在しない場合は<paramref name="defaultValue"/></returns>
        public static T GetOrDefault<T>(this ReadOnlySpan<T> span, int index, T defaultValue)
        {
            if ((uint)index >= (uint)span.Length) return defaultValue;
            return span[index];
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
        /// <param name="span">使用する<see cref="ReadOnlySpan{T}"/>のインスタンス</param>
        /// <param name="index">インデックス</param>
        /// <param name="count">範囲</param>
        /// <param name="defaultValue">既定値</param>
        /// <returns><paramref name="index"/>と<paramref name="count"/>に対応する範囲。存在しない場合は<paramref name="defaultValue"/></returns>
        public static ReadOnlySpan<T> SliceOrDefault<T>(this ReadOnlySpan<T> span, int index, int count, ReadOnlySpan<T> defaultValue)
        {
            if (index < 0 || count < 0 || index + count > span.Length) return defaultValue;
            return span.Slice(index, count);
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
        /// <param name="span">使用する<see cref="ReadOnlySpan{T}"/>のインスタンス</param>
        /// <param name="start">開始インデックス</param>
        /// <param name="defaultRange">既定値</param>
        /// <returns><paramref name="start"/>以降の範囲。存在しない場合は<paramref name="defaultRange"/></returns>
        public static ReadOnlySpan<T> SliceOrDefault<T>(this ReadOnlySpan<T> span, int start, ReadOnlySpan<T> defaultRange)
        {
            if ((uint)start >= (uint)span.Length) return defaultRange;
            return span[start..];
        }
    }
}
