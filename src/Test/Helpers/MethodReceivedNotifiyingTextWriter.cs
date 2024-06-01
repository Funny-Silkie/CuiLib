using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Helpers
{
    internal sealed class MethodReceivedNotifiyingTextWriter : TextWriter
    {
        private readonly StringBuilder builder;

        public MethodReceivedNotifiyingTextWriter()
        {
            builder = new StringBuilder();
        }

        /// <inheritdoc/>
        public override Encoding Encoding => throw new NotImplementedException();

        public string[] GetData()
        {
            string[] result = builder.ToString().Split(Environment.NewLine);
            if (result.Length > 0 && result[^1].Length == 0) return result[..^1];
            return result;
        }

        /// <inheritdoc/>
        public override void Flush() => builder.AppendLine("Flush()");

        /// <inheritdoc/>
        public override async Task FlushAsync()
        {
            builder.AppendLine("FlushAsync()");
            await Task.CompletedTask;
        }

#if NET8_0_OR_GREATER

        /// <inheritdoc/>
        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            builder.AppendLine("FlushAsync(CancellationToken)");
            await Task.CompletedTask;
        }

#endif

        /// <inheritdoc/>
        public override void Write(char value) => builder.Append("Write(char): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void Write(ReadOnlySpan<char> buffer) => builder.Append("Write(ReadOnlySpan<char>): ").Append(buffer).AppendLine();

        /// <inheritdoc/>
        public override void Write(char[]? buffer) => builder.Append("Write(char[]?): ").Append(buffer).AppendLine();

        /// <inheritdoc/>
        public override void Write(char[] buffer, int index, int count) => builder.Append("Write(char[], int, int): ").Append(buffer, index, count).AppendLine();

        /// <inheritdoc/>
        public override void Write(int value) => builder.Append("Write(int): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void Write(uint value) => builder.Append("Write(uint): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void Write(long value) => builder.Append("Write(long): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void Write(ulong value) => builder.Append("Write(ulong): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void Write(float value) => builder.Append("Write(float): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void Write(double value) => builder.Append("Write(double): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void Write(decimal value) => builder.Append("Write(decimal): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void Write(bool value) => builder.Append("Write(bool): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void Write(object? value) => builder.Append("Write(object?): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void Write(StringBuilder? value) => builder.Append("Write(StringBuilder?): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void Write(string? value) => builder.Append("Write(string?): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0) => builder.Append("Write(string, object?): ").AppendFormat(format, arg0).AppendLine();

        /// <inheritdoc/>
        public override void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1) => builder.Append("Write(string, object?, object?): ").AppendFormat(format, arg0, arg1).AppendLine();

        /// <inheritdoc/>
        public override void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2) => builder.Append("Write(string, object?, object?, object?): ").AppendFormat(format, arg0, arg1, arg2).AppendLine();

        /// <inheritdoc/>
        public override void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args) => builder.Append("Write(string, object?[]): ").AppendFormat(format, args).AppendLine();

        /// <inheritdoc/>
        public override async Task WriteAsync(char value)
        {
            builder.Append("WriteAsync(char): ").Append(value).AppendLine();
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            builder.Append("WriteAsync(ReadOnlyMemory<char>, CancellationToken): ").Append(buffer).AppendLine();
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task WriteAsync(char[] buffer, int index, int count)
        {
            builder.Append("WriteAsync(char[], int, int): ").Append(buffer, index, count).AppendLine();
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task WriteAsync(StringBuilder? value, CancellationToken cancellationToken = default)
        {
            builder.Append("WriteAsync(StringBuilder?, CancellationToken): ").Append(value).AppendLine();
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task WriteAsync(string? value)
        {
            builder.Append("WriteAsync(string?): ").Append(value).AppendLine();
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override void WriteLine() => builder.Append("WriteLine()").AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(char value) => builder.Append("WriteLine(char): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(ReadOnlySpan<char> buffer) => builder.Append("WriteLine(ReadOnlySpan<char>): ").Append(buffer).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(char[]? buffer) => builder.Append("WriteLine(char[]?): ").Append(buffer).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(char[] buffer, int index, int count) => builder.Append("WriteLine(char[], int, int): ").Append(buffer, index, count).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(int value) => builder.Append("WriteLine(int): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(uint value) => builder.Append("WriteLine(uint): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(long value) => builder.Append("WriteLine(long): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(ulong value) => builder.Append("WriteLine(ulong): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(float value) => builder.Append("WriteLine(float): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(double value) => builder.Append("WriteLine(double): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(decimal value) => builder.Append("WriteLine(decimal): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(bool value) => builder.Append("WriteLine(bool): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(object? value) => builder.Append("WriteLine(object?): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(StringBuilder? value) => builder.Append("WriteLine(StringBuilder?): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine(string? value) => builder.Append("WriteLine(string?): ").Append(value).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0) => builder.Append("WriteLine(string, object?): ").AppendFormat(format, arg0).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1) => builder.Append("WriteLine(string, object?, object?): ").AppendFormat(format, arg0, arg1).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2) => builder.Append("WriteLine(string, object?, object?, object?): ").AppendFormat(format, arg0, arg1, arg2).AppendLine();

        /// <inheritdoc/>
        public override void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args) => builder.Append("WriteLine(string, object?[]): ").AppendFormat(format, args).AppendLine();

        /// <inheritdoc/>
        public override async Task WriteLineAsync()
        {
            builder.Append("WriteLineAsync()").AppendLine();
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task WriteLineAsync(char value)
        {
            builder.Append("WriteLineAsync(char): ").Append(value).AppendLine();
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            builder.Append("WriteLineAsync(ReadOnlyMemory<char>, CancellationToken): ").Append(buffer).AppendLine();
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task WriteLineAsync(char[] buffer, int index, int count)
        {
            builder.Append("WriteLineAsync(char[], int, int): ").Append(buffer, index, count).AppendLine();
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task WriteLineAsync(StringBuilder? value, CancellationToken cancellationToken = default)
        {
            builder.Append("WriteLineAsync(StringBuilder?, CancellationToken): ").Append(value).AppendLine();
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task WriteLineAsync(string? value)
        {
            builder.Append("WriteLineAsync(string?): ").Append(value).AppendLine();
            await Task.CompletedTask;
        }
    }
}
