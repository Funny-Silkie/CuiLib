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
        private readonly string format;

        /// <summary>
        /// <see cref="DateTimeExactConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="format">フォーマット</param>
        /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
        public DateTimeExactConverter([StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format)
        {
            ThrowHelpers.ThrowIfNullOrEmpty(format);

            this.format = format;
        }

        /// <inheritdoc/>
        public DateTime Convert(string value)
        {
            return DateTime.ParseExact(value, format, null);
        }
    }
}
