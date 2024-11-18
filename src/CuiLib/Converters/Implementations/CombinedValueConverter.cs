using System;

namespace CuiLib.Converters.Implementations
{
    /// <summary>
    /// 結合された<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    /// <typeparam name="TIn">変換前の型</typeparam>
    /// <typeparam name="TMid">変換途上の型</typeparam>
    /// <typeparam name="TOut">変換後の型</typeparam>
    [Serializable]
    internal sealed class CombinedValueConverter<TIn, TMid, TOut> : IValueConverter<TIn, TOut>
    {
        private readonly IValueConverter<TIn, TMid> first;
        private readonly IValueConverter<TMid, TOut> second;

        /// <summary>
        /// <see cref="CombinedValueConverter{TIn, TMid, TOut}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="first">最初に変換を行うインスタンス</param>
        /// <param name="second">次に変換を行うインスタンス</param>
        /// <exception cref="ArgumentNullException"><paramref name="first"/>または<paramref name="second"/>がnull</exception>
        internal CombinedValueConverter(IValueConverter<TIn, TMid> first, IValueConverter<TMid, TOut> second)
        {
            ThrowHelpers.ThrowIfNull(first);
            ThrowHelpers.ThrowIfNull(second);

            this.first = first;
            this.second = second;
        }

        /// <inheritdoc/>
        public TOut Convert(TIn value)
        {
            TMid intermidiate = first.Convert(value);
            return second.Convert(intermidiate);
        }
    }
}
