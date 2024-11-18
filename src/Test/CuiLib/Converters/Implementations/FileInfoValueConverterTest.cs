using CuiLib.Converters.Implementations;
using NUnit.Framework;

namespace Test.CuiLib.Converters.Implementations
{
    [TestFixture]
    public class FileInfoValueConverterTest : TestBase
    {
        private FileInfoValueConverter converter;

        [SetUp]
        public void SetUp()
        {
            converter = new FileInfoValueConverter();
        }

        #region Ctors

        [Test]
        public void Ctor()
        {
            Assert.That(() => new FileInfoValueConverter(), Throws.Nothing);
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
            Assert.That(converter.Convert("test.txt").Name, Is.EqualTo("test.txt"));
        }

        [Test]
        public void Equals()
        {
            Assert.That(converter, Is.EqualTo(new FileInfoValueConverter()));
        }

        [Test]
        public new void GetHashCode()
        {
            Assert.That(converter.GetHashCode(), Is.EqualTo(new FileInfoValueConverter().GetHashCode()));
        }

        #endregion Methods
    }
}
