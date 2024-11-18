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
        /// <summary>
        /// 比較に使用する<see cref="IComparer{T}"/>のインスタンスを取得します。
        /// </summary>
        public IComparer<T> Comparer { get; }

        /// <summary>
        /// 比較対象の値を取得します。
        /// </summary>
        public T Comparison { get; }

        /// <summary>
        /// <see cref="GreaterThanValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="comparison">比較対象</param>
        /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
        internal GreaterThanValueChecker(T comparison, IComparer<T>? comparer)
        {
            Comparison = comparison;
            Comparer = comparer ?? Comparer<T>.Default;
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(T value)
        {
            if (Comparer.Compare(value, Comparison) > 0) return ValueCheckState.Success;
            return ValueCheckState.AsError($"値が{Comparison}以下です");
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
        /// <summary>
        /// 比較に使用する<see cref="IComparer{T}"/>のインスタンスを取得します。
        /// </summary>
        public IComparer<T> Comparer { get; }

        /// <summary>
        /// 比較対象の値を取得します。
        /// </summary>
        public T Comparison { get; }

        /// <summary>
        /// <see cref="GreaterThanOrEqualToValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="comparison">比較対象</param>
        /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
        internal GreaterThanOrEqualToValueChecker(T comparison, IComparer<T>? comparer)
        {
            Comparison = comparison;
            Comparer = comparer ?? Comparer<T>.Default;
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(T value)
        {
            if (Comparer.Compare(value, Comparison) >= 0) return ValueCheckState.Success;
            return ValueCheckState.AsError($"値が{Comparison}未満です");
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
        /// <summary>
        /// 比較に使用する<see cref="IComparer{T}"/>のインスタンスを取得します。
        /// </summary>
        public IComparer<T> Comparer { get; }

        /// <summary>
        /// 比較対象の値を取得します。
        /// </summary>
        public T Comparison { get; }

        /// <summary>
        /// <see cref="LessThanValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="comparison">比較対象</param>
        /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
        internal LessThanValueChecker(T comparison, IComparer<T>? comparer)
        {
            Comparison = comparison;
            Comparer = comparer ?? Comparer<T>.Default;
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(T value)
        {
            if (Comparer.Compare(value, Comparison) < 0) return ValueCheckState.Success;
            return ValueCheckState.AsError($"値が{Comparison}より大きいです");
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
        /// <summary>
        /// 比較に使用する<see cref="IComparer{T}"/>のインスタンスを取得します。
        /// </summary>
        public IComparer<T> Comparer { get; }

        /// <summary>
        /// 比較対象の値を取得します。
        /// </summary>
        public T Comparison { get; }

        /// <summary>
        /// <see cref="LessThanOrEqualToValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="comparison">比較対象</param>
        /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
        internal LessThanOrEqualToValueChecker(T comparison, IComparer<T>? comparer)
        {
            Comparison = comparison;
            Comparer = comparer ?? Comparer<T>.Default;
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(T value)
        {
            if (Comparer.Compare(value, Comparison) <= 0) return ValueCheckState.Success;
            return ValueCheckState.AsError($"値が{Comparison}より大きいです");
        }
    }
}
