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
        /// <summary>
        /// 比較を行う<see cref="IEqualityComparer{T}"/>のインスタンスを取得します。
        /// </summary>
        public IEqualityComparer<T> Comparer { get; }

        /// <summary>
        /// 比較対象の値を取得します。
        /// </summary>
        public T Comparison { get; }

        /// <summary>
        /// <see cref="NotEqualToValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="comparer">比較を行うオブジェクト。nullで<see cref="EqualityComparer{T}.Default"/></param>
        /// <param name="comparison">比較対象</param>
        internal NotEqualToValueChecker(IEqualityComparer<T>? comparer, T comparison)
        {
            Comparer = comparer ?? EqualityComparer<T>.Default;
            Comparison = comparison;
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(T value)
        {
            if (!Comparer.Equals(value, Comparison)) return ValueCheckState.Success;
            return ValueCheckState.AsError($"値は'{Comparison}'と異なる必要があります");
        }
    }
}
