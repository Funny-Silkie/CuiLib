using System;
using System.IO;

namespace Test.Helpers
{
    /// <summary>
    /// ファイル操作のヘルパーを表します。
    /// </summary>
    internal static class FileUtilHelpers
    {
        /// <summary>
        /// 存在しないファイルを表す<see cref="FileInfo"/>のインスタンスを取得します。
        /// </summary>
        /// <returns>存在しないファイルを表す<see cref="FileInfo"/>のインスタンス</returns>
        public static FileInfo GetNoExistingFile()
        {
            while (true)
            {
                Guid guid = Guid.NewGuid();
                var result = new FileInfo(guid.ToString() + ".tmp");
                if (!result.Exists) return result;
            }
        }

        /// <summary>
        /// 存在しないディレクトリを表す<see cref="DirectoryInfo"/>のインスタンスを取得します。
        /// </summary>
        /// <returns>存在しないディレクトリを表す<see cref="DirectoryInfo"/>のインスタンス</returns>
        public static DirectoryInfo GetNoExistingDirectory()
        {
            while (true)
            {
                Guid guid = Guid.NewGuid();
                var result = new DirectoryInfo(guid.ToString());
                if (!result.Exists) return result;
            }
        }
    }
}
