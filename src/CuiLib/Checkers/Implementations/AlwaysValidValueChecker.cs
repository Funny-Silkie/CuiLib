using System;

namespace CuiLib.Checkers.Implementations
{
    /// <summary>
    /// 常に<see cref="ValueCheckState.Success"/>を返す<see cref="IValueChecker{T}"/>の実装です。
    /// </summary>
    /// <typeparam name="T">検証する値の型</typeparam>
    [Serializable]
    internal sealed class AlwaysValidValueChecker<T> : IValueChecker<T>
    {
        /// <summary>
        /// <see cref="AlwaysValidValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        internal AlwaysValidValueChecker()
        {
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(T value) => ValueCheckState.Success;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is AlwaysValidValueChecker<T>;

        /// <inheritdoc/>
        public override int GetHashCode() => GetType().Name.GetHashCode();
    }
}
