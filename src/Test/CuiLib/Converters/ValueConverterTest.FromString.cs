using CuiLib;
using CuiLib.Converters;
using CuiLib.Converters.Implementations;
using CuiLib.Data;
using CuiLib.Options;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace Test.CuiLib.Converters
{
    public partial class ValueConverterTest
    {
#if NET7_0_OR_GREATER

        [Test]
        public void StringToIParsable()
        {
            IValueConverter<string, int> converter = ValueConverter.StringToIParsable<int>();

            Assert.That(converter, Is.TypeOf<ParsableValueConverter<int>>());
        }

#endif

        [Test]
        public void StringToSByte()
        {
            IValueConverter<string, sbyte> converter = ValueConverter.StringToSByte();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("0"), Is.EqualTo(0));
                Assert.That(converter.Convert("1"), Is.EqualTo(1));
                Assert.That(converter.Convert("-1"), Is.EqualTo(-1));
                Assert.That(converter.Convert(sbyte.MaxValue.ToString()), Is.EqualTo(sbyte.MaxValue));
                Assert.That(converter.Convert(sbyte.MinValue.ToString()), Is.EqualTo(sbyte.MinValue));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void StringToInt16()
        {
            IValueConverter<string, short> converter = ValueConverter.StringToInt16();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("0"), Is.EqualTo(0));
                Assert.That(converter.Convert("1"), Is.EqualTo(1));
                Assert.That(converter.Convert("-1"), Is.EqualTo(-1));
                Assert.That(converter.Convert(short.MaxValue.ToString()), Is.EqualTo(short.MaxValue));
                Assert.That(converter.Convert(short.MinValue.ToString()), Is.EqualTo(short.MinValue));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void StringToInt32()
        {
            IValueConverter<string, int> converter = ValueConverter.StringToInt32();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("0"), Is.EqualTo(0));
                Assert.That(converter.Convert("1"), Is.EqualTo(1));
                Assert.That(converter.Convert("-1"), Is.EqualTo(-1));
                Assert.That(converter.Convert(int.MaxValue.ToString()), Is.EqualTo(int.MaxValue));
                Assert.That(converter.Convert(int.MinValue.ToString()), Is.EqualTo(int.MinValue));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void StringToInt64()
        {
            IValueConverter<string, long> converter = ValueConverter.StringToInt64();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("0"), Is.EqualTo(0));
                Assert.That(converter.Convert("1"), Is.EqualTo(1));
                Assert.That(converter.Convert("-1"), Is.EqualTo(-1));
                Assert.That(converter.Convert(long.MaxValue.ToString()), Is.EqualTo(long.MaxValue));
                Assert.That(converter.Convert(long.MinValue.ToString()), Is.EqualTo(long.MinValue));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

#if NET7_0_OR_GREATER

        [Test]
        public void StringToInt128()
        {
            IValueConverter<string, Int128> converter = ValueConverter.StringToInt128();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("0"), Is.EqualTo((Int128)0));
                Assert.That(converter.Convert("1"), Is.EqualTo((Int128)1));
                Assert.That(converter.Convert("-1"), Is.EqualTo((Int128)(-1)));
                Assert.That(converter.Convert(Int128.MaxValue.ToString()), Is.EqualTo(Int128.MaxValue));
                Assert.That(converter.Convert(Int128.MinValue.ToString()), Is.EqualTo(Int128.MinValue));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

#endif

        [Test]
        public void StringToByte()
        {
            IValueConverter<string, byte> converter = ValueConverter.StringToByte();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("0"), Is.EqualTo(0));
                Assert.That(converter.Convert("1"), Is.EqualTo(1));
                Assert.That(converter.Convert(byte.MaxValue.ToString()), Is.EqualTo(byte.MaxValue));
                Assert.That(converter.Convert(byte.MinValue.ToString()), Is.EqualTo(byte.MinValue));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void StringToUInt16()
        {
            IValueConverter<string, ushort> converter = ValueConverter.StringToUInt16();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("0"), Is.EqualTo(0));
                Assert.That(converter.Convert("1"), Is.EqualTo(1));
                Assert.That(converter.Convert(ushort.MaxValue.ToString()), Is.EqualTo(ushort.MaxValue));
                Assert.That(converter.Convert(ushort.MinValue.ToString()), Is.EqualTo(ushort.MinValue));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void StringToUInt32()
        {
            IValueConverter<string, uint> converter = ValueConverter.StringToUInt32();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("0"), Is.EqualTo(0));
                Assert.That(converter.Convert("1"), Is.EqualTo(1));
                Assert.That(converter.Convert(uint.MaxValue.ToString()), Is.EqualTo(uint.MaxValue));
                Assert.That(converter.Convert(uint.MinValue.ToString()), Is.EqualTo(uint.MinValue));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void StringToUInt64()
        {
            IValueConverter<string, ulong> converter = ValueConverter.StringToUInt64();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("0"), Is.EqualTo(0));
                Assert.That(converter.Convert("1"), Is.EqualTo(1));
                Assert.That(converter.Convert(ulong.MaxValue.ToString()), Is.EqualTo(ulong.MaxValue));
                Assert.That(converter.Convert(ulong.MinValue.ToString()), Is.EqualTo(ulong.MinValue));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

#if NET7_0_OR_GREATER

        [Test]
        public void StringToUInt128()
        {
            IValueConverter<string, UInt128> converter = ValueConverter.StringToUInt128();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("0"), Is.EqualTo((UInt128)0));
                Assert.That(converter.Convert("1"), Is.EqualTo((UInt128)1));
                Assert.That(converter.Convert(UInt128.MaxValue.ToString()), Is.EqualTo(UInt128.MaxValue));
                Assert.That(converter.Convert(UInt128.MinValue.ToString()), Is.EqualTo(UInt128.MinValue));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

#endif

        [Test]
        public void StringToSingle()
        {
            IValueConverter<string, float> converter = ValueConverter.StringToSingle();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("0"), Is.EqualTo(0));
                Assert.That(converter.Convert("1"), Is.EqualTo(1));
                Assert.That(converter.Convert("-1"), Is.EqualTo(-1));
                Assert.That(converter.Convert("NaN"), Is.EqualTo(float.NaN));
                Assert.That(converter.Convert(float.PositiveInfinity.ToString()), Is.EqualTo(float.PositiveInfinity));
                Assert.That(converter.Convert(float.NegativeInfinity.ToString()), Is.EqualTo(float.NegativeInfinity));
                Assert.That(converter.Convert("1.234E+30"), Is.EqualTo(1.234E+30f));
                Assert.That(converter.Convert("-1.234E+30"), Is.EqualTo(-1.234E+30f));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void StringToDouble()
        {
            IValueConverter<string, double> converter = ValueConverter.StringToDouble();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("0"), Is.EqualTo(0));
                Assert.That(converter.Convert("1"), Is.EqualTo(1));
                Assert.That(converter.Convert("-1"), Is.EqualTo(-1));
                Assert.That(converter.Convert("NaN"), Is.EqualTo(double.NaN));
                Assert.That(converter.Convert(double.PositiveInfinity.ToString()), Is.EqualTo(double.PositiveInfinity));
                Assert.That(converter.Convert(double.NegativeInfinity.ToString()), Is.EqualTo(double.NegativeInfinity));
                Assert.That(converter.Convert("1.23456789E+300"), Is.EqualTo(1.23456789E+300));
                Assert.That(converter.Convert("-1.23456789E+300"), Is.EqualTo(-1.23456789E+300));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void StringToDecimal()
        {
            IValueConverter<string, decimal> converter = ValueConverter.StringToDecimal();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("0"), Is.EqualTo(0));
                Assert.That(converter.Convert("1"), Is.EqualTo(1));
                Assert.That(converter.Convert("-1"), Is.EqualTo(-1));
                Assert.That(converter.Convert(decimal.MaxValue.ToString()), Is.EqualTo(decimal.MaxValue));
                Assert.That(converter.Convert(decimal.MinValue.ToString()), Is.EqualTo(decimal.MinValue));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void StringToChar()
        {
            IValueConverter<string, char> converter = ValueConverter.StringToChar();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("T"), Is.EqualTo('T'));
                Assert.That(converter.Convert("T"), Is.Not.EqualTo('t'));
                Assert.That(converter.Convert(char.MaxValue.ToString()), Is.EqualTo(char.MaxValue));
                Assert.That(converter.Convert(char.MinValue.ToString()), Is.EqualTo(char.MinValue));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void StringToDateTime_WithoutArgs()
        {
            IValueConverter<string, DateTime> converter = ValueConverter.StringToDateTime();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("2024-05-06"), Is.EqualTo(new DateTime(2024, 5, 6)));
                Assert.That(converter.Convert("9999-12-31 23:59:59.9999999"), Is.EqualTo(DateTime.MaxValue));
                Assert.That(converter.Convert("0001-01-01 00:00:00"), Is.EqualTo(DateTime.MinValue));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void StringToDateTime_WithNullFormat()
        {
            Assert.That(() => ValueConverter.StringToDateTime(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void StringToDateTime_WithEmptyFormat()
        {
            Assert.That(() => ValueConverter.StringToDateTime(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void StringToDateTime_WithFormat_AsPositive()
        {
            IValueConverter<string, DateTime> converter = ValueConverter.StringToDateTime("yyyy/MM/dd HH:mm:ss");

            Assert.Multiple(() =>
            {
                Assert.That(converter, Is.TypeOf<DateTimeExactConverter>());
                Assert.That(((DateTimeExactConverter)converter).Format, Is.EqualTo("yyyy/MM/dd HH:mm:ss"));
            });
        }

#if NET6_0_OR_GREATER

        [Test]
        public void StringToDateOnly_WithoutArgs()
        {
            IValueConverter<string, DateOnly> converter = ValueConverter.StringToDateOnly();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("2024-05-06"), Is.EqualTo(new DateOnly(2024, 5, 6)));
                Assert.That(converter.Convert("9999-12-31"), Is.EqualTo(DateOnly.MaxValue));
                Assert.That(converter.Convert("0001-01-01"), Is.EqualTo(DateOnly.MinValue));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void StringToDateOnly_WithNullFormat()
        {
            Assert.That(() => ValueConverter.StringToDateOnly(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void StringToDateOnly_WithEmptyFormat()
        {
            Assert.That(() => ValueConverter.StringToDateOnly(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void StringToDateOnly_WithFormat_AsPositive()
        {
            IValueConverter<string, DateOnly> converter = ValueConverter.StringToDateOnly("yyyy/MM/dd");

            Assert.Multiple(() =>
            {
                Assert.That(converter, Is.TypeOf<DateOnlyExactConverter>());
                Assert.That(((DateOnlyExactConverter)converter).Format, Is.EqualTo("yyyy/MM/dd"));
            });
        }

        [Test]
        public void StringToTimeOnly_WithoutArgs()
        {
            IValueConverter<string, TimeOnly> converter = ValueConverter.StringToTimeOnly();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("11:42:31"), Is.EqualTo(new TimeOnly(11, 42, 31)));
                Assert.That(converter.Convert("23:59:59.9999999"), Is.EqualTo(TimeOnly.MaxValue));
                Assert.That(converter.Convert("00:00:00"), Is.EqualTo(TimeOnly.MinValue));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void StringToTimeOnly_WithNullFormat()
        {
            Assert.That(() => ValueConverter.StringToTimeOnly(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void StringToTimeOnly_WithEmptyFormat()
        {
            Assert.That(() => ValueConverter.StringToTimeOnly(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void StringToTimeOnly_WithFormat_AsPositive()
        {
            IValueConverter<string, TimeOnly> converter = ValueConverter.StringToTimeOnly("HH:mm:ss");
            TimeOnly value = converter.Convert("12:34:56");

            Assert.Multiple(() =>
            {
                Assert.That(converter, Is.TypeOf<TimeOnlyExactConverter>());
                Assert.That(((TimeOnlyExactConverter)converter).Format, Is.EqualTo("HH:mm:ss"));
            });
        }

#endif

        [Test]
        public void StringToTimeSpan_WithoutArgs()
        {
            IValueConverter<string, TimeSpan> converter = ValueConverter.StringToTimeSpan();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("3.14:15:52.65"), Is.EqualTo(new TimeSpan(3, 14, 15, 52, 650)));
                Assert.That(converter.Convert("10675199.02:48:05.4775807"), Is.EqualTo(TimeSpan.MaxValue));
                Assert.That(converter.Convert("-10675199.02:48:05.4775808"), Is.EqualTo(TimeSpan.MinValue));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void StringToTimeSpan_WithNullFormat()
        {
            Assert.That(() => ValueConverter.StringToTimeSpan(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void StringToTimeSpan_WithEmptyFormat()
        {
            Assert.That(() => ValueConverter.StringToTimeSpan(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void StringToTimeSpan_WithFormat_AsPositive()
        {
            IValueConverter<string, TimeSpan> converter = ValueConverter.StringToTimeSpan(@"d\.hh\:mm\:ss");

            Assert.Multiple(() =>
            {
                Assert.That(converter, Is.TypeOf<TimeSpanExactConverter>());
                Assert.That(((TimeSpanExactConverter)converter).Format, Is.EqualTo(@"d\.hh\:mm\:ss"));
            });
        }

        [Test]
        public void StringToDateTimeOffset_WithoutArgs()
        {
            IValueConverter<string, DateTimeOffset> converter = ValueConverter.StringToDateTimeOffset();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("2024-05-06 12:34:56"), Is.EqualTo(new DateTimeOffset(new DateTime(2024, 05, 06, 12, 34, 56))));
                Assert.That(converter.Convert("9999-12-31 23:59:59.9999999+00:00"), Is.EqualTo(DateTimeOffset.MaxValue));
                Assert.That(converter.Convert("0001-01-01 00:00:00+00:00"), Is.EqualTo(DateTimeOffset.MinValue));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void StringToDateTimeOffset_WithNullFormat()
        {
            Assert.That(() => ValueConverter.StringToDateTimeOffset(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void StringToDateTimeOffset_WithEmptyFormat()
        {
            Assert.That(() => ValueConverter.StringToDateTimeOffset(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void StringToDateTimeOffset_WithFormat_AsPositive()
        {
            IValueConverter<string, DateTimeOffset> converter = ValueConverter.StringToDateTimeOffset("yyyy/MM/dd HH:mm:ss");
            DateTimeOffset value = converter.Convert("2000/01/01 12:34:56");

            Assert.Multiple(() =>
            {
                Assert.That(converter, Is.TypeOf<DateTimeOffsetExactConverter>());
                Assert.That(((DateTimeOffsetExactConverter)converter).Format, Is.EqualTo("yyyy/MM/dd HH:mm:ss"));
            });
        }

        [Test]
        public void StringToValueRange()
        {
            IValueConverter<string, ValueRange> converter = ValueConverter.StringToValueRange();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("10"), Is.EqualTo(new ValueRange(10)));
                Assert.That(converter.Convert("1-3"), Is.EqualTo(new ValueRange(1, 3)));
                Assert.That(converter.Convert("-10"), Is.EqualTo(new ValueRange(end: 10)));
                Assert.That(converter.Convert("10-"), Is.EqualTo(new ValueRange(start: 10)));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void StringToValueRangeCollection()
        {
            IValueConverter<string, ValueRangeCollection> converter = ValueConverter.StringToValueRangeCollection();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("1,3-5,10-11"), Is.EqualTo(new[] { 1, new ValueRange(3, 5), new ValueRange(10, 11) }));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void StringToEnum_WithoutArgs()
        {
            IValueConverter<string, OptionType> converter = ValueConverter.StringToEnum<OptionType>();

            Assert.Multiple(() =>
            {
                Assert.That(converter, Is.TypeOf<EnumValueConverter<OptionType>>());
                Assert.That(((EnumValueConverter<OptionType>)converter).IgnoreCase, Is.EqualTo(false));
            });
        }

        [TestCase(true)]
        [TestCase(false)]
        public void StringToEnum_WithBoolean(bool ignoreCase)
        {
            IValueConverter<string, OptionType> converter = ValueConverter.StringToEnum<OptionType>(ignoreCase);

            Assert.Multiple(() =>
            {
                Assert.That(converter, Is.TypeOf<EnumValueConverter<OptionType>>());
                Assert.That(((EnumValueConverter<OptionType>)converter).IgnoreCase, Is.EqualTo(ignoreCase));
            });
        }

        [Test]
        public void StringToFileInfo()
        {
            IValueConverter<string, FileInfo> converter = ValueConverter.StringToFileInfo();

            Assert.That(converter, Is.TypeOf<FileInfoValueConverter>());
        }

        [Test]
        public void StringToFileInfos()
        {
            IValueConverter<string, FileInfo[]> converter = ValueConverter.StringToFileInfos();

            Assert.That(converter, Is.TypeOf<FilePatternValueConverter>());
        }

        [Test]
        public void StringToDirectoryInfo()
        {
            IValueConverter<string, DirectoryInfo> converter = ValueConverter.StringToDirectoryInfo();

            Assert.That(converter, Is.TypeOf<DirectoryInfoValueConverter>());
        }

        [Test]
        public void StringToDirectoryInfos()
        {
            IValueConverter<string, DirectoryInfo[]> converter = ValueConverter.StringToDirectoryInfos();

            Assert.That(converter, Is.TypeOf<DirectoryPatternValueConverter>());
        }

        [Test]
        public void StringToFileSystemInfos()
        {
            IValueConverter<string, FileSystemInfo[]> converter = ValueConverter.StringToFileSystemInfos();

            Assert.That(converter, Is.TypeOf<FileSystemPatternValueConverter>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void StringToStreamWriter_WithNullEncoding(bool append)
        {
            IValueConverter<string, StreamWriter> converter = ValueConverter.StringToStreamWriter(null, append);

            Assert.Multiple(() =>
            {
                Assert.That(converter, Is.TypeOf<StreamWriterValueConverter>());
                Assert.That(((StreamWriterValueConverter)converter).Encoding, Is.EqualTo(IOHelpers.UTF8N));
                Assert.That(((StreamWriterValueConverter)converter).Append, Is.EqualTo(append));
            });
        }

        [TestCase(true)]
        [TestCase(false)]
        public void StringToStreamWriter_WithSpecifiedEncoding(bool append)
        {
            IValueConverter<string, StreamWriter> converter = ValueConverter.StringToStreamWriter(Encoding.UTF32, append);

            Assert.Multiple(() =>
            {
                Assert.That(converter, Is.TypeOf<StreamWriterValueConverter>());
                Assert.That(((StreamWriterValueConverter)converter).Encoding, Is.EqualTo(Encoding.UTF32));
                Assert.That(((StreamWriterValueConverter)converter).Append, Is.EqualTo(append));
            });
        }

        [Test]
        public void StringToStreamReader_WithNullEncoding()
        {
            IValueConverter<string, StreamReader> converter = ValueConverter.StringToStreamReader(null);

            Assert.Multiple(() =>
            {
                Assert.That(converter, Is.TypeOf<StreamReaderValueConverter>());
                Assert.That(((StreamReaderValueConverter)converter).Encoding, Is.EqualTo(IOHelpers.UTF8N));
            });
        }

        [Test]
        public void StringToStreamReader_WithSpecifiedEncoding()
        {
            IValueConverter<string, StreamReader> converter = ValueConverter.StringToStreamReader(Encoding.UTF32);

            Assert.Multiple(() =>
            {
                Assert.That(converter, Is.TypeOf<StreamReaderValueConverter>());
                Assert.That(((StreamReaderValueConverter)converter).Encoding, Is.EqualTo(Encoding.UTF32));
            });
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ToConsoleOrFileWriter_WithNullEncoding(bool append)
        {
            IValueConverter<string, TextWriter> converter = ValueConverter.ToConsoleOrFileWriter(null, append);

            Assert.Multiple(() =>
            {
                Assert.That(converter, Is.TypeOf<FileOrConsoleWriterValueConverter>());
                Assert.That(((FileOrConsoleWriterValueConverter)converter).Encoding, Is.EqualTo(IOHelpers.UTF8N));
                Assert.That(((FileOrConsoleWriterValueConverter)converter).Append, Is.EqualTo(append));
            });
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ToConsoleOrFileWriter_WithSpecifiedEncoding(bool append)
        {
            var encoding = new UnicodeEncoding();
            IValueConverter<string, TextWriter> converter = ValueConverter.ToConsoleOrFileWriter(encoding, append);

            Assert.Multiple(() =>
            {
                Assert.That(converter, Is.TypeOf<FileOrConsoleWriterValueConverter>());
                Assert.That(((FileOrConsoleWriterValueConverter)converter).Encoding, Is.EqualTo(encoding));
                Assert.That(((FileOrConsoleWriterValueConverter)converter).Append, Is.EqualTo(append));
            });
        }

        [Test]
        public void ToConsoleOrFileReader_WithNullEncoding()
        {
            IValueConverter<string, TextReader> converter = ValueConverter.ToConsoleOrFileReader(null);

            Assert.Multiple(() =>
            {
                Assert.That(converter, Is.TypeOf<FileOrConsoleReaderValueConverter>());
                Assert.That(((FileOrConsoleReaderValueConverter)converter).Encoding, Is.EqualTo(IOHelpers.UTF8N));
            });
        }

        [Test]
        public void ToConsoleOrFileReader_WithSpecifiedEncoding()
        {
            var encoding = new UnicodeEncoding();
            IValueConverter<string, TextReader> converter = ValueConverter.ToConsoleOrFileReader(encoding: encoding);

            Assert.Multiple(() =>
            {
                Assert.That(converter, Is.TypeOf<FileOrConsoleReaderValueConverter>());
                Assert.That(((FileOrConsoleReaderValueConverter)converter).Encoding, Is.EqualTo(encoding));
            });
        }

        [Test]
        public void SplitToArray_NonGeneric_WithNullElementType()
        {
            Assert.That(() => ValueConverter.SplitToArray(null!, ",", ValueConverter.GetDefault<string>()), Throws.ArgumentNullException);
        }

        [Test]
        public void SplitToArray_NonGeneric_WithNullSeparator()
        {
            Assert.That(() => ValueConverter.SplitToArray(typeof(string), null!, ValueConverter.GetDefault<string>()), Throws.ArgumentNullException);
        }

        [Test]
        public void SplitToArray_NonGeneric_WithEmptySeparator()
        {
            Assert.That(() => ValueConverter.SplitToArray(typeof(string), string.Empty, ValueConverter.GetDefault<string>()), Throws.ArgumentException);
        }

        [Test]
        public void SplitToArray_NonGeneric_WithNullConverter()
        {
            Assert.That(() => ValueConverter.SplitToArray(typeof(string), ",", null!), Throws.ArgumentNullException);
        }

        [Test]
        public void SplitToArray_NonGeneric_AsPositive()
        {
            var innerConverter = ValueConverter.GetDefault<FileInfo>();
#pragma warning disable CA2263 // 型が既知の場合はジェネリック オーバーロードを優先する
            IValueConverter<string, Array> converter = ValueConverter.SplitToArray(typeof(FileInfo), ";", innerConverter, StringSplitOptions.RemoveEmptyEntries);
#pragma warning restore CA2263 // 型が既知の場合はジェネリック オーバーロードを優先する

            Assert.Multiple(() =>
            {
                Assert.That(converter, Is.TypeOf<ArrayValueConverter>());
                Assert.That(((ArrayValueConverter)converter).Separator, Is.EqualTo(";"));
                Assert.That(((ArrayValueConverter)converter).ElementType, Is.EqualTo(typeof(FileInfo)));
                Assert.That(((ArrayValueConverter)converter).ElementConverter, Is.EqualTo(innerConverter));
                Assert.That(((ArrayValueConverter)converter).SplitOptions, Is.EqualTo(StringSplitOptions.RemoveEmptyEntries));
            });
        }

        [Test]
        public void SplitToArray_Generic_WithNullSeperator()
        {
            Assert.That(() => ValueConverter.SplitToArray(null!, ValueConverter.GetDefault<int>()), Throws.ArgumentNullException);
        }

        [Test]
        public void SplitToArray_Generic_WithEmptySeperator()
        {
            Assert.That(() => ValueConverter.SplitToArray(string.Empty, ValueConverter.GetDefault<int>()), Throws.ArgumentException);
        }

        [Test]
        public void SplitToArray_Generic_WithNullConverter()
        {
            Assert.That(() => ValueConverter.SplitToArray<int>(",", null!), Throws.ArgumentNullException);
        }

        [Test]
        public void SplitToArray_Generic_AsPositive()
        {
            var innerConverter = ValueConverter.GetDefault<int>();
            IValueConverter<string, int[]> converter = ValueConverter.SplitToArray(",", innerConverter, StringSplitOptions.RemoveEmptyEntries);

            Assert.Multiple(() =>
            {
                Assert.That(converter, Is.TypeOf<ArrayValueConverter<int>>());
                Assert.That(((ArrayValueConverter<int>)converter).Separator, Is.EqualTo(","));
                Assert.That(((ArrayValueConverter<int>)converter).ElementConverter, Is.EqualTo(innerConverter));
                Assert.That(((ArrayValueConverter<int>)converter).SplitOptions, Is.EqualTo(StringSplitOptions.RemoveEmptyEntries));
            });
        }
    }
}
