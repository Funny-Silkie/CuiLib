using System;

namespace CuiLib.Converters.Implementations
{
    /// <summary>
    /// 値の素通しを行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    /// <typeparam name="T">扱う値の型</typeparam>
    [Serializable]
    internal sealed class ThroughValueConverter<T> : IValueConverter<T, T>
    {
        /// <summary>
        /// <see cref="ThroughValueConverter{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        internal ThroughValueConverter()
        {
        }

        /// <inheritdoc/>
        public T Convert(T value) => value;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is ThroughValueConverter<T>;

        /// <inheritdoc/>
        public override int GetHashCode() => GetType().GetHashCode();
    }
}
