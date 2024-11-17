using System;

namespace CuiLib.Converters.Implementations
{
    /// <summary>
    /// 列挙型の変換を行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    /// <typeparam name="T">列挙型の型</typeparam>
    [Serializable]
    internal sealed class EnumValueConverter<T> : IValueConverter<string, T>
        where T : struct, Enum
    {
        private readonly bool ignoreCase;

        /// <summary>
        /// <see cref="EnumValueConverter{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="ignoreCase">文字列の大文字小文字の区別を無視するかどうか</param>
        internal EnumValueConverter(bool ignoreCase)
        {
            this.ignoreCase = ignoreCase;
        }

        /// <inheritdoc/>
        public T Convert(string value)
        {
#if NETSTANDARD2_1_OR_GREATER || NET
            return Enum.Parse<T>(value, ignoreCase);
#else
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
#endif
        }
    }
}
