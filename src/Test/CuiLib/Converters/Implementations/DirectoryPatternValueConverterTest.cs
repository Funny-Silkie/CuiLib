using CuiLib.Converters.Implementations;
using CuiLib.Internal.Versions;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace Test.CuiLib.Converters.Implementations
{
    [TestFixture]
    public class DirectoryPatternValueConverterTest : TestBase
    {
        private DirectoryPatternValueConverter converter;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            foreach (string current in Enumerable.Range(4, 3).Select(x => $"{x}.test.tmp"))
                if (!Directory.Exists(current))
                    Directory.CreateDirectory(current);
        }

        [SetUp]
        public void SetUp()
        {
            converter = new DirectoryPatternValueConverter();
        }

        #region Ctors

        [Test]
        public void Ctor()
        {
            Assert.That(() => new DirectoryPatternValueConverter(), Throws.Nothing);
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
            Assert.That(converter.Convert("*.test.tmp").Select(x => x.Name).Order(), Is.EqualTo(new[] { "4.test.tmp", "5.test.tmp", "6.test.tmp" }));
        }

        [Test]
        public void Equals()
        {
            Assert.That(converter, Is.EqualTo(new DirectoryPatternValueConverter()));
        }

        [Test]
        public new void GetHashCode()
        {
            Assert.That(converter.GetHashCode(), Is.EqualTo(new DirectoryPatternValueConverter().GetHashCode()));
        }

        #endregion Methods
    }
}
