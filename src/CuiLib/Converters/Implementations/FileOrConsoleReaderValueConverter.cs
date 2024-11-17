﻿using System;
using System.IO;
using System.Text;

namespace CuiLib.Converters.Implementations
{
    /// <summary>
    /// <see cref="TextReader"/>を生成する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class FileOrConsoleReaderValueConverter : IValueConverter<string, TextReader>
    {
        private readonly Encoding encoding;

        /// <summary>
        /// <see cref="FileOrConsoleReaderValueConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="encoding">エンコーディング</param>
        /// <exception cref="ArgumentNullException"><paramref name="encoding"/>がnull</exception>
        internal FileOrConsoleReaderValueConverter(Encoding encoding)
        {
            ThrowHelpers.ThrowIfNull(encoding);

            this.encoding = encoding;
        }

        /// <inheritdoc/>
        public TextReader Convert(string value)
        {
            if (value is null or "-") return Console.In;
            return new StreamReader(value, encoding);
        }
    }
}
