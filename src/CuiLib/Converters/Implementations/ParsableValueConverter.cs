using System;

namespace CuiLib.Converters.Implementations
{
#if NET7_0_OR_GREATER

    /// <summary>
    /// <see cref="IParsable{TSelf}"/>を変換する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    /// <typeparam name="T">変換する<see cref="IParsable{TSelf}"/>を実装する型</typeparam>
    [Serializable]
    internal sealed class ParsableValueConverter<T> : IValueConverter<string, T>
        where T : IParsable<T>
    {
        /// <summary>
        /// <see cref="ParsableValueConverter{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        internal ParsableValueConverter()
        {
        }

        /// <inheritdoc/>
        public T Convert(string value) => T.Parse(value, null);

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is ParsableValueConverter<T>;

        /// <inheritdoc/>
        public override int GetHashCode() => GetType().GetHashCode();
    }

#endif
}
