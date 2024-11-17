using CuiLib.Converters.Implementations;
using CuiLib.Data;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace CuiLib.Converters
{
    public static partial class ValueConverter
    {
#if NET7_0_OR_GREATER

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

#endif

        /// <summary>
        /// 文字列から<see cref="sbyte"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="sbyte"/>に変換するインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IValueConverter<string, sbyte> StringToSByte()
        {
#if NET7_0_OR_GREATER
            return new ParsableValueConverter<sbyte>();
#else
            return FromDelegate<string, sbyte>(sbyte.Parse);
#endif
        }

        /// <summary>
        /// 文字列から<see cref="byte"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="byte"/>に変換するインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IValueConverter<string, byte> StringToByte()
        {
#if NET7_0_OR_GREATER
            return new ParsableValueConverter<byte>();
#else
            return FromDelegate<string, byte>(byte.Parse);
#endif
        }

        /// <summary>
        /// 文字列から<see cref="short"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="short"/>に変換するインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IValueConverter<string, short> StringToInt16()
        {
#if NET7_0_OR_GREATER
            return new ParsableValueConverter<short>();
#else
            return FromDelegate<string, short>(short.Parse);
#endif
        }

        /// <summary>
        /// 文字列から<see cref="ushort"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="ushort"/>に変換するインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IValueConverter<string, ushort> StringToUInt16()
        {
#if NET7_0_OR_GREATER
            return new ParsableValueConverter<ushort>();
#else
            return FromDelegate<string, ushort>(ushort.Parse);
#endif
        }

        /// <summary>
        /// 文字列から<see cref="int"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="int"/>に変換するインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IValueConverter<string, int> StringToInt32()
        {
#if NET7_0_OR_GREATER
            return new ParsableValueConverter<int>();
#else
            return FromDelegate<string, int>(int.Parse);
#endif
        }

        /// <summary>
        /// 文字列から<see cref="uint"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="uint"/>に変換するインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IValueConverter<string, uint> StringToUInt32()
        {
#if NET7_0_OR_GREATER
            return new ParsableValueConverter<uint>();
#else
            return FromDelegate<string, uint>(uint.Parse);
#endif
        }

        /// <summary>
        /// 文字列から<see cref="long"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="long"/>に変換するインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IValueConverter<string, long> StringToInt64()
        {
#if NET7_0_OR_GREATER
            return new ParsableValueConverter<long>();
#else
            return FromDelegate<string, long>(long.Parse);
#endif
        }

        /// <summary>
        /// 文字列から<see cref="ulong"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="ulong"/>に変換するインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IValueConverter<string, ulong> StringToUInt64()
        {
#if NET7_0_OR_GREATER
            return new ParsableValueConverter<ulong>();
#else
            return FromDelegate<string, ulong>(ulong.Parse);
#endif
        }

#if NET7_0_OR_GREATER

        /// <summary>
        /// 文字列から<see cref="long"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="long"/>に変換するインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IValueConverter<string, Int128> StringToInt128()
        {
            return new ParsableValueConverter<Int128>();
        }

        /// <summary>
        /// 文字列から<see cref="UInt128"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="UInt128"/>に変換するインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IValueConverter<string, UInt128> StringToUInt128()
        {
            return new ParsableValueConverter<UInt128>();
        }

#endif

        /// <summary>
        /// 文字列から<see cref="float"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="float"/>に変換するインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IValueConverter<string, float> StringToSingle()
        {
#if NET7_0_OR_GREATER
            return new ParsableValueConverter<float>();
#else
            return FromDelegate<string, float>(float.Parse);
#endif
        }

        /// <summary>
        /// 文字列から<see cref="double"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="double"/>に変換するインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IValueConverter<string, double> StringToDouble()
        {
#if NET7_0_OR_GREATER
            return new ParsableValueConverter<double>();
#else
            return FromDelegate<string, double>(double.Parse);
#endif
        }

        /// <summary>
        /// 文字列から<see cref="decimal"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="decimal"/>に変換するインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IValueConverter<string, decimal> StringToDecimal()
        {
#if NET7_0_OR_GREATER
            return new ParsableValueConverter<decimal>();
#else
            return FromDelegate<string, decimal>(decimal.Parse);
#endif
        }

        /// <summary>
        /// 文字列から<see cref="char"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="char"/>に変換するインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IValueConverter<string, char> StringToChar()
        {
#if NET7_0_OR_GREATER
            return new ParsableValueConverter<char>();
#else
            return FromDelegate<string, char>(char.Parse);
#endif
        }

        /// <summary>
        /// 文字列から<see cref="DateTime"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="DateTime"/>に変換するインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IValueConverter<string, DateTime> StringToDateTime()
        {
#if NET7_0_OR_GREATER
            return new ParsableValueConverter<DateTime>();
#else
            return FromDelegate<string, DateTime>(DateTime.Parse);
#endif
        }

        /// <summary>
        /// 文字列から<see cref="DateTime"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="DateTime"/>に変換するインスタンス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
        public static IValueConverter<string, DateTime> StringToDateTime([StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format)
        {
            return new DateTimeExactConverter(format);
        }

#if NET6_0_OR_GREATER

        /// <summary>
        /// 文字列から<see cref="DateOnly"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="DateOnly"/>に変換するインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IValueConverter<string, DateOnly> StringToDateOnly()
        {
#if NET7_0_OR_GREATER
            return new ParsableValueConverter<DateOnly>();
#else
            return FromDelegate<string, DateOnly>(DateOnly.Parse);
#endif
        }

        /// <summary>
        /// 文字列から<see cref="DateOnly"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="DateOnly"/>に変換するインスタンス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
        public static IValueConverter<string, DateOnly> StringToDateOnly([StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string format)
        {
            return new DateOnlyExactConverter(format);
        }

        /// <summary>
        /// 文字列から<see cref="TimeOnly"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="TimeOnly"/>に変換するインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IValueConverter<string, TimeOnly> StringToTimeOnly()
        {
#if NET7_0_OR_GREATER
            return new ParsableValueConverter<TimeOnly>();
#else
            return FromDelegate<string, TimeOnly>(TimeOnly.Parse);
#endif
        }

        /// <summary>
        /// 文字列から<see cref="TimeOnly"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="TimeOnly"/>に変換するインスタンス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
        public static IValueConverter<string, TimeOnly> StringToTimeOnly([StringSyntax(StringSyntaxAttribute.TimeOnlyFormat)] string format)
        {
            return new TimeOnlyExactConverter(format);
        }

#endif

        /// <summary>
        /// 文字列から<see cref="TimeSpan"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="TimeSpan"/>に変換するインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IValueConverter<string, TimeSpan> StringToTimeSpan()
        {
#if NET7_0_OR_GREATER
            return new ParsableValueConverter<TimeSpan>();
#else
            return FromDelegate<string, TimeSpan>(TimeSpan.Parse);
#endif
        }

        /// <summary>
        /// 文字列から<see cref="TimeSpan"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="TimeSpan"/>に変換するインスタンス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
        public static IValueConverter<string, TimeSpan> StringToTimeSpan([StringSyntax(StringSyntaxAttribute.TimeSpanFormat)] string format)
        {
            return new TimeSpanExactConverter(format);
        }

        /// <summary>
        /// 文字列から<see cref="DateTimeOffset"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="DateTimeOffset"/>に変換するインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IValueConverter<string, DateTimeOffset> StringToDateTimeOffset()
        {
#if NET7_0_OR_GREATER
            return new ParsableValueConverter<DateTimeOffset>();
#else
            return FromDelegate<string, DateTimeOffset>(DateTimeOffset.Parse);
#endif
        }

        /// <summary>
        /// 文字列から<see cref="DateTimeOffset"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="DateTimeOffset"/>に変換するインスタンス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="format"/>が空文字</exception>
        public static IValueConverter<string, DateTimeOffset> StringToDateTimeOffset([StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format)
        {
            return new DateTimeOffsetExactConverter(format);
        }

        /// <summary>
        /// 文字列から<see cref="ValueRange"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="ValueRange"/>に変換するインスタンス</returns>
        public static IValueConverter<string, ValueRange> StringToValueRange()
        {
#if NET7_0_OR_GREATER
            return new ParsableValueConverter<ValueRange>();
#else
            return FromDelegate<string, ValueRange>(ValueRange.Parse);
#endif
        }

        /// <summary>
        /// 文字列から<see cref="ValueRangeCollection"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>文字列から<see cref="ValueRangeCollection"/>に変換するインスタンス</returns>
        public static IValueConverter<string, ValueRangeCollection> StringToValueRangeCollection()
        {
#if NET7_0_OR_GREATER
            return new ParsableValueConverter<ValueRangeCollection>();
#else
            return FromDelegate<string, ValueRangeCollection>(ValueRangeCollection.Parse);
#endif
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
        /// 検索パターンから<see cref="FileInfo"/>一覧に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>ファイル検索パターンから<see cref="FileInfo"/>一覧に変換するインスタンス</returns>
        public static IValueConverter<string, FileInfo[]> StringToFileInfos()
        {
            return new FilePatternValueConverter();
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
        /// 検索パターンから<see cref="DirectoryInfo"/>一覧に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>検索パターンから<see cref="DirectoryInfo"/>一覧に変換するインスタンス</returns>
        public static IValueConverter<string, DirectoryInfo[]> StringToDirectoryInfos()
        {
            return new DirectoryPatternValueConverter();
        }

        /// <summary>
        /// 検索パターンから<see cref="FileSystemInfo"/>一覧に変換するインスタンスを生成します。
        /// </summary>
        /// <returns>ファイル検索パターンから<see cref="FileSystemInfo"/>一覧に変換するインスタンス</returns>
        public static IValueConverter<string, FileSystemInfo[]> StringToFileSystemInfos()
        {
            return new FileSystemPatternValueConverter();
        }

        /// <summary>
        /// 文字列から<see cref="StreamWriter"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <param name="encoding">エンコーディング</param>
        /// <param name="append">上書きせずに末尾に追加するかどうか</param>
        /// <returns>文字列から<see cref="StreamWriter"/>に変換するインスタンス</returns>
        public static IValueConverter<string, StreamWriter> StringToStreamWriter(Encoding? encoding = null, bool append = false)
        {
            return new StreamWriterValueConverter(encoding ?? IOHelpers.UTF8N, append);
        }

        /// <summary>
        /// 文字列から<see cref="StreamReader"/>に変換するインスタンスを生成します。
        /// </summary>
        /// <param name="encoding">エンコーディング</param>
        /// <returns>文字列から<see cref="StreamReader"/>に変換するインスタンス</returns>
        public static IValueConverter<string, StreamReader> StringToStreamReader(Encoding? encoding = null)
        {
            return new StreamReaderValueConverter(encoding ?? IOHelpers.UTF8N);
        }

        /// <summary>
        /// ファイルまたはコンソールウィンドウへ文字を出力する<see cref="TextWriter"/>を生成するインスタンスを生成します。
        /// </summary>
        /// <param name="encoding">エンコーディング</param>
        /// <param name="append">上書きせずに末尾に追加するかどうか</param>
        /// <returns>ファイルまたはコンソールウィンドウへ文字を出力する<see cref="TextWriter"/>を生成するインスタンス</returns>
        public static IValueConverter<string, TextWriter> ToConsoleOrFileWriter(Encoding? encoding = null, bool append = false)
        {
            return new FileOrConsoleWriterValueConverter(encoding ?? IOHelpers.UTF8N, append);
        }

        /// <summary>
        /// ファイルまたはコンソールウィンドウから文字を読み取る<see cref="TextReader"/>を生成するインスタンスを生成します。
        /// </summary>
        /// <param name="encoding">エンコーディング</param>
        /// <returns>ファイルまたはコンソールウィンドウから文字を読み取る<see cref="TextReader"/>を生成するインスタンス</returns>
        public static IValueConverter<string, TextReader> ToConsoleOrFileReader(Encoding? encoding = null)
        {
            return new FileOrConsoleReaderValueConverter(encoding ?? IOHelpers.UTF8N);
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
    }
}
