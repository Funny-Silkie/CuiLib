using CuiLib.Checkers.Implementations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.RegularExpressions;

namespace CuiLib.Checkers
{
    /// <summary>
    /// <see cref="IValueChecker{T}"/>を提供するクラスです。
    /// </summary>
    public static partial class ValueChecker
    {
        /// <summary>
        /// 必ず<see cref="ValueCheckState.Success"/>を返すインスタンスを取得します。
        /// </summary>
        public static IValueChecker<T> AlwaysValid<T>()
        {
            return new AlwaysValidValueChecker<T>();
        }

        /// <summary>
        /// デリゲートを使用するインスタンスを取得します。
        /// </summary>
        /// <param name="func">検証関数</param>
        /// <exception cref="ArgumentNullException"><paramref name="func"/>がnull</exception>
        public static IValueChecker<T> FromDelegate<T>(Func<T?, ValueCheckState> func)
        {
            return new DelegateValueChecker<T>(func);
        }

        /// <summary>
        /// 複数の<see cref="IValueChecker{T}"/>をAND結合します。
        /// </summary>
        /// <param name="source">評価する関数のリスト</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/>の要素がnull</exception>
        [Obsolete($"Use And(ReadOnlySpan<IValueChecker<T>>) instead.")]
        public static IValueChecker<T> And<T>(params IValueChecker<T>[] source)
        {
            return new AndValueChecker<T>(source);
        }

        /// <summary>
        /// 複数の<see cref="IValueChecker{T}"/>をAND結合します。
        /// </summary>
        /// <param name="source">評価する関数のリスト</param>
        /// <exception cref="ArgumentException"><paramref name="source"/>の要素がnull</exception>
        public static IValueChecker<T> And<T>(params ReadOnlySpan<IValueChecker<T>> source)
        {
            return new AndValueChecker<T>(source);
        }

        /// <summary>
        /// 複数の<see cref="IValueChecker{T}"/>をOR結合します。
        /// </summary>
        /// <param name="source">評価する関数のリスト</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/>の要素がnull</exception>
        [Obsolete($"Use Or(ReadOnlySpan<IValueChecker<T>>) instead.")]
        public static IValueChecker<T> Or<T>(params IValueChecker<T>[] source)
        {
            return new OrValueChecker<T>(source);
        }

        /// <summary>
        /// 複数の<see cref="IValueChecker{T}"/>をOR結合します。
        /// </summary>
        /// <param name="source">評価する関数のリスト</param>
        /// <exception cref="ArgumentException"><paramref name="source"/>の要素がnull</exception>
        public static IValueChecker<T> Or<T>(params ReadOnlySpan<IValueChecker<T>> source)
        {
            return new OrValueChecker<T>(source);
        }

        /// <summary>
        /// 値が対象より大きいかを検証します。
        /// </summary>
        /// <param name="comparison">比較対象</param>
        public static IValueChecker<T> GreaterThan<T>(T comparison)
            where T : IComparable<T>
        {
            return new GreaterThanValueChecker<T>(comparison, null);
        }

        /// <summary>
        /// 値が対象より大きいかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
        public static IValueChecker<T> GreaterThan<T>(T comparison, IComparer<T>? comparer)
            where T : IComparable<T>
        {
            return new GreaterThanValueChecker<T>(comparison, comparer);
        }

        /// <summary>
        /// 値が対象以上であるかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        public static IValueChecker<T> GreaterThanOrEqualTo<T>(T comparison)
            where T : IComparable<T>
        {
            return new GreaterThanOrEqualToValueChecker<T>(comparison, null);
        }

        /// <summary>
        /// 値が対象以上であるかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
        public static IValueChecker<T> GreaterThanOrEqualTo<T>(T comparison, IComparer<T>? comparer)
            where T : IComparable<T>
        {
            return new GreaterThanOrEqualToValueChecker<T>(comparison, comparer);
        }

        /// <summary>
        /// 値が対象より小さいかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        public static IValueChecker<T> LessThan<T>(T comparison)
            where T : IComparable<T>
        {
            return new LessThanValueChecker<T>(comparison, null);
        }

        /// <summary>
        /// 値が対象より小さいかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
        public static IValueChecker<T> LessThan<T>(T comparison, IComparer<T>? comparer)
            where T : IComparable<T>
        {
            return new LessThanValueChecker<T>(comparison, comparer);
        }

