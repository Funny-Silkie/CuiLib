using System;
using System.Diagnostics.CodeAnalysis;

namespace CuiLib.Converters.Implementations
{
#if NET6_0_OR_GREATER

    /// <summary>
    /// フォーマット付きで<see cref="TimeOnly"/>への変換を行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class TimeOnlyExactConverter : IValueConverter<string, TimeOnly>
    {
        private readonly string format;

        /// <summary>
        /// <see cref="TimeOnlyExactConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="format">フォーマット</param>
        /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
        public TimeOnlyExactConverter([StringSyntax(StringSyntaxAttribute.TimeOnlyFormat)] string format)
        {
            ThrowHelpers.ThrowIfNullOrEmpty(format);

            this.format = format;
        }

        /// <inheritdoc/>
        public TimeOnly Convert(string value)
        {
            return TimeOnly.ParseExact(value, format, null);
        }
    }

#endif
}
