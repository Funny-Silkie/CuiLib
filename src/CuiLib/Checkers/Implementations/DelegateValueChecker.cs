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
        /// <summary>
        /// 条件判定関数を取得します。
        /// </summary>
        public Func<T, ValueCheckState> Condition { get; }

        /// <summary>
        /// <see cref="DelegateValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="func">検証関数</param>
        /// <exception cref="ArgumentNullException"><paramref name="func"/>がnull</exception>
        internal DelegateValueChecker(Func<T, ValueCheckState> func)
        {
            ThrowHelpers.ThrowIfNull(func);

            Condition = func;
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(T value)
        {
            try
            {
                return Condition.Invoke(value);
            }
            catch (Exception e) when (e is not ArgumentAnalysisException)
            {
                ThrowHelpers.ThrowAsOptionParseFailed(e);
                return default;
            }
        }
    }
}
