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
        public static IValueChecker<T> AlwaysSuccess<T>()
        {
            return new AlwaysSuccessValueChecker<T>();
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
        public static IValueChecker<T> And<T>(params IValueChecker<T>[] source)
        {
            return IValueChecker<T>.And(source);
        }

        /// <summary>
        /// 複数の<see cref="IValueChecker{T}"/>をOR結合します。
        /// </summary>
        /// <param name="source">評価する関数のリスト</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/>の要素がnull</exception>
        public static IValueChecker<T> Or<T>(params IValueChecker<T>[] source)
        {
            return IValueChecker<T>.Or(source);
        }

        /// <summary>
        /// 値が対象より大きいかを検証します。
        /// </summary>
        /// <param name="comparison">比較対象</param>
        public static IValueChecker<T> Larger<T>(T comparison)
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
        public static IValueChecker<T> Larger<T>(T comparison, IComparer<T>? comparer)
            where T : IComparable<T>
        {
            return new LargerValueChecker<T>(comparison, comparer);
        }

        /// <summary>
        /// 値が対象以上であるかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        public static IValueChecker<T> LargerOrEqual<T>(T comparison)
            where T : IComparable<T>
        {
            return new LargerOrEqualValueChecker<T>(comparison, null);
        }

        /// <summary>
        /// 値が対象以上であるかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
        public static IValueChecker<T> LargerOrEqual<T>(T comparison, IComparer<T>? comparer)
            where T : IComparable<T>
        {
            return new LargerOrEqualValueChecker<T>(comparison, comparer);
        }

        /// <summary>
        /// 値が対象より小さいかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        public static IValueChecker<T> Lower<T>(T comparison)
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
        public static IValueChecker<T> Lower<T>(T comparison, IComparer<T>? comparer)
            where T : IComparable<T>
        {
            return new LowerValueChecker<T>(comparison, comparer);
        }

        /// <summary>
        /// 値が対象以下であるかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        public static IValueChecker<T> LowerOrEqual<T>(T comparison)
            where T : IComparable<T>
        {
            return new LowerOrEqualValueChecker<T>(comparison, null);
        }

        /// <summary>
        /// 値が対象以下であるかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
        public static IValueChecker<T> LowerOrEqual<T>(T comparison, IComparer<T>? comparer)
            where T : IComparable<T>
        {
            return new LowerOrEqualValueChecker<T>(comparison, comparer);
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
        public static IValueChecker<TElement> Contains<TCollection, TElement>(TCollection source)
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
        public static IValueChecker<TElement> Contains<TCollection, TElement>(TCollection source, IEqualityComparer<TElement?>? comparer)
            where TCollection : IEnumerable<TElement>
        {
            return new ContainsValueChecker<TCollection, TElement>(source, comparer);
        }

        /// <summary>
        /// 文字列が空でないかを検証します。
        /// </summary>
        public static IValueChecker<string?> NotEmpty()
        {
            return new NotEmptyValueChecker();
        }

        /// <summary>
        /// 文字列が指定の値で始まるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">開始文字</param>
        public static IValueChecker<string> StartWith(char comparison)
        {
            return new StartWithValueChecker(comparison);
        }

        /// <summary>
        /// 文字列が指定の値で始まるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">開始文字</param>
        /// <param name="stringComparison">文字列の比較方法</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stringComparison"/>が非定義の値</exception>
        public static IValueChecker<string> StartWith(char comparison, StringComparison stringComparison)
        {
            return new StartWithValueChecker(comparison, stringComparison);
        }

        /// <summary>
        /// 文字列が指定の値で始まるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">開始文字列</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="comparison"/>が空文字</exception>
        public static IValueChecker<string> StartWith(string comparison)
        {
            return new StartWithValueChecker(comparison);
        }

        /// <summary>
        /// 文字列が指定の値で始まるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">開始文字列</param>
        /// <param name="stringComparison">文字列の比較方法</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="comparison"/>が空文字</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stringComparison"/>が非定義の値</exception>
        public static IValueChecker<string> StartWith(string comparison, StringComparison stringComparison)
        {
            return new StartWithValueChecker(comparison, stringComparison);
        }

        /// <summary>
        /// 文字列が指定の値で終わるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">終了文字</param>
        public static IValueChecker<string> EndWith(char comparison)
        {
            return new EndWithValueChecker(comparison);
        }

        /// <summary>
        /// 文字列が指定の値で終わるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">終了文字</param>
        /// <param name="stringComparison">文字列の比較方法</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stringComparison"/>が非定義の値</exception>
        public static IValueChecker<string> EndWith(char comparison, StringComparison stringComparison)
        {
            return new EndWithValueChecker(comparison, stringComparison);
        }

        /// <summary>
        /// 文字列が指定の値で終わるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">終了文字列</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="comparison"/>が空文字</exception>
        public static IValueChecker<string> EndWith(string comparison)
        {
            return new EndWithValueChecker(comparison);
        }

        /// <summary>
        /// 文字列が指定の値で終わるかどうかを検証します。
        /// </summary>
        /// <param name="comparison">終了文字列</param>
        /// <param name="stringComparison">文字列の比較方法</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="comparison"/>が空文字</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stringComparison"/>が非定義の値</exception>
        public static IValueChecker<string> EndWith(string comparison, StringComparison stringComparison)
        {
            return new EndWithValueChecker(comparison, stringComparison);
        }

        /// <summary>
        /// 値が等しいかどうかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        public static IValueChecker<T> Equals<T>(T comparison)
        {
            return new EqualsValueChecker<T>(null, comparison);
        }

        /// <summary>
        /// 値が等しいかどうかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparer">比較を行うオブジェクト。nullで<see cref="EqualityComparer{T}.Default"/></param>
        /// <param name="comparison">比較対象</param>
        public static IValueChecker<T> Equals<T>(T comparison, IEqualityComparer<T>? comparer)
        {
            return new EqualsValueChecker<T>(comparer, comparison);
        }

        /// <summary>
        /// 値が異なるかどうかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparison">比較対象</param>
        public static IValueChecker<T> NotEquals<T>(T comparison)
        {
            return new NotEqualsValueChecker<T>(null, comparison);
        }

        /// <summary>
        /// 値が異なるかどうかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="comparer">比較を行うオブジェクト。nullで<see cref="EqualityComparer{T}.Default"/></param>
        /// <param name="comparison">比較対象</param>
        public static IValueChecker<T> NotEquals<T>(T comparison, IEqualityComparer<T>? comparer)
        {
            return new NotEqualsValueChecker<T>(comparer, comparison);
        }

        /// <summary>
        /// ファイルパスが存在するかどうかを検証します。
        /// </summary>
        public static IValueChecker<string> FileExists()
        {
            return new FileExistsValueChecker();
        }

        /// <summary>
        /// ディレクトリパスが存在するかどうかを検証します。
        /// </summary>
        public static IValueChecker<string> DirectoryExists()
        {
            return new DirectoryExistsValueChecker();
        }

        /// <summary>
        /// ファイルが読み込める状態にあるかどうかを検証します。
        /// </summary>
        public static IValueChecker<FileInfo> VerifySourceFile()
        {
            return new SourceFileChecker();
        }

        /// <summary>
        /// ファイルが書き込める状態にあるかどうかを検証します。
        /// </summary>
        /// <param name="allowMissedDir">ディレクトリの非存在を許容するかどうか</param>
        /// <param name="allowOverwrite">ファイルの上書きを許容するかどうか</param>
        public static IValueChecker<FileInfo> VerifyDestinationFile(bool allowMissedDir, bool allowOverwrite)
        {
            return new DestinationFileChecker()
            {
                AllowMissedDirectory = allowMissedDir,
                AllowOverwrite = allowOverwrite,
            };
        }

        /// <summary>
        /// ディレクトリが読み込める状態にあるかどうかを検証します。
        /// </summary>
        public static IValueChecker<DirectoryInfo> VerifySourceDirectory()
        {
            return new SourceDirectoryChecker();
        }

        /// <summary>
        /// 正規表現にマッチするかどうかを検証します。
        /// </summary>
        /// <param name="pattern">正規表現</param>
        /// <exception cref="ArgumentNullException"><paramref name="pattern"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException">正規表現解析エラー</exception>
        public static IValueChecker<string> IsRegexMatch(
#if NET7_0_OR_GREATER
                                                         [StringSyntax(StringSyntaxAttribute.Regex)]
#endif
                                                         string pattern)
        {
            var regex = new Regex(pattern);
            return IsRegexMatch(regex);
        }

        /// <summary>
        /// 正規表現にマッチするかどうかを検証します。
        /// </summary>
        /// <param name="pattern">正規表現</param>
        /// <param name="options">正規表現オプション</param>
        /// <exception cref="ArgumentNullException"><paramref name="pattern"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException">正規表現解析エラー</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="options"/>が無効な範囲</exception>
        public static IValueChecker<string> IsRegexMatch(
#if NET7_0_OR_GREATER
                                                         [StringSyntax(StringSyntaxAttribute.Regex)]
#endif
                                                         string pattern,
                                                         RegexOptions options)
        {
            var regex = new Regex(pattern, options);
            return IsRegexMatch(regex);
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
        public static IValueChecker<string> IsRegexMatch(
#if NET7_0_OR_GREATER
                                                         [StringSyntax(StringSyntaxAttribute.Regex)]
#endif
                                                         string pattern,
                                                         RegexOptions options,
                                                         TimeSpan matchTimeout)
        {
            var regex = new Regex(pattern, options, matchTimeout);
            return IsRegexMatch(regex);
        }

        /// <summary>
        /// 正規表現にマッチするかどうかを検証します。
        /// </summary>
        /// <param name="regex">正規表現オブジェクト</param>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/>が<see langword="null"/></exception>
        public static IValueChecker<string> IsRegexMatch(Regex regex)
        {
            return new RegexMatchValueChecker(regex);
        }
    }
}
