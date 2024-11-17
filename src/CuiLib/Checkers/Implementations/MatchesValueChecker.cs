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
        private readonly Regex regex;

        /// <summary>
        /// <see cref="MatchesValueChecker"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="regex">使用する正規表現オブジェクト</param>
        /// <exception cref="ArgumentNullException"><paramref name="regex"/>が<see langword="null"/></exception>
        internal MatchesValueChecker(Regex regex)
        {
            ThrowHelpers.ThrowIfNull(regex);

            this.regex = regex;
        }

        public ValueCheckState CheckValue(string value)
        {
            if (regex.IsMatch(value)) return ValueCheckState.Success;
            return ValueCheckState.AsError($"正規表現'{regex}'にマッチしません");
        }
    }
}
