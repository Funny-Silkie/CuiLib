using System;
using System.Diagnostics.CodeAnalysis;

namespace CuiLib.Converters.Implementations
{
    /// <summary>
    /// フォーマット付きで<see cref="DateTimeOffset"/>への変換を行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class DateTimeOffsetExactConverter : IValueConverter<string, DateTimeOffset>
    {
        private readonly string format;

        /// <summary>
        /// <see cref="DateTimeOffsetExactConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="format">フォーマット</param>
        /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
        public DateTimeOffsetExactConverter([StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format)
        {
            ThrowHelpers.ThrowIfNullOrEmpty(format);

            this.format = format;
        }

        /// <inheritdoc/>
        public DateTimeOffset Convert(string value)
        {
            return DateTimeOffset.ParseExact(value, format, null);
        }
    }
}
