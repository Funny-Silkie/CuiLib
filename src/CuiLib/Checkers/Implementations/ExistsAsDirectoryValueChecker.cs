using System;
using System.IO;

namespace CuiLib.Checkers.Implementations
{
    /// <summary>
    /// ディレクトリが存在するかどうかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class ExistsAsDirectoryValueChecker : IValueChecker<string>
    {
        /// <summary>
        /// <see cref="ExistsAsDirectoryValueChecker"/>の新しいインスタンスを初期化します。
        /// </summary>
        internal ExistsAsDirectoryValueChecker()
        {
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(string value)
        {
            if (Directory.Exists(value)) return ValueCheckState.Success;
            return ValueCheckState.AsError($"ディレクトリ'{value}'が存在しません");
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is ExistsAsDirectoryValueChecker;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return GetType().Name.GetHashCode();
        }
    }
}
