using System;
using System.IO;
using System.Text;

namespace CuiLib.Options
{
    public static partial class ValueConverter
    {
        /// <summary>
        /// デリゲートで変換を行う<see cref="ValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        /// <typeparam name="TIn">変換前の型</typeparam>
        /// <typeparam name="TOut">変換後の型</typeparam>
        [Serializable]
        private sealed class DelegateValueConverter<TIn, TOut> : ValueConverter<TIn, TOut>
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
            public override TOut Convert(TIn value)
            {
                return converter.Invoke(value);
            }
        }

        /// <summary>
        /// 値の素通しを行う<see cref="ValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        private sealed class ThroughValueConverter : ValueConverter<string, string>
        {
            /// <summary>
            /// <see cref="ThroughValueConverter"/>の新しいインスタンスを初期化します。
            /// </summary>
            internal ThroughValueConverter()
            {
            }

            /// <inheritdoc/>
            public override string Convert(string value)
            {
                return value;
            }

            /// <inheritdoc/>
            public override bool Equals(object? obj) => obj is ThroughValueConverter;

            /// <inheritdoc/>
            public override int GetHashCode() => GetType().GetHashCode();
        }

        /// <summary>
        /// <see cref="IParsable{TSelf}"/>を変換する<see cref="ValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        /// <typeparam name="T">変換する<see cref="IParsable{TSelf}"/>を実装する型</typeparam>
        [Serializable]
        private sealed class ParsableValueConverter<T> : ValueConverter<string, T>
            where T : IParsable<T>
        {
            /// <summary>
            /// <see cref="ParsableValueConverter{T}"/>の新しいインスタンスを初期化します。
            /// </summary>
            internal ParsableValueConverter()
            {
            }

            /// <inheritdoc/>
            public override T Convert(string value)
            {
                return T.Parse(value, null);
            }

            /// <inheritdoc/>
            public override bool Equals(object? obj) => obj is ParsableValueConverter<T>;

            /// <inheritdoc/>
            public override int GetHashCode() => GetType().GetHashCode();
        }

        /// <summary>
        /// 列挙型の変換を行う<see cref="ValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        [Serializable]
        private sealed class EnumValueConverter : ValueConverter<string, Enum>
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
            public override Enum Convert(string value)
            {
                return (Enum)Enum.Parse(enumType, value);
            }
        }

        /// <summary>
        /// 列挙型の変換を行う<see cref="ValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        /// <typeparam name="T">列挙型の型</typeparam>
        [Serializable]
        private sealed class EnumValueConverter<T> : ValueConverter<string, T>
            where T : struct, Enum
        {
            /// <summary>
            /// <see cref="EnumValueConverter{T}"/>の新しいインスタンスを初期化します。
            /// </summary>
            internal EnumValueConverter()
            {
            }

            /// <inheritdoc/>
            public override T Convert(string value)
            {
                return Enum.Parse<T>(value);
            }

            /// <inheritdoc/>
            public override bool Equals(object? obj) => obj is EnumValueConverter<T>;

            /// <inheritdoc/>
            public override int GetHashCode() => GetType().GetHashCode();
        }

        /// <summary>
        /// <see cref="FileInfo"/>を生成する<see cref="ValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        [Serializable]
        private sealed class FileInfoValueConverter : ValueConverter<string, FileInfo>
        {
            /// <summary>
            /// <see cref="FileInfoValueConverter"/>の新しいインスタンスを初期化します。
            /// </summary>
            internal FileInfoValueConverter()
            {
            }

            /// <inheritdoc/>
            public override FileInfo Convert(string value)
            {
                return new FileInfo(value);
            }

            /// <inheritdoc/>
            public override bool Equals(object? obj) => obj is FileInfoValueConverter;

            /// <inheritdoc/>
            public override int GetHashCode() => GetType().GetHashCode();
        }

        /// <summary>
        /// <see cref="DirectoryInfo"/>を生成する<see cref="ValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        [Serializable]
        private sealed class DirectoryInfoValueConverter : ValueConverter<string, DirectoryInfo>
        {
            /// <summary>
            /// <see cref="DirectoryInfoValueConverter"/>の新しいインスタンスを初期化します。
            /// </summary>
            internal DirectoryInfoValueConverter()
            {
            }

            /// <inheritdoc/>
            public override DirectoryInfo Convert(string value)
            {
                return new DirectoryInfo(value);
            }

            /// <inheritdoc/>
            public override bool Equals(object? obj) => obj is DirectoryInfoValueConverter;

            /// <inheritdoc/>
            public override int GetHashCode() => GetType().GetHashCode();
        }

        /// <summary>
        /// <see cref="TextWriter"/>を生成する<see cref="ValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        [Serializable]
        private sealed class FileOrConsoleWriterValueConverter : ValueConverter<string?, TextWriter>
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
            public override TextWriter Convert(string? value)
            {
                if (value is null) return Console.Out;
                return new StreamWriter(value, append, encoding);
            }
        }

        /// <summary>
        /// <see cref="TextReader"/>を生成する<see cref="ValueConverter{TIn, TOut}"/>のクラスです。
        /// </summary>
        [Serializable]
        private sealed class FileOrConsoleReaderValueConverter : ValueConverter<string?, TextReader>
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
            public override TextReader Convert(string? value)
            {
                if (value is null) return Console.In;
                return new StreamReader(value, encoding);
            }
        }
    }
}
