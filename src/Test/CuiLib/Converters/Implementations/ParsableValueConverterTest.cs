using CuiLib.Converters.Implementations;
using NUnit.Framework;

namespace Test.CuiLib.Converters.Implementations
{
#if NET7_0_OR_GREATER

    [TestFixture]
    public class ParsableValueConverterTest : TestBase
    {
        private ParsableValueConverter<int> converter;

        [SetUp]
        public void SetUp()
        {
            converter = new ParsableValueConverter<int>();
        }

        #region Ctors

        [Test]
        public void Ctor()
        {
            Assert.That(() => new ParsableValueConverter<int>(), Throws.Nothing);
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void Convert_WithNull()
        {
            Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Convert_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("123"), Is.EqualTo(123));
                Assert.That(converter.Convert("-8000"), Is.EqualTo(-8000));
            });
        }

        [Test]
        public void Equals()
        {
            Assert.That(new ParsableValueConverter<int>(), Is.EqualTo(converter));
        }

        [Test]
        public new void GetHashCode()
        {
            Assert.That(new ParsableValueConverter<int>().GetHashCode(), Is.EqualTo(converter.GetHashCode()));
        }

        #endregion Methods
    }

#endif
}
