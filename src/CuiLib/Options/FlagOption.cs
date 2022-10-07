using System;

namespace CuiLib.Options
{
    /// <summary>
    /// フラグを表すオプションです。
    /// </summary>
    [Serializable]
    public sealed class FlagOption : Option<bool>
    {
        /// <inheritdoc/>
        public override sealed bool IsValued => false;

        /// <inheritdoc/>
        public override string? RawValue => null;

        /// <inheritdoc/>
        public override bool Required
        {
            get => false;
            set
            {
            }
        }

        /// <inheritdoc/>
        public override bool Value => ValueAvailable && DefaultValue;

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
