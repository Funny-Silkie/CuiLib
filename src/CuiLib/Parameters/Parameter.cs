using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CuiLib.Checkers;
using CuiLib.Converters;

namespace CuiLib.Parameters
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
        public abstract bool IsArray { get; }

        /// <summary>
        /// 必須の値かどうかを取得または設定します。
        /// </summary>
        public bool Required { get; set; }

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
                    ThrowHelpers.ThrowAsEmptyCollection();
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
        /// <exception cref="ArgumentNullException"><paramref name="name"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/>が空文字</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>が0未満</exception>
        protected Parameter(string name, int index)
        {
            ThrowHelpers.ThrowIfNullOrEmpty(name);
            ThrowHelpers.ThrowIfNegative(index);

            Name = name;
            Index = index;
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
        [Obsolete($"Use a constructor of {nameof(SingleValueParameter<T>)} instead")]
        public static SingleValueParameter<T> Create<T>(string name, int index) => new SingleValueParameter<T>(name, index);

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
        [Obsolete($"Use a constructor of {nameof(MultipleValueParameter<T>)} instead")]
        public static MultipleValueParameter<T> CreateAsArray<T>(string name, int index) => new MultipleValueParameter<T>(name, index);

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
            _rawValues = [rawValue];
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
    public abstract class Parameter<T> : Parameter
    {
        /// <summary>
        /// デフォルトの値を取得または設定します。
        /// </summary>
        public T DefaultValue { get; set; } = default!;

        /// <summary>
        /// 値を取得します。
        /// </summary>
        /// <exception cref="InvalidOperationException">インスタンスが空の配列を表す</exception>
        public abstract T Value { get; }

        /// <summary>
        /// <see cref="Parameter{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name">パラメータ名</param>
        /// <param name="index">インデックス</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/>が空文字</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>が0未満</exception>
        protected internal Parameter(string name, int index)
            : base(name, index)
        {
        }
    }
}
