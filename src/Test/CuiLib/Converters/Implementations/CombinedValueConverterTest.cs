using CuiLib.Converters;
using CuiLib.Converters.Implementations;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Converters.Implementations
{
    [TestFixture]
    public class CombinedValueConverterTest : TestBase
    {
        private CombinedValueConverter<string, int, int> converter;
        private IValueConverter<string, int> first;
        private IValueConverter<int, int> second;

        [SetUp]
        public void SetUp()
        {
            first = ValueConverter.GetDefault<int>();
            second = ValueConverter.FromDelegate<int, int>(x => checked(x * 2));
            converter = new CombinedValueConverter<string, int, int>(first, second);
        }

        #region Ctors

        [Test]
        public void Ctor_WithNull()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => new CombinedValueConverter<string, int, long>(ValueConverter.GetDefault<int>(), null!), Throws.ArgumentNullException);
                Assert.That(() => new CombinedValueConverter<object, string, int>(null!, ValueConverter.GetDefault<int>()), Throws.ArgumentNullException);
            });
        }

        [Test]
        public void Ctor_AsPositive()
        {
            Assert.That(() => new CombinedValueConverter<string, int, int>(first, second), Throws.Nothing);
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void Convert_AsErrorAtFirstConversion()
        {
            Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Convert_AsErrorAtSecondConversion()
        {
            Assert.That(() => converter.Convert(int.MaxValue.ToString()), Throws.TypeOf<OverflowException>());
        }

        [Test]
        public void Convert_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("1"), Is.EqualTo(2));
                Assert.That(converter.Convert("0"), Is.EqualTo(0));
                Assert.That(converter.Convert("-100"), Is.EqualTo(-200));
            });
        }

        #endregion Methods
    }
}
