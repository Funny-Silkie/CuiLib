using System;
using System.Collections.Generic;

namespace CuiLib.Checkers.Implementations
{
    /// <summary>
    /// 値が異なるかどうかを検証します。
    /// </summary>
    /// <typeparam name="T">検証する値の型</typeparam>
    [Serializable]
    internal sealed class NotEqualToValueChecker<T> : IValueChecker<T>
    {
        private readonly IEqualityComparer<T> comparer;
        private readonly T comparison;

        /// <summary>
        /// <see cref="NotEqualToValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="comparer">比較を行うオブジェクト。nullで<see cref="EqualityComparer{T}.Default"/></param>
        /// <param name="comparison">比較対象</param>
        internal NotEqualToValueChecker(IEqualityComparer<T>? comparer, T comparison)
        {
            this.comparer = comparer ?? EqualityComparer<T>.Default;
            this.comparison = comparison;
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(T value)
        {
            if (!comparer.Equals(value, comparison)) return ValueCheckState.Success;
            return ValueCheckState.AsError($"値は'{comparison}'と異なる必要があります");
        }
    }
}
