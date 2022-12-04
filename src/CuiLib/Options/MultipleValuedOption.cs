using System;
using System.Linq;

namespace CuiLib.Options
{
    /// <summary>
    /// 複数の値をとるコマンドのオプションを表します。
    /// </summary>
    /// <typeparam name="T">オプションの値の型</typeparam>
    [Serializable]
    public class MultipleValuedOption<T> : ValuedOption<T[]>
    {
        /// <inheritdoc/>
        internal override OptionType OptionType => OptionType.Valued | OptionType.MultiValue;

        /// <inheritdoc/>
        public override string? ValueTypeName => ValueConverter.GetValueTypeString<T>();

        /// <inheritdoc/>
        public override T[] Value
        {
            get
            {
                if (ValueAvailable)
                {
                    T[] result = Array.ConvertAll(RawValues.ToArray(), x =>
                    {
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
                        if (!ValueConverter.Convert(x, out Exception? error, out T ret))
#pragma warning restore CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
                        {
                            ThrowHelper.ThrowAsOptionParseFailed(error);
                            return default;
                        }
                        return ret;
                    });

                    ValueCheckState state = Checker.CheckValue(result);
                    ThrowHelper.ThrowIfInvalidState(state);

                    return result;
                }
                if (Required) ThrowHelper.ThrowAsEmptyOption(this);

                return DefaultValue;
            }
        }

        /// <summary>
        /// <see cref="MultipleValuedOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        public MultipleValuedOption(char shortName) : base(shortName)
        {
            DefaultValue = Array.Empty<T>();
        }

        /// <summary>
        /// <see cref="MultipleValuedOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        public MultipleValuedOption(string fullName) : base(fullName)
        {
            DefaultValue = Array.Empty<T>();
        }

        /// <summary>
        /// <see cref="MultipleValuedOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        public MultipleValuedOption(char shortName, string fullName) : base(shortName, fullName)
        {
            DefaultValue = Array.Empty<T>();
        }
    }
}
