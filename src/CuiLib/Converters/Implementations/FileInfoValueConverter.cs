using System;
using System.IO;

namespace CuiLib.Converters.Implementations
{
    /// <summary>
    /// <see cref="FileInfo"/>を生成する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class FileInfoValueConverter : IValueConverter<string, FileInfo>
    {
        /// <summary>
        /// <see cref="FileInfoValueConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        internal FileInfoValueConverter()
        {
        }

        /// <inheritdoc/>
        public FileInfo Convert(string value) => new FileInfo(value);

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is FileInfoValueConverter;

        /// <inheritdoc/>
        public override int GetHashCode() => GetType().GetHashCode();
    }
}
