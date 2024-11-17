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
        /// <summary>
        /// 末尾にテキストを追加するかどうかを表す値を取得します。
        /// </summary>
        public bool Append { get; }

        /// <summary>
        /// エンコーディング情報を取得します。
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// <see cref="FileOrConsoleWriterValueConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="encoding">エンコーディング</param>
        /// <param name="append">上書きせずに末尾に追加するかどうか</param>
        /// <exception cref="ArgumentNullException"><paramref name="encoding"/>がnull</exception>
        internal FileOrConsoleWriterValueConverter(Encoding encoding, bool append)
        {
            ThrowHelpers.ThrowIfNull(encoding);

            Encoding = encoding;
            Append = append;
        }

        /// <inheritdoc/>
        public TextWriter Convert(string value)
        {
            if (value is null or "-") return Console.Out;
            return new StreamWriter(value, Append, Encoding);
        }
    }
}
