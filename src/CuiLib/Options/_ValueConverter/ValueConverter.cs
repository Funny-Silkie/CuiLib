using System;

namespace CuiLib.Options
{
    /// <summary>
    /// 値の変換を行うクラスです。
    /// </summary>
    /// <typeparam name="TIn">変換前の型</typeparam>
    /// <typeparam name="TOut">変換後の型</typeparam>
    [Serializable]
    public abstract class ValueConverter<TIn, TOut>
    {
        /// <summary>
        /// <see cref="ValueConverter{TIn, TOut}"/>の新しいインスタンスを初期化します。
        /// </summary>
        protected ValueConverter()
        {
        }

        /// <summary>
        /// 値の変換を行います。
        /// </summary>
        /// <param name="value">変換する値</param>
        /// <returns>変換後の値</returns>
        public abstract TOut Convert(TIn value);
    }
}
