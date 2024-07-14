using System;
using CuiLib.Checkers;
using CuiLib.Converters;

namespace CuiLib.Options
{
    /// <summary>
    /// 1つの値をとるコマンドのオプションを表します。
    /// </summary>
    /// <typeparam name="T">オプションの値の型</typeparam>
    [Serializable]
    public class SingleValueOption<T> : ValueSpecifiedOption<T>, IValuedOption
    {
        /// <inheritdoc/>
        internal override OptionType OptionType => OptionType.Valued | OptionType.SingleValue;

        /// <summary>
        /// 値の変換を行う<see cref="IValueConverter{TIn, TOut}"/>を取得または設定します。
        /// </summary>
        public IValueConverter<string, T> Converter
        {
            get => _converter ?? ValueConverter.GetDefault<T>();
            set => _converter = value;
        }

        private IValueConverter<string, T>? _converter;

        /// <summary>
        /// 値の妥当性を検証する関数を取得または設定します。
        /// </summary>
        /// <remarks>既定値では無条件でOK</remarks>
        /// <exception cref="ArgumentNullException">設定しようとした値がnull</exception>
        public IValueChecker<T> Checker
        {
            get => _checker;
            set
            {
                ThrowHelpers.ThrowIfNull(value);
                _checker = value;
            }
        }

        private IValueChecker<T> _checker = ValueChecker.AlwaysValid<T>();

        /// <inheritdoc/>
        public override T Value
        {
            get
            {
                if (ValueAvailable)
                {
                    T result;
                    try
                    {
                        result = Converter.Convert(RawValues[0]);
                    }
                    catch (Exception e)
                    {
                        ThrowHelpers.ThrowAsOptionParseFailed(e);
                        return default;
                    }

                    ValueCheckState state = Checker.CheckValue(result);
                    ThrowHelpers.ThrowIfInvalidState(state);

                    return result;
                }
                if (Required) ThrowHelpers.ThrowAsEmptyOption(this);

                return DefaultValue;
            }
        }

        int IValuedOption.ValueCount => 1;

        /// <summary>
        /// <see cref="SingleValueOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        public SingleValueOption(char shortName) : base(shortName)
        {
            Initialize();
        }

        /// <summary>
        /// <see cref="SingleValueOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        public SingleValueOption(string fullName) : base(fullName)
        {
            Initialize();
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
            Initialize();
        }

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        private void Initialize()
        {
            ValueTypeName = ValueConverter.GetValueTypeString<T>();
        }
    }
}
