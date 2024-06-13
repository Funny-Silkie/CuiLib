﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CuiLib.Checkers
{
    public static partial class ValueChecker
    {
        /// <summary>
        /// 常に<see cref="ValueCheckState.Success"/>を返す<see cref="IValueChecker{T}"/>の実装です。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        [Serializable]
        private sealed class AlwaysSuccessValueChecker<T> : IValueChecker<T>
        {
            /// <summary>
            /// <see cref="AlwaysSuccessValueChecker{T}"/>の新しいインスタンスを初期化します。
            /// </summary>
            internal AlwaysSuccessValueChecker()
            {
            }

            /// <inheritdoc/>
            public ValueCheckState CheckValue(T value) => ValueCheckState.Success;

            /// <inheritdoc/>
            public override bool Equals(object? obj) => obj is AlwaysSuccessValueChecker<T>;

            /// <inheritdoc/>
            public override int GetHashCode() => GetType().Name.GetHashCode();
        }

        /// <summary>
        /// デリゲートを使用する<see cref="IValueChecker{T}"/>の実装です。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        [Serializable]
        private sealed class DelegateValueChecker<T> : IValueChecker<T>
        {
            private readonly Func<T, ValueCheckState> func;

            /// <summary>
            /// <see cref="DelegateValueChecker{T}"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="func">検証関数</param>
            /// <exception cref="ArgumentNullException"><paramref name="func"/>がnull</exception>
            internal DelegateValueChecker(Func<T, ValueCheckState> func)
            {
                ArgumentNullException.ThrowIfNull(func);

                this.func = func;
            }

            /// <inheritdoc/>
            public ValueCheckState CheckValue(T value)
            {
                try
                {
                    return func.Invoke(value);
                }
                catch (Exception e) when (e is not ArgumentAnalysisException)
                {
                    ThrowHelpers.ThrowAsOptionParseFailed(e);
                    return default;
                }
            }
        }

        /// <summary>
        /// 値が対象より大きいかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        [Serializable]
        private sealed class LargerValueChecker<T> : IValueChecker<T>
            where T : IComparable<T>
        {
            private readonly IComparer<T> comparer;
            private readonly T comparison;

            /// <summary>
            /// <see cref="LargerValueChecker{T}"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="comparison">比較対象</param>
            /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
            internal LargerValueChecker(T comparison, IComparer<T>? comparer)
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
        private sealed class LargerOrEqualValueChecker<T> : IValueChecker<T>
            where T : IComparable<T>
        {
            private readonly IComparer<T> comparer;
            private readonly T comparison;

            /// <summary>
            /// <see cref="LargerOrEqualValueChecker{T}"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="comparison">比較対象</param>
            /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
            internal LargerOrEqualValueChecker(T comparison, IComparer<T>? comparer)
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
        private sealed class LowerValueChecker<T> : IValueChecker<T>
            where T : IComparable<T>
        {
            private readonly IComparer<T> comparer;
            private readonly T comparison;

            /// <summary>
            /// <see cref="LowerValueChecker{T}"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="comparison">比較対象</param>
            /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
            internal LowerValueChecker(T comparison, IComparer<T>? comparer)
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
        private sealed class LowerOrEqualValueChecker<T> : IValueChecker<T>
            where T : IComparable<T>
        {
            private readonly IComparer<T> comparer;
            private readonly T comparison;

            /// <summary>
            /// <see cref="LowerOrEqualValueChecker{T}"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="comparison">比較対象</param>
            /// <param name="comparer">比較オブジェクト。nullで<see cref="Comparer{T}.Default"/></param>
            internal LowerOrEqualValueChecker(T comparison, IComparer<T>? comparer)
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

        /// <summary>
        /// 列挙型が定義された値かどうかを検証します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [Serializable]
        private sealed class DefinedEnumValueChecker<T> : IValueChecker<T>
            where T : struct, Enum
        {
            /// <summary>
            /// <see cref="DefinedEnumValueChecker{T}"/>の新しいインスタンスを初期化します。
            /// </summary>
            internal DefinedEnumValueChecker()
            {
            }

            /// <inheritdoc/>
            public ValueCheckState CheckValue(T value)
            {
                if (Enum.IsDefined(value)) return ValueCheckState.Success;
                return ValueCheckState.AsError($"定義されていない値です。[{string.Join(", ", Enum.GetNames<T>())}]の中から選択してください");
            }

            /// <inheritdoc/>
            public override bool Equals(object? obj) => obj is DefinedEnumValueChecker<T>;

            /// <inheritdoc/>
            public override int GetHashCode() => GetType().Name.GetHashCode();
        }

        /// <summary>
        /// 要素がコレクションに含まれているかどうかを検証します。
        /// </summary>
        /// <typeparam name="TCollection">コレクションの型</typeparam>
        /// <typeparam name="TElement">要素の型</typeparam>
        [Serializable]
        private sealed class ContainsValueChecker<TCollection, TElement> : IValueChecker<TElement>
            where TCollection : IEnumerable<TElement>
        {
            private readonly TCollection source;
            private readonly IEqualityComparer<TElement> comparer;

            /// <summary>
            /// <see cref="ContainsValueChecker{TCollection, TElement}"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="source">候補のコレクション</param>
            /// <param name="comparer">要素を比較するオブジェクト。nullで<see cref="EqualityComparer{T}.Default"/></param>
            /// <exception cref="ArgumentNullException"><paramref name="source"/>がnull</exception>
            /// <exception cref="ArgumentException"><paramref name="source"/>が空</exception>
            internal ContainsValueChecker(TCollection source, IEqualityComparer<TElement>? comparer)
            {
                ArgumentNullException.ThrowIfNull(source);
                if (!source.Any()) throw new ArgumentException("空のコレクションです", nameof(source));

                this.source = source;
                this.comparer = comparer ?? EqualityComparer<TElement>.Default;
            }

            /// <inheritdoc/>
            public ValueCheckState CheckValue(TElement value)
            {
                if (source.Contains(value, comparer)) return ValueCheckState.Success;
                return ValueCheckState.AsError($"値が含まれていません。[{string.Join(", ", source)}]の何れかを選択してください");
            }
        }

        /// <summary>
        /// 文字列が空でないかを検証します。
        /// </summary>
        [Serializable]
        private sealed class NotEmptyValueChecker : IValueChecker<string?>
        {
            /// <summary>
            /// <see cref="NotEmptyValueChecker"/>の新しいインスタンスを初期化します。
            /// </summary>
            internal NotEmptyValueChecker()
            {
            }

            /// <inheritdoc/>
            public ValueCheckState CheckValue(string? value)
            {
                if (string.IsNullOrEmpty(value)) return ValueCheckState.AsError("空文字です");
                return ValueCheckState.Success;
            }

            /// <inheritdoc/>
            public override bool Equals(object? obj) => obj is NotEmptyValueChecker;

            /// <inheritdoc/>
            public override int GetHashCode() => GetType().Name.GetHashCode();
        }

        /// <summary>
        /// 文字列が指定の値で始まるかどうかを検証します。
        /// </summary>
        [Serializable]
        private sealed class StartWithValueChecker : IValueChecker<string>
        {
            private readonly string comparison;
            private readonly StringComparison stringComparison;

            /// <summary>
            /// <see cref="StartWithValueChecker"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="comparison">開始文字</param>
            /// <param name="stringComparison">文字列の比較方法</param>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="stringComparison"/>が非定義の値</exception>
            internal StartWithValueChecker(char comparison, StringComparison stringComparison = StringComparison.CurrentCulture)
            {
                ThrowHelpers.ThrowIfNotDefined(stringComparison);

                this.comparison = comparison.ToString();
                this.stringComparison = stringComparison;
            }

            /// <summary>
            /// <see cref="StartWithValueChecker"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="comparison">開始文字列</param>
            /// <param name="stringComparison">文字列の比較方法</param>
            /// <exception cref="ArgumentNullException"><paramref name="comparison"/>がnull</exception>
            /// <exception cref="ArgumentException"><paramref name="comparison"/>が空文字</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="stringComparison"/>が非定義の値</exception>
            internal StartWithValueChecker(string comparison, StringComparison stringComparison = StringComparison.CurrentCulture)
            {
                ThrowHelpers.ThrowIfNullOrEmpty(comparison);
                ThrowHelpers.ThrowIfNotDefined(stringComparison);

                this.comparison = comparison;
                this.stringComparison = stringComparison;
            }

            /// <inheritdoc/>
            public ValueCheckState CheckValue(string value)
            {
                if (value is null || !value.StartsWith(comparison, stringComparison)) return ValueCheckState.AsError($"値は'{comparison}'で始まる必要があります");
                return ValueCheckState.Success;
            }
        }

        /// <summary>
        /// 文字列が指定の値で終わるかどうかを検証します。
        /// </summary>
        [Serializable]
        private sealed class EndWithValueChecker : IValueChecker<string>
        {
            private readonly string comparison;
            private readonly StringComparison stringComparison;

            /// <summary>
            /// <see cref="EndWithValueChecker"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="comparison">終了文字</param>
            /// <param name="stringComparison">文字列の比較方法</param>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="stringComparison"/>が非定義の値</exception>
            internal EndWithValueChecker(char comparison, StringComparison stringComparison = StringComparison.CurrentCulture)
            {
                ThrowHelpers.ThrowIfNotDefined(stringComparison);

                this.comparison = comparison.ToString();
                this.stringComparison = stringComparison;
            }

            /// <summary>
            /// <see cref="EndWithValueChecker"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="comparison">終了文字列</param>
            /// <param name="stringComparison">文字列の比較方法</param>
            /// <exception cref="ArgumentNullException"><paramref name="comparison"/>がnull</exception>
            /// <exception cref="ArgumentException"><paramref name="comparison"/>が空文字</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="stringComparison"/>が非定義の値</exception>
            internal EndWithValueChecker(string comparison, StringComparison stringComparison = StringComparison.CurrentCulture)
            {
                ThrowHelpers.ThrowIfNullOrEmpty(comparison);
                ThrowHelpers.ThrowIfNotDefined(stringComparison);

                this.comparison = comparison;
                this.stringComparison = stringComparison;
            }

            /// <inheritdoc/>
            public ValueCheckState CheckValue(string value)
            {
                if (value is null || !value.EndsWith(comparison, stringComparison)) return ValueCheckState.AsError($"値は'{comparison}'で終わる必要があります");
                return ValueCheckState.Success;
            }
        }

        /// <summary>
        /// 値が等しいかどうかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        [Serializable]
        private sealed class EqualsValueChecker<T> : IValueChecker<T>
        {
            private readonly IEqualityComparer<T> comparer;
            private readonly T comparison;

            /// <summary>
            /// <see cref="EqualsValueChecker{T}"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="comparer">比較を行うオブジェクト。nullで<see cref="EqualityComparer{T}.Default"/></param>
            /// <param name="comparison">比較対象</param>
            internal EqualsValueChecker(IEqualityComparer<T>? comparer, T comparison)
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

        /// <summary>
        /// 値が異なるかどうかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        [Serializable]
        private sealed class NotEqualsValueChecker<T> : IValueChecker<T>
        {
            private readonly IEqualityComparer<T> comparer;
            private readonly T comparison;

            /// <summary>
            /// <see cref="NotEqualsValueChecker{T}"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="comparer">比較を行うオブジェクト。nullで<see cref="EqualityComparer{T}.Default"/></param>
            /// <param name="comparison">比較対象</param>
            internal NotEqualsValueChecker(IEqualityComparer<T>? comparer, T comparison)
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

        /// <summary>
        /// ファイルパスが存在するかどうかを検証します。
        /// </summary>
        [Serializable]
        private sealed class FileExistsValueChecker : IValueChecker<string>
        {
            /// <summary>
            /// <see cref="FileExistsValueChecker"/>の新しいインスタンスを初期化します。
            /// </summary>
            internal FileExistsValueChecker()
            {
            }

            /// <inheritdoc/>
            public ValueCheckState CheckValue(string value)
            {
                if (File.Exists(value)) return ValueCheckState.Success;
                return ValueCheckState.AsError($"ファイル'{value}'が存在しません");
            }

            /// <inheritdoc/>
            public override bool Equals(object? obj)
            {
                return obj is FileExistsValueChecker;
            }

            /// <inheritdoc/>
            public override int GetHashCode()
            {
                return GetType().Name.GetHashCode();
            }
        }

        /// <summary>
        /// ディレクトリが存在するかどうかを検証します。
        /// </summary>
        [Serializable]
        private sealed class DirectoryExistsValueChecker : IValueChecker<string>
        {
            /// <summary>
            /// <see cref="DirectoryExistsValueChecker"/>の新しいインスタンスを初期化します。
            /// </summary>
            internal DirectoryExistsValueChecker()
            {
            }

            /// <inheritdoc/>
            public ValueCheckState CheckValue(string value)
            {
                if (Directory.Exists(value)) return ValueCheckState.Success;
                return ValueCheckState.AsError($"ディレクトリ'{value}'が存在しません");
            }

            /// <inheritdoc/>
            public override bool Equals(object? obj)
            {
                return obj is DirectoryExistsValueChecker;
            }

            /// <inheritdoc/>
            public override int GetHashCode()
            {
                return GetType().Name.GetHashCode();
            }
        }

        /// <summary>
        /// 読み込むファイルを検証します。
        /// </summary>
        [Serializable]
        internal class SourceFileChecker : IValueChecker<FileInfo>
        {
            /// <summary>
            /// <see cref="SourceFileChecker"/>の新しいインスタンスを初期化します。
            /// </summary>
            public SourceFileChecker()
            {
            }

            /// <inheritdoc/>
            public ValueCheckState CheckValue(FileInfo value)
            {
                ArgumentNullException.ThrowIfNull(value);

                DirectoryInfo? directory = value.Directory;
                if (directory is not null && !directory.Exists) return ValueCheckState.AsError($"ファイル'{value.Name}'のディレクトリが存在しません");
                if (!value.Exists) return ValueCheckState.AsError($"ファイル'{value.Name}'が存在しません");
                return ValueCheckState.Success;
            }
        }

        /// <summary>
        /// 出力ファイルを検証します。
        /// </summary>
        [Serializable]
        internal class DestinationFileChecker : IValueChecker<FileInfo>
        {
            /// <summary>
            /// 存在しないディレクトリを許容するかどうかを表す値を取得または設定します。
            /// </summary>
            public bool AllowMissedDirectory { get; set; }

            /// <summary>
            /// 上書きを許容するかどうかを表す値を取得または設定します。
            /// </summary>
            public bool AllowOverwrite { get; set; }

            /// <summary>
            /// <see cref="DestinationFileChecker"/>の新しいインスタンスを初期化します。
            /// </summary>
            public DestinationFileChecker()
            {
                AllowMissedDirectory = true;
                AllowOverwrite = true;
            }

            /// <inheritdoc/>
            public ValueCheckState CheckValue(FileInfo value)
            {
                ArgumentNullException.ThrowIfNull(value);

                DirectoryInfo? directory = value.Directory;
                if (!AllowMissedDirectory && directory is not null && !directory.Exists) return ValueCheckState.AsError($"ファイル'{value.Name}'のディレクトリが存在しません");
                if (!AllowOverwrite && value.Exists) return ValueCheckState.AsError($"ファイル'{value.Name}'が既に存在しています");
                return ValueCheckState.Success;
            }
        }

        /// <summary>
        /// 読み込むディレクトリを検証します。
        /// </summary>
        [Serializable]
        internal class SourceDirectoryChecker : IValueChecker<DirectoryInfo>
        {
            /// <summary>
            /// <see cref="SourceDirectoryChecker"/>の新しいインスタンスを初期化します。
            /// </summary>
            public SourceDirectoryChecker()
            {
            }

            /// <inheritdoc/>
            public ValueCheckState CheckValue(DirectoryInfo value)
            {
                ArgumentNullException.ThrowIfNull(value);

                if (!value.Exists) return ValueCheckState.AsError($"ディレクトリ'{value.Name}'が存在しません");
                return ValueCheckState.Success;
            }
        }

        /// <summary>
        /// 正規表現にマッチするかどうかを検証します。
        /// </summary>
        [Serializable]
        private sealed class RegexMatchValueChecker : IValueChecker<string>
        {
            private readonly Regex regex;

            /// <summary>
            /// <see cref="RegexMatchValueChecker"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="regex">使用する正規表現オブジェクト</param>
            /// <exception cref="ArgumentNullException"><paramref name="regex"/>が<see langword="null"/></exception>
            internal RegexMatchValueChecker(Regex regex)
            {
                ArgumentNullException.ThrowIfNull(regex);

                this.regex = regex;
            }

            public ValueCheckState CheckValue(string value)
            {
                if (regex.IsMatch(value)) return ValueCheckState.Success;
                return ValueCheckState.AsError($"正規表現'{regex}'にマッチしません");
            }
        }
    }
}