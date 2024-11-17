using System;
using System.IO;

namespace CuiLib.Checkers.Implementations
{
    /// <summary>
    /// ファイルパスが存在するかどうかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class ExistsAsFileValueChecker : IValueChecker<string>
    {
        /// <summary>
        /// <see cref="ExistsAsFileValueChecker"/>の新しいインスタンスを初期化します。
        /// </summary>
        internal ExistsAsFileValueChecker()
        {
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(string value)
        {
            if (File.Exists(value)) return ValueCheckState.Success;
            return ValueCheckState.AsError($"ファイル'{value}'が存在しません");
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is ExistsAsFileValueChecker;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return GetType().Name.GetHashCode();
        }
    }
}
