using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CuiLib.Options
{
    /// <summary>
    /// コマンドのオプションを表します。
    /// </summary>
    /// <typeparam name="T">オプションの値の型</typeparam>
    [Serializable]
    public abstract class Option<T>
    {
        /// <summary>
        /// オプションが値をとるかどうかを取得します。
        /// </summary>
        public abstract bool IsValued { get; }

        /// <summary>
        /// 文字列としての値を取得します。
        /// </summary>
        public string? RawValue { get; internal set; }

        /// <summary>
        /// オプション名を取得します。
        /// </summary>
        public string? FullName { get; }

        /// <summary>
        /// 短縮名を取得します。
        /// </summary>
        public string? ShortName { get; }

        /// <summary>
        /// オプションの値を取得します。
        /// </summary>
        public T? Value { get; private set; }

        /// <summary>
        /// <see cref="Option{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        protected Option(char shortName)
        {
            ShortName = shortName.ToString();
        }

        /// <summary>
        /// <see cref="Option{T}"/>の新しいインスタンスを初期化します。
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
        /// <see cref="Option{T}"/>の新しいインスタンスを初期化します。
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
        /// 文字列から値を変換します。
        /// </summary>
        /// <param name="value">文字列</param>
        /// <param name="result">変換後の値</param>
        /// <returns><paramref name="value"/>の変換に成功したらtrue，それ以外でfalse</returns>
        protected static bool Convert(string? value, [MaybeNullWhen(true)] out T? result)
        {
            var type = typeof(T);
            result = default;
            if (value is null) return default(T) is null;

            if (type == typeof(string)) Unsafe.As<T?, string>(ref result) = value;
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
            else return false;

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
    }
}
