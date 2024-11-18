using System;
using System.IO;

namespace CuiLib.Converters.Implementations
{
    /// <summary>
    /// <see cref="FileSystemInfo"/>の一覧を生成する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class FileSystemPatternValueConverter : IValueConverter<string, FileSystemInfo[]>
    {
        /// <summary>
        /// <see cref="FileSystemPatternValueConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        internal FileSystemPatternValueConverter()
        {
        }

        /// <inheritdoc/>
        public FileSystemInfo[] Convert(string value)
        {
            ThrowHelpers.ThrowIfNullOrEmpty(value);

            return new DirectoryInfo(".").GetFileSystemInfos(value);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is FileSystemPatternValueConverter;

        /// <inheritdoc/>
        public override int GetHashCode() => GetType().GetHashCode();
    }
}
