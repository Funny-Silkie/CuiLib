using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace CuiLib.Converters
{
    /// <summary>
    /// 値の変換を扱います。
    /// </summary>
    public static partial class ValueConverter
    {
        /// <summary>
        /// <see cref="IValueConverter{TIn, TOut}"/>を結合します。
        /// </summary>
        /// <typeparam name="TIn">変換前の型</typeparam>
        /// <typeparam name="TMid">変換途上の型</typeparam>
        /// <typeparam name="TOut">変換後の型</typeparam>
        /// <param name="first">最初に変換を行う<see cref="IValueConverter{TIn, TOut}"/></param>
        /// <param name="second">次に変換を行う<see cref="IValueConverter{TIn, TOut}"/></param>
        /// <returns><typeparamref name="TIn"/>から<typeparamref name="TOut"/>へ変換を行う<see cref="IValueConverter{TIn, TOut}"/>のインスタンス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="first"/>または<paramref name="second"/>がnull</exception>
        public static IValueConverter<TIn, TOut> Combine<TIn, TMid, TOut>(this IValueConverter<TIn, TMid> first, IValueConverter<TMid, TOut> second)
        {
            return new CombinedValueConverter<TIn, TMid, TOut>(first, second);
        }

        /// <summary>
        /// デリゲートから<see cref="IValueConverter{TIn, TOut}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="converter">使用するデリゲート</param>
        /// <returns><paramref name="converter"/>で変換する<see cref="IValueConverter{TIn, TOut}"/>のインスタンス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/>がnull</exception>
        public static IValueConverter<TIn, TOut> FromDelegate<TIn, TOut>(Converter<TIn, TOut> converter)
        {
            return new DelegateValueConverter<TIn, TOut>(converter);
        }

        /// <summary>
        /// 文字列から<see cref="IParsable{TSelf}"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <typeparam name="TParsable"><see cref="IParsable{TSelf}"/>を実装する型</typeparam>
        /// <returns>文字列から<typeparamref name="TParsable"/>に変換するインスタンス</returns>
        public static IValueConverter<string, TParsable> StringToIParsable<TParsable>()
            where TParsable : IParsable<TParsable>
        {
            return new ParsableValueConverter<TParsable>();
        }

        /// <summary>
        /// 文字列から列挙型に変換するインスタンスを生成します。
        /// </summary>
        /// <typeparam name="TEnum">列挙型</typeparam>
        /// <returns>文字列から列挙型に変換するインスタンス</returns>
        public static IValueConverter<string, TEnum> StringToEnum<TEnum>()
            where TEnum : struct, Enum
        {
            return StringToEnum<TEnum>(false);
        }

        /// <summary>
        /// 文字列から列挙型に変換するインスタンスを生成します。
        /// </summary>
        /// <typeparam name="TEnum">列挙型</typeparam>
        /// <param name="ignoreCase">文字列の大文字小文字の区別を無視するかどうか</param>
        /// <returns>文字列から列挙型に変換するインスタンス</returns>
        public static IValueConverter<string, TEnum> StringToEnum<TEnum>(bool ignoreCase)
            where TEnum : struct, Enum
        {
            return new EnumValueConverter<TEnum>(ignoreCase);
        }

        /// <summary>
        /// 文字列から<see cref="FileInfo"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="FileInfo"/>に変換するインスタンス</returns>
        public static IValueConverter<string, FileInfo> StringToFileInfo()
        {
            return new FileInfoValueConverter();
        }

        /// <summary>
        /// 文字列から<see cref="DirectoryInfo"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="DirectoryInfo"/>に変換するインスタンス</returns>
        public static IValueConverter<string, DirectoryInfo> StringToDirectoryInfo()
        {
            return new DirectoryInfoValueConverter();
        }

        /// <summary>
        /// ファイルまたはコンソールウィンドウへ文字を出力する<see cref="TextWriter"/>を生成するインスタンスを生成します。
        /// </summary>
        /// <param name="encoding">エンコーディング</param>
        /// <param name="append">上書きせずに末尾に追加するかどうか</param>
        /// <returns>ファイルまたはコンソールウィンドウへ文字を出力する<see cref="TextWriter"/>を生成するインスタンス</returns>
        public static IValueConverter<string, TextWriter> ToConsoleOrFileWriter(Encoding? encoding = null, bool append = false)
        {
            return new FileOrConsoleWriterValueConverter(encoding ?? Encoding.UTF8, append);
        }

        /// <summary>
        /// ファイルまたはコンソールウィンドウから文字を読み取る<see cref="TextReader"/>を生成するインスタンスを生成します。
        /// </summary>
        /// <param name="encoding">エンコーディング</param>
        /// <returns>ファイルまたはコンソールウィンドウから文字を読み取る<see cref="TextReader"/>を生成するインスタンス</returns>
        public static IValueConverter<string, TextReader> ToConsoleOrFileReader(Encoding? encoding = null)
        {
            return new FileOrConsoleReaderValueConverter(encoding ?? Encoding.UTF8);
        }

        /// <summary>
        /// 文字列を分割して配列に変換するインスタンスを生成します。
        /// </summary>
        /// <param name="elementType">要素の型</param>
        /// <param name="separator">区切り文字</param>
        /// <param name="elementConverter">要素の変換を行う<see cref="IValueConverter{TIn, TOut}"/>のインスタンス</param>
        /// <param name="splitOptions">文字列分割時のオプション</param>
        /// <exception cref="ArgumentNullException"><paramref name="elementType"/>または<paramref name="separator"/>，<paramref name="elementConverter"/>の何れかが<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="separator"/>が空文字</exception>
        public static IValueConverter<string, Array> SplitToArray(Type elementType, string separator, IValueConverter<string, object?> elementConverter, StringSplitOptions splitOptions = StringSplitOptions.None)
        {
            return new ArrayValueConverter(separator, elementType, elementConverter, splitOptions);
        }

        /// <summary>
        /// 文字列を分割して配列に変換するインスタンスを生成します。
        /// </summary>
        /// <typeparam name="T">配列の要素の型</typeparam>
        /// <param name="separator">区切り文字</param>
        /// <param name="elementConverter">要素の変換を行う<see cref="IValueConverter{TIn, TOut}"/>のインスタンス</param>
        /// <param name="splitOptions">文字列分割時のオプション</param>
        /// <exception cref="ArgumentNullException"><paramref name="separator"/>または<paramref name="elementConverter"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="separator"/>が空文字</exception>
        public static IValueConverter<string, T[]> SplitToArray<T>(string separator, IValueConverter<string, T> elementConverter, StringSplitOptions splitOptions = StringSplitOptions.None)
        {
            return new ArrayValueConverter<T>(separator, elementConverter, splitOptions);
        }

        /// <summary>
        /// デフォルトのインスタンスを取得します。
        /// </summary>
        /// <typeparam name="T">変換先の型</typeparam>
        /// <returns>デフォルトのインスタンス</returns>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/>が無効</exception>
        public static IValueConverter<string, T> GetDefault<T>()
        {
            return Unsafe.As<IValueConverter<string, T>>(GetDefault(typeof(T)));
        }

        /// <summary>
        /// デフォルトのインスタンスを取得します。
        /// </summary>
        /// <param name="type">変換先の型</param>
        /// <returns>デフォルトのインスタンス</returns>
        /// <exception cref="NotSupportedException"><paramref name="type"/>が無効</exception>
        private static IValueConverter<string, object?> GetDefault(Type type)
        {
            if (type == typeof(string)) return Cast(new ThroughValueConverter<string>());
            if (type.IsSZArray)
            {
                Type elementType = type.GetElementType()!;
                Type converterType = typeof(ArrayValueConverter<>).MakeGenericType(elementType);

                var ctorArgTypes = new[] { typeof(string), typeof(IValueConverter<,>).MakeGenericType(typeof(string), elementType), typeof(StringSplitOptions) };
                ConstructorInfo ctor = converterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, ctorArgTypes) ?? throw new InvalidOperationException();

                return Cast(ctor.Invoke([",", GetDefault(elementType), StringSplitOptions.None]));
            }

            if (type == typeof(FileInfo)) return Cast(new FileInfoValueConverter());
            if (type == typeof(DirectoryInfo)) return Cast(new DirectoryInfoValueConverter());
            if (type == typeof(TextReader)) return Cast(new FileOrConsoleReaderValueConverter(new UTF8Encoding(false)));
            if (type == typeof(StreamReader)) return Cast(new StreamReaderValueConverter(new UTF8Encoding(false)));
            if (type == typeof(TextWriter)) return Cast(new FileOrConsoleWriterValueConverter(new UTF8Encoding(false), false));
            if (type == typeof(StreamWriter)) return Cast(new StreamWriterValueConverter(new UTF8Encoding(false), false));
            if (type == typeof(int)) return Cast(new ParsableValueConverter<int>());
            if (type == typeof(double)) return Cast(new ParsableValueConverter<double>());
            if (type == typeof(DateTime)) return Cast(new ParsableValueConverter<DateTime>());
            if (type == typeof(long)) return Cast(new ParsableValueConverter<long>());
            if (type == typeof(ulong)) return Cast(new ParsableValueConverter<ulong>());
            if (type == typeof(TimeSpan)) return Cast(new ParsableValueConverter<TimeSpan>());
            if (type == typeof(DateOnly)) return Cast(new ParsableValueConverter<DateOnly>());
            if (type == typeof(TimeOnly)) return Cast(new ParsableValueConverter<TimeOnly>());
            if (type == typeof(char)) return Cast(new ParsableValueConverter<char>());
            if (type == typeof(float)) return Cast(new ParsableValueConverter<float>());
            if (type == typeof(decimal)) return Cast(new ParsableValueConverter<decimal>());
            if (type == typeof(uint)) return Cast(new ParsableValueConverter<uint>());
            if (type == typeof(sbyte)) return Cast(new ParsableValueConverter<sbyte>());
            if (type == typeof(byte)) return Cast(new ParsableValueConverter<byte>());
            if (type == typeof(short)) return Cast(new ParsableValueConverter<short>());
            if (type == typeof(ushort)) return Cast(new ParsableValueConverter<ushort>());
            if (type.IsEnum)
            {
                Type converterType = typeof(EnumValueConverter<>).MakeGenericType([type]);
                ConstructorInfo ctor = converterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, [typeof(bool)]) ?? throw new InvalidOperationException();
                return Cast(ctor.Invoke(parameters: [false]));
            }

            if (type == typeof(DateTimeOffset)) return Cast(new ParsableValueConverter<DateTimeOffset>());

            throw new NotSupportedException();

            static IValueConverter<string, object?> Cast(object converter)
            {
                return Unsafe.As<IValueConverter<string, object?>>(converter);
            }
        }

        /// <summary>
        /// 値の種類を表す文字列を取得します。
        /// </summary>
        /// <typeparam name="T">値の型</typeparam>
        /// <returns><typeparamref name="T"/>に対応する文字列</returns>
        public static string GetValueTypeString<T>()
        {
            return GetValueTypeString(typeof(T));
        }

        /// <summary>
        /// 値の種類を表す文字列を取得します。
        /// </summary>
        /// <param name="type">値の型</param>
        /// <returns><paramref name="type"/>に対応する文字列</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetValueTypeString(Type type)
        {
            if (type == typeof(string)) return "string";
            if (type.IsSZArray) return $"{GetValueTypeString(type.GetElementType()!)}[]";
            if (type == typeof(FileInfo)) return "file";
            if (type == typeof(DirectoryInfo)) return "directory";
            if (type == typeof(int)) return "int";
            if (type == typeof(TextReader)) return "file";
            if (type == typeof(TextWriter)) return "file";
            if (type == typeof(sbyte)) return "int";
            if (type == typeof(double)) return "float";
            if (type == typeof(long)) return "long";
            if (type == typeof(ulong)) return "ulong";
            if (type == typeof(DateTime)) return "date time";
            if (type == typeof(short)) return "int";
            if (type == typeof(byte)) return "int";
            if (type == typeof(ushort)) return "int";
            if (type == typeof(uint)) return "uint";
            if (type == typeof(float)) return "float";
            if (type == typeof(decimal)) return "decimal";
            if (type == typeof(char)) return "char";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(TimeSpan)) return "date time";
            if (type == typeof(DateOnly)) return "date";
            if (type == typeof(TimeOnly)) return "time";
            if (type.IsEnum) return "string";
            return "string";
        }
    }
}
