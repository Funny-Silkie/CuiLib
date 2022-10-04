using System;

namespace CuiLib.Options
{
    /// <summary>
    /// フラグを表すオプションです。
    /// </summary>
    [Serializable]
    public class FlagOption : Option<bool>
    {
        /// <inheritdoc/>
        public override sealed bool IsValued => false;

        /// <summary>
        /// <see cref="FlagOption"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        public FlagOption(char shortName) : base(shortName)
        {
        }

        /// <summary>
        /// <see cref="FlagOption"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        public FlagOption(string fullName) : base(fullName)
        {
        }

        /// <summary>
        /// <see cref="FlagOption"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        public FlagOption(char shortName, string fullName) : base(shortName, fullName)
        {
        }
    }
}
