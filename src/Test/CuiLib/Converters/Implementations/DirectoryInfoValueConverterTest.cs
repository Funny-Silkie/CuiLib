using CuiLib.Converters.Implementations;
using NUnit.Framework;

namespace Test.CuiLib.Converters.Implementations
{
    [TestFixture]
    public class DirectoryInfoValueConverterTest : TestBase
    {
        private DirectoryInfoValueConverter converter;

        [SetUp]
        public void SetUp()
        {
            converter = new DirectoryInfoValueConverter();
        }

        #region Ctors

        [Test]
        public void Ctor()
        {
            Assert.That(() => new DirectoryInfoValueConverter(), Throws.Nothing);
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
            Assert.That(() => converter.Convert(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void Convert_AsPositive()
        {
            Assert.That(converter.Convert("folder").Name, Is.EqualTo("folder"));
        }

        [Test]
        public void Equals()
        {
            Assert.That(converter, Is.EqualTo(new DirectoryInfoValueConverter()));
        }

        [Test]
        public new void GetHashCode()
        {
            Assert.That(converter.GetHashCode(), Is.EqualTo(new DirectoryInfoValueConverter().GetHashCode()));
        }

        #endregion Methods
    }
}
