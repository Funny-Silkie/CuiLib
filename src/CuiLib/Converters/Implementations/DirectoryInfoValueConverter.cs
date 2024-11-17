using System;
using System.IO;

namespace CuiLib.Converters.Implementations
{
    /// <summary>
    /// <see cref="DirectoryInfo"/>を生成する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class DirectoryInfoValueConverter : IValueConverter<string, DirectoryInfo>
    {
        /// <summary>
        /// <see cref="DirectoryInfoValueConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        internal DirectoryInfoValueConverter()
        {
        }

        /// <inheritdoc/>
        public DirectoryInfo Convert(string value)
        {
            return new DirectoryInfo(value);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is DirectoryInfoValueConverter;

        /// <inheritdoc/>
        public override int GetHashCode() => GetType().GetHashCode();
    }
}
