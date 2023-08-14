using System;
using System.IO;
using System.Text;

namespace CuiLib.Options
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
                ArgumentNullException.ThrowIfNull(converter);

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

        /// <summary>
        /// 列挙型の変換を行う<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        [Serializable]
        private sealed class EnumValueConverter : IValueConverter<string, Enum>
        {
            private readonly Type enumType;

            /// <summary>
            /// <see cref="EnumValueConverter"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="enumType">列挙型の型</param>
            /// <exception cref="ArgumentNullException"><paramref name="enumType"/>がnull</exception>
            internal EnumValueConverter(Type enumType)
            {
                ArgumentNullException.ThrowIfNull(enumType);
                this.enumType = enumType;
            }

            /// <inheritdoc/>
            public Enum Convert(string value)
            {
                return (Enum)Enum.Parse(enumType, value);
            }
        }

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
                ArgumentNullException.ThrowIfNull(encoding);

                this.encoding = encoding;
                this.append = append;
            }

            /// <inheritdoc/>
            public TextWriter Convert(string value)
            {
                if (value is null) return Console.Out;
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
                ArgumentNullException.ThrowIfNull(encoding);

                this.encoding = encoding;
            }

            /// <inheritdoc/>
            public TextReader Convert(string value)
            {
                if (value is null) return Console.In;
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
                ArgumentNullException.ThrowIfNull(encoding);

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
                ArgumentNullException.ThrowIfNull(encoding);

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
                ArgumentNullException.ThrowIfNull(elementType);
                ArgumentNullException.ThrowIfNull(converter);
                ArgumentException.ThrowIfNullOrEmpty(separator);

                this.separator = separator;
                this.elementType = elementType;
                elementConverter = converter;
                this.splitOptions = splitOptions;
            }

            /// <inheritdoc/>
            public Array Convert(string value)
            {
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
                ArgumentNullException.ThrowIfNull(converter);
                ArgumentException.ThrowIfNullOrEmpty(separator);

                this.separator = separator;
                elementConverter = converter;
                this.splitOptions = splitOptions;
            }

            /// <inheritdoc/>
            public T[] Convert(string value)
            {
                if (value.Length == 0) return Array.Empty<T>();
                string[] elements = value.Split(separator, splitOptions);
                return Array.ConvertAll(elements, elementConverter.Convert);
            }
        }
    }
}
