using System;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CuiLib.Options;
using CuiLib.Checkers;
using CuiLib.Parameters;

namespace CuiLib
{
    /// <summary>
    /// 例外処理に関連する実装を行います。
    /// </summary>
    internal static class ThrowHelpers
    {
        /// <summary>
        /// ファイル名に使用できない文字を取得します。
        /// </summary>
        private static char[] InvalidFileNameChars => _invalidFileNameChars ??= Path.GetInvalidFileNameChars();

        private static char[]? _invalidFileNameChars;

        /// <summary>
        /// パスに使用できない文字を取得します。
        /// </summary>
        private static char[] InvalidPathChars => _invalidPathChars ??= Path.GetInvalidPathChars();

        private static char[]? _invalidPathChars;

        #region ThrowAs

        /// <summary>
        /// オプションの値変換の失敗として<see cref="ArgumentAnalysisException"/>をスローします。
        /// </summary>
        /// <param name="exception">変換時の例外</param>
        [DoesNotReturn]
        public static void ThrowAsOptionParseFailed(Exception? exception)
        {
            throw new ArgumentAnalysisException(exception?.Message, exception);
        }

        /// <summary>
        /// 未入力のオプションとして<see cref="ArgumentAnalysisException"/>をスローします。
        /// </summary>
        /// <param name="option">当該オプション</param>
        public static void ThrowAsEmptyOption(NamedOption? option)
        {
            if (option == null) return;

            string message = "オプション";
            if (option.FullName != null)
            {
                if (option.ShortName != null) message += $"'--{option.FullName}' ('-{option.ShortName}') ";
                else message += $"'--{option.FullName}'";
            }
            else if (option.ShortName != null) message += $"'-{option.ShortName}'";
            else return;

            message += "を入力してください";

            throw new ArgumentAnalysisException(message);
        }

        /// <summary>
        /// 未入力のパラメータとして<see cref="ArgumentAnalysisException"/>をスローします。
        /// </summary>
        /// <param name="parameter">当該パラメータ</param>
        public static void ThrowAsEmptyParameter(Parameter? parameter)
        {
            if (parameter == null) return;

            throw new ArgumentAnalysisException($"パラメータ'{parameter.Name}'を入力してください");
        }

        /// <summary>
        /// 空のインスタンスとして<see cref="InvalidOperationException"/>をスローします。
        /// </summary>
        [DoesNotReturn]
        public static void ThrowAsEmptyCollection()
        {
            throw new InvalidOperationException("インスタンスが空です");
        }

        #endregion ThrowAs

        #region ThrowIf

