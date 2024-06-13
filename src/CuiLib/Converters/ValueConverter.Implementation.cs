using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace CuiLib.Converters
{
    public static partial class ValueConverter
    {
        /// <summary>
        /// デリゲートで変換を行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        /// <typeparam name="TIn">変換前の型</typeparam>
        /// <typeparam name="TOut">変換後の型</typeparam>
        [Serializable]
        private sealed class DelegateValueConverter<TIn, TOut> : IValueConverter<TIn, TOut>
        {
            private readonly Converter<TIn, TOut> converter;

            /// <summary>
            /// <see cref="DelegateValueConverter{TIn, TOut}"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="converter">使用するデリゲート</param>
            /// <exception cref="ArgumentNullException"><paramref name="converter"/>がnull</exception>
            internal DelegateValueConverter(Converter<TIn, TOut> converter)
            {
                ThrowHelpers.ThrowIfNull(converter);

                this.converter = converter;
            }

            /// <inheritdoc/>
            public TOut Convert(TIn value)
            {
                return converter.Invoke(value);
            }
        }

        /// <summary>
        /// 値の素通しを行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        /// <typeparam name="T">扱う値の型</typeparam>
        [Serializable]
        private sealed class ThroughValueConverter<T> : IValueConverter<T, T>
        {
            /// <summary>
            /// <see cref="ThroughValueConverter{T}"/>の新しいインスタンスを初期化します。
            /// </summary>
            internal ThroughValueConverter()
            {
            }

            /// <inheritdoc/>
            public T Convert(T value)
            {
                return value;
            }

            /// <inheritdoc/>
            public override bool Equals(object? obj) => obj is ThroughValueConverter<T>;

            /// <inheritdoc/>
            public override int GetHashCode() => GetType().GetHashCode();
        }

#if NET7_0_OR_GREATER

        /// <summary>
        /// <see cref="IParsable{TSelf}"/>を変換する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        /// <typeparam name="T">変換する<see cref="IParsable{TSelf}"/>を実装する型</typeparam>
        [Serializable]
        private sealed class ParsableValueConverter<T> : IValueConverter<string, T>
            where T : IParsable<T>
        {
            /// <summary>
            /// <see cref="ParsableValueConverter{T}"/>の新しいインスタンスを初期化します。
            /// </summary>
            internal ParsableValueConverter()
            {
            }

            /// <inheritdoc/>
            public T Convert(string value)
            {
                return T.Parse(value, null);
            }

            /// <inheritdoc/>
            public override bool Equals(object? obj) => obj is ParsableValueConverter<T>;

            /// <inheritdoc/>
            public override int GetHashCode() => GetType().GetHashCode();
        }

#endif

        /// <summary>
        /// 列挙型の変換を行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        /// <typeparam name="T">列挙型の型</typeparam>
        [Serializable]
        private sealed class EnumValueConverter<T> : IValueConverter<string, T>
            where T : struct, Enum
        {
            private readonly bool ignoreCase;

            /// <summary>
            /// <see cref="EnumValueConverter{T}"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="ignoreCase">文字列の大文字小文字の区別を無視するかどうか</param>
            internal EnumValueConverter(bool ignoreCase)
            {
                this.ignoreCase = ignoreCase;
            }

            /// <inheritdoc/>
            public T Convert(string value)
            {
                return Enum.Parse<T>(value, ignoreCase);
            }
        }

        /// <summary>
        /// フォーマット付きで<see cref="DateTime"/>への変換を行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        [Serializable]
        internal sealed class DateTimeExactConverter : IValueConverter<string, DateTime>
        {
            private readonly string format;

            /// <summary>
            /// <see cref="DateTimeExactConverter"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="format">フォーマット</param>
            /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
            /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
            public DateTimeExactConverter(
#if NET7_0_OR_GREATER
                                          [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
#endif
                                          string format)
            {
                ThrowHelpers.ThrowIfNullOrEmpty(format);

                this.format = format;
            }

            /// <inheritdoc/>
            public DateTime Convert(string value)
            {
                return DateTime.ParseExact(value, format, null);
            }
        }

#if NET6_0_OR_GREATER

        /// <summary>
        /// フォーマット付きで<see cref="DateOnly"/>への変換を行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        [Serializable]
        internal sealed class DateOnlyExactConverter : IValueConverter<string, DateOnly>
        {
            private readonly string format;

            /// <summary>
            /// <see cref="DateOnlyExactConverter"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="format">フォーマット</param>
            /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
            /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
            public DateOnlyExactConverter(
#if NET7_0_OR_GREATER
                                          [StringSyntax(StringSyntaxAttribute.DateOnlyFormat)]
#endif
                                          string format)
            {
                ThrowHelpers.ThrowIfNullOrEmpty(format);

                this.format = format;
            }

            /// <inheritdoc/>
            public DateOnly Convert(string value)
            {
                return DateOnly.ParseExact(value, format, null);
            }
        }

        /// <summary>
        /// フォーマット付きで<see cref="TimeOnly"/>への変換を行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        [Serializable]
        internal sealed class TimeOnlyExactConverter : IValueConverter<string, TimeOnly>
        {
            private readonly string format;

            /// <summary>
            /// <see cref="TimeOnlyExactConverter"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="format">フォーマット</param>
            /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
            /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
            public TimeOnlyExactConverter(
#if NET7_0_OR_GREATER
                                          [StringSyntax(StringSyntaxAttribute.TimeOnlyFormat)]
#endif
                                          string format)
            {
                ThrowHelpers.ThrowIfNullOrEmpty(format);

                this.format = format;
            }

            /// <inheritdoc/>
            public TimeOnly Convert(string value)
            {
                return TimeOnly.ParseExact(value, format, null);
            }
        }

#endif

        /// <summary>
        /// フォーマット付きで<see cref="TimeSpan"/>への変換を行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        [Serializable]
        internal sealed class TimeSpanExactConverter : IValueConverter<string, TimeSpan>
        {
            private readonly string format;

            /// <summary>
            /// <see cref="TimeSpanExactConverter"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="format">フォーマット</param>
            /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
            /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
            public TimeSpanExactConverter(
#if NET7_0_OR_GREATER
                                          [StringSyntax(StringSyntaxAttribute.TimeSpanFormat)]
#endif
                                          string format)
            {
                ThrowHelpers.ThrowIfNullOrEmpty(format);

                this.format = format;
            }

            /// <inheritdoc/>
            public TimeSpan Convert(string value)
            {
                return TimeSpan.ParseExact(value, format, null);
            }
        }

        /// <summary>
        /// フォーマット付きで<see cref="DateTimeOffset"/>への変換を行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        [Serializable]
        internal sealed class DateTimeOffsetExactConverter : IValueConverter<string, DateTimeOffset>
        {
            private readonly string format;

            /// <summary>
            /// <see cref="DateTimeOffsetExactConverter"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="format">フォーマット</param>
            /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
            /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
            public DateTimeOffsetExactConverter(
#if NET7_0_OR_GREATER
                                                [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
#endif
                                                string format)
            {
                ThrowHelpers.ThrowIfNullOrEmpty(format);

                this.format = format;
            }

            /// <inheritdoc/>
            public DateTimeOffset Convert(string value)
            {
                return DateTimeOffset.ParseExact(value, format, null);
            }
        }

        /// <summary>
        /// <see cref="FileInfo"/>を生成する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        [Serializable]
        private sealed class FileInfoValueConverter : IValueConverter<string, FileInfo>
        {
            /// <summary>
            /// <see cref="FileInfoValueConverter"/>の新しいインスタンスを初期化します。
            /// </summary>
            internal FileInfoValueConverter()
            {
            }

            /// <inheritdoc/>
            public FileInfo Convert(string value)
            {
                return new FileInfo(value);
            }

            /// <inheritdoc/>
            public override bool Equals(object? obj) => obj is FileInfoValueConverter;

            /// <inheritdoc/>
            public override int GetHashCode() => GetType().GetHashCode();
        }

        /// <summary>
        /// <see cref="DirectoryInfo"/>を生成する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        [Serializable]
        private sealed class DirectoryInfoValueConverter : IValueConverter<string, DirectoryInfo>
        {
            /// <summary>
            /// <see cref="DirectoryInfoValueConverter"/>の新しいインスタンスを初期化します。
            /// </summary>
            internal DirectoryInfoValueConverter()
            {
            }

            /// <inheritdoc/>
            public DirectoryInfo Convert(string value)
            {
                return new DirectoryInfo(value);
            }

            /// <inheritdoc/>
            public override bool Equals(object? obj) => obj is DirectoryInfoValueConverter;

            /// <inheritdoc/>
            public override int GetHashCode() => GetType().GetHashCode();
        }

        /// <summary>
        /// <see cref="TextWriter"/>を生成する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        [Serializable]
        private sealed class FileOrConsoleWriterValueConverter : IValueConverter<string, TextWriter>
        {
            private readonly bool append;
            private readonly Encoding encoding;

            /// <summary>
            /// <see cref="FileOrConsoleWriterValueConverter"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="encoding">エンコーディング</param>
            /// <param name="append">上書きせずに末尾に追加するかどうか</param>
            /// <exception cref="ArgumentNullException"><paramref name="encoding"/>がnull</exception>
            internal FileOrConsoleWriterValueConverter(Encoding encoding, bool append)
            {
                ThrowHelpers.ThrowIfNull(encoding);

                this.encoding = encoding;
                this.append = append;
            }

            /// <inheritdoc/>
            public TextWriter Convert(string value)
            {
                if (value is null or "-") return Console.Out;
                return new StreamWriter(value, append, encoding);
            }
        }

        /// <summary>
        /// <see cref="TextReader"/>を生成する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        [Serializable]
        private sealed class FileOrConsoleReaderValueConverter : IValueConverter<string, TextReader>
        {
            private readonly Encoding encoding;

            /// <summary>
            /// <see cref="FileOrConsoleReaderValueConverter"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="encoding">エンコーディング</param>
            /// <exception cref="ArgumentNullException"><paramref name="encoding"/>がnull</exception>
            internal FileOrConsoleReaderValueConverter(Encoding encoding)
            {
                ThrowHelpers.ThrowIfNull(encoding);

                this.encoding = encoding;
            }

            /// <inheritdoc/>
            public TextReader Convert(string value)
            {
                if (value is null or "-") return Console.In;
                return new StreamReader(value, encoding);
            }
        }

        /// <summary>
        /// <see cref="StreamWriter"/>を生成する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        [Serializable]
        private sealed class StreamWriterValueConverter : IValueConverter<string, StreamWriter>
        {
            private readonly bool append;
            private readonly Encoding encoding;

            /// <summary>
            /// <see cref="StreamWriterValueConverter"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="encoding">エンコーディング</param>
            /// <param name="append">上書きせずに末尾に追加するかどうか</param>
            /// <exception cref="ArgumentNullException"><paramref name="encoding"/>がnull</exception>
            internal StreamWriterValueConverter(Encoding encoding, bool append)
            {
                ThrowHelpers.ThrowIfNull(encoding);

                this.encoding = encoding;
                this.append = append;
            }

            /// <inheritdoc/>
            public StreamWriter Convert(string value)
            {
                return new StreamWriter(value, append, encoding);
            }
        }

        /// <summary>
        /// <see cref="StreamReader"/>を生成する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        [Serializable]
        private sealed class StreamReaderValueConverter : IValueConverter<string, StreamReader>
        {
            private readonly Encoding encoding;

            /// <summary>
            /// <see cref="StreamReaderValueConverter"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="encoding">エンコーディング</param>
            /// <exception cref="ArgumentNullException"><paramref name="encoding"/>がnull</exception>
            internal StreamReaderValueConverter(Encoding encoding)
            {
                ThrowHelpers.ThrowIfNull(encoding);

                this.encoding = encoding;
            }

            /// <inheritdoc/>
            public StreamReader Convert(string value)
            {
                return new StreamReader(value, encoding);
            }
        }

        /// <summary>
        /// 配列を生成する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        [Serializable]
        private sealed class ArrayValueConverter : IValueConverter<string, Array>
        {
            private readonly IValueConverter<string, object?> elementConverter;
            private readonly Type elementType;
            private readonly string separator;
            private readonly StringSplitOptions splitOptions;

            /// <summary>
            /// <see cref="ArrayValueConverter{T}"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="elementType">要素の型</param>
            /// <param name="separator">区切り文字</param>
            /// <param name="converter">要素の変換を行う<see cref="IValueConverter{TIn, TOut}"/>のインスタンス</param>
            /// <param name="splitOptions">文字列分割時のオプション</param>
            internal ArrayValueConverter(string separator, Type elementType, IValueConverter<string, object?> converter, StringSplitOptions splitOptions)
            {
                ThrowHelpers.ThrowIfNull(elementType);
                ThrowHelpers.ThrowIfNull(converter);
                ThrowHelpers.ThrowIfNullOrEmpty(separator);

                this.separator = separator;
                this.elementType = elementType;
                elementConverter = converter;
                this.splitOptions = splitOptions;
            }

            /// <inheritdoc/>
            public Array Convert(string value)
            {
                ThrowHelpers.ThrowIfNull(value);

                if (value.Length == 0) return Array.CreateInstance(elementType, 0);

                string[] elements = value.Split(separator, splitOptions);
                Array result = Array.CreateInstance(elementType, elements.Length);
                for (int i = 0; i < elements.Length; i++) result.SetValue(elementConverter.Convert(elements[i]), i);
                return result;
            }
        }

        /// <summary>
        /// 配列を生成する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        /// <typeparam name="T">配列の要素の型</typeparam>
        [Serializable]
        private sealed class ArrayValueConverter<T> : IValueConverter<string, T[]>
        {
            private readonly IValueConverter<string, T> elementConverter;
            private readonly string separator;
            private readonly StringSplitOptions splitOptions;

            /// <summary>
            /// <see cref="ArrayValueConverter{T}"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="separator">区切り文字</param>
            /// <param name="converter">要素の変換を行う<see cref="IValueConverter{TIn, TOut}"/>のインスタンス</param>
            /// <param name="splitOptions">文字列分割時のオプション</param>
            internal ArrayValueConverter(string separator, IValueConverter<string, T> converter, StringSplitOptions splitOptions)
            {
                ThrowHelpers.ThrowIfNull(converter);
                ThrowHelpers.ThrowIfNullOrEmpty(separator);

                this.separator = separator;
                elementConverter = converter;
                this.splitOptions = splitOptions;
            }

            /// <inheritdoc/>
            public T[] Convert(string value)
            {
                ThrowHelpers.ThrowIfNull(value);

                if (value.Length == 0) return [];

                string[] elements = value.Split(separator, splitOptions);
                return Array.ConvertAll(elements, elementConverter.Convert);
            }
        }
    }
}
