using CuiLib.Converters;
using CuiLib.Converters.Implementations;
using NUnit.Framework;
using System;
using System.IO;

namespace Test.CuiLib.Converters.Implementations
{
    [TestFixture]
    public class ArrayValueConverterNonGenericTest : TestBase
    {
        private ArrayValueConverter converter;

        [SetUp]
        public void SetUp()
        {
            converter = new ArrayValueConverter(";", typeof(FileInfo), ValueConverter.GetDefault<FileInfo>(), StringSplitOptions.None);
        }

        #region Ctors

        [Test]
        public void Ctor_WithNullSeparator()
        {
            Assert.That(() => new ArrayValueConverter(null!, typeof(string), ValueConverter.GetDefault<string>(), StringSplitOptions.None), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithEmptySeparator()
        {
            Assert.That(() => new ArrayValueConverter(string.Empty, typeof(string), ValueConverter.GetDefault<string>(), StringSplitOptions.None), Throws.ArgumentException);
        }

        [Test]
        public void Ctor_WithNullConverter()
        {
            Assert.That(() => new ArrayValueConverter(",", typeof(string), null!, StringSplitOptions.None), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithNullElementType()
        {
            Assert.That(() => new ArrayValueConverter(",", null!, ValueConverter.GetDefault<string>(), StringSplitOptions.None), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_AsPositive()
        {
            IValueConverter<string, FileInfo> innerConverter = ValueConverter.GetDefault<FileInfo>();
            var converter = new ArrayValueConverter(";", typeof(FileInfo), innerConverter, StringSplitOptions.None);

            Assert.Multiple(() =>
            {
                Assert.That(converter.Separator, Is.EqualTo(";"));
                Assert.That(converter.ElementType, Is.EqualTo(typeof(FileInfo)));
                Assert.That(converter.ElementConverter, Is.EqualTo(innerConverter));
                Assert.That(converter.SplitOptions, Is.EqualTo(StringSplitOptions.None));
            });
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void Convert_WithNull()
        {
            Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Convert_WithEmpty()
        {
            Assert.That(converter.Convert(string.Empty), Is.Empty);
        }

        [Test]
        public void Convert_AsPositive_OnSingleElement()
        {
            Array array = converter.Convert("a.txt");

            Assert.Multiple(() =>
            {
                Assert.That(array, Has.Length.EqualTo(1));
                Assert.That(((FileInfo)array.GetValue(0)!).Name, Is.EqualTo("a.txt"));
            });
        }

        [Test]
        public void Convert_AsPositive_OnMultipleElement()
        {
            Array array = converter.Convert("a.txt;b.txt");

            Assert.Multiple(() =>
            {
                Assert.That(array, Has.Length.EqualTo(2));
                Assert.That(((FileInfo)array.GetValue(0)!).Name, Is.EqualTo("a.txt"));
                Assert.That(((FileInfo)array.GetValue(1)!).Name, Is.EqualTo("b.txt"));
            });
        }

        #endregion Methods
    }
}
