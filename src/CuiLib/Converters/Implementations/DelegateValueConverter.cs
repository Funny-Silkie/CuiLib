using System;

namespace CuiLib.Converters.Implementations
{
    /// <summary>
    /// デリゲートで変換を行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    /// <typeparam name="TIn">変換前の型</typeparam>
    /// <typeparam name="TOut">変換後の型</typeparam>
    [Serializable]
    internal sealed class DelegateValueConverter<TIn, TOut> : IValueConverter<TIn, TOut>
    {
        private readonly Converter<TIn, TOut> converter;

        /// <summary>
        /// <see cref="DelegateValueConverter{TIn, TOut}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="converter">使用するデリゲート</param>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/>がnull</exception>
        internal DelegateValueConverter(Converter<TIn, TOut> converter)
        {
            ThrowHelpers.ThrowIfNull(converter);

            this.converter = converter;
        }

        /// <inheritdoc/>
        public TOut Convert(TIn value)
        {
            return converter.Invoke(value);
        }
    }
}
