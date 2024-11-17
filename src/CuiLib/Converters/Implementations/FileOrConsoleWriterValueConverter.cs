using System;
using System.IO;
using System.Text;

namespace CuiLib.Converters.Implementations
{
    /// <summary>
    /// <see cref="TextWriter"/>を生成する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class FileOrConsoleWriterValueConverter : IValueConverter<string, TextWriter>
    {
        private readonly bool append;
        private readonly Encoding encoding;

        /// <summary>
        /// <see cref="FileOrConsoleWriterValueConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="encoding">エンコーディング</param>
        /// <param name="append">上書きせずに末尾に追加するかどうか</param>
        /// <exception cref="ArgumentNullException"><paramref name="encoding"/>がnull</exception>
        internal FileOrConsoleWriterValueConverter(Encoding encoding, bool append)
        {
            ThrowHelpers.ThrowIfNull(encoding);

            this.encoding = encoding;
            this.append = append;
        }

        /// <inheritdoc/>
        public TextWriter Convert(string value)
        {
            if (value is null or "-") return Console.Out;
            return new StreamWriter(value, append, encoding);
        }
    }
}
