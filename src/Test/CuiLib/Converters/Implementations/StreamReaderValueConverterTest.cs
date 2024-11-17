using CuiLib;
using CuiLib.Converters.Implementations;
using NUnit.Framework;
using System.IO;
using System.Text;
using Test.Helpers;

namespace Test.CuiLib.Converters.Implementations
{
    [TestFixture]
    public class StreamReaderValueConverterTest : TestBase
    {
        private StreamReaderValueConverter converter;

        [SetUp]
        public void SetUp()
        {
            converter = new StreamReaderValueConverter(IOHelpers.UTF8N);
        }

        #region Ctors

        [Test]
        public void Ctor_WithNullEncoding()
        {
            Assert.That(() => new StreamReaderValueConverter(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_AsPositive()
        {
            var converter = new StreamReaderValueConverter(Encoding.UTF32);

            Assert.That(converter.Encoding, Is.EqualTo(Encoding.UTF32));
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
        public void Convert_WithMissingFile()
        {
            Assert.That(() => converter.Convert(FileUtilHelpers.GetNoExistingFile().Name), Throws.TypeOf<FileNotFoundException>());
        }

        [Test]
        public void Convert_AsPositive()
        {
            FileInfo target = FileUtilHelpers.GetNoExistingFile();

            try
            {
                target.Create().Dispose();

                using StreamReader reader = converter.Convert(target.FullName);
                Assert.That(reader.CurrentEncoding, Is.EqualTo(IOHelpers.UTF8N));
            }
            finally
            {
                target.Delete();
            }
        }

        #endregion Methods
    }
}
