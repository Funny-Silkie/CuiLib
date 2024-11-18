using CuiLib.Converters.Implementations;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Converters.Implementations
{
    [TestFixture]
    public class TimeSpanExactConverterTest : TestBase
    {
        private TimeSpanExactConverter converter;

        [SetUp]
        public void SetUp()
        {
            converter = new TimeSpanExactConverter(@"d\.hh\:mm\:ss");
        }

        #region Ctors

        [Test]
        public void Ctor_WithNull()
        {
            Assert.That(() => new TimeSpanExactConverter(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithEmpty()
        {
            Assert.That(() => new TimeSpanExactConverter(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void Ctor_AsPositive()
        {
            var converter = new TimeSpanExactConverter(@"d\.hh\:mm\:ss");

            Assert.That(converter.Format, Is.EqualTo(@"d\.hh\:mm\:ss"));
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
            TimeSpan value = converter.Convert("1.12:34:56");

            Assert.Multiple(() =>
            {
                Assert.That(value.Days, Is.EqualTo(1));
                Assert.That(value.Hours, Is.EqualTo(12));
                Assert.That(value.Minutes, Is.EqualTo(34));
                Assert.That(value.Seconds, Is.EqualTo(56));
                Assert.That(value.Milliseconds, Is.EqualTo(0));
#if NET7_0_OR_GREATER
                Assert.That(value.Microseconds, Is.EqualTo(0));
#endif
            });
        }

        #endregion Methods
    }
}
