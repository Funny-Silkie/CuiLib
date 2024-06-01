using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Helpers
{
    internal sealed class BufferedTextWriter : TextWriter
    {
        private readonly StringBuilder builder;

        public BufferedTextWriter()
        {
            builder = new StringBuilder();
        }

        /// <inheritdoc/>
        public override Encoding Encoding => throw new NotImplementedException();

        public string GetData() => builder.ToString();

        /// <inheritdoc/>
        public override void Flush()
        {
        }

        /// <inheritdoc/>
        public override async Task FlushAsync() => await Task.CompletedTask;

#if NET8_0_OR_GREATER

        /// <inheritdoc/>
        public override async Task FlushAsync(CancellationToken cancellationToken) => await Task.CompletedTask;

#endif

        /// <inheritdoc/>
        public override void Write(char value) => builder.Append(value);

        /// <inheritdoc/>
        public override void Write(ReadOnlySpan<char> buffer) => builder.Append(buffer);

        /// <inheritdoc/>
        public override void Write(char[]? buffer) => builder.Append(buffer);

        /// <inheritdoc/>
        public override void Write(char[] buffer, int index, int count) => builder.Append(buffer, index, count);

        /// <inheritdoc/>
        public override void Write(int value) => builder.Append(value);

        /// <inheritdoc/>
        public override void Write(uint value) => builder.Append(value);

        /// <inheritdoc/>
        public override void Write(long value) => builder.Append(value);

        /// <inheritdoc/>
        public override void Write(ulong value) => builder.Append(value);

        /// <inheritdoc/>
        public override void Write(float value) => builder.Append(value);

        /// <inheritdoc/>
        public override void Write(double value) => builder.Append(value);

        /// <inheritdoc/>
        public override void Write(decimal value) => builder.Append(value);

        /// <inheritdoc/>
        public override void Write(bool value) => builder.Append(value);

        /// <inheritdoc/>
        public override void Write(object? value) => builder.Append(value);

        /// <inheritdoc/>
        public override void Write(StringBuilder? value) => builder.Append(value);

        /// <inheritdoc/>
        public override void Write(string? value) => builder.Append(value);

        /// <inheritdoc/>
        public override void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0) => builder.AppendFormat(format, arg0);

        /// <inheritdoc/>
        public override void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1) => builder.AppendFormat(format, arg0, arg1);

        /// <inheritdoc/>
        public override void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2) => builder.AppendFormat(format, arg0, arg1, arg2);

        /// <inheritdoc/>
        public override void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args) => builder.AppendFormat(format, args);

        /// <inheritdoc/>
        public override async Task WriteAsync(char value)
        {
            builder.Append(value);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            builder.Append(buffer);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task WriteAsync(char[] buffer, int index, int count)
        {
            builder.Append(buffer, index, count);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task WriteAsync(StringBuilder? value, CancellationToken cancellationToken = default)
        {
            builder.Append(value);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task WriteAsync(string? value)
        {
            builder.Append(value);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override void WriteLine() => builder.AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(char value) => builder.Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(ReadOnlySpan<char> buffer) => builder.Append(buffer).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(char[]? buffer) => builder.Append(buffer).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(char[] buffer, int index, int count) => builder.Append(buffer, index, count).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(int value) => builder.Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(uint value) => builder.Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(long value) => builder.Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(ulong value) => builder.Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(float value) => builder.Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(double value) => builder.Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(decimal value) => builder.Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(bool value) => builder.Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(object? value) => builder.Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(StringBuilder? value) => builder.Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(string? value) => builder.Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0) => builder.AppendFormat(format, arg0).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1) => builder.AppendFormat(format, arg0, arg1).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2) => builder.AppendFormat(format, arg0, arg1, arg2).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args) => builder.AppendFormat(format, args).AppendLine();

        /// <inheritdoc/>
        public override async Task WriteLineAsync()
        {
            builder.AppendLine();
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task WriteLineAsync(char value)
        {
            builder.Append(value).AppendLine();
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            builder.Append(buffer).AppendLine();
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task WriteLineAsync(char[] buffer, int index, int count)
        {
            builder.Append(buffer, index, count).AppendLine();
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task WriteLineAsync(StringBuilder? value, CancellationToken cancellationToken = default)
        {
            builder.Append(value).AppendLine();
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task WriteLineAsync(string? value)
        {
            builder.Append(value).AppendLine();
            await Task.CompletedTask;
        }
    }
}
