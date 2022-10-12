using System;
using System.Collections.Generic;

namespace CuiLib.Options
{
    /// <summary>
    /// <see cref="ValueChecker{T}"/>を提供するクラスです。
    /// </summary>
    public static partial class ValueChecker
    {
        /// <summary>
        /// 必ず<see cref="ValueCheckState.Success"/>を返すインスタンスを取得します。
        /// </summary>
        public static ValueChecker<T> AlwaysSuccess<T>()
        {
            return new AlwaysSuccessValueChecker<T>();
        }

        /// <summary>
        /// デリゲートを使用するインスタンスを取得します。
        /// </summary>
        /// <param name="func">検証関数</param>
        /// <exception cref="ArgumentNullException"><paramref name="func"/>がnull</exception>
        public static ValueChecker<T> FromDelegate<T>(Func<T?, ValueCheckState> func)
        {
            return new DelegateValueChecker<T>(func);
        }

        /// <summary>
        /// 値が対象より大きいかを検証します。
        /// </summary>
        /// <param name="comparison">比較対象</param>
        public static ValueChecker<T> Larger<T>(T? comparison)
            where T : IComparable<T>
        {
            return new LargerValueChecker<T>(comparison, null);
        }

        /// <summary>
        /// 値が対象より大きいかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
        public static ValueChecker<T> Larger<T>(T? comparison, IComparer<T>? comparer)
            where T : IComparable<T>
        {
            return new LargerValueChecker<T>(comparison, comparer);
        }

        /// <summary>
        /// 値が対象以上であるかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        public static ValueChecker<T> LargerThan<T>(T? comparison)
            where T : IComparable<T>
        {
            return new LargerThanValueChecker<T>(comparison, null);
        }

        /// <summary>
        /// 値が対象以上であるかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
        public static ValueChecker<T> LargerThan<T>(T? comparison, IComparer<T>? comparer)
            where T : IComparable<T>
        {
            return new LargerThanValueChecker<T>(comparison, comparer);
        }

        /// <summary>
        /// 値が対象より小さいかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        public static ValueChecker<T> Lower<T>(T? comparison)
            where T : IComparable<T>
        {
            return new LowerValueChecker<T>(comparison, null);
        }

        /// <summary>
        /// 値が対象より小さいかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
        public static ValueChecker<T> Lower<T>(T? comparison, IComparer<T>? comparer)
            where T : IComparable<T>
        {
            return new LowerValueChecker<T>(comparison, comparer);
        }

        /// <summary>
        /// 値が対象以下であるかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        public static ValueChecker<T> LowerThan<T>(T? comparison)
            where T : IComparable<T>
        {
            return new LowerThanValueChecker<T>(comparison, null);
        }

        /// <summary>
        /// 値が対象以下であるかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
        public static ValueChecker<T> LowerThan<T>(T? comparison, IComparer<T>? comparer)
            where T : IComparable<T>
        {
            return new LowerThanValueChecker<T>(comparison, comparer);
        }

        /// <summary>
        /// 列挙型が定義された値かどうかを検証します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static ValueChecker<T> Defined<T>()
            where T : struct, Enum
        {
            return new DefinedEnumValueChecker<T>();
        }

        /// <summary>
        /// 要素がコレクションに含まれているかどうかを検証します。
        /// </summary>
        /// <typeparam name="TCollection">コレクションの型</typeparam>
        /// <typeparam name="TElement">要素の型</typeparam>
        /// <param name="source">候補のコレクション</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/>が空</exception>
        public static ValueChecker<TElement> Contains<TCollection, TElement>(TCollection source)
            where TCollection : IEnumerable<TElement>
        {
            return new ContainsValueChecker<TCollection, TElement>(source, null);
        }

        /// <summary>
        /// 要素がコレクションに含まれているかどうかを検証します。
        /// </summary>
        /// <typeparam name="TCollection">コレクションの型</typeparam>
        /// <typeparam name="TElement">要素の型</typeparam>
        /// <param name="source">候補のコレクション</param>
        /// <param name="comparer">要素を比較するオブジェクト。nullで<see cref="EqualityComparer{T}.Default"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/>が空</exception>
        public static ValueChecker<TElement> Contains<TCollection, TElement>(TCollection source, IEqualityComparer<TElement?>? comparer)
            where TCollection : IEnumerable<TElement>
        {
            return new ContainsValueChecker<TCollection, TElement>(source, comparer);
        }

        /// <summary>
        /// 文字列が指定の値で始まるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">開始文字</param>
        public static ValueChecker<string> StartWith(char comparison)
        {
            return new StartWithValueChecker(comparison);
        }

        /// <summary>
        /// 文字列が指定の値で始まるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">開始文字列</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="comparison"/>が空文字</exception>
        public static ValueChecker<string> StartWith(string comparison)
        {
            return new StartWithValueChecker(comparison);
        }

        /// <summary>
        /// 文字列が指定の値で終わるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">終了文字</param>
        public static ValueChecker<string> EndWith(char comparison)
        {
            return new EndWithValueChecker(comparison);
        }

        /// <summary>
        /// 文字列が指定の値で終わるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">終了文字列</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="comparison"/>が空文字</exception>
        public static ValueChecker<string> EndWith(string comparison)
        {
            return new EndWithValueChecker(comparison);
        }
    }
}
