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
        private readonly Encoding encoding;

        /// <summary>
        /// <see cref="StreamReaderValueConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="encoding">エンコーディング</param>
        /// <exception cref="ArgumentNullException"><paramref name="encoding"/>がnull</exception>
        internal StreamReaderValueConverter(Encoding encoding)
        {
            ThrowHelpers.ThrowIfNull(encoding);

            this.encoding = encoding;
        }

        /// <inheritdoc/>
        public StreamReader Convert(string value)
        {
            return new StreamReader(value, encoding);
        }
    }
}
