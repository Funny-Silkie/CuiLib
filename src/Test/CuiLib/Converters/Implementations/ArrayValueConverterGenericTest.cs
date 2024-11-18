using CuiLib.Converters;
using CuiLib.Converters.Implementations;
using NUnit.Framework;
using System;
using System.IO;

namespace Test.CuiLib.Converters.Implementations
{
    [TestFixture]
    public class ArrayValueConverterGenericTest : TestBase
    {
        private ArrayValueConverter<int> converter;

        [SetUp]
        public void SetUp()
        {
            converter = new ArrayValueConverter<int>(",", ValueConverter.GetDefault<int>(), StringSplitOptions.None);
        }

        #region Ctors

        [Test]
        public void Ctor_WithNullSeparator()
        {
            Assert.That(() => new ArrayValueConverter<string>(null!, ValueConverter.GetDefault<string>(), StringSplitOptions.None), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithEmptySeparator()
        {
            Assert.That(() => new ArrayValueConverter<string>(string.Empty, ValueConverter.GetDefault<string>(), StringSplitOptions.None), Throws.ArgumentException);
        }

        [Test]
        public void Ctor_WithNullConverter()
        {
            Assert.That(() => new ArrayValueConverter<string>(",", null!, StringSplitOptions.None), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_AsPositive()
        {
            IValueConverter<string, int> innerConverter = ValueConverter.GetDefault<int>();
            var converter = new ArrayValueConverter<int>(",", innerConverter, StringSplitOptions.None);

            Assert.Multiple(() =>
            {
                Assert.That(converter.Separator, Is.EqualTo(","));
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
            Assert.That(converter.Convert("1"), Is.EqualTo(new[] { 1 }));
        }

        [Test]
        public void Convert_AsPositive_OnMultipleElement()
        {
            Assert.That(converter.Convert("1,-2,3"), Is.EqualTo(new[] { 1, -2, 3 }));
        }

        #endregion Methods
    }
}