        /// <summary>
        /// 値が対象以下であるかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        public static IValueChecker<T> LessThanOrEqualTo<T>(T comparison)
            where T : IComparable<T>
        {
            return new LessThanOrEqualToValueChecker<T>(comparison, null);
        }

        /// <summary>
        /// 値が対象以下であるかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
        public static IValueChecker<T> LessThanOrEqualTo<T>(T comparison, IComparer<T>? comparer)
            where T : IComparable<T>
        {
            return new LessThanOrEqualToValueChecker<T>(comparison, comparer);
        }

        /// <summary>
        /// 列挙型が定義された値かどうかを検証します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static IValueChecker<T> Defined<T>()
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
        public static IValueChecker<TElement> ContainedIn<TCollection, TElement>(TCollection source)
            where TCollection : IEnumerable<TElement>
        {
            return new ContainedInValueChecker<TCollection, TElement>(source, null);
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
        public static IValueChecker<TElement> ContainedIn<TCollection, TElement>(TCollection source, IEqualityComparer<TElement?>? comparer)
            where TCollection : IEnumerable<TElement>
        {
            return new ContainedInValueChecker<TCollection, TElement>(source, comparer);
        }

        /// <summary>
        /// 文字列が空であるかを検証します。
        /// </summary>
        public static IValueChecker<string?> Empty()
        {
            return new EmptyValueChecker();
        }

        /// <summary>
        /// 文字列が空であるかを検証します。
        /// </summary>
        public static IValueChecker<IEnumerable<TElement>?> Empty<TElement>()
        {
            return new EmptyValueChecker<TElement>();
        }

        /// <summary>
        /// 文字列が空でないかを検証します。
        /// </summary>
        public static IValueChecker<string?> NotEmpty()
        {
            return new NotEmptyValueChecker();
        }

        /// <summary>
        /// 文字列が空であるかを検証します。
        /// </summary>
        public static IValueChecker<IEnumerable<TElement>?> NotEmpty<TElement>()
        {
            return new NotEmptyValueChecker<TElement>();
        }

        /// <summary>
        /// 文字列が指定の値で始まるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">開始文字</param>
        public static IValueChecker<string> StartsWith(char comparison)
        {
            return new StartsWithValueChecker(comparison);
        }

        /// <summary>
        /// 文字列が指定の値で始まるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">開始文字</param>
        /// <param name="stringComparison">文字列の比較方法</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stringComparison"/>が非定義の値</exception>
        public static IValueChecker<string> StartsWith(char comparison, StringComparison stringComparison)
        {
            return new StartsWithValueChecker(comparison, stringComparison);
        }

        /// <summary>
        /// 文字列が指定の値で始まるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">開始文字列</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="comparison"/>が空文字</exception>
        public static IValueChecker<string> StartsWith(string comparison)
        {
            return new StartsWithValueChecker(comparison);
        }

        /// <summary>
        /// 文字列が指定の値で始まるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">開始文字列</param>
        /// <param name="stringComparison">文字列の比較方法</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="comparison"/>が空文字</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stringComparison"/>が非定義の値</exception>
        public static IValueChecker<string> StartsWith(string comparison, StringComparison stringComparison)
        {
            return new StartsWithValueChecker(comparison, stringComparison);
        }

        /// <summary>
        /// 文字列が指定の値で終わるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">終了文字</param>
        public static IValueChecker<string> EndsWith(char comparison)
        {
            return new EndsWithValueChecker(comparison);
        }

        /// <summary>
        /// 文字列が指定の値で終わるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">終了文字</param>
        /// <param name="stringComparison">文字列の比較方法</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stringComparison"/>が非定義の値</exception>
        public static IValueChecker<string> EndsWith(char comparison, StringComparison stringComparison)
        {
            return new EndsWithValueChecker(comparison, stringComparison);
        }

        /// <summary>
        /// 文字列が指定の値で終わるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">終了文字列</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="comparison"/>が空文字</exception>
        public static IValueChecker<string> EndsWith(string comparison)
        {
            return new EndsWithValueChecker(comparison);
        }

        /// <summary>
        /// 文字列が指定の値で終わるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">終了文字列</param>
        /// <param name="stringComparison">文字列の比較方法</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="comparison"/>が空文字</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stringComparison"/>が非定義の値</exception>
        public static IValueChecker<string> EndsWith(string comparison, StringComparison stringComparison)
        {
            return new EndsWithValueChecker(comparison, stringComparison);
        }

        /// <summary>
        /// 値が等しいかどうかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        public static IValueChecker<T> EqualTo<T>(T comparison)
        {
            return new EqualToValueChecker<T>(null, comparison);
        }

        /// <summary>
        /// 値が等しいかどうかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparer">比較を行うオブジェクト。nullで<see cref="EqualityComparer{T}.Default"/></param>
        /// <param name="comparison">比較対象</param>
        public static IValueChecker<T> EqualTo<T>(T comparison, IEqualityComparer<T>? comparer)
        {
            return new EqualToValueChecker<T>(comparer, comparison);
        }

        /// <summary>
        /// 値が異なるかどうかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        public static IValueChecker<T> NotEqualTo<T>(T comparison)
        {
            return new NotEqualToValueChecker<T>(null, comparison);
        }

        /// <summary>
        /// 値が異なるかどうかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparer">比較を行うオブジェクト。nullで<see cref="EqualityComparer{T}.Default"/></param>
        /// <param name="comparison">比較対象</param>
        public static IValueChecker<T> NotEqualTo<T>(T comparison, IEqualityComparer<T>? comparer)
        {
            return new NotEqualToValueChecker<T>(comparer, comparison);
        }

        /// <summary>
        /// ファイルパスが存在するかどうかを検証します。
        /// </summary>
        public static IValueChecker<string> ExistsAsFile()
        {
            return new ExistsAsFileValueChecker();
        }

        /// <summary>
        /// ディレクトリパスが存在するかどうかを検証します。
        /// </summary>
        public static IValueChecker<string> ExistsAsDirectory()
        {
            return new ExistsAsDirectoryValueChecker();
        }

        /// <summary>
        /// ファイルが読み込める状態にあるかどうかを検証します。
        /// </summary>
        public static IValueChecker<FileInfo> ValidSourceFile()
        {
            return new ValidSourceFileChecker();
        }

        /// <summary>
        /// ファイルが書き込める状態にあるかどうかを検証します。
        /// </summary>
        /// <param name="allowMissedDir">ディレクトリの非存在を許容するかどうか</param>
        /// <param name="allowOverwrite">ファイルの上書きを許容するかどうか</param>
        public static IValueChecker<FileInfo> ValidDestinationFile(bool allowMissedDir, bool allowOverwrite)
        {
            return new ValidDestinationFileChecker()
            {
                AllowMissedDirectory = allowMissedDir,
                AllowOverwrite = allowOverwrite,
            };
        }

        /// <summary>
        /// ディレクトリが読み込める状態にあるかどうかを検証します。
        /// </summary>
        public static IValueChecker<DirectoryInfo> ValidSourceDirectory()
        {
            return new ValidSourceDirectoryChecker();
        }

        /// <summary>
        /// 正規表現にマッチするかどうかを検証します。
        /// </summary>
        /// <param name="pattern">正規表現</param>
        /// <exception cref="ArgumentNullException"><paramref name="pattern"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException">正規表現解析エラー</exception>
        public static IValueChecker<string> Matches([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
        {
            var regex = new Regex(pattern);
            return Matches(regex);
        }

        /// <summary>
        /// 正規表現にマッチするかどうかを検証します。
        /// </summary>
        /// <param name="pattern">正規表現</param>
        /// <param name="options">正規表現オプション</param>
        /// <exception cref="ArgumentNullException"><paramref name="pattern"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException">正規表現解析エラー</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="options"/>が無効な範囲</exception>
        public static IValueChecker<string> Matches([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, RegexOptions options)
        {
            var regex = new Regex(pattern, options);
            return Matches(regex);
        }

        /// <summary>
        /// 正規表現にマッチするかどうかを検証します。
        /// </summary>
        /// <param name="pattern">正規表現</param>
        /// <param name="options">正規表現オプション</param>
        /// <param name="matchTimeout">マッチ判定時のタイムアウト時間</param>
        /// <exception cref="ArgumentNullException"><paramref name="pattern"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException">正規表現解析エラー</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="options"/>または<paramref name="matchTimeout"/>が無効な範囲</exception>
        public static IValueChecker<string> Matches([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, RegexOptions options, TimeSpan matchTimeout)
        {
            var regex = new Regex(pattern, options, matchTimeout);
            return Matches(regex);
        }

        /// <summary>
        /// 正規表現にマッチするかどうかを検証します。
        /// </summary>
        /// <param name="regex">正規表現オブジェクト</param>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/>が<see langword="null"/></exception>
        public static IValueChecker<string> Matches(Regex regex)
        {
            return new MatchesValueChecker(regex);
        }
    }
}
