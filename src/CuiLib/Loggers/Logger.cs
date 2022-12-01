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
    public class Logger : IDisposable
    {
        private readonly List<WriterEntry> writers;

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
                else RemoveLogFile(Console.Out);
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
                else RemoveLogFile(Console.Error);
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
        /// ログファイルを削除します。
        /// </summary>
        /// <param name="path">削除するログファイルのパス</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/>が無効</exception>
        /// <exception cref="PathTooLongException"><paramref name="path"/>が長すぎる</exception>
        /// <returns>ログファイルを削除できたらtrue，それ以外でfalse</returns>
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
        /// ログファイルを削除します。
        /// </summary>
        /// <param name="writer">削除するログファイルのライター</param>
        /// <returns>ログファイルを削除できたらtrue，それ以外でfalse</returns>
        private bool RemoveLogFile(TextWriter writer)
        {
            int index = GetLogIndex(writer);
            if (index < 0) return false;
            WriterEntry entry = writers[index];
            if (entry.MustDisposed) entry.Writer.Dispose();
            writers.RemoveAt(index);
            return true;
        }

        #endregion Collection Operation

        #region Write Operation

        /// <inheritdoc cref="TextWriter.Write(char)"/>
        public void Write(char value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc cref="TextWriter.Write(ReadOnlySpan{char})"/>
        public void Write(ReadOnlySpan<char> buffer)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(buffer);
        }

        /// <inheritdoc cref="TextWriter.Write(char[])"/>
        public void Write(char[]? buffer)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(buffer);
        }

        /// <inheritdoc cref="TextWriter.Write(char[], int, int)"/>
        public void Write(char[] buffer, int index, int count)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(buffer, index, count);
        }

        /// <inheritdoc cref="TextWriter.Write(int)"/>
        public void Write(int value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc cref="TextWriter.Write(uint)"/>
        public void Write(uint value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc cref="TextWriter.Write(ulong)"/>
        public void Write(ulong value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc cref="TextWriter.Write(long)"/>
        public void Write(long value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc cref="TextWriter.Write(float)"/>
        public void Write(float value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc cref="TextWriter.Write(double)"/>
        public void Write(double value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc cref="TextWriter.Write(decimal)"/>
        public void Write(decimal value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc cref="TextWriter.Write(bool)"/>
        public void Write(bool value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc cref="TextWriter.Write(object)"/>
        public void Write(object? value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc cref="TextWriter.Write(StringBuilder)"/>
        public void Write(StringBuilder? value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc cref="TextWriter.Write(string)"/>
        public void Write(string? value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc cref="TextWriter.Write(string, object)"/>
        public void Write(string format, object? arg0)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(format, arg0);
        }

        /// <inheritdoc cref="TextWriter.Write(string, object, object)"/>
        public void Write(string format, object? arg0, object? arg1)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(format, arg0, arg1);
        }

        /// <inheritdoc cref="TextWriter.Write(string, object, object, object)"/>
        public void Write(string format, object? arg0, object? arg1, object? arg2)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(format, arg0, arg1, arg2);
        }

        /// <inheritdoc cref="TextWriter.Write(string, object[])"/>
        public void Write(string format, params object?[] arg)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(format, arg);
        }

        /// <inheritdoc cref="TextWriter.WriteAsync(char)"/>
        public async Task WriteAsync(char value)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteAsync(value);
        }

        /// <inheritdoc cref="TextWriter.WriteAsync(ReadOnlyMemory{char}, CancellationToken)"/>
        public async Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteAsync(buffer, cancellationToken);
        }

        /// <inheritdoc cref="TextWriter.WriteAsync(char[])"/>
        public async Task WriteAsync(char[]? buffer)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteAsync(buffer);
        }

        /// <inheritdoc cref="TextWriter.WriteAsync(char[], int, int)"/>
        public async Task WriteAsync(char[] buffer, int index, int count)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteAsync(buffer, index, count);
        }

        /// <inheritdoc cref="TextWriter.WriteAsync(StringBuilder, CancellationToken)"/>
        public async Task WriteAsync(StringBuilder? value, CancellationToken cancellationToken = default)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteAsync(value, cancellationToken);
        }

        /// <inheritdoc cref="TextWriter.WriteAsync(string)"/>
        public async Task WriteAsync(string? value)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteAsync(value);
        }

        /// <inheritdoc cref="TextWriter.WriteLine()"/>
        public void WriteLine()
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine();
        }

        /// <inheritdoc cref="TextWriter.WriteLine(char)"/>
        public void WriteLine(char value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc cref="TextWriter.WriteLine(ReadOnlySpan{char})"/>
        public void WriteLine(ReadOnlySpan<char> buffer)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(buffer);
        }

        /// <inheritdoc cref="TextWriter.WriteLine(char[])"/>
        public void WriteLine(char[]? buffer)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(buffer);
        }

        /// <inheritdoc cref="TextWriter.WriteLine(char[], int, int)"/>
        public void WriteLine(char[] buffer, int index, int count)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(buffer, index, count);
        }

        /// <inheritdoc cref="TextWriter.WriteLine(int)"/>
        public void WriteLine(int value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc cref="TextWriter.WriteLine(uint)"/>
        public void WriteLine(uint value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc cref="TextWriter.WriteLine(ulong)"/>
        public void WriteLine(ulong value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc cref="TextWriter.WriteLine(long)"/>
        public void WriteLine(long value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc cref="TextWriter.WriteLine(float)"/>
        public void WriteLine(float value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc cref="TextWriter.WriteLine(double)"/>
        public void WriteLine(double value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc cref="TextWriter.WriteLine(decimal)"/>
        public void WriteLine(decimal value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc cref="TextWriter.WriteLine(bool)"/>
        public void WriteLine(bool value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc cref="TextWriter.WriteLine(object)"/>
        public void WriteLine(object? value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc cref="TextWriter.WriteLine(StringBuilder)"/>
        public void WriteLine(StringBuilder? value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc cref="TextWriter.WriteLine(string)"/>
        public void WriteLine(string? value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc cref="TextWriter.WriteLine(string, object)"/>
        public void WriteLine(string format, object? arg0)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(format, arg0);
        }

        /// <inheritdoc cref="TextWriter.WriteLine(string, object, object)"/>
        public void WriteLine(string format, object? arg0, object? arg1)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(format, arg0, arg1);
        }

        /// <inheritdoc cref="TextWriter.WriteLine(string, object, object, object)"/>
        public void WriteLine(string format, object? arg0, object? arg1, object? arg2)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(format, arg0, arg1, arg2);
        }

        /// <inheritdoc cref="TextWriter.WriteLine(string, object[])"/>
        public void WriteLine(string format, params object?[] arg)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(format, arg);
        }

        /// <inheritdoc cref="TextWriter.WriteLineAsync()"/>
        public async Task WriteLineAsync()
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteLineAsync();
        }

        /// <inheritdoc cref="TextWriter.WriteLineAsync(char)"/>
        public async Task WriteLineAsync(char value)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteLineAsync(value);
        }

        /// <inheritdoc cref="TextWriter.WriteLineAsync(ReadOnlyMemory{char}, CancellationToken)"/>
        public async Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteLineAsync(buffer, cancellationToken);
        }

        /// <inheritdoc cref="TextWriter.WriteLineAsync(char[])"/>
        public async Task WriteLineAsync(char[]? buffer)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteLineAsync(buffer);
        }

        /// <inheritdoc cref="TextWriter.WriteLineAsync(char[], int, int)"/>
        public async Task WriteLineAsync(char[] buffer, int index, int count)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteLineAsync(buffer, index, count);
        }

        /// <inheritdoc cref="TextWriter.WriteLineAsync(StringBuilder, CancellationToken)"/>
        public async Task WriteLineAsync(StringBuilder? value, CancellationToken cancellationToken = default)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteLineAsync(value, cancellationToken);
        }

        /// <inheritdoc cref="TextWriter.WriteLineAsync(string)"/>
        public async Task WriteLineAsync(string? value)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteLineAsync(value);
        }

        #endregion Write Operation

        #region IDisposable

        private bool disposedValue;

        /// <summary>
        /// インスタンスを破棄します。
        /// </summary>
        /// <param name="disposing">アンマネージドリソースも破棄するかどうか</param>
        protected virtual void Dispose(bool disposing)
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

        /// <summary>
        /// インスタンスを破棄します。
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        /// <summary>
        /// <see cref="TextWriter"/>のエントリーです。
        /// </summary>
        private sealed class WriterEntry
        {
            public bool MustDisposed;
            public string? Path;
            public TextWriter Writer;

            internal WriterEntry(TextWriter writer)
            {
                Writer = writer;
            }
        }
    }
}
