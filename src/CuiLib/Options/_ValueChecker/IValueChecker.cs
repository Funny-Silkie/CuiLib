using System;

namespace CuiLib.Options
{
    /// <summary>
    /// 値の検証を提供する基底クラスです。
    /// </summary>
    /// <typeparam name="T">検証する値の型</typeparam>
    public interface IValueChecker<in T>
    {
        /// <summary>
        /// 値の妥当性を検証します。
        /// </summary>
        /// <param name="value">検証する値</param>
        /// <returns>検証結果</returns>
        ValueCheckState CheckValue(T value);

        /// <summary>
        /// 二つの<see cref="IValueChecker{T}"/>をAND結合します。
        /// </summary>
        /// <param name="first">最初の評価</param>
        /// <param name="second">2番目の評価</param>
        /// <exception cref="ArgumentNullException"><paramref name="first"/>または<paramref name="second"/>がnull</exception>
        static IValueChecker<T> And(IValueChecker<T> first, IValueChecker<T> second)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);

            if (first == second) return first;

            return new AndValueChecker<T>(first, second);
        }

        /// <summary>
        /// 複数の<see cref="IValueChecker{T}"/>をAND結合します。
        /// </summary>
        /// <param name="source">評価する関数のリスト</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/>の要素がnull</exception>
        static IValueChecker<T> And(params IValueChecker<T>[] source)
        {
            return new AndValueChecker<T>(source);
        }

        /// <summary>
        /// 二つの<see cref="IValueChecker{T}"/>をOR結合します。
        /// </summary>
        /// <param name="first">最初の評価</param>
        /// <param name="second">2番目の評価</param>
        /// <exception cref="ArgumentNullException"><paramref name="first"/>または<paramref name="second"/>がnull</exception>
        static IValueChecker<T> Or(IValueChecker<T> first, IValueChecker<T> second)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);

            if (first == second) return first;

            return new OrValueChecker<T>(first, second);
        }

        /// <summary>
        /// 複数の<see cref="IValueChecker{T}"/>をOR結合します。
        /// </summary>
        /// <param name="source">評価する関数のリスト</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/>の要素がnull</exception>
        static IValueChecker<T> Or(params IValueChecker<T>[] source)
        {
            return new OrValueChecker<T>(source);
        }

#pragma warning disable CS1591 // 公開されている型またはメンバーの XML コメントがありません

        static IValueChecker<T> operator &(IValueChecker<T> left, IValueChecker<T> right)
        {
            return And(left, right);
        }

        static IValueChecker<T> operator |(IValueChecker<T> left, IValueChecker<T> right)
        {
            return Or(left, right);
        }

#pragma warning restore CS1591 // 公開されている型またはメンバーの XML コメントがありません
    }
}
