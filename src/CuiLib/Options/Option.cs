using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace CuiLib.Options
{
    /// <summary>
    /// コマンドのオプションを表します。
    /// </summary>
    [Serializable]
    public abstract class Option
    {
        /// <summary>
        /// オプション名を取得します。
        /// </summary>
        public string? FullName { get; }

        /// <summary>
        /// 短縮名を取得します。
        /// </summary>
        public string? ShortName { get; }

        /// <summary>
        /// オプションの説明を取得または設定します。
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 取得する値の種類を取得します。
        /// </summary>
        public abstract string? ValueTypeName { get; }

        /// <summary>
        /// 値を受け取ったかどうかを表す値を取得します。
        /// </summary>
        public abstract bool ValueAvailable { get; }

        /// <summary>
        /// オプションの種類を取得します。
        /// </summary>
        internal abstract OptionType OptionType { get; }

        /// <summary>
        /// オプションが値をとるかどうかを取得します。
        /// </summary>
        internal bool IsValued => OptionType.HasFlag(OptionType.Valued);

        /// <summary>
        /// 複数の値を取れるかどうかを取得します。
        /// </summary>
        internal bool CanMultiValue => OptionType.HasFlag(OptionType.MultiValue);

        /// <summary>
        /// 必須のオプションかどうかを取得または設定します。
        /// </summary>
        public abstract bool Required { get; set; }

        /// <summary>
        /// 既定値の文字列を取得します。
        /// </summary>
        internal abstract string? DefaultValueString { get; }

        /// <summary>
        /// <see cref="Option"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        protected Option(char shortName)
        {
            ShortName = shortName.ToString();
        }

        /// <summary>
        /// <see cref="Option"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        protected Option(string fullName)
        {
            ThrowHelper.ThrowIfNullOrEmpty(fullName);

            FullName = fullName;
        }

        /// <summary>
        /// <see cref="Option"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        protected Option(char shortName, string fullName)
        {
            ThrowHelper.ThrowIfNullOrEmpty(fullName);

            FullName = fullName;
            ShortName = shortName.ToString();
        }

        /// <summary>
        /// 設定されている値をクリアします。
        /// </summary>
        internal abstract void ClearValue();

        /// <summary>
        /// 値を設定します。
        /// </summary>
        /// <param name="rawValue">文字列としての値</param>
        internal abstract void ApplyValue(string rawValue);
    }

    /// <summary>
    /// コマンドのオプションを表します。
    /// </summary>
    /// <typeparam name="T">オプションの値の型</typeparam>
    [Serializable]
    public abstract class Option<T> : Option
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
        internal override sealed string? DefaultValueString => DefaultValue?.ToString();

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
        /// オプションの値を取得します。
        /// </summary>
        /// <exception cref="ArgumentAnalysisException">値の変換に失敗-または-変換後の値が無効</exception>
        public abstract T Value { get; }

        /// <summary>
        /// <see cref="Option{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        protected Option(char shortName) : base(shortName)
        {
        }

        /// <summary>
        /// <see cref="Option{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        protected Option(string fullName) : base(fullName)
        {
        }

        /// <summary>
        /// <see cref="Option{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        protected Option(char shortName, string fullName) : base(shortName, fullName)
        {
        }

        /// <summary>
        /// 指定した名前がインスタンスのオプション名に一致するかどうかを判定します。
        /// </summary>
        /// <param name="name">判定するオプション名</param>
        /// <returns><paramref name="name"/>がインスタンスのオプション名だったらtrue，それ以外でfalse</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/>が空文字</exception>
        public bool MatchName(string name)
        {
            ThrowHelper.ThrowIfNullOrEmpty(name);

            return (ShortName != null && ShortName == name) || (FullName != null && FullName == name);
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
