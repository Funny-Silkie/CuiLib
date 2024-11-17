using System;
using System.Collections.Generic;

namespace CuiLib.Checkers.Implementations
{
    /// <summary>
    /// 値が等しいかどうかを検証します。
    /// </summary>
    /// <typeparam name="T">検証する値の型</typeparam>
    [Serializable]
    internal sealed class EqualToValueChecker<T> : IValueChecker<T>
    {
        private readonly IEqualityComparer<T> comparer;
        private readonly T comparison;

        /// <summary>
        /// <see cref="EqualToValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="comparer">比較を行うオブジェクト。nullで<see cref="EqualityComparer{T}.Default"/></param>
        /// <param name="comparison">比較対象</param>
        internal EqualToValueChecker(IEqualityComparer<T>? comparer, T comparison)
        {
            this.comparer = comparer ?? EqualityComparer<T>.Default;
            this.comparison = comparison;
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(T value)
        {
            if (comparer.Equals(value, comparison)) return ValueCheckState.Success;
            return ValueCheckState.AsError($"値は'{comparison}'と等しい必要があります");
        }
    }
}
