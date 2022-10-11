using System;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CuiLib.Options;

namespace CuiLib
{
    /// <summary>
    /// 例外処理に関連する実装を行います。
    /// </summary>
    public static class ThrowHelper
    {
        private static char[]? _invalidPathChars;
        private static char[]? _invalidFileNameChars;

        /// <summary>
        /// ファイル名に使用できない文字を取得します。
        /// </summary>
        /// <returns>ファイル名に使用できない文字</returns>
        private static char[] GetInvalidFileNameChars() => _invalidFileNameChars ??= Path.GetInvalidFileNameChars();

        /// <summary>
        /// パスに使用できない文字を取得します。
        /// </summary>
        /// <returns>パスに使用できない文字</returns>
        private static char[] GetInvalidPathChars() => _invalidPathChars ??= Path.GetInvalidPathChars();

        #region Catch

        /// <summary>
        /// 例外をキャッチします。
        /// </summary>
        /// <param name="action">実行処理</param>
        /// <param name="error">発生した例外。例外が発生しなかった場合はnull</param>
        /// <returns>例外が発生せず処理が終了したらtrue，それ以外でfalse</returns>
        public static bool Catch(Action action, [NotNullWhen(false)] out Exception? error)
        {
            ArgumentNullException.ThrowIfNull(action);

            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                error = e;
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>
        /// 例外をキャッチします。
        /// </summary>
        /// <typeparam name="TArg">引数の型</typeparam>
        /// <param name="action">実行処理</param>
        /// <param name="arg"><paramref name="action"/>の引数</param>
        /// <param name="error">発生した例外。例外が発生しなかった場合はnull</param>
        /// <returns>例外が発生せず処理が終了したらtrue，それ以外でfalse</returns>
        public static bool Catch<TArg>(Action<TArg?> action, TArg? arg, [NotNullWhen(false)] out Exception? error)
        {
            ArgumentNullException.ThrowIfNull(action);

            try
            {
                action.Invoke(arg);
            }
            catch (Exception e)
            {
                error = e;
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>
        /// 例外をキャッチします。
        /// </summary>
        /// <typeparam name="TArg1">引数の型</typeparam>
        /// <typeparam name="TArg2">引数の型</typeparam>
        /// <param name="action">実行処理</param>
        /// <param name="arg1"><paramref name="action"/>の引数1</param>
        /// <param name="arg2"><paramref name="action"/>の引数2</param>
        /// <param name="error">発生した例外。例外が発生しなかった場合はnull</param>
        /// <returns>例外が発生せず処理が終了したらtrue，それ以外でfalse</returns>
        public static bool Catch<TArg1, TArg2>(Action<TArg1?, TArg2?> action, TArg1? arg1, TArg2? arg2, [NotNullWhen(false)] out Exception? error)
        {
            ArgumentNullException.ThrowIfNull(action);

            try
            {
                action.Invoke(arg1, arg2);
            }
            catch (Exception e)
            {
                error = e;
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>
        /// 例外をキャッチします。
        /// </summary>
        /// <typeparam name="TArg1">引数の型</typeparam>
        /// <typeparam name="TArg2">引数の型</typeparam>
        /// <typeparam name="TArg3">引数の型</typeparam>
        /// <param name="action">実行処理</param>
        /// <param name="arg1"><paramref name="action"/>の引数1</param>
        /// <param name="arg2"><paramref name="action"/>の引数2</param>
        /// <param name="arg3"><paramref name="action"/>の引数3</param>
        /// <param name="error">発生した例外。例外が発生しなかった場合はnull</param>
        /// <returns>例外が発生せず処理が終了したらtrue，それ以外でfalse</returns>
        public static bool Catch<TArg1, TArg2, TArg3>(Action<TArg1?, TArg2?, TArg3?> action, TArg1? arg1, TArg2? arg2, TArg3? arg3, [NotNullWhen(false)] out Exception? error)
        {
            ArgumentNullException.ThrowIfNull(action);

            try
            {
                action.Invoke(arg1, arg2, arg3);
            }
            catch (Exception e)
            {
                error = e;
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>
        /// 例外をキャッチします。
        /// </summary>
        /// <typeparam name="TResult">生成されるオブジェクトの型</typeparam>
        /// <param name="factory"><typeparamref name="TResult"/>の生成処理</param>
        /// <param name="result"><paramref name="factory"/>の生成するオブジェクト。例外発生時は既定値</param>
        /// <param name="error">発生した例外。例外が発生しなかった場合はnull</param>
        /// <returns>例外が発生せず処理が終了したらtrue，それ以外でfalse</returns>
        public static bool Catch<TResult>(Func<TResult?> factory, [MaybeNullWhen(true)] out TResult? result, [NotNullWhen(false)] out Exception? error)
        {
            ArgumentNullException.ThrowIfNull(factory);

            try
            {
                result = factory.Invoke();
            }
            catch (Exception e)
            {
                result = default;
                error = e;
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>
        /// 例外をキャッチします。
        /// </summary>
        /// <typeparam name="TArg">引数の型</typeparam>
        /// <typeparam name="TResult">生成されるオブジェクトの型</typeparam>
        /// <param name="factory"><typeparamref name="TResult"/>の生成処理</param>
        /// <param name="arg"><paramref name="factory"/>の引数</param>
        /// <param name="result"><paramref name="factory"/>の生成するオブジェクト。例外発生時は既定値</param>
        /// <param name="error">発生した例外。例外が発生しなかった場合はnull</param>
        /// <returns>例外が発生せず処理が終了したらtrue，それ以外でfalse</returns>
        public static bool Catch<TArg, TResult>(Func<TArg?, TResult?> factory, TArg? arg, [MaybeNullWhen(true)] out TResult? result, [NotNullWhen(false)] out Exception? error)
        {
            ArgumentNullException.ThrowIfNull(factory);

            try
            {
                result = factory.Invoke(arg);
            }
            catch (Exception e)
            {
                result = default;
                error = e;
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>
        /// 例外をキャッチします。
        /// </summary>
        /// <typeparam name="TArg1">引数の型</typeparam>
        /// <typeparam name="TArg2">引数の型</typeparam>
        /// <typeparam name="TResult">生成されるオブジェクトの型</typeparam>
        /// <param name="factory"><typeparamref name="TResult"/>の生成処理</param>
        /// <param name="arg1"><paramref name="factory"/>の引数1</param>
        /// <param name="arg2"><paramref name="factory"/>の引数2</param>
        /// <param name="result"><paramref name="factory"/>の生成するオブジェクト。例外発生時は既定値</param>
        /// <param name="error">発生した例外。例外が発生しなかった場合はnull</param>
        /// <returns>例外が発生せず処理が終了したらtrue，それ以外でfalse</returns>
        public static bool Catch<TArg1, TArg2, TResult>(Func<TArg1?, TArg2?, TResult?> factory, TArg1? arg1, TArg2? arg2, [MaybeNullWhen(true)] out TResult? result, [NotNullWhen(false)] out Exception? error)
        {
            ArgumentNullException.ThrowIfNull(factory);

            try
            {
                result = factory.Invoke(arg1, arg2);
            }
            catch (Exception e)
            {
                result = default;
                error = e;
                return false;
            }
            error = null;
            return true;
        }

        /// <summary>
        /// 例外をキャッチします。
        /// </summary>
        /// <typeparam name="TArg1">引数の型</typeparam>
        /// <typeparam name="TArg2">引数の型</typeparam>
        /// <typeparam name="TArg3">引数の型</typeparam>
        /// <typeparam name="TResult">生成されるオブジェクトの型</typeparam>
        /// <param name="factory"><typeparamref name="TResult"/>の生成処理</param>
        /// <param name="arg1"><paramref name="factory"/>の引数1</param>
        /// <param name="arg2"><paramref name="factory"/>の引数2</param>
        /// <param name="arg3"><paramref name="factory"/>の引数3</param>
        /// <param name="result"><paramref name="factory"/>の生成するオブジェクト。例外発生時は既定値</param>
        /// <param name="error">発生した例外。例外が発生しなかった場合はnull</param>
        /// <returns>例外が発生せず処理が終了したらtrue，それ以外でfalse</returns>
        public static bool Catch<TArg1, TArg2, TArg3, TResult>(Func<TArg1?, TArg2?, TArg3?, TResult?> factory, TArg1? arg1, TArg2? arg2, TArg3? arg3, [MaybeNullWhen(true)] out TResult? result, [NotNullWhen(false)] out Exception? error)
        {
            ArgumentNullException.ThrowIfNull(factory);

            try
            {
                result = factory.Invoke(arg1, arg2, arg3);
            }
            catch (Exception e)
            {
                result = default;
                error = e;
                return false;
            }
            error = null;
            return true;
        }

        #endregion Catch

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
        public static void ThrowAsEmptyOption(Option? option)
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
        /// 文字列がnullまたか空文字であるかどうかを検証します。
        /// </summary>
        /// <param name="value">文字列</param>
        /// <param name="name">引数名</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/>が空文字</exception>
        public static void ThrowIfNullOrEmpty(string? value, [CallerArgumentExpression("value")] string? name = null)
        {
            ArgumentNullException.ThrowIfNull(value, name);
            if (value.Length == 0) throw new ArgumentException("空文字です", name);
        }

        /// <summary>
        /// ファイル名に使用できない文字が含まれているかどうかを検証します。
        /// </summary>
        /// <param name="fileName">ファイル名またはファイルパス</param>
        /// <param name="name">引数名</param>
        /// <exception cref="ArgumentException"><paramref name="fileName"/>に使用できない文字が含まれている</exception>
        public static void ThrowIfHasInvalidFileNameChar(string fileName, [CallerArgumentExpression("fileName")] string? name = null)
        {
            int index = fileName.IndexOfAny(GetInvalidFileNameChars());
            if (index >= 0) throw new ArgumentException($"ファイル名に使用できない文字'{fileName[index]}'が含まれています", name);
        }

        /// <summary>
        /// パスに使用できない文字が含まれているかどうかを検証します。
        /// </summary>
        /// <param name="path">パス</param>
        /// <param name="name">引数名</param>
        /// <exception cref="ArgumentException"><paramref name="path"/>に使用できない文字が含まれている</exception>
        public static void ThrowIfHasInvalidPathChar(string path, [CallerArgumentExpression("path")] string? name = null)
        {
            int index = path.IndexOfAny(GetInvalidPathChars());
            if (index >= 0) throw new ArgumentException($"パスに使用できない文字'{path[index]}'が含まれています", name);
        }

        /// <summary>
        /// 値が0未満であるかどうかを検証します。
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="name">引数名</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ThrowIfNegative(int value, [CallerArgumentExpression("value")] string? name = null)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(name, $"値が0未満です ('{value}')");
        }

        /// <summary>
        /// 値が0未満であるかどうかを検証します。
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="name">引数名</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ThrowIfNegative(long value, [CallerArgumentExpression("value")] string? name = null)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(name, $"値が0未満です ('{value}')");
        }

        /// <summary>
        /// 値が0未満であるかどうかを検証します。
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="name">引数名</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ThrowIfNegative(double value, [CallerArgumentExpression("value")] string? name = null)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(name, $"値が0未満です ('{value}')");
        }

        /// <summary>
        /// 値が0未満であるかどうかを検証します。
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="name">引数名</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ThrowIfNegative(decimal value, [CallerArgumentExpression("value")] string? name = null)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(name, $"値が0未満です ('{value}')");
        }

        /// <summary>
        /// オプションの変換結果が無効であるかどうかを検証します。
        /// </summary>
        /// <param name="state">オプションの変換結果</param>
        /// <exception cref="ArgumentAnalysisException"><paramref name="state"/>が無効</exception>
        public static void ThrowIfInvalidState(ValueCheckState state)
        {
            if (!state.IsValid) throw new ArgumentAnalysisException(state.Error);
        }

        #endregion ThrowIf
    }
}
