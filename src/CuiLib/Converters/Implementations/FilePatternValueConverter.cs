using System;
using System.IO;

namespace CuiLib.Converters.Implementations
{
    /// <summary>
    /// <see cref="FileInfo"/>の一覧を生成する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class FilePatternValueConverter : IValueConverter<string, FileInfo[]>
    {
        /// <summary>
        /// <see cref="FilePatternValueConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        internal FilePatternValueConverter()
        {
        }

        /// <inheritdoc/>
        public FileInfo[] Convert(string value)
        {
            ThrowHelpers.ThrowIfNullOrEmpty(value);

            return new DirectoryInfo(".").GetFiles(value);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is FilePatternValueConverter;

        /// <inheritdoc/>
        public override int GetHashCode() => GetType().GetHashCode();
    }
}
