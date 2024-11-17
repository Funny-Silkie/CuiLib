using System;
using System.Diagnostics.CodeAnalysis;

namespace CuiLib.Converters.Implementations
{
    /// <summary>
    /// フォーマット付きで<see cref="TimeSpan"/>への変換を行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class TimeSpanExactConverter : IValueConverter<string, TimeSpan>
    {
        private readonly string format;

        /// <summary>
        /// <see cref="TimeSpanExactConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="format">フォーマット</param>
        /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
        public TimeSpanExactConverter([StringSyntax(StringSyntaxAttribute.TimeSpanFormat)] string format)
        {
            ThrowHelpers.ThrowIfNullOrEmpty(format);

            this.format = format;
        }

        /// <inheritdoc/>
        public TimeSpan Convert(string value)
        {
            return TimeSpan.ParseExact(value, format, null);
        }
    }
}
