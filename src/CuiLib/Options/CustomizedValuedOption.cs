using System;

namespace CuiLib.Options
{
    /// <summary>
    /// 変換処理を自前で実装するオプションのクラスです。
    /// </summary>
    /// <typeparam name="T">値の型</typeparam>
    [Serializable]
    public abstract class CustomizedValuedOption<T> : ValuedOption<T>
    {
        /// <inheritdoc/>
        public override T? Value
        {
            get
            {
                if (ValueAvailable)
                {
                    T? result = ConvertFromRaw(RawValue);

                    ValueCheckState state = Checker.CheckValue(result);
                    ThrowHelper.ThrowIfInvalidState(state);

                    return result;
                }
                return DefaultValue;
            }
        }

        /// <summary>
        /// <see cref="CustomizedValuedOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        public CustomizedValuedOption(char shortName) : base(shortName)
        {
        }

        /// <summary>
        /// <see cref="CustomizedValuedOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        public CustomizedValuedOption(string fullName) : base(fullName)
        {
        }

        /// <summary>
        /// <see cref="CustomizedValuedOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        public CustomizedValuedOption(char shortName, string fullName) : base(shortName, fullName)
        {
        }

        /// <summary>
        /// 文字列から値を生成します。
        /// </summary>
        /// <param name="value">使用する文字列</param>
        /// <returns><paramref name="value"/>から生成された<typeparamref name="T"/>の値。失敗したら既定値</returns>
        protected abstract T? ConvertFromRaw(string? value);
    }
}
