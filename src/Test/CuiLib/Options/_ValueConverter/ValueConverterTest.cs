﻿using CuiLib.Options;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using Test.Helpers;

namespace Test.CuiLib.Options._ValueConverter
{
    [TestFixture]
    public class ValueConverterTest
    {
        [Test]
        public void Combine_WithNull()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentNullException>(() => ValueConverter.Combine<string, int, long>(ValueConverter.GetDefault<int>(), null!));
                Assert.Throws<ArgumentNullException>(() => ValueConverter.Combine<object, string, int>(null!, ValueConverter.GetDefault<int>()));
            });
        }

        [Test]
        public void Combine_AsPositive()
        {
            IValueConverter<string, int> converter = ValueConverter.GetDefault<int>().Combine(ValueConverter.FromDelegate<int, int>(x => checked(x * 2)));

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("1"), Is.EqualTo(2));
                Assert.That(converter.Convert("0"), Is.EqualTo(0));
                Assert.That(converter.Convert("-100"), Is.EqualTo(-200));

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<OverflowException>(() => converter.Convert(int.MaxValue.ToString()));
            });
        }

        [Test]
        public void FromDelegate_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => ValueConverter.FromDelegate<string, int>(null!));
        }

        [Test]
        public void FromDelegate_WithDelegate()
        {
            IValueConverter<string, int> converter = ValueConverter.FromDelegate<string, int>(int.Parse);

            Assert.That(converter.Convert("123"), Is.EqualTo(123));
        }

        [Test]
        public void StringToIParsable_Convert()
        {
            IValueConverter<string, int> converter = ValueConverter.StringToIParsable<int>();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("123"), Is.EqualTo(123));
                Assert.That(converter.Convert("-8000"), Is.EqualTo(-8000));

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
            });
        }

#pragma warning disable NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void StringToIParsable_Equals()
        {
            Assert.That(ValueConverter.StringToIParsable<int>(), Is.EqualTo(ValueConverter.StringToIParsable<int>()));
        }

        [Test]
        public void StringToIParsable_GetHashCode()
        {
            Assert.That(ValueConverter.StringToIParsable<int>().GetHashCode(), Is.EqualTo(ValueConverter.StringToIParsable<int>().GetHashCode()));
        }

#pragma warning restore NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void StringToEnum_WithoutArgs()
        {
            IValueConverter<string, OptionType> converter = ValueConverter.StringToEnum<OptionType>();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("None"), Is.EqualTo(OptionType.None));
                Assert.That(converter.Convert("Flag"), Is.EqualTo(OptionType.Flag));

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<ArgumentException>(() => converter.Convert("none"));
                Assert.Throws<ArgumentException>(() => converter.Convert("fLAG"));
                Assert.Throws<ArgumentException>(() => converter.Convert("!!"));
            });
        }

        [Test]
        public void StringToEnum_WithBoolean_AsCaseSensitive()
        {
            IValueConverter<string, OptionType> converter = ValueConverter.StringToEnum<OptionType>(false);

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("None"), Is.EqualTo(OptionType.None));
                Assert.That(converter.Convert("Flag"), Is.EqualTo(OptionType.Flag));

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<ArgumentException>(() => converter.Convert("none"));
                Assert.Throws<ArgumentException>(() => converter.Convert("fLAG"));
                Assert.Throws<ArgumentException>(() => converter.Convert("!!"));
            });
        }

        [Test]
        public void StringToEnum_WithBoolean_AsIgnoreCase()
        {
            IValueConverter<string, OptionType> converter = ValueConverter.StringToEnum<OptionType>(true);

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("None"), Is.EqualTo(OptionType.None));
                Assert.That(converter.Convert("none"), Is.EqualTo(OptionType.None));
                Assert.That(converter.Convert("Flag"), Is.EqualTo(OptionType.Flag));
                Assert.That(converter.Convert("fLAG"), Is.EqualTo(OptionType.Flag));

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<ArgumentException>(() => converter.Convert("!!"));
            });
        }

        [Test]
        public void StringToFileInfo_Convert()
        {
            IValueConverter<string, FileInfo> converter = ValueConverter.StringToFileInfo();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("test.txt").Name, Is.EqualTo("test.txt"));

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<ArgumentException>(() => converter.Convert(string.Empty));
            });
        }

