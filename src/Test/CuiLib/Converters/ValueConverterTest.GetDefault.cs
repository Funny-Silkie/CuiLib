using CuiLib;
using CuiLib.Converters;
using CuiLib.Data;
using CuiLib.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Test.Helpers;

namespace Test.CuiLib.Converters
{
    public partial class ValueConverterTest
    {
        [Test]
        public void GetDefault_AsString()
        {
            IValueConverter<string, string> converter = ValueConverter.GetDefault<string>();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("hoge"), Is.EqualTo("hoge"));
                Assert.That(converter.Convert(string.Empty), Is.Empty);
            });
        }

        [Test]
        public void GetDefault_AsSZArray()
        {
            IValueConverter<string, int[]> converter = ValueConverter.GetDefault<int[]>();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert(string.Empty), Is.Empty);
                Assert.That(converter.Convert("1"), Is.EqualTo(new[] { 1 }));
                Assert.That(converter.Convert("1,2,3"), Is.EqualTo(new[] { 1, 2, 3 }));
            });
        }

        [Test]
        public void GetDefault_AsFileInfo()
        {
            IValueConverter<string, FileInfo> converter = ValueConverter.GetDefault<FileInfo>();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("test.txt").Name, Is.EqualTo("test.txt"));
                Assert.That(converter.Convert("test").Name, Is.EqualTo("test"));
            });
        }

        [Test]
        public void GetDefault_AsDirectoryInfo()
        {
            IValueConverter<string, DirectoryInfo> converter = ValueConverter.GetDefault<DirectoryInfo>();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("test").Name, Is.EqualTo("test"));
                Assert.That(converter.Convert("test/new").Name, Is.EqualTo("new"));
            });
        }

        [Test]
        public void GetDefault_AsTextReader()
        {
            IValueConverter<string, TextReader> converter = ValueConverter.GetDefault<TextReader>();
            FileInfo target = FileUtilHelpers.GetNoExistingFile();

            try
            {
                target.Create().Dispose();

                Assert.Multiple(() =>
                {
                    using (var reader = converter.Convert(target.FullName))
                    {
                        Assert.That(reader, Is.InstanceOf<StreamReader>());
                        Assert.That(((StreamReader)reader).CurrentEncoding, Is.EqualTo(IOHelpers.UTF8N));
                    }
                    Assert.That(converter.Convert(null!), Is.EqualTo(Console.In));

                    Assert.That(() => converter.Convert(string.Empty), Throws.ArgumentException);
                    Assert.That(() => converter.Convert(FileUtilHelpers.GetNoExistingFile().Name), Throws.TypeOf<FileNotFoundException>());
                });
            }
            finally
            {
                target.Delete();
            }
        }

        [Test]
        public void GetDefault_AsStreamReader()
        {
            IValueConverter<string, StreamReader> converter = ValueConverter.GetDefault<StreamReader>();
            FileInfo target = FileUtilHelpers.GetNoExistingFile();

            try
            {
                target.Create().Dispose();

                Assert.Multiple(() =>
                {
                    using (StreamReader reader = converter.Convert(target.FullName))
                    {
                        Assert.That(reader, Is.InstanceOf<StreamReader>());
                        Assert.That(reader.CurrentEncoding, Is.EqualTo(IOHelpers.UTF8N));
                    }

                    Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                    Assert.That(() => converter.Convert(string.Empty), Throws.ArgumentException);
                    Assert.That(() => converter.Convert(FileUtilHelpers.GetNoExistingFile().Name), Throws.TypeOf<FileNotFoundException>());
                });
            }
            finally
            {
                target.Delete();
            }
        }

        [Test]
        public void GetDefault_AsTextWriter()
        {
            IValueConverter<string, TextWriter> converter = ValueConverter.GetDefault<TextWriter>();
            FileInfo target = FileUtilHelpers.GetNoExistingFile();

            try
            {
                Assert.Multiple(() =>
                {
                    Assert.Multiple(() =>
                    {
                        using TextWriter writer = converter.Convert(target.Name);
                        Assert.That(writer, Is.InstanceOf<StreamWriter>());
                        Assert.That(writer.Encoding, Is.EqualTo(IOHelpers.UTF8N));

                        target.Refresh();
                        Assert.That(target.Exists, Is.True);
                    });

                    Assert.That(converter.Convert(null!), Is.EqualTo(Console.Out));

                    Assert.That(() => converter.Convert(string.Empty), Throws.ArgumentException);
                });
            }
            finally
            {
                target.Delete();
            }
        }

        [Test]
        public void GetDefault_AsStreamWriter()
        {
            IValueConverter<string, StreamWriter> converter = ValueConverter.GetDefault<StreamWriter>();
            FileInfo target = FileUtilHelpers.GetNoExistingFile();

            try
            {
                Assert.Multiple(() =>
                {
                    Assert.Multiple(() =>
                    {
                        using StreamWriter writer = converter.Convert(target.Name);
                        Assert.That(writer, Is.InstanceOf<StreamWriter>());
                        Assert.That(writer.Encoding, Is.EqualTo(IOHelpers.UTF8N));
                        target.Refresh();
                        Assert.That(target.Exists, Is.True);
                    });

                    Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                    Assert.That(() => converter.Convert(string.Empty), Throws.ArgumentException);
                });
            }
            finally
            {
                target.Delete();
            }
        }

        [Test]
        public void GetDefault_AsSByte()
        {
            IValueConverter<string, sbyte> converter = ValueConverter.GetDefault<sbyte>();

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
        public void GetDefault_AsInt16()
        {
            IValueConverter<string, short> converter = ValueConverter.GetDefault<short>();

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
        public void GetDefault_AsInt32()
        {
            IValueConverter<string, int> converter = ValueConverter.GetDefault<int>();

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
        public void GetDefault_AsInt64()
        {
            IValueConverter<string, long> converter = ValueConverter.GetDefault<long>();

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
        public void GetDefault_AsInt128()
        {
            IValueConverter<string, Int128> converter = ValueConverter.GetDefault<Int128>();

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
        public void GetDefault_AsByte()
        {
            IValueConverter<string, byte> converter = ValueConverter.GetDefault<byte>();

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
        public void GetDefault_AsUInt16()
        {
            IValueConverter<string, ushort> converter = ValueConverter.GetDefault<ushort>();

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
        public void GetDefault_AsUInt32()
        {
            IValueConverter<string, uint> converter = ValueConverter.GetDefault<uint>();

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
        public void GetDefault_AsUInt64()
        {
            IValueConverter<string, ulong> converter = ValueConverter.GetDefault<ulong>();

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
        public void GetDefault_AsUInt128()
        {
            IValueConverter<string, UInt128> converter = ValueConverter.GetDefault<UInt128>();

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
        public void GetDefault_AsSingle()
        {
            IValueConverter<string, float> converter = ValueConverter.GetDefault<float>();

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
        public void GetDefault_AsDouble()
        {
            IValueConverter<string, double> converter = ValueConverter.GetDefault<double>();

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
        public void GetDefault_AsDeimal()
        {
            IValueConverter<string, decimal> converter = ValueConverter.GetDefault<decimal>();

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
        public void GetDefault_AsChar()
        {
            IValueConverter<string, char> converter = ValueConverter.GetDefault<char>();

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
        public void GetDefault_AsDateTime()
        {
            IValueConverter<string, DateTime> converter = ValueConverter.GetDefault<DateTime>();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("2024-05-06"), Is.EqualTo(new DateTime(2024, 5, 6)));
                Assert.That(converter.Convert("9999-12-31 23:59:59.9999999"), Is.EqualTo(DateTime.MaxValue));
                Assert.That(converter.Convert("0001-01-01 00:00:00"), Is.EqualTo(DateTime.MinValue));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

#if NET6_0_OR_GREATER

        [Test]
        public void GetDefault_AsDateOnly()
        {
            IValueConverter<string, DateOnly> converter = ValueConverter.GetDefault<DateOnly>();

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
        public void GetDefault_AsTimeOnly()
        {
            IValueConverter<string, TimeOnly> converter = ValueConverter.GetDefault<TimeOnly>();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("11:42:31"), Is.EqualTo(new TimeOnly(11, 42, 31)));
                Assert.That(converter.Convert("23:59:59.9999999"), Is.EqualTo(TimeOnly.MaxValue));
                Assert.That(converter.Convert("00:00:00"), Is.EqualTo(TimeOnly.MinValue));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

#endif

        [Test]
        public void GetDefault_AsTimeSpan()
        {
            IValueConverter<string, TimeSpan> converter = ValueConverter.GetDefault<TimeSpan>();

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
        public void GetDefault_AsDateTimeOffset()
        {
            IValueConverter<string, DateTimeOffset> converter = ValueConverter.GetDefault<DateTimeOffset>();

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
        public void GetDefault_AsValueRange()
        {
            IValueConverter<string, ValueRange> converter = ValueConverter.GetDefault<ValueRange>();

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
        public void GetDefault_AsValueRangeCollection()
        {
            IValueConverter<string, ValueRangeCollection> converter = ValueConverter.GetDefault<ValueRangeCollection>();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("1,3-5,10-11"), Is.EqualTo(new[] { 1, new ValueRange(3, 5), new ValueRange(10, 11) }));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void GetDefault_AsEnum()
        {
            IValueConverter<string, OptionType> converter = ValueConverter.GetDefault<OptionType>();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("None"), Is.EqualTo(OptionType.None));
                Assert.That(converter.Convert("Flag"), Is.EqualTo(OptionType.Flag));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert("!!"), Throws.ArgumentException);
                Assert.That(() => converter.Convert("none"), Throws.ArgumentException);
                Assert.That(() => converter.Convert("flag"), Throws.ArgumentException);
            });
        }

        [Test]
        public void GetDefault_AsNotSupported()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueConverter.GetDefault<object>(), Throws.TypeOf<NotSupportedException>());
                Assert.That(() => ValueConverter.GetDefault<Enum>(), Throws.TypeOf<NotSupportedException>());
                Assert.That(() => ValueConverter.GetDefault<ValueType>(), Throws.TypeOf<NotSupportedException>());
                Assert.That(() => ValueConverter.GetDefault<ValueConverterTest>(), Throws.TypeOf<NotSupportedException>());
            });
        }

        [Test]
        public void GetValueTypeString()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ValueConverter.GetValueTypeString<string>(), Is.EqualTo("string"));
                Assert.That(ValueConverter.GetValueTypeString<string[]>(), Is.EqualTo("string[]"));
                Assert.That(ValueConverter.GetValueTypeString<int[]>(), Is.EqualTo("int[]"));
                Assert.That(ValueConverter.GetValueTypeString<FileInfo>(), Is.EqualTo("file"));
                Assert.That(ValueConverter.GetValueTypeString<DirectoryInfo>(), Is.EqualTo("directory"));
                Assert.That(ValueConverter.GetValueTypeString<TextWriter>(), Is.EqualTo("file"));
                Assert.That(ValueConverter.GetValueTypeString<TextReader>(), Is.EqualTo("file"));
                Assert.That(ValueConverter.GetValueTypeString<sbyte>(), Is.EqualTo("int"));
                Assert.That(ValueConverter.GetValueTypeString<short>(), Is.EqualTo("int"));
                Assert.That(ValueConverter.GetValueTypeString<int>(), Is.EqualTo("int"));
                Assert.That(ValueConverter.GetValueTypeString<long>(), Is.EqualTo("long"));
                Assert.That(ValueConverter.GetValueTypeString<byte>(), Is.EqualTo("int"));
                Assert.That(ValueConverter.GetValueTypeString<ushort>(), Is.EqualTo("int"));
                Assert.That(ValueConverter.GetValueTypeString<uint>(), Is.EqualTo("uint"));
                Assert.That(ValueConverter.GetValueTypeString<ulong>(), Is.EqualTo("ulong"));
                Assert.That(ValueConverter.GetValueTypeString<float>(), Is.EqualTo("float"));
                Assert.That(ValueConverter.GetValueTypeString<double>(), Is.EqualTo("float"));
                Assert.That(ValueConverter.GetValueTypeString<decimal>(), Is.EqualTo("decimal"));
                Assert.That(ValueConverter.GetValueTypeString<char>(), Is.EqualTo("char"));
                Assert.That(ValueConverter.GetValueTypeString<bool>(), Is.EqualTo("bool"));
                Assert.That(ValueConverter.GetValueTypeString<DateTime>(), Is.EqualTo("date time"));
                Assert.That(ValueConverter.GetValueTypeString<TimeSpan>(), Is.EqualTo("date time"));
#if NET6_0_OR_GREATER
                Assert.That(ValueConverter.GetValueTypeString<DateOnly>(), Is.EqualTo("date"));
                Assert.That(ValueConverter.GetValueTypeString<TimeOnly>(), Is.EqualTo("time"));
#endif

                Assert.That(ValueConverter.GetValueTypeString<OptionType>(), Is.EqualTo("string"));
                Assert.That(ValueConverter.GetValueTypeString<object>(), Is.EqualTo("string"));
            });
        }
    }
}
