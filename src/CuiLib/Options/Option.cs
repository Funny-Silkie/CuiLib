using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;

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
        public T? DefaultValue { get; set; }

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
        public virtual T? Value
        {
            get
            {
                if (ValueAvailable)
                {
                    if (!Convert(RawValue, out Exception? error, out T? result))
                    {
                        ThrowHelper.ThrowAsOptionParseFailed(error);
                        return default;
                    }

                    ValueCheckState state = Checker.CheckValue(result);
                    ThrowHelper.ThrowIfInvalidState(state);

                    return result;
                }
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
        /// 文字列から値を変換します。
        /// </summary>
        /// <param name="value">文字列</param>
        /// <param name="error">変換時のエラー</param>
        /// <param name="result">変換後の値</param>
        /// <returns><paramref name="value"/>の変換に成功したらtrue，それ以外でfalse</returns>
        protected static bool Convert(string? value, [NotNullWhen(false)] out Exception? error, [MaybeNullWhen(true)] out T? result)
        {
            error = null;
            var type = typeof(T);
            result = default;
            if (value is null) return default(T) is null;

            try
            {
                if (type == typeof(string)) Unsafe.As<T?, string>(ref result) = value;
                else if (type.IsSubclassOf(typeof(FileInfo))) Unsafe.As<T?, FileInfo>(ref result) = new FileInfo(value);
                else if (type.IsSubclassOf(typeof(DirectoryInfo))) Unsafe.As<T?, DirectoryInfo>(ref result) = new DirectoryInfo(value);
                else if (type == typeof(int)) Unsafe.As<T?, int>(ref result) = int.Parse(value);
                else if (type == typeof(sbyte)) Unsafe.As<T?, sbyte>(ref result) = sbyte.Parse(value);
                else if (type == typeof(double)) Unsafe.As<T?, double>(ref result) = double.Parse(value);
                else if (type == typeof(long)) Unsafe.As<T?, long>(ref result) = long.Parse(value);
                else if (type == typeof(ulong)) Unsafe.As<T?, ulong>(ref result) = ulong.Parse(value);
                else if (type == typeof(DateTime)) Unsafe.As<T?, DateTime>(ref result) = DateTime.Parse(value);
                else if (type == typeof(short)) Unsafe.As<T?, short>(ref result) = short.Parse(value);
                else if (type == typeof(byte)) Unsafe.As<T?, byte>(ref result) = byte.Parse(value);
                else if (type == typeof(ushort)) Unsafe.As<T?, ushort>(ref result) = ushort.Parse(value);
                else if (type == typeof(uint)) Unsafe.As<T?, uint>(ref result) = uint.Parse(value);
                else if (type == typeof(float)) Unsafe.As<T?, float>(ref result) = float.Parse(value);
                else if (type == typeof(decimal)) Unsafe.As<T?, decimal>(ref result) = decimal.Parse(value);
                else if (type == typeof(char)) Unsafe.As<T?, char>(ref result) = char.Parse(value);
                else if (type == typeof(bool)) Unsafe.As<T?, bool>(ref result) = bool.Parse(value);
                else if (type == typeof(TimeSpan)) Unsafe.As<T?, TimeSpan>(ref result) = TimeSpan.Parse(value);
                else if (type == typeof(DateOnly)) Unsafe.As<T?, DateOnly>(ref result) = DateOnly.Parse(value);
                else if (type == typeof(TimeOnly)) Unsafe.As<T?, TimeOnly>(ref result) = TimeOnly.Parse(value);
                else if (type.IsEnum) Unsafe.As<T?, object>(ref result) = Enum.Parse(type, value);
                else
                {
                    error = new NotSupportedException("無効な型への変換です");
                    return false;
                }
            }
            catch (Exception e)
            {
                error = e;
                return false;
            }

            return true;
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
        }
    }
}
