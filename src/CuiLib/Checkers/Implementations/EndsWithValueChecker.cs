using System;

namespace CuiLib.Checkers.Implementations
{
    /// <summary>
    /// 文字列が指定の値で終わるかどうかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class EndsWithValueChecker : IValueChecker<string>
    {
        /// <summary>
        /// 検索する値を取得します。
        /// </summary>
        public string Comparison { get; }

        /// <summary>
        /// 比較方式を取得します。
        /// </summary>
        public StringComparison StringComparison { get; }

        /// <summary>
        /// <see cref="EndsWithValueChecker"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="comparison">終了文字</param>
        /// <param name="stringComparison">文字列の比較方法</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stringComparison"/>が非定義の値</exception>
        internal EndsWithValueChecker(char comparison, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            ThrowHelpers.ThrowIfNotDefined(stringComparison);

            Comparison = comparison.ToString();
            StringComparison = stringComparison;
        }

        /// <summary>
        /// <see cref="EndsWithValueChecker"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="comparison">終了文字列</param>
        /// <param name="stringComparison">文字列の比較方法</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="comparison"/>が空文字</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stringComparison"/>が非定義の値</exception>
        internal EndsWithValueChecker(string comparison, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            ThrowHelpers.ThrowIfNullOrEmpty(comparison);
            ThrowHelpers.ThrowIfNotDefined(stringComparison);

            Comparison = comparison;
            StringComparison = stringComparison;
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(string value)
        {
            if (value is null || !value.EndsWith(Comparison, StringComparison)) return ValueCheckState.AsError($"値は'{Comparison}'で終わる必要があります");
            return ValueCheckState.Success;
        }
    }
}
