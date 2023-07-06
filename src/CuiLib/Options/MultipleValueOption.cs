using System;

namespace CuiLib.Options
{
    /// <summary>
    /// 複数の値をとるコマンドのオプションを表します。
    /// </summary>
    /// <typeparam name="T">オプションの値の型</typeparam>
    [Serializable]
    public class MultipleValueOption<T> : ValuedOption<T[]>
    {
        /// <inheritdoc/>
        internal override OptionType OptionType => OptionType.Valued | OptionType.MultiValue;

        /// <inheritdoc/>
        public override string? ValueTypeName => ValueConverter.GetValueTypeString<T>();

        /// <summary>
        /// 値の変換を行う<see cref="ValueChecker{T}"/>を取得または設定します。
        /// </summary>
        public ValueConverter<string, T> Converter
        {
            get => _converter ?? ValueConverter.GetDefault<T>();
            set => _converter = value;
        }

        private ValueConverter<string, T>? _converter;

        /// <summary>
        /// 値の妥当性を検証する関数を取得または設定します。
        /// </summary>
        /// <remarks>既定値では無条件でOK</remarks>
        /// <exception cref="ArgumentNullException">設定しようとした値がnull</exception>
        public ValueChecker<T> Checker
        {
            get => _checker;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                _checker = value;
            }
        }

        private ValueChecker<T> _checker = ValueChecker.AlwaysSuccess<T>();

        /// <inheritdoc/>
        public override T[] Value
        {
            get
            {
                if (ValueAvailable)
                {
                    if (_valueCache is null)
                    {
                        if (RawValues.Count == 0) return Array.Empty<T>();
                        var result = new T[RawValues.Count];
                        for (int i = 0; i < result.Length; i++)
                        {
                            try
                            {
                                result[i] = Converter.Convert(RawValues[i]);
                            }
                            catch (Exception e)
                            {
                                ThrowHelper.ThrowAsOptionParseFailed(e);
                                return default;
                            }
                            ValueCheckState state = Checker.CheckValue(result[i]);
                            ThrowHelper.ThrowIfInvalidState(state);
                        }

                        _valueCache = result;
                    }
                    return _valueCache;
                }
                if (Required) ThrowHelper.ThrowAsEmptyOption(this);

                return DefaultValue;
            }
        }

        private T[]? _valueCache;

        /// <inheritdoc/>
        internal override string? DefaultValueString => $"[{string.Join(", ", DefaultValue)}]";

        /// <summary>
        /// <see cref="MultipleValueOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        public MultipleValueOption(char shortName) : base(shortName)
        {
            DefaultValue = Array.Empty<T>();
        }

        /// <summary>
        /// <see cref="MultipleValueOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        public MultipleValueOption(string fullName) : base(fullName)
        {
            DefaultValue = Array.Empty<T>();
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
            DefaultValue = Array.Empty<T>();
        }

        /// <inheritdoc/>
        internal override void ClearValue()
        {
            base.ClearValue();
            _valueCache = null;
        }

        /// <inheritdoc/>
        internal override void ApplyValue(string rawValue)
        {
            base.ApplyValue(rawValue);
            _valueCache = null;
        }
    }
}
