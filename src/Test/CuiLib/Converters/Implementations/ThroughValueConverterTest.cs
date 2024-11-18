using CuiLib.Converters.Implementations;
using NUnit.Framework;

namespace Test.CuiLib.Converters.Implementations
{
    [TestFixture]
    public class ThroughValueConverterTest : TestBase
    {
        private ThroughValueConverter<string> converter;

        [SetUp]
        public void SetUp()
        {
            converter = new ThroughValueConverter<string>();
        }

        #region Ctors

        [Test]
        public void Ctor()
        {
            Assert.That(() => new ThroughValueConverter<string>(), Throws.Nothing);
        }

        #endregion Ctors

        #region Methods

        [TestCase(null!)]
        [TestCase("")]
        [TestCase("hoge")]
        public void Convert(string value)
        {
            Assert.That(converter.Convert(value), Is.EqualTo(value));
        }

        [Test]
        public void Equals()
        {
            Assert.That(converter, Is.EqualTo(new ThroughValueConverter<string>()));
        }

        [Test]
        public new void GetHashCode()
        {
            Assert.That(converter.GetHashCode(), Is.EqualTo(new ThroughValueConverter<string>().GetHashCode()));
        }

        #endregion Methods
    }
}