#pragma warning disable NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void StringToFileInfo_Equals()
        {
            Assert.That(ValueConverter.StringToFileInfo(), Is.EqualTo(ValueConverter.StringToFileInfo()));
        }

        [Test]
        public void StringToFileInfo_GetHashCode()
        {
            Assert.That(ValueConverter.StringToFileInfo().GetHashCode(), Is.EqualTo(ValueConverter.StringToFileInfo().GetHashCode()));
        }

#pragma warning restore NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void StringToDirectoryInfo_Convert()
        {
            IValueConverter<string, DirectoryInfo> converter = ValueConverter.StringToDirectoryInfo();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("folder").Name, Is.EqualTo("folder"));

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<ArgumentException>(() => converter.Convert(string.Empty));
            });
        }

#pragma warning disable NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void StringToDirectoryInfo_Equals()
        {
            Assert.That(ValueConverter.StringToDirectoryInfo(), Is.EqualTo(ValueConverter.StringToDirectoryInfo()));
        }

        [Test]
        public void StringToDirectoryInfo_GetHashCode()
        {
            Assert.That(ValueConverter.StringToDirectoryInfo().GetHashCode(), Is.EqualTo(ValueConverter.StringToDirectoryInfo().GetHashCode()));
        }

#pragma warning restore NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void ToConsoleOrFileWriter_WithNullEncoding()
        {
            IValueConverter<string, TextWriter> converter = ValueConverter.ToConsoleOrFileWriter(encoding: null);
            FileInfo target = FileUtilHelpers.GetNoExistingFile();

            try
            {
                Assert.Multiple(() =>
                {
                    Assert.Multiple(() =>
                    {
                        using TextWriter writer = converter.Convert(target.Name);
                        Assert.That(writer, Is.InstanceOf<StreamWriter>());
                        Assert.That(writer.Encoding, Is.InstanceOf<UTF8Encoding>());

                        target.Refresh();
                        Assert.That(target.Exists, Is.True);
                    });

                    Assert.That(converter.Convert(null!), Is.EqualTo(Console.Out));

                    Assert.Throws<ArgumentException>(() => converter.Convert(string.Empty));
                });
            }
            finally
            {
                target.Delete();
            }
        }

        [Test]
        public void ToConsoleOrFileWriter_WithSpecifiedEncoding()
        {
            var encoding = new UnicodeEncoding();
            IValueConverter<string, TextWriter> converter = ValueConverter.ToConsoleOrFileWriter(encoding: encoding);
            FileInfo target = FileUtilHelpers.GetNoExistingFile();

            try
            {
                Assert.Multiple(() =>
                {
                    Assert.Multiple(() =>
                    {
                        using TextWriter writer = converter.Convert(target.Name);
                        Assert.That(writer, Is.InstanceOf<StreamWriter>());
                        Assert.That(writer.Encoding, Is.EqualTo(encoding));

                        target.Refresh();
                        Assert.That(target.Exists, Is.True);
                    });

                    Assert.That(converter.Convert(null!), Is.EqualTo(Console.Out));

                    Assert.Throws<ArgumentException>(() => converter.Convert(string.Empty));
                });
            }
            finally
            {
                target.Delete();
            }
        }

        [Test]
        public void ToConsoleOrFileReader_WithNullEncoding()
        {
            IValueConverter<string, TextReader> converter = ValueConverter.ToConsoleOrFileReader(encoding: null);
            FileInfo target = FileUtilHelpers.GetNoExistingFile();

            try
            {
                target.Create().Dispose();

                Assert.Multiple(() =>
                {
                    using (var reader = converter.Convert(target.FullName))
                    {
                        Assert.That(reader, Is.InstanceOf<StreamReader>());
                        Assert.That(((StreamReader)reader).CurrentEncoding, Is.InstanceOf<UTF8Encoding>());
                    }
                    Assert.That(converter.Convert(null!), Is.EqualTo(Console.In));

                    Assert.Throws<ArgumentException>(() => converter.Convert(string.Empty));
                    Assert.Throws<FileNotFoundException>(() => converter.Convert(FileUtilHelpers.GetNoExistingFile().Name));
                });
            }
            finally
            {
                target.Delete();
            }
        }

        [Test]
        public void ToConsoleOrFileReader_WithSpecifiedEncoding()
        {
            var encoding = new UnicodeEncoding();
            IValueConverter<string, TextReader> converter = ValueConverter.ToConsoleOrFileReader(encoding: encoding);
            FileInfo target = FileUtilHelpers.GetNoExistingFile();

            try
            {
                target.Create().Dispose();

                Assert.Multiple(() =>
                {
                    using (var reader = converter.Convert(target.FullName))
                    {
                        Assert.That(reader, Is.InstanceOf<StreamReader>());
                        Assert.That(((StreamReader)reader).CurrentEncoding, Is.EqualTo(encoding));
                    }
                    Assert.That(converter.Convert(null!), Is.EqualTo(Console.In));

                    Assert.Throws<ArgumentException>(() => converter.Convert(string.Empty));
                    Assert.Throws<FileNotFoundException>(() => converter.Convert(FileUtilHelpers.GetNoExistingFile().Name));
                });
            }
            finally
            {
                target.Delete();
            }
        }

        [Test]
        public void SplitToArray_NonGeneric_WithNullElementType()
        {
            Assert.Throws<ArgumentNullException>(() => ValueConverter.SplitToArray(null!, ",", ValueConverter.GetDefault<string>()));
        }

        [Test]
        public void SplitToArray_NonGeneric_WithNullSeparator()
        {
            Assert.Throws<ArgumentNullException>(() => ValueConverter.SplitToArray(typeof(string), null!, ValueConverter.GetDefault<string>()));
        }

        [Test]
        public void SplitToArray_NonGeneric_WithEmptySeparator()
        {
            Assert.Throws<ArgumentException>(() => ValueConverter.SplitToArray(typeof(string), string.Empty, ValueConverter.GetDefault<string>()));
        }

        [Test]
        public void SplitToArray_NonGeneric_WithNullConverter()
        {
            Assert.Throws<ArgumentNullException>(() => ValueConverter.SplitToArray(typeof(string), ",", null!));
        }

        [Test]
        public void SplitToArray_NonGeneric_AsPositive()
        {
            IValueConverter<string, Array> converter = ValueConverter.SplitToArray(typeof(FileInfo), ";", ValueConverter.GetDefault<FileInfo>());

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert(string.Empty), Is.Empty);
                Assert.Multiple(() =>
                {
                    Array array = converter.Convert("a.txt");
                    Assert.That(array, Has.Length.EqualTo(1));
                    Assert.That(((FileInfo)array.GetValue(0)!).Name, Is.EqualTo("a.txt"));
                });
                Assert.Multiple(() =>
                {
                    Array array = converter.Convert("a.txt;b.txt");
                    Assert.That(array, Has.Length.EqualTo(2));
                    Assert.That(((FileInfo)array.GetValue(0)!).Name, Is.EqualTo("a.txt"));
                    Assert.That(((FileInfo)array.GetValue(1)!).Name, Is.EqualTo("b.txt"));
                });

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
            });
        }

        [Test]
        public void SplitToArray_Generic_WithNullSeperator()
        {
            Assert.Throws<ArgumentNullException>(() => ValueConverter.SplitToArray(null!, ValueConverter.GetDefault<int>()));
        }

        [Test]
        public void SplitToArray_Generic_WithEmptySeperator()
        {
            Assert.Throws<ArgumentException>(() => ValueConverter.SplitToArray(string.Empty, ValueConverter.GetDefault<int>()));
        }

        [Test]
        public void SplitToArray_Generic_WithNullConverter()
        {
            Assert.Throws<ArgumentNullException>(() => ValueConverter.SplitToArray<int>(",", null!));
        }

        [Test]
        public void SplitToArray_Generic_AsPositive()
        {
            IValueConverter<string, int[]> converter = ValueConverter.SplitToArray(",", ValueConverter.GetDefault<int>());

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert(string.Empty), Is.Empty);
                Assert.That(converter.Convert("1"), Is.EqualTo(new[] { 1 }));
                Assert.That(converter.Convert("1,-2,3"), Is.EqualTo(new[] { 1, -2, 3 }));

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
            });
        }

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
                        Assert.That(((StreamReader)reader).CurrentEncoding, Is.InstanceOf<UTF8Encoding>());
                    }
                    Assert.That(converter.Convert(null!), Is.EqualTo(Console.In));

                    Assert.Throws<ArgumentException>(() => converter.Convert(string.Empty));
                    Assert.Throws<FileNotFoundException>(() => converter.Convert(FileUtilHelpers.GetNoExistingFile().Name));
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
                        Assert.That(reader.CurrentEncoding, Is.InstanceOf<UTF8Encoding>());
                    }

                    Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                    Assert.Throws<ArgumentException>(() => converter.Convert(string.Empty));
                    Assert.Throws<FileNotFoundException>(() => converter.Convert(FileUtilHelpers.GetNoExistingFile().Name));
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
                        Assert.That(writer.Encoding, Is.InstanceOf<UTF8Encoding>());

                        target.Refresh();
                        Assert.That(target.Exists, Is.True);
                    });

                    Assert.That(converter.Convert(null!), Is.EqualTo(Console.Out));

                    Assert.Throws<ArgumentException>(() => converter.Convert(string.Empty));
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
                        Assert.That(writer.Encoding, Is.InstanceOf<UTF8Encoding>());
                        target.Refresh();
                        Assert.That(target.Exists, Is.True);
                    });

                    Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                    Assert.Throws<ArgumentException>(() => converter.Convert(string.Empty));
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

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<FormatException>(() => converter.Convert("!!"));
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

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<FormatException>(() => converter.Convert("!!"));
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

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<FormatException>(() => converter.Convert("!!"));
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

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<FormatException>(() => converter.Convert("!!"));
            });
        }

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

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<FormatException>(() => converter.Convert("!!"));
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

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<FormatException>(() => converter.Convert("!!"));
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

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<FormatException>(() => converter.Convert("!!"));
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

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<FormatException>(() => converter.Convert("!!"));
            });
        }

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
                Assert.That(converter.Convert(float.MaxValue.ToString()), Is.EqualTo(float.MaxValue));
                Assert.That(converter.Convert(float.MinValue.ToString()), Is.EqualTo(float.MinValue));

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<FormatException>(() => converter.Convert("!!"));
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
                Assert.That(converter.Convert(double.MaxValue.ToString()), Is.EqualTo(double.MaxValue));
                Assert.That(converter.Convert(double.MinValue.ToString()), Is.EqualTo(double.MinValue));

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<FormatException>(() => converter.Convert("!!"));
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

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<FormatException>(() => converter.Convert("!!"));
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

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<FormatException>(() => converter.Convert("!!"));
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

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<FormatException>(() => converter.Convert("!!"));
            });
        }

        [Test]
        public void GetDefault_AsDateOnly()
        {
            IValueConverter<string, DateOnly> converter = ValueConverter.GetDefault<DateOnly>();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("2024-05-06"), Is.EqualTo(new DateOnly(2024, 5, 6)));
                Assert.That(converter.Convert("9999-12-31"), Is.EqualTo(DateOnly.MaxValue));
                Assert.That(converter.Convert("0001-01-01"), Is.EqualTo(DateOnly.MinValue));

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<FormatException>(() => converter.Convert("!!"));
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

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<FormatException>(() => converter.Convert("!!"));
            });
        }

        [Test]
        public void GetDefault_AsTimeSpan()
        {
            IValueConverter<string, TimeSpan> converter = ValueConverter.GetDefault<TimeSpan>();

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("3.14:15:52.65"), Is.EqualTo(new TimeSpan(3, 14, 15, 52, 650)));
                Assert.That(converter.Convert("10675199.02:48:05.4775807"), Is.EqualTo(TimeSpan.MaxValue));
                Assert.That(converter.Convert("-10675199.02:48:05.4775808"), Is.EqualTo(TimeSpan.MinValue));

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<FormatException>(() => converter.Convert("!!"));
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

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<FormatException>(() => converter.Convert("!!"));
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

                Assert.Throws<ArgumentNullException>(() => converter.Convert(null!));
                Assert.Throws<ArgumentException>(() => converter.Convert("!!"));
                Assert.Throws<ArgumentException>(() => converter.Convert("none"));
                Assert.Throws<ArgumentException>(() => converter.Convert("flag"));
            });
        }

        [Test]
        public void GetDefault_AsNotSupported()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<NotSupportedException>(() => ValueConverter.GetDefault<object>());
                Assert.Throws<NotSupportedException>(() => ValueConverter.GetDefault<Enum>());
                Assert.Throws<NotSupportedException>(() => ValueConverter.GetDefault<ValueType>());
                Assert.Throws<NotSupportedException>(() => ValueConverter.GetDefault<ValueConverterTest>());
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
                Assert.That(ValueConverter.GetValueTypeString<DateOnly>(), Is.EqualTo("date"));
                Assert.That(ValueConverter.GetValueTypeString<TimeOnly>(), Is.EqualTo("time"));

                Assert.That(ValueConverter.GetValueTypeString<OptionType>(), Is.EqualTo("string"));
                Assert.That(ValueConverter.GetValueTypeString<object>(), Is.EqualTo("string"));
            });
        }
    }
}