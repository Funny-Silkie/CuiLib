using System;

namespace CuiLib.Options
{
    /// <summary>
    /// 値をとるコマンドのオプションを表します。
    /// </summary>
    /// <typeparam name="T">オプションの値の型</typeparam>
    [Serializable]
    public abstract class ValuedOption<T> : Option<T>
    {
        /// <summary>
        /// 必須かどうかを取得または設定します。
        /// </summary>
        public bool IsRequired { get; set; }

        /// <inheritdoc/>
        public override sealed bool Required { get; set; }

        /// <summary>
        /// <see cref="ValuedOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        protected ValuedOption(char shortName) : base(shortName)
        {
        }

        /// <summary>
        /// <see cref="ValuedOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        protected ValuedOption(string fullName) : base(fullName)
        {
        }

        /// <summary>
        /// <see cref="ValuedOption{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        protected ValuedOption(char shortName, string fullName) : base(shortName, fullName)
        {
        }
    }
}
