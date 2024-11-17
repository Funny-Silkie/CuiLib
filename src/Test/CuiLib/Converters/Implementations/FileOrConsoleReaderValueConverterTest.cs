using CuiLib;
using CuiLib.Converters.Implementations;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using Test.Helpers;

namespace Test.CuiLib.Converters.Implementations
{
    [TestFixture]
    public class FileOrConsoleReaderValueConverterTest : TestBase
    {
        private FileOrConsoleReaderValueConverter converter;

        [SetUp]
        public void SetUp()
        {
            converter = new FileOrConsoleReaderValueConverter(IOHelpers.UTF8N);
        }

        #region Ctors

        [Test]
        public void Ctor_WithNullEncoding()
        {
            Assert.That(() => new FileOrConsoleReaderValueConverter(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_AsPositive()
        {
            var converter = new FileOrConsoleReaderValueConverter(Encoding.UTF32);

            Assert.That(converter.Encoding, Is.EqualTo(Encoding.UTF32));
        }

        #endregion Ctors

        #region Methods

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

        [TestCase(null!)]
        [TestCase("-")]
        public void Convert_AsPositive_OnConsole(string value)
        {
            Assert.That(() => converter.Convert(value), Is.EqualTo(Console.In));
        }

        [Test]
        public void Convert_AsPositive()
        {
            FileInfo target = FileUtilHelpers.GetNoExistingFile();

            try
            {
                target.Create().Dispose();
                using var reader = converter.Convert(target.FullName);

                Assert.Multiple(() =>
                {
                    Assert.That(reader, Is.InstanceOf<StreamReader>());
                    Assert.That(((StreamReader)reader).CurrentEncoding, Is.EqualTo(IOHelpers.UTF8N));
                });
            }
            finally
            {
                target.Delete();
            }
        }

        #endregion Methods
    }
}
