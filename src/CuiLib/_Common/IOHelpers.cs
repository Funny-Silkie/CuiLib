using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuiLib
{
    /// <summary>
    /// I/O処理のヘルパークラスです。
    /// </summary>
    public static class IOHelpers
    {
        /// <summary>
        /// UTF-8Nのエンコーディングを取得します。
        /// </summary>
        public static Encoding UTF8N { get; } = new UTF8Encoding(false);

        /// <summary>
        /// ディレクトリが存在しない場合に生成します。
        /// </summary>
        /// <param name="directory">ディレクトリのパス</param>
        /// <returns><paramref name="directory"/>に対応する<see cref="DirectoryInfo"/>のインスタンス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="directory"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="directory"/><paramref name="directory"/>が無効</exception>
        /// <exception cref="System.Security.SecurityException">アクセス権限がない</exception>
        /// <exception cref="PathTooLongException"><paramref name="directory"/>が長すぎる</exception>
        /// <exception cref="IOException">ディレクトリの生成に失敗した</exception>
        public static DirectoryInfo EnsureDirectory(string directory)
        {
            var result = new DirectoryInfo(directory);
            if (!result.Exists) result.Create();
            return result;
        }

        /// <summary>
        /// 全ての文字列を読み取ります。
        /// </summary>
        /// <param name="file">使用する<see cref="FileInfo"/>のインスタンス</param>
        /// <returns>ファイルの全ての文字列</returns>
        /// <exception cref="ArgumentNullException"><paramref name="file"/>がnull</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="file"/>のディレクトリが存在しない</exception>
        /// <exception cref="FileNotFoundException"><paramref name="file"/>のファイルが存在しない</exception>
        /// <exception cref="System.Security.SecurityException">ファイルへのアクセス権限がない</exception>
        /// <exception cref="UnauthorizedAccessException">ファイルへのアクセスが拒否された</exception>
        /// <exception cref="OutOfMemoryException">メモリが足りない</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        public static string OpenReadAllText(this FileInfo file)
        {
            ArgumentNullException.ThrowIfNull(file);

            using StreamReader reader = file.OpenText();
            return reader.ReadToEnd();
        }

        /// <summary>
        /// 全ての文字列を読み取ります。
        /// </summary>
        /// <param name="file">使用する<see cref="FileInfo"/>のインスタンス</param>
        /// <returns>ファイルの全ての文字列</returns>
        /// <exception cref="ArgumentNullException"><paramref name="file"/>がnull</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="file"/>のディレクトリが存在しない</exception>
        /// <exception cref="FileNotFoundException"><paramref name="file"/>のファイルが存在しない</exception>
        /// <exception cref="System.Security.SecurityException">ファイルへのアクセス権限がない</exception>
        /// <exception cref="UnauthorizedAccessException">ファイルへのアクセスが拒否された</exception>
        /// <exception cref="InvalidOperationException">readerが読み取り処理に使用中</exception>
        /// <exception cref="ArgumentOutOfRangeException">文字列長が<see cref="int.MaxValue"/>を超える</exception>
        public static async Task<string> OpenReadAllTextAsync(this FileInfo file)
        {
            ArgumentNullException.ThrowIfNull(file);

            using StreamReader reader = file.OpenText();
            return await reader.ReadToEndAsync();
        }

        /// <summary>
        /// イテレータとして1行ずつ文字列を読み出します。
        /// </summary>
        /// <param name="file">使用する<see cref="FileInfo"/>のインスタンス</param>
        /// <returns>各行を返すイテレータ</returns>
        /// <exception cref="ArgumentNullException"><paramref name="file"/>がnull</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="file"/>のディレクトリが存在しない</exception>
        /// <exception cref="FileNotFoundException"><paramref name="file"/>のファイルが存在しない</exception>
        /// <exception cref="System.Security.SecurityException">ファイルへのアクセス権限がない</exception>
        /// <exception cref="UnauthorizedAccessException">ファイルへのアクセスが拒否された</exception>
        /// <exception cref="OutOfMemoryException">メモリが足りない</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        public static IEnumerable<string> OpenIterateLines(this FileInfo file)
        {
            ArgumentNullException.ThrowIfNull(file);

            return Inner(file);

            static IEnumerable<string> Inner(FileInfo file)
            {
                using StreamReader reader = file.OpenText();
                while (!reader.EndOfStream) yield return reader.ReadLine()!;
            }
        }

        /// <summary>
        /// イテレータとして1行ずつ文字列を読み出します。
        /// </summary>
        /// <param name="file">使用する<see cref="FileInfo"/>のインスタンス</param>
        /// <returns>各行を返すイテレータ</returns>
        /// <exception cref="ArgumentNullException"><paramref name="file"/>がnull</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="file"/>のディレクトリが存在しない</exception>
        /// <exception cref="FileNotFoundException"><paramref name="file"/>のファイルが存在しない</exception>
        /// <exception cref="System.Security.SecurityException">ファイルへのアクセス権限がない</exception>
        /// <exception cref="UnauthorizedAccessException">ファイルへのアクセスが拒否された</exception>
        /// <exception cref="InvalidOperationException">readerが読み取り処理に使用中</exception>
        /// <exception cref="ArgumentOutOfRangeException">文字列長が<see cref="int.MaxValue"/>を超える</exception>
        public static IAsyncEnumerable<string> OpenIterateLinesAsync(this FileInfo file)
        {
            ArgumentNullException.ThrowIfNull(file);

            return Inner(file);

            static async IAsyncEnumerable<string> Inner(FileInfo file)
            {
                using StreamReader reader = file.OpenText();
                while (!reader.EndOfStream) yield return (await reader.ReadLineAsync())!;
            }
        }

        /// <summary>
        /// 1行ずつ文字列を読み出します。
        /// </summary>
        /// <param name="file">使用する<see cref="FileInfo"/>のインスタンス</param>
        /// <returns>ファイルの各行</returns>
        /// <exception cref="ArgumentNullException"><paramref name="file"/>がnull</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="file"/>のディレクトリが存在しない</exception>
        /// <exception cref="FileNotFoundException"><paramref name="file"/>のファイルが存在しない</exception>
        /// <exception cref="System.Security.SecurityException">ファイルへのアクセス権限がない</exception>
        /// <exception cref="UnauthorizedAccessException">ファイルへのアクセスが拒否された</exception>
        /// <exception cref="OutOfMemoryException">メモリが足りない</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        public static string[] OpenReadLines(this FileInfo file)
        {
            return file.OpenIterateLines().ToArray();
        }

        /// <summary>
        /// 1行ずつ文字列を読み出します。
        /// </summary>
        /// <param name="file">使用する<see cref="FileInfo"/>のインスタンス</param>
        /// <returns>ファイルの各行</returns>
        /// <exception cref="ArgumentNullException"><paramref name="file"/>がnull</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="file"/>のディレクトリが存在しない</exception>
        /// <exception cref="FileNotFoundException"><paramref name="file"/>のファイルが存在しない</exception>
        /// <exception cref="System.Security.SecurityException">ファイルへのアクセス権限がない</exception>
        /// <exception cref="UnauthorizedAccessException">ファイルへのアクセスが拒否された</exception>
        /// <exception cref="InvalidOperationException">readerが読み取り処理に使用中</exception>
        /// <exception cref="ArgumentOutOfRangeException">文字列長が<see cref="int.MaxValue"/>を超える</exception>
        public static async Task<string[]> OpenReadLinesAsync(this FileInfo file)
        {
            return await file.OpenIterateLinesAsync().ToArrayAsync();
        }

        /// <summary>
        /// イテレータとして1行ずつ文字列を読み出します。
        /// </summary>
        /// <param name="reader">使用する<see cref="StreamReader"/>のインスタンス</param>
        /// <returns>各行を返すイテレータ</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/>がnull</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="reader"/>が既に破棄されている</exception>
        /// <exception cref="OutOfMemoryException">メモリが足りない</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        public static IEnumerable<string> IterateLines(this StreamReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader);

            return Inner(reader);

            static IEnumerable<string> Inner(StreamReader reader)
            {
                while (!reader.EndOfStream) yield return reader.ReadLine()!;
            }
        }

        /// <summary>
        /// イテレータとして1行ずつ文字列を読み出します。
        /// </summary>
        /// <param name="reader">使用する<see cref="StreamReader"/>のインスタンス</param>
        /// <returns>各行を返すイテレータ</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/>がnull</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="reader"/>が既に破棄されている</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        public static IAsyncEnumerable<string> IterateLinesAsync(this StreamReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader);

            return Inner(reader);

            async static IAsyncEnumerable<string> Inner(StreamReader reader)
            {
                while (!reader.EndOfStream) yield return (await reader.ReadLineAsync())!;
            }
        }
    }
}
