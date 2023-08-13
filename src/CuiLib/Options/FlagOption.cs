using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CuiLib.Options
{
    /// <summary>
    /// フラグを表すオプションです。
    /// </summary>
    [Serializable]
    public sealed class FlagOption : ValuedOption<bool>
    {
        /// <inheritdoc/>
        internal override OptionType OptionType => OptionType.Flag | OptionType.SingleValue;

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override ReadOnlyCollection<string>? RawValues => null;

        /// <inheritdoc/>
        public override string? ValueTypeName => null;

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Required
        {
            get => false;
            set => throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override bool Value => ValueAvailable || DefaultValue;

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
