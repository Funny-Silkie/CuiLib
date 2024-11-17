using System;
using System.IO;
using System.Text;

namespace CuiLib.Converters.Implementations
{
    /// <summary>
    /// <see cref="StreamReader"/>を生成する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class StreamReaderValueConverter : IValueConverter<string, StreamReader>
    {
        /// <summary>
        /// 末尾にテキストを追加するかどうかを表す値を取得します。
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// <see cref="StreamReaderValueConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="encoding">エンコーディング</param>
        /// <exception cref="ArgumentNullException"><paramref name="encoding"/>がnull</exception>
        internal StreamReaderValueConverter(Encoding encoding)
        {
            ThrowHelpers.ThrowIfNull(encoding);

            Encoding = encoding;
        }

        /// <inheritdoc/>
        public StreamReader Convert(string value) => new StreamReader(value, Encoding);
    }
}
