using System;

namespace CuiLib.Checkers.Implementations
{
    /// <summary>
    /// デリゲートを使用する<see cref="IValueChecker{T}"/>の実装です。
    /// </summary>
    /// <typeparam name="T">検証する値の型</typeparam>
    [Serializable]
    internal sealed class DelegateValueChecker<T> : IValueChecker<T>
    {
        private readonly Func<T, ValueCheckState> func;

        /// <summary>
        /// <see cref="DelegateValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="func">検証関数</param>
        /// <exception cref="ArgumentNullException"><paramref name="func"/>がnull</exception>
        internal DelegateValueChecker(Func<T, ValueCheckState> func)
        {
            ThrowHelpers.ThrowIfNull(func);

            this.func = func;
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(T value)
        {
            try
            {
                return func.Invoke(value);
            }
            catch (Exception e) when (e is not ArgumentAnalysisException)
            {
                ThrowHelpers.ThrowAsOptionParseFailed(e);
                return default;
            }
        }
    }
}
