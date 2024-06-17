using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CuiLib.Logging
{
    /// <summary>
    /// ロガーのクラスです。
    /// </summary>
    public class Logger : TextWriter
    {
        internal readonly List<WriterEntry> writers;

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
                if (value) writers.Add(new WriterEntry(Console.Out) { IsConsoleWriter = true });
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
                if (value) writers.Add(new WriterEntry(Console.Error) { IsConsoleWriter = true });
                else RemoveLog(Console.Error);
            }
        }

        private bool _consoleErrorLogEnabled;

        /// <summary>
        /// デフォルトの文字エンコードを取得または設定します。
        /// </summary>
        /// <remarks>初期値はBOM無しUTF-8</remarks>
        /// <exception cref="ArgumentNullException">設定しようとした値が<see langword="null"/></exception>
        public Encoding DefaultEncoding
        {
            get => _defaultEncoding;
            set
            {
                ThrowHelpers.ThrowIfNull(value, nameof(value));
                _defaultEncoding = value;
            }
        }

        private Encoding _defaultEncoding = IOHelpers.UTF8N;

        /// <summary>
        /// <see cref="Logger"/>の新しいインスタンスを初期化します。
        /// </summary>
        public Logger()
        {
            writers = [];
        }

        /// <summary>
        /// <see cref="Logger"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="logFile">ログファイルのパス</param>
        /// <param name="append">末尾追加モードにするかどうか</param>
        /// <param name="encoding">文字コード，<see langword="null"/>で<see cref="IOHelpers.UTF8N"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="logFile"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="logFile"/>が無効</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="logFile"/>のディレクトリが存在しない</exception>
        /// <exception cref="IOException"><paramref name="logFile"/>の書式が無効</exception>
        /// <exception cref="PathTooLongException"><paramref name="logFile"/>が長すぎる</exception>
        /// <exception cref="UnauthorizedAccessException">アクセスが拒否された</exception>
        /// <exception cref="System.Security.SecurityException">アクセス権限がない</exception>
        public Logger(string logFile, bool append = true, Encoding? encoding = null)
        {
            writers = [];
            AddLogFile(logFile, append, encoding);
        }

        /// <summary>
        /// <see cref="Logger"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="logFile">ログファイルの情報</param>
        /// <param name="append">末尾追加モードにするかどうか</param>
        /// <param name="encoding">文字コード，<see langword="null"/>で<see cref="IOHelpers.UTF8N"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="logFile"/>がnull</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="logFile"/>のディレクトリが存在しない</exception>
        /// <exception cref="IOException"><paramref name="logFile"/>の書式が無効</exception>
        /// <exception cref="PathTooLongException"><paramref name="logFile"/>のパスが長すぎる</exception>
        /// <exception cref="UnauthorizedAccessException">アクセスが拒否された</exception>
        /// <exception cref="System.Security.SecurityException">アクセス権限がない</exception>
        public Logger(FileInfo logFile, bool append = true, Encoding? encoding = null)
        {
            writers = [];
            AddLogFile(logFile, append, encoding);
        }

        /// <summary>
        /// 出力先の<see cref="TextWriter"/>を全て取得します。
        /// </summary>
        /// <param name="includeStdoutAndError">標準出力・エラー出力も含めるかどうか</param>
        /// <returns>出力先の<see cref="TextWriter"/>一覧</returns>
        internal IEnumerable<TextWriter> GetAllTargets(bool includeStdoutAndError = true)
        {
            IEnumerable<WriterEntry> target = includeStdoutAndError ? writers : writers.Where(x => !x.IsConsoleWriter);

            foreach (TextWriter current in target.Select(x => x.Writer)) yield return current;
        }

        #region Collection Operations

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
        /// <param name="encoding">文字コード。<see langword="null"/>で<see cref="DefaultEncoding"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/>が無効</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="path"/>のディレクトリが存在しない</exception>
        /// <exception cref="IOException"><paramref name="path"/>の書式が無効</exception>
        /// <exception cref="PathTooLongException"><paramref name="path"/>が長すぎる</exception>
        /// <exception cref="UnauthorizedAccessException">アクセスが拒否された</exception>
        /// <exception cref="System.Security.SecurityException">アクセス権限がない</exception>
        public void AddLogFile(string path, bool append = false, Encoding? encoding = null)
        {
            var writer = new StreamWriter(path, append, encoding ?? DefaultEncoding);
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
        /// <param name="encoding">文字コード。<see langword="null"/>で<see cref="DefaultEncoding"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="file"/>がnull</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="file"/>のディレクトリが存在しない</exception>
        /// <exception cref="IOException">ファイルが既に開かれている</exception>
        /// <exception cref="PathTooLongException"><paramref name="file"/>のパスが長すぎる</exception>
        /// <exception cref="UnauthorizedAccessException">アクセスが拒否された</exception>
        /// <exception cref="System.Security.SecurityException">アクセス権限がない</exception>
        public void AddLogFile(FileInfo file, bool append = false, Encoding? encoding = null)
        {
            ThrowHelpers.ThrowIfNull(file);

            FileStream stream = file.Open(append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read);
#if NET6_0_OR_GREATER
            var writer = new StreamWriter(stream, encoding ?? DefaultEncoding, leaveOpen: false);
#else
            var writer = new StreamWriter(stream, encoding ?? DefaultEncoding, 1024, false);
#endif
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
            ThrowHelpers.ThrowIfNull(writer);

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
            ThrowHelpers.ThrowIfNull(path);

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
            ThrowHelpers.ThrowIfNull(writer);

            int index = GetLogIndex(writer);
            if (index < 0) return false;
            WriterEntry entry = writers[index];
            if (entry.MustDisposed) entry.Writer.Dispose();
            writers.RemoveAt(index);
            return true;
        }

        #endregion Collection Operations

        #region Write Operations

        /// <inheritdoc/>
        public override void Flush()
        {
            foreach (WriterEntry entry in writers) entry.Writer.Flush();
        }

        /// <inheritdoc/>
        public override async Task FlushAsync()
        {
            foreach (WriterEntry entry in writers) await entry.Writer.FlushAsync();
        }

#if NET8_0_OR_GREATER

        /// <inheritdoc/>
        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.FlushAsync(cancellationToken);
        }

