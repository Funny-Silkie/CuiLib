using CuiLib.Converters.Implementations;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Converters.Implementations
{
#if NET6_0_OR_GREATER

    [TestFixture]
    public class TimeOnlyExactConverterTest : TestBase
    {
        private TimeOnlyExactConverter converter;

        [SetUp]
        public void SetUp()
        {
            converter = new TimeOnlyExactConverter("HH:mm:ss");
        }

        #region Ctors

        [Test]
        public void Ctor_WithNull()
        {
            Assert.That(() => new TimeOnlyExactConverter(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithEmpty()
        {
            Assert.That(() => new TimeOnlyExactConverter(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void Ctor_AsPositive()
        {
            var converter = new TimeOnlyExactConverter("HH:mm:ss");

            Assert.That(converter.Format, Is.EqualTo("HH:mm:ss"));
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
            TimeOnly value = converter.Convert("12:34:56");

            Assert.Multiple(() =>
            {
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

#endif
}
