using System;
using System.IO;

namespace CuiLib.Checkers.Implementations
{
    /// <summary>
    /// 出力ファイルを検証します。
    /// </summary>
    [Serializable]
    internal class ValidDestinationFileChecker : IValueChecker<FileInfo>
    {
        /// <summary>
        /// 存在しないディレクトリを許容するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool AllowMissedDirectory { get; set; }

        /// <summary>
        /// 上書きを許容するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool AllowOverwrite { get; set; }

        /// <summary>
        /// <see cref="ValidDestinationFileChecker"/>の新しいインスタンスを初期化します。
        /// </summary>
        public ValidDestinationFileChecker()
        {
            AllowMissedDirectory = true;
            AllowOverwrite = true;
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(FileInfo value)
        {
            ThrowHelpers.ThrowIfNull(value);

            DirectoryInfo? directory = value.Directory;
            if (!AllowMissedDirectory && directory is not null && !directory.Exists) return ValueCheckState.AsError($"ファイル'{value.Name}'のディレクトリが存在しません");
            if (!AllowOverwrite && value.Exists) return ValueCheckState.AsError($"ファイル'{value.Name}'が既に存在しています");
            return ValueCheckState.Success;
        }
    }
}
