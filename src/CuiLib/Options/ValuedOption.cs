using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace CuiLib.Options
{
    /// <summary>
    /// 値を持つのオプションを表します。
    /// </summary>
    /// <typeparam name="T">オプションの値の型</typeparam>
    [Serializable]
    public abstract class ValuedOption<T> : NamedOption
    {
        /// <summary>
        /// 文字列としての値を取得します。
        /// </summary>
        protected virtual ReadOnlyCollection<string>? RawValues => _rawValues?.AsReadOnly();

        private List<string>? _rawValues;

        /// <summary>
        /// 値を受け取ったかどうかを表す値を取得します。
        /// </summary>
        [MemberNotNullWhen(true, nameof(RawValues))]
        public override sealed bool ValueAvailable => _valueAvailable;

        private bool _valueAvailable;

        /// <summary>
        /// デフォルトの値を取得または設定します。
        /// </summary>
        public T DefaultValue { get; set; } = default!;

        /// <inheritdoc/>
        internal override string? DefaultValueString => DefaultValue?.ToString();

        /// <summary>
        /// オプションの値を取得します。
        /// </summary>
        /// <exception cref="ArgumentAnalysisException">値の変換に失敗-または-変換後の値が無効</exception>
        public abstract T Value { get; }

        /// <summary>
        /// <see cref="ValuedOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        protected ValuedOption(char shortName) : base(shortName)
        {
        }

        /// <summary>
        /// <see cref="ValuedOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        protected ValuedOption(string fullName) : base(fullName)
        {
        }

        /// <summary>
        /// <see cref="ValuedOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        protected ValuedOption(char shortName, string fullName) : base(shortName, fullName)
        {
        }

        /// <summary>
        /// 設定されている値をクリアします。
        /// </summary>
        internal override void ClearValue()
        {
            _valueAvailable = false;
            _rawValues = null;
        }

        /// <summary>
        /// 値を設定します。
        /// </summary>
        /// <param name="rawValue">文字列としての値</param>
        internal override void ApplyValue(string rawValue)
        {
            _rawValues ??= new List<string>();
            _rawValues.Add(rawValue);
            _valueAvailable = true;
            _ = ValueAvailable;
        }
    }
}
