using System;
using System.IO;

namespace CuiLib.Checkers.Implementations
{
    /// <summary>
    /// 読み込むファイルを検証します。
    /// </summary>
    [Serializable]
    internal class ValidSourceFileChecker : IValueChecker<FileInfo>
    {
        /// <summary>
        /// <see cref="ValidSourceFileChecker"/>の新しいインスタンスを初期化します。
        /// </summary>
        public ValidSourceFileChecker()
        {
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(FileInfo value)
        {
            ThrowHelpers.ThrowIfNull(value);

            DirectoryInfo? directory = value.Directory;
            if (directory is not null && !directory.Exists) return ValueCheckState.AsError($"ファイル'{value.Name}'のディレクトリが存在しません");
            if (!value.Exists) return ValueCheckState.AsError($"ファイル'{value.Name}'が存在しません");
            return ValueCheckState.Success;
        }
    }
}
