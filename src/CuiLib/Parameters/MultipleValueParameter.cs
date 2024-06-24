using CuiLib.Checkers;
using CuiLib.Converters;
using System;

namespace CuiLib.Parameters
{
    /// <summary>
    /// 複数の値を取る<see cref="Parameter{T}"/>の実装です。
    /// </summary>
    /// <typeparam name="T">値の型</typeparam>
    [Serializable]
    public class MultipleValueParameter<T> : Parameter<T[]>
    {
        /// <inheritdoc/>
        public override sealed bool IsArray => true;

        /// <inheritdoc/>
        public override T[] Value
        {
            get
            {
                if (ValueAvailable)
                {
                    if (_valueCache is null)
                    {
                        if (RawValues.Length == 0) return [];
                        var result = new T[RawValues.Length];
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
                if (Required) ThrowHelpers.ThrowAsEmptyParameter(this);

                return DefaultValue;
            }
        }

        [NonSerialized]
        private T[]? _valueCache;

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

        /// <summary>
        /// <see cref="MultipleValueParameter{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name">パラメータ名</param>
        /// <param name="index">インデックス</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="name"/>が空文字</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>が0未満</exception>
        public MultipleValueParameter(string name, int index) : base(name, index)
        {
            DefaultValue = [];
        }

        /// <inheritdoc/>
        internal override void SetValue(string rawValue)
        {
            base.SetValue(rawValue);
            _valueCache = null;
        }

        /// <inheritdoc/>
        internal override void SetValue(ReadOnlySpan<string> rawValues)
        {
            base.SetValue(rawValues);
            _valueCache = null;
        }
    }
}