#endif

        /// <inheritdoc/>
        public override void Write(char value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

#if NETSTANDARD2_1_OR_GREATER || NET

        /// <inheritdoc/>
        public override void Write(ReadOnlySpan<char> buffer)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(buffer);
        }

#endif

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
        public override void Write(long value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(ulong value)
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

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override void Write(StringBuilder? value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

#endif

        /// <inheritdoc/>
        public override void Write(string? value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(
#if NET7_0_OR_GREATER
                                   [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
#endif
                                   string format,
                                   object? arg0)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(format, arg0);
        }

        /// <inheritdoc/>
        public override void Write(
#if NET7_0_OR_GREATER
                                   [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
#endif
                                   string format,
                                   object? arg0,
                                   object? arg1)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(format, arg0, arg1);
        }

        /// <inheritdoc/>
        public override void Write(
#if NET7_0_OR_GREATER
                                   [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
#endif
                                   string format,
                                   object? arg0,
                                   object? arg1,
                                   object? arg2)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(format, arg0, arg1, arg2);
        }

        /// <inheritdoc/>
        public override void Write(
#if NET7_0_OR_GREATER
                                   [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
#endif
                                   string format,
                                   params object?[] arg)
        {
            foreach (WriterEntry entry in writers) entry.Writer.Write(format, arg);
        }

        /// <inheritdoc/>
        public override async Task WriteAsync(char value)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteAsync(value);
        }

#if NETSTANDARD2_1_OR_GREATER || NET

        /// <inheritdoc/>
        public override async Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteAsync(buffer, cancellationToken);
        }

#endif

        /// <inheritdoc/>
        public override async Task WriteAsync(char[] buffer, int index, int count)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteAsync(buffer, index, count);
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override async Task WriteAsync(StringBuilder? value, CancellationToken cancellationToken = default)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteAsync(value, cancellationToken);
        }

#endif

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

#if NETSTANDARD2_1_OR_GREATER || NET

        /// <inheritdoc/>
        public override void WriteLine(ReadOnlySpan<char> buffer)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(buffer);
        }

#endif

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

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override void WriteLine(StringBuilder? value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

#endif

        /// <inheritdoc/>
        public override void WriteLine(string? value)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(
#if NET7_0_OR_GREATER
                                       [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
#endif
                                       string format,
                                       object? arg0)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(format, arg0);
        }

        /// <inheritdoc/>
        public override void WriteLine(
#if NET7_0_OR_GREATER
                                       [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
#endif
                                       string format,
                                       object? arg0,
                                       object? arg1)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(format, arg0, arg1);
        }

        /// <inheritdoc/>
        public override void WriteLine(
#if NET7_0_OR_GREATER
                                       [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
#endif
                                       string format,
                                       object? arg0,
                                       object? arg1,
                                       object? arg2)
        {
            foreach (WriterEntry entry in writers) entry.Writer.WriteLine(format, arg0, arg1, arg2);
        }

        /// <inheritdoc/>
        public override void WriteLine(
#if NET7_0_OR_GREATER
                                       [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
#endif
                                       string format,
                                       params object?[] arg)
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

#if NETSTANDARD2_1_OR_GREATER || NET

        /// <inheritdoc/>
        public override async Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteLineAsync(buffer, cancellationToken);
        }

#endif

        /// <inheritdoc/>
        public override async Task WriteLineAsync(char[] buffer, int index, int count)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteLineAsync(buffer, index, count);
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override async Task WriteLineAsync(StringBuilder? value, CancellationToken cancellationToken = default)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteLineAsync(value, cancellationToken);
        }

#endif

        /// <inheritdoc/>
        public override async Task WriteLineAsync(string? value)
        {
            foreach (WriterEntry entry in writers) await entry.Writer.WriteLineAsync(value);
        }

        #endregion Write Operations

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
        internal sealed class WriterEntry
        {
            public bool MustDisposed;
            public string? Path;
            public readonly TextWriter Writer;
            public bool IsConsoleWriter;

            internal WriterEntry(TextWriter writer)
            {
                Writer = writer;
            }
        }
    }
}
