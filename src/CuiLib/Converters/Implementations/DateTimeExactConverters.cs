using System;
using System.Diagnostics.CodeAnalysis;

namespace CuiLib.Converters.Implementations
{
    /// <summary>
    /// フォーマット付きで<see cref="DateTime"/>への変換を行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class DateTimeExactConverter : IValueConverter<string, DateTime>
    {
        /// <summary>
        /// フォーマットを取得します。
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// <see cref="DateTimeExactConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="format">フォーマット</param>
        /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
        public DateTimeExactConverter([StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format)
        {
            ThrowHelpers.ThrowIfNullOrEmpty(format);

            Format = format;
        }

        /// <inheritdoc/>
        public DateTime Convert(string value) => DateTime.ParseExact(value, Format, null);
    }

#if NET6_0_OR_GREATER

    /// <summary>
    /// フォーマット付きで<see cref="DateOnly"/>への変換を行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class DateOnlyExactConverter : IValueConverter<string, DateOnly>
    {
        /// <summary>
        /// フォーマットを取得します。
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// <see cref="DateOnlyExactConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="format">フォーマット</param>
        /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
        public DateOnlyExactConverter([StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format)
        {
            ThrowHelpers.ThrowIfNullOrEmpty(format);

            Format = format;
        }

        /// <inheritdoc/>
        public DateOnly Convert(string value) => DateOnly.ParseExact(value, Format, null);
    }

    /// <summary>
    /// フォーマット付きで<see cref="TimeOnly"/>への変換を行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class TimeOnlyExactConverter : IValueConverter<string, TimeOnly>
    {
        /// <summary>
        /// フォーマットを取得します。
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// <see cref="TimeOnlyExactConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="format">フォーマット</param>
        /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
        public TimeOnlyExactConverter([StringSyntax(StringSyntaxAttribute.TimeOnlyFormat)] string format)
        {
            ThrowHelpers.ThrowIfNullOrEmpty(format);

            Format = format;
        }

        /// <inheritdoc/>
        public TimeOnly Convert(string value) => TimeOnly.ParseExact(value, Format, null);
    }

#endif

    /// <summary>
    /// フォーマット付きで<see cref="TimeSpan"/>への変換を行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class TimeSpanExactConverter : IValueConverter<string, TimeSpan>
    {
        /// <summary>
        /// フォーマットを取得します。
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// <see cref="TimeSpanExactConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="format">フォーマット</param>
        /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
        public TimeSpanExactConverter([StringSyntax(StringSyntaxAttribute.TimeSpanFormat)] string format)
        {
            ThrowHelpers.ThrowIfNullOrEmpty(format);

            Format = format;
        }

        /// <inheritdoc/>
        public TimeSpan Convert(string value) => TimeSpan.ParseExact(value, Format, null);
    }

    /// <summary>
    /// フォーマット付きで<see cref="DateTimeOffset"/>への変換を行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class DateTimeOffsetExactConverter : IValueConverter<string, DateTimeOffset>
    {
        /// <summary>
        /// フォーマットを取得します。
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// <see cref="DateTimeOffsetExactConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="format">フォーマット</param>
        /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
        public DateTimeOffsetExactConverter([StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format)
        {
            ThrowHelpers.ThrowIfNullOrEmpty(format);

            Format = format;
        }

        /// <inheritdoc/>
        public DateTimeOffset Convert(string value) => DateTimeOffset.ParseExact(value, Format, null);
    }
}
