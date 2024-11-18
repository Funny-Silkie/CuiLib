using CuiLib.Converters.Implementations;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Converters.Implementations
{
    [TestFixture]
    public class DelegateValueConverterTest : TestBase
    {
        private DelegateValueConverter<string, int> converter;

        [SetUp]
        public void SetUp()
        {
            converter = new DelegateValueConverter<string, int>(int.Parse);
        }

        #region Ctors

        [Test]
        public void Ctor_WithNull()
        {
            Assert.That(() => new DelegateValueConverter<string, int>(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_AsPositive()
        {
            var converter = new DelegateValueConverter<string, int>(int.Parse);

            Assert.That(converter.Converter, Is.EqualTo((Converter<string, int>)int.Parse));
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void Convert()
        {
            Assert.That(converter.Convert("123"), Is.EqualTo(123));
        }

        #endregion Methods
    }
}
