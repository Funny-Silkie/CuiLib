using System;
using System.IO;
using System.Text;

namespace CuiLib.Converters.Implementations
{
    /// <summary>
    /// <see cref="StreamWriter"/>を生成する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class StreamWriterValueConverter : IValueConverter<string, StreamWriter>
    {
        /// <summary>
        /// 末尾にテキストを追加するかどうかを表す値を取得します。
        /// </summary>
        public bool Append { get; }

        /// <summary>
        /// エンコーディング情報を取得します。
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// <see cref="StreamWriterValueConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="encoding">エンコーディング</param>
        /// <param name="append">上書きせずに末尾に追加するかどうか</param>
        /// <exception cref="ArgumentNullException"><paramref name="encoding"/>がnull</exception>
        internal StreamWriterValueConverter(Encoding encoding, bool append)
        {
            ThrowHelpers.ThrowIfNull(encoding);

            Encoding = encoding;
            Append = append;
        }

        /// <inheritdoc/>
        public StreamWriter Convert(string value) => new StreamWriter(value, Append, Encoding);
    }
}
