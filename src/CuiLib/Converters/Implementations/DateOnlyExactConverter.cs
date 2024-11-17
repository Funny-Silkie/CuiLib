using System;
using System.Diagnostics.CodeAnalysis;

namespace CuiLib.Converters.Implementations
{
#if NET6_0_OR_GREATER

    /// <summary>
    /// フォーマット付きで<see cref="DateOnly"/>への変換を行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class DateOnlyExactConverter : IValueConverter<string, DateOnly>
    {
        private readonly string format;

        /// <summary>
        /// <see cref="DateOnlyExactConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="format">フォーマット</param>
        /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
        public DateOnlyExactConverter([StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format)
        {
            ThrowHelpers.ThrowIfNullOrEmpty(format);

            this.format = format;
        }

        /// <inheritdoc/>
        public DateOnly Convert(string value)
        {
            return DateOnly.ParseExact(value, format, null);
        }
    }

#endif
}
