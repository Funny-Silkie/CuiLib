using System;

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
        /// オプションが値をとるかどうかを取得します。
        /// </summary>
        public abstract bool IsValued { get; }

        /// <summary>
        /// 値を受け取ったかどうかを表す値を取得します。
        /// </summary>
        public abstract bool ValueAvailable { get; }

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
        internal abstract void SetValue(string? rawValue);
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
        public virtual string? RawValue => _rawValue;

        private string? _rawValue;

        /// <summary>
        /// 値を受け取ったかどうかを表す値を取得します。
        /// </summary>
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
        public virtual T Value
        {
            get
            {
                if (ValueAvailable)
                {
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
                    if (!ValueConverter.Convert(RawValue, out Exception? error, out T result))
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
            _rawValue = null;
        }

        /// <summary>
        /// 値を設定します。
        /// </summary>
        /// <param name="rawValue">文字列としての値</param>
        internal override void SetValue(string? rawValue)
        {
            _rawValue = rawValue;
            _valueAvailable = true;
            _ = ValueAvailable;
        }
    }
}
