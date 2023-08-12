using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CuiLib.Log
{
    /// <summary>
    /// ロガーのクラスです。
    /// </summary>
    public class Logger : TextWriter
    {
        private readonly List<WriterEntry> writers;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public override Encoding Encoding => throw new NotSupportedException();

        /// <summary>
        /// <see cref="Console.Out"/>へのログ出力をするかどうかの値を取得または設定します。
        /// </summary>
        public bool ConsoleStdoutLogEnabled
        {
            get => _consoleStdoutLogEnabled;
            set
            {
                if (_consoleStdoutLogEnabled == value) return;
                _consoleStdoutLogEnabled = value;
                if (value) writers.Add(new WriterEntry(Console.Out));
                else RemoveLog(Console.Out);
            }
        }

        private bool _consoleStdoutLogEnabled;

        /// <summary>
        /// <see cref="Console.Error"/>へのログ出力をするかどうかの値を取得または設定します。
        /// </summary>
        public bool ConsoleErrorEnabled
        {
            get => _consoleErrorLogEnabled;
            set
            {
                if (_consoleErrorLogEnabled == value) return;
                _consoleErrorLogEnabled = value;
                if (value) writers.Add(new WriterEntry(Console.Error));
                else RemoveLog(Console.Error);
            }
        }

        private bool _consoleErrorLogEnabled;

        /// <summary>
        /// <see cref="Logger"/>の新しいインスタンスを初期化します。
        /// </summary>
        public Logger()
        {
            writers = new List<WriterEntry>();
        }

        /// <summary>
        /// <see cref="Logger"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="logFile">ログファイルのパス</param>
        /// <exception cref="ArgumentNullException"><paramref name="logFile"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="logFile"/>が無効</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="logFile"/>のディレクトリが存在しない</exception>
        /// <exception cref="IOException"><paramref name="logFile"/>の書式が無効</exception>
        /// <exception cref="PathTooLongException"><paramref name="logFile"/>が長すぎる</exception>
        /// <exception cref="UnauthorizedAccessException">アクセスが拒否された</exception>
        /// <exception cref="System.Security.SecurityException">アクセス権限がない</exception>
        public Logger(string logFile)
        {
            writers = new List<WriterEntry>();
            AddLogFile(logFile);
        }

        #region Collection Operation

        /// <summary>
        /// 指定したパスのログのインデックスを取得します。
        /// </summary>
        /// <param name="path">ログファイルのパス</param>
        /// <returns><paramref name="path"/>に対応するインデックス</returns>
        private int GetLogIndex(string path)
        {
            for (int i = 0; i < writers.Count; i++)
                if (writers[i].Path == path)
                    return i;
            return -1;
        }

        /// <summary>
        /// 指定したライターのログのインデックスを取得します。
        /// </summary>
        /// <param name="writer">ログファイルのライター</param>
        /// <returns><paramref name="writer"/>に対応するインデックス</returns>
        private int GetLogIndex(TextWriter writer)
        {
            for (int i = 0; i < writers.Count; i++)
                if (writers[i].Writer == writer)
                    return i;
            return -1;
        }

        /// <summary>
        /// ログファイルを追加します。
        /// </summary>
        /// <param name="path">ログファイルのパス</param>
        /// <param name="append">末尾にテキストを追加するかどうか</param>
        /// <param name="encoding">エンコーディング。nullでUTF-8N</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/>が無効</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="path"/>のディレクトリが存在しない</exception>
        /// <exception cref="IOException"><paramref name="path"/>の書式が無効</exception>
        /// <exception cref="PathTooLongException"><paramref name="path"/>が長すぎる</exception>
        /// <exception cref="UnauthorizedAccessException">アクセスが拒否された</exception>
        /// <exception cref="System.Security.SecurityException">アクセス権限がない</exception>
        public void AddLogFile(string path, bool append = false, Encoding? encoding = null)
        {
            var writer = new StreamWriter(path, append, encoding ?? IOHelper.UTF8N);
            writers.Add(new WriterEntry(writer)
            {
                MustDisposed = true,
                Path = Path.GetFullPath(path),
            });
        }

        /// <summary>
        /// ログファイルを追加します。
        /// </summary>
        /// <param name="file">ログファイルの情報</param>
        /// <param name="append">末尾にテキストを追加するかどうか</param>
        /// <param name="encoding">エンコーディング。nullでUTF-8N</param>
        /// <exception cref="ArgumentNullException"><paramref name="file"/>がnull</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="file"/>のディレクトリが存在しない</exception>
        /// <exception cref="FileNotFoundException"><paramref name="file"/>が存在しない</exception>
        /// <exception cref="IOException">ファイルが既に開かれている</exception>
        /// <exception cref="UnauthorizedAccessException">アクセスが拒否された</exception>
        /// <exception cref="System.Security.SecurityException">アクセス権限がない</exception>
        public void AddLogFile(FileInfo file, bool append = false, Encoding? encoding = null)
        {
            FileStream stream = file.Open(append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read);
            var writer = new StreamWriter(stream, encoding ?? IOHelper.UTF8N, leaveOpen: false);
            writers.Add(new WriterEntry(writer)
            {
                MustDisposed = true,
                Path = file.FullName,
            });
        }

        /// <summary>
        /// ログ出力先を追加します。
        /// </summary>
        /// <param name="writer">追加するログ出力先</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>がnull</exception>
        public void AddLog(TextWriter writer)
        {
            ArgumentNullException.ThrowIfNull(writer);

            writers.Add(new WriterEntry(writer));
        }

        /// <summary>
        /// 指定したログファイルを管理中かどうかを検索します。
        /// </summary>
        /// <param name="path">検索するログファイルのパス</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/>が無効</exception>
        /// <exception cref="PathTooLongException"><paramref name="path"/>が長すぎる</exception>
        /// <returns>ログファイルの存在が確認できたらtrue，それ以外でfalse</returns>
        public bool HasLogFile(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            return GetLogIndex(Path.GetFullPath(path)) >= 0;
        }

        /// <summary>
        /// ログ出力を解除します。
        /// </summary>
        /// <param name="path">出力解除するログファイルのパス</param>
        /// <returns>ログ出力を解除できたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="path"/>が無効</exception>
        /// <exception cref="PathTooLongException"><paramref name="path"/>が長すぎる</exception>
        public bool RemoveLogFile(string path)
        {
            int index = GetLogIndex(Path.GetFullPath(path));
            if (index < 0) return false;
            WriterEntry entry = writers[index];
            if (entry.MustDisposed) entry.Writer.Dispose();
            writers.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// ログ出力を解除します。
        /// </summary>
        /// <param name="writer">出力解除するログファイルのライター</param>
        /// <returns>ログ出力を解除できたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>が<see langword="null"/></exception>
        public bool RemoveLog(TextWriter writer)
        {
            ArgumentNullException.ThrowIfNull(writer);

            int index = GetLogIndex(writer);
            if (index < 0) return false;
            WriterEntry entry = writers[index];
            if (entry.MustDisposed) entry.Writer.Dispose();
            writers.RemoveAt(index);
            return true;
        }

        #endregion Collection Operation

        #region Write Operation

        /// <inheritdoc/>
        public override void Write(char value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(ReadOnlySpan<char> buffer)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(buffer);
        }

        /// <inheritdoc/>
        public override void Write(char[]? buffer)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(buffer);
        }

        /// <inheritdoc/>
        public override void Write(char[] buffer, int index, int count)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(buffer, index, count);
        }

        /// <inheritdoc/>
        public override void Write(int value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(uint value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(ulong value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(long value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(float value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(double value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(decimal value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(bool value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(object? value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(StringBuilder? value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(string? value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(string format, object? arg0)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(format, arg0);
        }

        /// <inheritdoc/>
        public override void Write(string format, object? arg0, object? arg1)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(format, arg0, arg1);
        }

        /// <inheritdoc/>
        public override void Write(string format, object? arg0, object? arg1, object? arg2)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(format, arg0, arg1, arg2);
        }

        /// <inheritdoc/>
        public override void Write(string format, params object?[] arg)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(format, arg);
        }

        /// <inheritdoc/>
        public override async Task WriteAsync(char value)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteAsync(value);
        }

        /// <inheritdoc/>
        public override async Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteAsync(buffer, cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task WriteAsync(char[] buffer, int index, int count)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteAsync(buffer, index, count);
        }

        /// <inheritdoc/>
        public override async Task WriteAsync(StringBuilder? value, CancellationToken cancellationToken = default)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteAsync(value, cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task WriteAsync(string? value)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteAsync(value);
        }

        /// <inheritdoc/>
        public override void WriteLine()
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine();
        }

        /// <inheritdoc/>
        public override void WriteLine(char value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(ReadOnlySpan<char> buffer)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(buffer);
        }

        /// <inheritdoc/>
        public override void WriteLine(char[]? buffer)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(buffer);
        }

        /// <inheritdoc/>
        public override void WriteLine(char[] buffer, int index, int count)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(buffer, index, count);
        }

        /// <inheritdoc/>
        public override void WriteLine(int value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(uint value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(ulong value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(long value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(float value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(double value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(decimal value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(bool value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(object? value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(StringBuilder? value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(string? value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(string format, object? arg0)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(format, arg0);
        }

        /// <inheritdoc/>
        public override void WriteLine(string format, object? arg0, object? arg1)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(format, arg0, arg1);
        }

        /// <inheritdoc/>
        public override void WriteLine(string format, object? arg0, object? arg1, object? arg2)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(format, arg0, arg1, arg2);
        }

        /// <inheritdoc/>
        public override void WriteLine(string format, params object?[] arg)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(format, arg);
        }

        /// <inheritdoc/>
        public override async Task WriteLineAsync()
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteLineAsync();
        }

        /// <inheritdoc/>
        public override async Task WriteLineAsync(char value)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteLineAsync(value);
        }

        /// <inheritdoc/>
        public override async Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteLineAsync(buffer, cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task WriteLineAsync(char[] buffer, int index, int count)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteLineAsync(buffer, index, count);
        }

        /// <inheritdoc/>
        public override async Task WriteLineAsync(StringBuilder? value, CancellationToken cancellationToken = default)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteLineAsync(value, cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task WriteLineAsync(string? value)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteLineAsync(value);
        }

        #endregion Write Operation

        #region IDisposable

        private bool disposedValue;

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (WriterEntry entry in writers)
                        if (entry.MustDisposed)
                            entry.Writer.Dispose();
                    writers.Clear();
                }

                disposedValue = true;
            }
        }

        #endregion IDisposable

        /// <summary>
        /// <see cref="TextWriter"/>のエントリーです。
        /// </summary>
        private sealed class WriterEntry
        {
            public bool MustDisposed;
            public string? Path;
            public readonly TextWriter Writer;

            internal WriterEntry(TextWriter writer)
            {
                Writer = writer;
            }
        }
    }
}
