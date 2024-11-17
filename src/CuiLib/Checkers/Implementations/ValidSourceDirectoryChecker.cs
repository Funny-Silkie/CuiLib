using System;
using System.IO;

namespace CuiLib.Checkers.Implementations
{
    /// <summary>
    /// 読み込むディレクトリを検証します。
    /// </summary>
    [Serializable]
    internal class ValidSourceDirectoryChecker : IValueChecker<DirectoryInfo>
    {
        /// <summary>
        /// <see cref="ValidSourceDirectoryChecker"/>の新しいインスタンスを初期化します。
        /// </summary>
        public ValidSourceDirectoryChecker()
        {
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(DirectoryInfo value)
        {
            ThrowHelpers.ThrowIfNull(value);

            if (!value.Exists) return ValueCheckState.AsError($"ディレクトリ'{value.Name}'が存在しません");
            return ValueCheckState.Success;
        }
    }
}
