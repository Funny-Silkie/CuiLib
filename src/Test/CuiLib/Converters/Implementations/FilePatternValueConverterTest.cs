using CuiLib.Converters.Implementations;
using CuiLib.Internal.Versions;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace Test.CuiLib.Converters.Implementations
{
    [TestFixture]
    public class FilePatternValueConverterTest : TestBase
    {
        private FilePatternValueConverter converter;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            foreach (string current in Enumerable.Range(1, 3).Select(x => $"{x}.test.tmp"))
                if (!File.Exists(current))
                    File.Create(current).Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            converter = new FilePatternValueConverter();
        }

        #region Ctors

        [Test]
        public void Ctor()
        {
            Assert.That(() => new FilePatternValueConverter(), Throws.Nothing);
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
            Assert.That(converter.Convert("*.test.tmp").Select(x => x.Name).Order(), Is.EqualTo(new[] { "1.test.tmp", "2.test.tmp", "3.test.tmp" }));
        }

        [Test]
        public void Equals()
        {
            Assert.That(converter, Is.EqualTo(new FilePatternValueConverter()));
        }

        [Test]
        public new void GetHashCode()
        {
            Assert.That(converter.GetHashCode(), Is.EqualTo(new FilePatternValueConverter().GetHashCode()));
        }

        #endregion Methods
    }
}
