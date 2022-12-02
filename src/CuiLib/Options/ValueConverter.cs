using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;

namespace CuiLib.Options
{
    /// <summary>
    /// 値の変換を扱います。
    /// </summary>
    [Serializable]
    public static class ValueConverter
    {
        /// <summary>
        /// 文字列から値を変換します。
        /// </summary>
        /// <typeparam name="T">値の型</typeparam>
        /// <param name="value">文字列</param>
        /// <param name="error">
        /// 変換時のエラー
        /// <list type="bullet">
        /// <item>
        /// <term><see cref="FormatException"/></term>
        /// <description><paramref name="value"/>のフォーマットが無効</description>
        /// </item>
        /// <item>
        /// <term><see cref="NotSupportedException"/></term>
        /// <description><typeparamref name="T"/>が変換不能な型</description>
        /// </item>
        /// </list>
        /// </param>
        /// <param name="result">変換後の値</param>
        /// <returns><paramref name="value"/>の変換に成功したらtrue，それ以外でfalse</returns>
        public static bool Convert<T>(string? value, [NotNullWhen(false)] out Exception? error, [MaybeNullWhen(false)] out T result)
        {
            error = null;
            var type = typeof(T);
            result = default!;
            if (value is null) return default(T) is null;

            try
            {
                if (type == typeof(string)) Unsafe.As<T, string>(ref result) = value;
                else if (type == typeof(FileInfo)) Unsafe.As<T, FileInfo>(ref result) = new FileInfo(value);
                else if (type == typeof(DirectoryInfo)) Unsafe.As<T, DirectoryInfo>(ref result) = new DirectoryInfo(value);
                else if (type == typeof(int)) Unsafe.As<T, int>(ref result) = int.Parse(value);
                else if (type == typeof(sbyte)) Unsafe.As<T, sbyte>(ref result) = sbyte.Parse(value);
                else if (type == typeof(double)) Unsafe.As<T, double>(ref result) = double.Parse(value);
                else if (type == typeof(long)) Unsafe.As<T, long>(ref result) = long.Parse(value);
                else if (type == typeof(ulong)) Unsafe.As<T, ulong>(ref result) = ulong.Parse(value);
                else if (type == typeof(DateTime)) Unsafe.As<T, DateTime>(ref result) = DateTime.Parse(value);
                else if (type == typeof(short)) Unsafe.As<T, short>(ref result) = short.Parse(value);
                else if (type == typeof(byte)) Unsafe.As<T, byte>(ref result) = byte.Parse(value);
                else if (type == typeof(ushort)) Unsafe.As<T, ushort>(ref result) = ushort.Parse(value);
                else if (type == typeof(uint)) Unsafe.As<T, uint>(ref result) = uint.Parse(value);
                else if (type == typeof(float)) Unsafe.As<T, float>(ref result) = float.Parse(value);
                else if (type == typeof(decimal)) Unsafe.As<T, decimal>(ref result) = decimal.Parse(value);
                else if (type == typeof(char)) Unsafe.As<T, char>(ref result) = char.Parse(value);
                else if (type == typeof(bool)) Unsafe.As<T, bool>(ref result) = bool.Parse(value);
                else if (type == typeof(TimeSpan)) Unsafe.As<T, TimeSpan>(ref result) = TimeSpan.Parse(value);
                else if (type == typeof(DateOnly)) Unsafe.As<T, DateOnly>(ref result) = DateOnly.Parse(value);
                else if (type == typeof(TimeOnly)) Unsafe.As<T, TimeOnly>(ref result) = TimeOnly.Parse(value);
                else if (type.IsEnum) Unsafe.As<T, object>(ref result) = Enum.Parse(type, value);
                else
                {
                    error = new NotSupportedException("無効な型への変換です");
                    result = default;
                    return false;
                }
            }
            catch (Exception e)
            {
                error = e;
                result = default;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 値の種類を表す文字列を取得します。
        /// </summary>
        /// <typeparam name="T">値の型</typeparam>
        /// <returns><typeparamref name="T"/>に対応する文字列。存在しない場合はnull</returns>
        public static string? GetValueTypeString<T>()
        {
            var type = typeof(T);
            if (type == typeof(string)) return "string";
            else if (type == typeof(FileInfo)) return "file";
            else if (type == typeof(DirectoryInfo)) return "directory";
            else if (type == typeof(int)) return "int";
            else if (type == typeof(sbyte)) return "int";
            else if (type == typeof(double)) return "float";
            else if (type == typeof(long)) return "long";
            else if (type == typeof(ulong)) return "long";
            else if (type == typeof(DateTime)) return "date time";
            else if (type == typeof(short)) return "int";
            else if (type == typeof(byte)) return "int";
            else if (type == typeof(ushort)) return "int";
            else if (type == typeof(uint)) return "uint";
            else if (type == typeof(float)) return "float";
            else if (type == typeof(decimal)) return "decimal";
            else if (type == typeof(char)) return "char";
            else if (type == typeof(bool)) return "bool";
            else if (type == typeof(TimeSpan)) return "date time";
            else if (type == typeof(DateOnly)) return "date";
            else if (type == typeof(TimeOnly)) return "time";
            else if (type.IsEnum) return "string";
            return null;
        }
    }
}
