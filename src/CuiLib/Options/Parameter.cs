using System;
using System.Diagnostics.CodeAnalysis;

namespace CuiLib.Options
{
    /// <summary>
    /// パラメータ引数を表します。
    /// </summary>
    [Serializable]
    public abstract class Parameter
    {
        /// <summary>
        /// インデックスを取得します。
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// オプションの説明を取得または設定します。
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// パラメータ名を取得します。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 配列かどうかを取得します。
        /// </summary>
        public bool IsArray { get; }

        /// <summary>
        /// 値を受け取ったかどうかを表す値を取得します。
        /// </summary>
        [MemberNotNullWhen(true, nameof(_rawValues), nameof(RawValue), nameof(RawValues))]
        public virtual bool ValueAvailable => _rawValues is not null;

        /// <summary>
        /// 文字列としての値を取得します。
        /// </summary>
        /// <exception cref="InvalidOperationException">インスタンスが空の配列を表す</exception>
        public string? RawValue
        {
            get
            {
                try
                {
                    return _rawValues?[0];
                }
                catch (IndexOutOfRangeException)
                {
                    ThrowHelper.ThrowAsEmptyCollection();
                    return default;
                }
            }
        }

        /// <summary>
        /// 文字列としての値を取得します。
        /// </summary>
        public string[]? RawValues => _rawValues;

        private string[]? _rawValues;

        /// <summary>
        /// <see cref="Parameter"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name">パラメータ名</param>
        /// <param name="index">インデックス</param>
        /// <param name="isArray">配列かどうか</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/>が空文字</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>が0未満</exception>
        protected Parameter(string name, int index, bool isArray)
        {
            ThrowHelper.ThrowIfNullOrEmpty(name);
            ThrowHelper.ThrowIfNegative(index);

            Name = name;
            Index = index;
            IsArray = isArray;
        }

        /// <summary>
        /// 単一の値をとる<see cref="Parameter{T}"/>の新しいインスタンスを生成します。
        /// </summary>
        /// <typeparam name="T">パラメータの値の型</typeparam>
        /// <param name="name">パラメータ名</param>
        /// <param name="index">インデックス</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/>が空文字</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>が0未満</exception>
        /// <returns>単一の値をとる<see cref="Parameter{T}"/>の新しいインスタンス</returns>
        public static Parameter<T> Create<T>(string name, int index) => new Parameter<T>(name, index, false);

        /// <summary>
        /// 配列をとしての<see cref="Parameter{T}"/>の新しいインスタンスを生成します。
        /// </summary>
        /// <typeparam name="T">パラメータの値の型</typeparam>
        /// <param name="name">パラメータ名</param>
        /// <param name="index">インデックス</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/>が空文字</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>が0未満</exception>
        /// <returns>配列をとしての<see cref="Parameter{T}"/>の新しいインスタンス</returns>
        public static Parameter<T> CreateAsArray<T>(string name, int index) => new Parameter<T>(name, index, true);

        /// <summary>
        /// 設定されている値をクリアします。
        /// </summary>
        internal virtual void ClearValue()
        {
            _rawValues = null;
        }

        /// <summary>
        /// 値を設定します。
        /// </summary>
        /// <param name="rawValue">文字列としての値</param>
        internal virtual void SetValue(string rawValue)
        {
            _rawValues = new[] { rawValue };
        }

        /// <summary>
        /// 値を設定します。
        /// </summary>
        /// <param name="rawValues">文字列としての値</param>
        internal virtual void SetValue(ReadOnlySpan<string> rawValues)
        {
            _rawValues = rawValues.ToArray();
        }
    }

    /// <summary>
    /// パラメータ引数を表します。
    /// </summary>
    /// <typeparam name="T">パラメータの値の型</typeparam>
    [Serializable]
    public class Parameter<T> : Parameter
    {
        /// <inheritdoc/>
        [MemberNotNullWhen(true, nameof(Values))]
        public override bool ValueAvailable => base.ValueAvailable;

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
        /// 値を取得します。
        /// </summary>
        /// <exception cref="InvalidOperationException">インスタンスが空の配列を表す</exception>
        public T Value
        {
            get
            {
                if (Values is null) return default!;
                try
                {
                    return Values[0];
                }
                catch (IndexOutOfRangeException)
                {
                    ThrowHelper.ThrowAsEmptyCollection();
                    return default;
                }
            }
        }

        /// <summary>
        /// 値を取得します。
        /// </summary>
        public T[]? Values
        {
            get
            {
                if (_values is not null) return _values;
                if (!ValueAvailable) return null;
                _values = Array.ConvertAll(RawValues, x =>
                {
                    T result;
                    try
                    {
                        result = Converter.Convert(x);
                    }
                    catch (Exception e)
                    {
                        ThrowHelper.ThrowAsOptionParseFailed(e);
                        return default;
                    }
                    ValueCheckState state = Checker.CheckValue(result);
                    ThrowHelper.ThrowIfInvalidState(state);
                    return result;
                });
                return _values;
            }
        }

        private T[]? _values;

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
                ArgumentNullException.ThrowIfNull(_checker);

                _checker = value;
            }
        }

        private ValueChecker<T> _checker = ValueChecker.AlwaysSuccess<T>();

        /// <summary>
        /// <see cref="Parameter{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name">パラメータ名</param>
        /// <param name="index">インデックス</param>
        /// <param name="isArray">配列かどうか</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/>が空文字</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>が0未満</exception>
        internal Parameter(string name, int index, bool isArray)
            : base(name, index, isArray)
        {
        }
    }
}
