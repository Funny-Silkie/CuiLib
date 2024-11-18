using CuiLib.Converters.Implementations;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Converters.Implementations
{
#if NET6_0_OR_GREATER

    [TestFixture]
    public class DateOnlyExactConverterTest : TestBase
    {
        private DateOnlyExactConverter converter;

        [SetUp]
        public void SetUp()
        {
            converter = new DateOnlyExactConverter("yyyy/MM/dd");
        }

        #region Ctors

        [Test]
        public void Ctor_WithNull()
        {
            Assert.That(() => new DateOnlyExactConverter(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithEmpty()
        {
            Assert.That(() => new DateOnlyExactConverter(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void Ctor_AsPositive()
        {
            var converter = new DateOnlyExactConverter("yyyy/MM/dd");

            Assert.That(converter.Format, Is.EqualTo("yyyy/MM/dd"));
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void Convert_WithInvalidFormat()
        {
            Assert.That(() => converter.Convert("!!!"), Throws.TypeOf<FormatException>());
        }

        [Test]
        public void Convert_AsPositive()
        {
            DateOnly value = converter.Convert("2000/01/01");

            Assert.Multiple(() =>
            {
                Assert.That(value.Year, Is.EqualTo(2000));
                Assert.That(value.Month, Is.EqualTo(1));
                Assert.That(value.Day, Is.EqualTo(1));
            });
        }

        #endregion Methods
    }

#endif
}
