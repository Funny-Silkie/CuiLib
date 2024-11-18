using CuiLib;
using CuiLib.Converters;
using CuiLib.Converters.Implementations;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using Test.Helpers;

namespace Test.CuiLib.Converters.Implementations
{
    [TestFixture]
    public class FileOrConsoleWriterValueConverterTest : TestBase
    {
        private FileOrConsoleWriterValueConverter overwriteConverter;
        private FileOrConsoleWriterValueConverter appendConverter;

        [SetUp]
        public void SetUp()
        {
            overwriteConverter = new FileOrConsoleWriterValueConverter(IOHelpers.UTF8N, false);
            appendConverter = new FileOrConsoleWriterValueConverter(IOHelpers.UTF8N, true);
        }

        #region Ctors

        [Test]
        public void Ctor_WithNullEncoding()
        {
            Assert.That(() => new FileOrConsoleWriterValueConverter(null!, false), Throws.ArgumentNullException);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Ctor_AsPositive(bool append)
        {
            var converter = new FileOrConsoleWriterValueConverter(Encoding.UTF8, append);

            Assert.Multiple(() =>
            {
                Assert.That(converter.Encoding, Is.EqualTo(Encoding.UTF8));
                Assert.That(converter.Append, Is.EqualTo(append));
            });
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void Convert_WithEmpty()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => overwriteConverter.Convert(string.Empty), Throws.ArgumentException);
                Assert.That(() => appendConverter.Convert(string.Empty), Throws.ArgumentException);
            });
        }

        [TestCase(null!)]
        [TestCase("-")]
        public void Convert_AsPositive_OnConsole(string value)
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => overwriteConverter.Convert(value), Is.EqualTo(Console.Out));
                Assert.That(() => appendConverter.Convert(value), Is.EqualTo(Console.Out));
            });
        }

        [Test]
        public void Convert_AsPositive_OnNoAppending()
        {
            FileInfo target = FileUtilHelpers.GetNoExistingFile();
            using (StreamWriter writer = target.CreateText()) writer.Write("hoge");

            try
            {
                using TextWriter writer = overwriteConverter.Convert(target.Name);
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
                using TextWriter writer = appendConverter.Convert(target.Name);
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
