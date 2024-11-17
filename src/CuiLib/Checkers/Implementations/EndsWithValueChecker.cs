using System;

namespace CuiLib.Checkers.Implementations
{
    /// <summary>
    /// 文字列が指定の値で終わるかどうかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class EndsWithValueChecker : IValueChecker<string>
    {
        private readonly string comparison;
        private readonly StringComparison stringComparison;

        /// <summary>
        /// <see cref="EndsWithValueChecker"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="comparison">終了文字</param>
        /// <param name="stringComparison">文字列の比較方法</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stringComparison"/>が非定義の値</exception>
        internal EndsWithValueChecker(char comparison, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            ThrowHelpers.ThrowIfNotDefined(stringComparison);

            this.comparison = comparison.ToString();
            this.stringComparison = stringComparison;
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
}
