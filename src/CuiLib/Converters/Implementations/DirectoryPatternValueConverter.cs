using System;
using System.IO;

namespace CuiLib.Converters.Implementations
{
    /// <summary>
    /// <see cref="DirectoryInfo"/>の一覧を生成する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class DirectoryPatternValueConverter : IValueConverter<string, DirectoryInfo[]>
    {
        /// <summary>
        /// <see cref="DirectoryPatternValueConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        internal DirectoryPatternValueConverter()
        {
        }

        /// <inheritdoc/>
        public DirectoryInfo[] Convert(string value)
        {
            ThrowHelpers.ThrowIfNullOrEmpty(value);

            return new DirectoryInfo(".").GetDirectories(value);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is DirectoryPatternValueConverter;

        /// <inheritdoc/>
        public override int GetHashCode() => GetType().GetHashCode();
    }
}
