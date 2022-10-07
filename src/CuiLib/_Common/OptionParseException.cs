using System;
using System.Runtime.Serialization;

namespace CuiLib
{
    /// <summary>
    /// コマンド引数解析過程の例外を表します。
    /// </summary>
    [Serializable]
    public class ArgumentAnalysisException : Exception
    {
        /// <summary>
        /// <see cref="ArgumentAnalysisException"/>の新しいインスタンスを初期化します。
        /// </summary>
        public ArgumentAnalysisException()
        {
        }

        /// <summary>
        /// <see cref="ArgumentAnalysisException"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        public ArgumentAnalysisException(string? message) : base(message)
        {
        }

        /// <summary>
        /// <see cref="ArgumentAnalysisException"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="inner">内部例外</param>
        public ArgumentAnalysisException(string? message, Exception? inner) : base(message, inner)
        {
        }

        /// <summary>
        /// <see cref="ArgumentAnalysisException"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="info">シリアライズ情報を格納するオブジェクト</param>
        /// <param name="context">シリアル化ストリームの情報</param>
        /// <exception cref="ArgumentNullException"><paramref name="info"/>がnull</exception>
        /// <exception cref="SerializationException">デシリアライズ中に例外が発生した</exception>
        protected ArgumentAnalysisException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
