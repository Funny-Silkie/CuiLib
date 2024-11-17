using CuiLib;
using CuiLib.Converters.Implementations;
using NUnit.Framework;
using System.IO;
using System.Text;
using Test.Helpers;

namespace Test.CuiLib.Converters.Implementations
{
    [TestFixture]
    public class StreamWriterValueConverterTest : TestBase
    {
        private StreamWriterValueConverter overwriteConverter;
        private StreamWriterValueConverter appendConverter;

        [SetUp]
        public void SetUp()
        {
            overwriteConverter = new StreamWriterValueConverter(IOHelpers.UTF8N, false);
            appendConverter = new StreamWriterValueConverter(IOHelpers.UTF8N, true);
        }

        #region Ctors

        [Test]
        public void Ctor_WithNullEncoding()
        {
            Assert.That(() => new StreamWriterValueConverter(null!, false), Throws.ArgumentNullException);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Ctor_AsPositive(bool append)
        {
            var converter = new StreamWriterValueConverter(Encoding.UTF8, append);

            Assert.Multiple(() =>
            {
                Assert.That(converter.Encoding, Is.EqualTo(Encoding.UTF8));
                Assert.That(converter.Append, Is.EqualTo(append));
            });
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void Convert_WithNull()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => overwriteConverter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => appendConverter.Convert(null!), Throws.ArgumentNullException);
            });
        }

        [Test]
        public void Convert_WithEmpty()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => overwriteConverter.Convert(string.Empty), Throws.ArgumentException);
                Assert.That(() => appendConverter.Convert(string.Empty), Throws.ArgumentException);
            });
        }

        [Test]
        public void Convert_AsPositive_OnNoAppending()
        {
            FileInfo target = FileUtilHelpers.GetNoExistingFile();
            using (StreamWriter writer = target.CreateText()) writer.Write("hoge");

            try
            {
                using StreamWriter writer = overwriteConverter.Convert(target.Name);
                Assert.That(writer.Encoding, Is.EqualTo(IOHelpers.UTF8N));

                target.Refresh();
                Assert.That(target.Length, Is.Zero);
            }
            finally
            {
                target.Delete();
            }
        }

        [Test]
        public void Convert_AsPositive_OnAppending()
        {
            FileInfo target = FileUtilHelpers.GetNoExistingFile();
            using (StreamWriter writer = target.CreateText()) writer.Write("hoge");

            try
            {
                using StreamWriter writer = appendConverter.Convert(target.Name);
                Assert.That(writer.Encoding, Is.EqualTo(IOHelpers.UTF8N));

                target.Refresh();
                Assert.That(target.Length, Is.GreaterThan(0));
            }
            finally
            {
                target.Delete();
            }
        }

        #endregion Methods
    }
}
