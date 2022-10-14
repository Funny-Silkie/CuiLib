using System;
using System.IO;

namespace CuiLib.Options
{
    /// <summary>
    /// 読み込むファイルを検証します。
    /// </summary>
    [Serializable]
    public class SourceFileChecker : ValueChecker<FileInfo>
    {
        /// <summary>
        /// <see cref="SourceFileChecker"/>の新しいインスタンスを初期化します。
        /// </summary>
        public SourceFileChecker()
        {
        }

        /// <inheritdoc/>
        public override ValueCheckState CheckValue(FileInfo? value)
        {
            ArgumentNullException.ThrowIfNull(value);

            DirectoryInfo? directory = value.Directory;
            if (directory is not null && !directory.Exists) return ValueCheckState.AsError($"ファイル'{value.Name}'のディレクトリが存在しません");
            if (!value.Exists) return ValueCheckState.AsError($"ファイル'{value.Name}'が存在しません");
            return ValueCheckState.Success;
        }
    }

    /// <summary>
    /// 出力ファイルを検証します。
    /// </summary>
    [Serializable]
    public class DestinationFileChecker : ValueChecker<FileInfo>
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
        /// <see cref="DestinationFileChecker"/>の新しいインスタンスを初期化します。
        /// </summary>
        public DestinationFileChecker()
        {
            AllowMissedDirectory = true;
            AllowOverwrite = true;
        }

        /// <inheritdoc/>
        public override ValueCheckState CheckValue(FileInfo? value)
        {
            ArgumentNullException.ThrowIfNull(value);

            DirectoryInfo? directory = value.Directory;
            if (!AllowMissedDirectory && directory is not null && !directory.Exists) return ValueCheckState.AsError($"ファイル'{value.Name}'のディレクトリが存在しません");
            if (!AllowOverwrite && value.Exists) return ValueCheckState.AsError($"ファイル'{value.Name}'が既に存在しています");
            return ValueCheckState.Success;
        }
    }

    /// <summary>
    /// 読み込むディレクトリを検証します。
    /// </summary>
    [Serializable]
    public class SourceDirectoryChecker : ValueChecker<DirectoryInfo>
    {
        /// <summary>
        /// <see cref="SourceDirectoryChecker"/>の新しいインスタンスを初期化します。
        /// </summary>
        public SourceDirectoryChecker()
        {
        }

        /// <inheritdoc/>
        public override ValueCheckState CheckValue(DirectoryInfo? value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (!value.Exists) return ValueCheckState.AsError($"ディレクトリ'{value.Name}'が存在しません");
            return ValueCheckState.Success;
        }
    }
}
