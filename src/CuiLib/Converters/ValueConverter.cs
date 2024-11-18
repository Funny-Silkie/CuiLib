using CuiLib.Converters.Implementations;
using CuiLib.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

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
        /// <see cref="IValueConverter{TIn, TOut}"/>を結合します。
        /// </summary>
        /// <typeparam name="TIn">変換前の型</typeparam>
        /// <typeparam name="TMid">変換途上の型</typeparam>
        /// <typeparam name="TOut">変換後の型</typeparam>
        /// <param name="first">最初に変換を行う<see cref="IValueConverter{TIn, TOut}"/></param>
        /// <param name="secondConverter">次に変換を行う<see cref="IValueConverter{TIn, TOut}"/>を表す変換関数</param>
        /// <returns><typeparamref name="TIn"/>から<typeparamref name="TOut"/>へ変換を行う<see cref="IValueConverter{TIn, TOut}"/>のインスタンス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="first"/>または<paramref name="secondConverter"/>がnull</exception>
        public static IValueConverter<TIn, TOut> Combine<TIn, TMid, TOut>(this IValueConverter<TIn, TMid> first, Converter<TMid, TOut> secondConverter)
        {
            ThrowHelpers.ThrowIfNull(first);

            return new CombinedValueConverter<TIn, TMid, TOut>(first, FromDelegate(secondConverter));
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
            if (type == typeof(FileInfo[]) || type == typeof(IEnumerable<FileInfo>)) return Cast(new FilePatternValueConverter());
            if (type == typeof(DirectoryInfo[]) || type == typeof(IEnumerable<DirectoryInfo>)) return Cast(new DirectoryPatternValueConverter());
            if (type == typeof(FileSystemInfo[]) || type == typeof(IEnumerable<FileSystemInfo>)) return Cast(new FileSystemPatternValueConverter());

#if NETSTANDARD2_1_OR_GREATER || NET
            if (type.IsSZArray)
#else
            if (type.IsArray && type.GetArrayRank() == 1)
#endif
            {
                Type elementType = type.GetElementType()!;
                Type converterType = typeof(ArrayValueConverter<>).MakeGenericType(elementType);

                var ctorArgTypes = new[] { typeof(string), typeof(IValueConverter<,>).MakeGenericType(typeof(string), elementType), typeof(StringSplitOptions) };
                ConstructorInfo ctor = converterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, binder: null, ctorArgTypes, modifiers: null) ?? throw new InvalidOperationException();

                return Cast(ctor.Invoke([",", GetDefault(elementType), StringSplitOptions.None]));
            }

            if (type == typeof(FileInfo)) return Cast(new FileInfoValueConverter());
            if (type == typeof(DirectoryInfo)) return Cast(new DirectoryInfoValueConverter());
            if (type == typeof(TextReader)) return Cast(new FileOrConsoleReaderValueConverter(IOHelpers.UTF8N));
            if (type == typeof(StreamReader)) return Cast(new StreamReaderValueConverter(IOHelpers.UTF8N));
            if (type == typeof(TextWriter)) return Cast(new FileOrConsoleWriterValueConverter(IOHelpers.UTF8N, false));
            if (type == typeof(StreamWriter)) return Cast(new StreamWriterValueConverter(IOHelpers.UTF8N, false));
            if (type == typeof(int)) return Cast(StringToInt32());
            if (type == typeof(double)) return Cast(StringToDouble());
            if (type == typeof(DateTime)) return Cast(StringToDateTime());
            if (type == typeof(long)) return Cast(StringToInt64());
            if (type == typeof(ulong)) return Cast(StringToUInt64());
            if (type == typeof(TimeSpan)) return Cast(StringToTimeSpan());
#if NET6_0_OR_GREATER
            if (type == typeof(DateOnly)) return Cast(StringToDateOnly());
            if (type == typeof(TimeOnly)) return Cast(StringToTimeOnly());
#endif
            if (type == typeof(ValueRange)) return Cast(StringToValueRange());
            if (type == typeof(ValueRangeCollection)) return Cast(StringToValueRangeCollection());
            if (type == typeof(char)) return Cast(StringToChar());
            if (type == typeof(float)) return Cast(StringToSingle());
            if (type == typeof(decimal)) return Cast(StringToDecimal());
            if (type == typeof(uint)) return Cast(StringToUInt32());
            if (type == typeof(sbyte)) return Cast(StringToSByte());
            if (type == typeof(byte)) return Cast(StringToByte());
            if (type == typeof(short)) return Cast(StringToInt16());
            if (type == typeof(ushort)) return Cast(StringToUInt16());
            if (type.IsEnum)
            {
                Type converterType = typeof(EnumValueConverter<>).MakeGenericType([type]);
                ConstructorInfo ctor = converterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, binder: null, [typeof(bool)], modifiers: null) ?? throw new InvalidOperationException();
                return Cast(ctor.Invoke(parameters: [false]));
            }

            if (type == typeof(DateTimeOffset)) return Cast(StringToDateTimeOffset());
#if NET7_0_OR_GREATER
            if (type == typeof(Int128)) return Cast(StringToInt128());
            if (type == typeof(UInt128)) return Cast(StringToUInt128());
#endif

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
#if NETSTANDARD2_1_OR_GREATER || NET
            if (type.IsSZArray)
#else
            if (type.IsArray && type.GetArrayRank() == 1)
#endif
            {
                return $"{GetValueTypeString(type.GetElementType()!)}[]";
            }
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
#if NET6_0_OR_GREATER
            if (type == typeof(DateOnly)) return "date";
            if (type == typeof(TimeOnly)) return "time";
#endif
            if (type.IsEnum) return "string";
            return "string";
        }
    }
}
