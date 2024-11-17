using System;
using System.Collections.Generic;

namespace CuiLib.Checkers.Implementations
{
    /// <summary>
    /// 値が対象より大きいかを検証します。
    /// </summary>
    /// <typeparam name="T">検証する値の型</typeparam>
    [Serializable]
    internal sealed class GreaterThanValueChecker<T> : IValueChecker<T>
        where T : IComparable<T>
    {
        private readonly IComparer<T> comparer;
        private readonly T comparison;

        /// <summary>
        /// <see cref="GreaterThanValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="comparison">比較対象</param>
        /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
        internal GreaterThanValueChecker(T comparison, IComparer<T>? comparer)
        {
            this.comparison = comparison;
            this.comparer = comparer ?? Comparer<T>.Default;
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(T value)
        {
            if (comparer.Compare(value, comparison) > 0) return ValueCheckState.Success;
            return ValueCheckState.AsError($"値が{comparison}以下です");
        }
    }

    /// <summary>
    /// 値が対象以上であるかを検証します。
    /// </summary>
    /// <typeparam name="T">検証する値の型</typeparam>
    [Serializable]
    internal sealed class GreaterThanOrEqualToValueChecker<T> : IValueChecker<T>
        where T : IComparable<T>
    {
        private readonly IComparer<T> comparer;
        private readonly T comparison;

        /// <summary>
        /// <see cref="GreaterThanOrEqualToValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="comparison">比較対象</param>
        /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
        internal GreaterThanOrEqualToValueChecker(T comparison, IComparer<T>? comparer)
        {
            this.comparison = comparison;
            this.comparer = comparer ?? Comparer<T>.Default;
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(T value)
        {
            if (comparer.Compare(value, comparison) >= 0) return ValueCheckState.Success;
            return ValueCheckState.AsError($"値が{comparison}未満です");
        }
    }

    /// <summary>
    /// 値が対象より小さいかを検証します。
    /// </summary>
    /// <typeparam name="T">検証する値の型</typeparam>
    [Serializable]
    internal sealed class LessThanValueChecker<T> : IValueChecker<T>
        where T : IComparable<T>
    {
        private readonly IComparer<T> comparer;
        private readonly T comparison;

        /// <summary>
        /// <see cref="LessThanValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="comparison">比較対象</param>
        /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
        internal LessThanValueChecker(T comparison, IComparer<T>? comparer)
        {
            this.comparison = comparison;
            this.comparer = comparer ?? Comparer<T>.Default;
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(T value)
        {
            if (comparer.Compare(value, comparison) < 0) return ValueCheckState.Success;
            return ValueCheckState.AsError($"値が{comparison}より大きいです");
        }
    }

    /// <summary>
    /// 値が対象以下であるかを検証します。
    /// </summary>
    /// <typeparam name="T">検証する値の型</typeparam>
    [Serializable]
    internal sealed class LessThanOrEqualToValueChecker<T> : IValueChecker<T>
        where T : IComparable<T>
    {
        private readonly IComparer<T> comparer;
        private readonly T comparison;

        /// <summary>
        /// <see cref="LessThanOrEqualToValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="comparison">比較対象</param>
        /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
        internal LessThanOrEqualToValueChecker(T comparison, IComparer<T>? comparer)
        {
            this.comparison = comparison;
            this.comparer = comparer ?? Comparer<T>.Default;
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(T value)
        {
            if (comparer.Compare(value, comparison) <= 0) return ValueCheckState.Success;
            return ValueCheckState.AsError($"値が{comparison}より大きいです");
        }
    }
}
