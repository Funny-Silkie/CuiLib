using System;
using CuiLib.Checkers;
using CuiLib.Converters;

namespace CuiLib.Options
{
    /// <summary>
    /// 複数の値をとるコマンドのオプションを表します。
    /// </summary>
    /// <typeparam name="T">オプションの値の型</typeparam>
    [Serializable]
    public class MultipleValueOption<T> : ValueSpecifiedOption<T[]>
    {
        /// <inheritdoc/>
        internal override OptionType OptionType => OptionType.Valued | OptionType.MultiValue;

        /// <inheritdoc/>
        public override string? ValueTypeName => ValueConverter.GetValueTypeString<T>();

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
        public override T[] Value
        {
            get
            {
                if (ValueAvailable)
                {
                    if (_valueCache is null)
                    {
                        if (RawValues.Count == 0) return [];
                        var result = new T[RawValues.Count];
                        for (int i = 0; i < result.Length; i++)
                        {
                            try
                            {
                                result[i] = Converter.Convert(RawValues[i]);
                            }
                            catch (Exception e)
                            {
                                ThrowHelpers.ThrowAsOptionParseFailed(e);
                                return default;
                            }
                            ValueCheckState state = Checker.CheckValue(result[i]);
                            ThrowHelpers.ThrowIfInvalidState(state);
                        }

                        _valueCache = result;
                    }
                    return _valueCache;
                }
                if (Required) ThrowHelpers.ThrowAsEmptyOption(this);

                return DefaultValue;
            }
        }

        private T[]? _valueCache;

        /// <summary>
        /// <see cref="MultipleValueOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        public MultipleValueOption(char shortName) : base(shortName)
        {
            DefaultValue = [];
        }

        /// <summary>
        /// <see cref="MultipleValueOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        public MultipleValueOption(string fullName) : base(fullName)
        {
            DefaultValue = [];
        }

        /// <summary>
        /// <see cref="MultipleValueOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        public MultipleValueOption(char shortName, string fullName) : base(shortName, fullName)
        {
            DefaultValue = [];
        }

        /// <inheritdoc/>
        internal override void ClearValue()
        {
            base.ClearValue();
            _valueCache = null;
        }

        /// <inheritdoc/>
        internal override void ApplyValue(string name, string rawValue)
        {
            base.ApplyValue(name, rawValue);
            _valueCache = null;
        }
    }
}
