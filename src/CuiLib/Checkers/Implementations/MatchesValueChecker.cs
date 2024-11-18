using System;
using System.Text.RegularExpressions;

namespace CuiLib.Checkers.Implementations
{
    /// <summary>
    /// 正規表現にマッチするかどうかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class MatchesValueChecker : IValueChecker<string>
    {
        /// <summary>
        /// 正規表現オブジェクトを取得します。
        /// </summary>
        public Regex Regex { get; }

        /// <summary>
        /// <see cref="MatchesValueChecker"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="regex">使用する正規表現オブジェクト</param>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/>が<see langword="null"/></exception>
        internal MatchesValueChecker(Regex regex)
        {
            ThrowHelpers.ThrowIfNull(regex);

            Regex = regex;
        }

        public ValueCheckState CheckValue(string value)
        {
            if (Regex.IsMatch(value)) return ValueCheckState.Success;
            return ValueCheckState.AsError($"正規表現'{Regex}'にマッチしません");
        }
    }
}
