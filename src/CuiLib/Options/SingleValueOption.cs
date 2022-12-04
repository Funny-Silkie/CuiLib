using System;

namespace CuiLib.Options
{
    /// <summary>
    /// 1つの値をとるコマンドのオプションを表します。
    /// </summary>
    /// <typeparam name="T">オプションの値の型</typeparam>
    [Serializable]
    public class SingleValueOption<T> : ValuedOption<T>
    {
        /// <inheritdoc/>
        internal override OptionType OptionType => OptionType.Valued | OptionType.SingleValue;

        /// <inheritdoc/>
        public override string? ValueTypeName => ValueConverter.GetValueTypeString<T>();

        /// <inheritdoc/>
        public override T Value
        {
            get
            {
                if (ValueAvailable)
                {
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
                    if (!ValueConverter.Convert(RawValues[0], out Exception? error, out T result))
#pragma warning restore CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
                    {
                        ThrowHelper.ThrowAsOptionParseFailed(error);
                        return default;
                    }

                    ValueCheckState state = Checker.CheckValue(result);
                    ThrowHelper.ThrowIfInvalidState(state);

                    return result;
                }
                if (Required) ThrowHelper.ThrowAsEmptyOption(this);

                return DefaultValue;
            }
        }

        /// <summary>
        /// <see cref="SingleValueOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        public SingleValueOption(char shortName) : base(shortName)
        {
        }

        /// <summary>
        /// <see cref="SingleValueOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        public SingleValueOption(string fullName) : base(fullName)
        {
        }

        /// <summary>
        /// <see cref="SingleValueOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        public SingleValueOption(char shortName, string fullName) : base(shortName, fullName)
        {
        }
    }
}
