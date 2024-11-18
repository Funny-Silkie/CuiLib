using CuiLib.Converters.Implementations;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Converters.Implementations
{
    [TestFixture]
    public class DateTimeOffsetExactConverterTest : TestBase
    {
        private DateTimeOffsetExactConverter converter;

        [SetUp]
        public void SetUp()
        {
            converter = new DateTimeOffsetExactConverter("yyyy/MM/dd HH:mm:ss");
        }

        #region Ctors

        [Test]
        public void Ctor_WithNull()
        {
            Assert.That(() => new DateTimeOffsetExactConverter(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithEmpty()
        {
            Assert.That(() => new DateTimeOffsetExactConverter(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void Ctor_AsPositive()
        {
            var converter = new DateTimeOffsetExactConverter("yyyy/MM/dd HH:mm:ss");

            Assert.That(converter.Format, Is.EqualTo("yyyy/MM/dd HH:mm:ss"));
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
            DateTimeOffset value = converter.Convert("2000/01/01 12:34:56");

            Assert.Multiple(() =>
            {
                Assert.That(value.Year, Is.EqualTo(2000));
                Assert.That(value.Month, Is.EqualTo(1));
                Assert.That(value.Day, Is.EqualTo(1));
                Assert.That(value.Hour, Is.EqualTo(12));
                Assert.That(value.Minute, Is.EqualTo(34));
                Assert.That(value.Second, Is.EqualTo(56));
                Assert.That(value.Millisecond, Is.EqualTo(0));
#if NET7_0_OR_GREATER
                Assert.That(value.Microsecond, Is.EqualTo(0));
#endif
            });
        }

        #endregion Methods
    }
}
