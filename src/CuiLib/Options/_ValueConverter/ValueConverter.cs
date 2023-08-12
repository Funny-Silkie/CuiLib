using System;

namespace CuiLib.Options
{
    /// <summary>
    /// 値の変換を行うクラスです。
    /// </summary>
    /// <typeparam name="TIn">変換前の型</typeparam>
    /// <typeparam name="TOut">変換後の型</typeparam>
    [Obsolete($"{nameof(IValueConverter<TIn, TOut>)}を代わりに使用してください")]
    [Serializable]
    public abstract class ValueConverter<TIn, TOut> : IValueConverter<TIn, TOut>
    {
        /// <summary>
        /// <see cref="ValueConverter{TIn, TOut}"/>の新しいインスタンスを初期化します。
        /// </summary>
        protected ValueConverter()
        {
        }

        /// <inheritdoc/>
        public abstract TOut Convert(TIn value);
    }
}
