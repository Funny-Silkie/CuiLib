using System;

namespace CuiLib.Options
{
    /// <summary>
    /// 値の検証を提供するクラスです。
    /// </summary>
    /// <typeparam name="T">検証する値の型</typeparam>
    [Serializable]
    public abstract class ValueChecker<T>
    {
        /// <summary>
        /// <see cref="ValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        protected ValueChecker()
        {
        }

        /// <summary>
        /// 二つの<see cref="ValueChecker{T}"/>をAND結合します。
        /// </summary>
        /// <param name="first">最初の評価</param>
        /// <param name="second">2番目の評価</param>
        /// <exception cref="ArgumentNullException"><paramref name="first"/>または<paramref name="second"/>がnull</exception>
        public static ValueChecker<T> And(ValueChecker<T> first, ValueChecker<T> second)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);

            if (first == second) return first;

            return new AndValueChecker<T>(first, second);
        }

        /// <summary>
        /// 複数の<see cref="ValueChecker{T}"/>をAND結合します。
        /// </summary>
        /// <param name="source">評価する関数のリスト</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/>の要素がnull</exception>
        public static ValueChecker<T> And(params ValueChecker<T>[] source)
        {
            return new AndValueChecker<T>(source);
        }

        /// <summary>
        /// 二つの<see cref="ValueChecker{T}"/>をOR結合します。
        /// </summary>
        /// <param name="first">最初の評価</param>
        /// <param name="second">2番目の評価</param>
        /// <exception cref="ArgumentNullException"><paramref name="first"/>または<paramref name="second"/>がnull</exception>
        public static ValueChecker<T> Or(ValueChecker<T> first, ValueChecker<T> second)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);

            if (first == second) return first;

            return new OrValueChecker<T>(first, second);
        }

        /// <summary>
        /// 複数の<see cref="ValueChecker{T}"/>をOR結合します。
        /// </summary>
        /// <param name="source">評価する関数のリスト</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/>の要素がnull</exception>
        public static ValueChecker<T> Or(params ValueChecker<T>[] source)
        {
            return new OrValueChecker<T>(source);
        }

        /// <summary>
        /// 値の妥当性を検証します。
        /// </summary>
        /// <param name="value">検証する値</param>
        /// <returns>検証結果</returns>
        public abstract ValueCheckState CheckValue(T value);

#pragma warning disable CS1591 // 公開されている型またはメンバーの XML コメントがありません

        public static ValueChecker<T> operator &(ValueChecker<T> left, ValueChecker<T> right)
        {
            return And(left, right);
        }

        public static ValueChecker<T> operator |(ValueChecker<T> left, ValueChecker<T> right)
        {
            return Or(left, right);
        }

#pragma warning restore CS1591 // 公開されている型またはメンバーの XML コメントがありません
    }
}