        /// <summary>
        /// 値が<see langword="null"/>であるかどうかを検証します。
        /// </summary>
        /// <typeparam name="T">検証する値の型</typeparam>
        /// <param name="value">検証する値</param>
        /// <param name="name">パラメータ名</param>
#if NET6_0_OR_GREATER

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNull<T>([NotNull] T? value, [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            ArgumentNullException.ThrowIfNull(value, name);
        }

#else
        public static void ThrowIfNull<T>([NotNull] T? value, string? name = null)
        {
            if (value is null) throw new ArgumentNullException(name);
        }
#endif

        /// <summary>
        /// 文字列がnullまたか空文字であるかどうかを検証します。
        /// </summary>
        /// <param name="value">文字列</param>
        /// <param name="name">引数名</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/>が空文字</exception>
#if NET7_0_OR_GREATER

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNullOrEmpty([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(value, name);
        }

#else
        public static void ThrowIfNullOrEmpty([NotNull] string? value,
#if NET6_0_OR_GREATER
                                              [CallerArgumentExpression(nameof(value))]
#endif
                                              string? name = null)
        {
            ThrowIfNull(value, name);
            if (value.Length == 0) throw new ArgumentException("空文字です", name);
        }
#endif

        /// <summary>
        /// ファイル名に使用できない文字が含まれているかどうかを検証します。
        /// </summary>
        /// <param name="fileName">ファイル名またはファイルパス</param>
        /// <param name="name">引数名</param>
        /// <exception cref="ArgumentException"><paramref name="fileName"/>に使用できない文字が含まれている</exception>
        public static void ThrowIfHasInvalidFileNameChar(string fileName,
#if NET6_0_OR_GREATER
                                                         [CallerArgumentExpression(nameof(fileName))]
#endif
                                                         string? name = null)
        {
            int index = fileName.IndexOfAny(InvalidFileNameChars);
            if (index >= 0) throw new ArgumentException($"ファイル名に使用できない文字'{fileName[index]}'が含まれています", name);
        }

        /// <summary>
        /// パスに使用できない文字が含まれているかどうかを検証します。
        /// </summary>
        /// <param name="path">パス</param>
        /// <param name="name">引数名</param>
        /// <exception cref="ArgumentException"><paramref name="path"/>に使用できない文字が含まれている</exception>
        public static void ThrowIfHasInvalidPathChar(string path,
#if NET6_0_OR_GREATER
                                                     [CallerArgumentExpression(nameof(path))]
#endif
                                                     string? name = null)
        {
            int index = path.IndexOfAny(InvalidPathChars);
            if (index >= 0) throw new ArgumentException($"パスに使用できない文字'{path[index]}'が含まれています", name);
        }

        /// <summary>
        /// 値が0未満であるかどうかを検証します。
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="name">引数名</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ThrowIfNegative(int value,
#if NET6_0_OR_GREATER
                                          [CallerArgumentExpression(nameof(value))]
#endif
                                          string? name = null)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(name, $"値が0未満です ('{value}')");
        }

        /// <summary>
        /// 値が0未満であるかどうかを検証します。
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="name">引数名</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ThrowIfNegative(long value,
#if NET6_0_OR_GREATER
                                          [CallerArgumentExpression(nameof(value))]
#endif
                                          string? name = null)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(name, $"値が0未満です ('{value}')");
        }

        /// <summary>
        /// 値が0未満であるかどうかを検証します。
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="name">引数名</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ThrowIfNegative(double value,
#if NET6_0_OR_GREATER
                                           [CallerArgumentExpression(nameof(value))]
#endif
                                           string? name = null)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(name, $"値が0未満です ('{value}')");
        }

        /// <summary>
        /// 値が0未満であるかどうかを検証します。
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="name">引数名</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ThrowIfNegative(decimal value,
#if NET6_0_OR_GREATER
                                           [CallerArgumentExpression(nameof(value))]
#endif
                                           string? name = null)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(name, $"値が0未満です ('{value}')");
        }

        /// <summary>
        /// 列挙型が定義されていないかどうかを検証します。
        /// </summary>
        /// <typeparam name="TEnum">検証する列挙型</typeparam>
        /// <param name="value">検証する値</param>
        /// <param name="name">引数名</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/>が未定義の値</exception>
#if NET6_0_OR_GREATER

        public static void ThrowIfNotDefined<TEnum>(TEnum value, [CallerArgumentExpression(nameof(value))] string? name = null)
            where TEnum : struct, Enum
        {
            if (!Enum.IsDefined(value)) throw new ArgumentOutOfRangeException(name, "定義されていない列挙型の値です");
        }

#else

        public static void ThrowIfNotDefined<TEnum>(TEnum value, string? name = null)
            where TEnum : struct, Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), value)) throw new ArgumentOutOfRangeException(name, "定義されていない列挙型の値です");
        }
#endif

        /// <summary>
        /// オプションの検証結果が無効であるかどうかを検証します。
        /// </summary>
        /// <param name="state">オプションの検証結果</param>
        /// <exception cref="ArgumentAnalysisException"><paramref name="state"/>が無効</exception>
        public static void ThrowIfInvalidState(ValueCheckState state)
        {
            if (!state.IsValid) throw new ArgumentAnalysisException(state.Error);
        }

        #endregion ThrowIf
    }
}
