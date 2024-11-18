using CuiLib.Checkers.Implementations;
using NUnit.Framework;
using System.IO;
using Test.Helpers;

namespace Test.CuiLib.Checkers.Implementations
{
    [TestFixture]
    public class ValidDestinationFileCheckerTest : TestBase
    {
        private ValidDestinationFileChecker checker;

        [SetUp]
        public void SetUp()
        {
            checker = new ValidDestinationFileChecker();
        }

        #region Ctors

        [Test]
        public void Ctor()
        {
            Assert.That(() => new ValidDestinationFileChecker(), Throws.Nothing);
        }

        #endregion Ctors

        #region Properties

        [Test]
        public void AllowMissedDirectory_Get_OnDefault()
        {
            Assert.That(checker.AllowMissedDirectory, Is.True);
        }

        [Test]
        public void AllowOverwrite_Get_OnDefault()
        {
            Assert.That(checker.AllowOverwrite, Is.True);
        }

        #endregion Properties

        #region Methods

        [Test]
        public void CheckValue_WithNull()
        {
            Assert.That(() => checker.CheckValue(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void CheckValue_AsPositive_OnAllowAll()
        {
            FileInfo existing = FileUtilHelpers.GetNoExistingFile();
            try
            {
                existing.Create().Dispose();

                Assert.Multiple(() =>
                {
                    Assert.That(checker.CheckValue(existing).IsValid, Is.True);
                    Assert.That(checker.CheckValue(new FileInfo(Path.Combine(FileUtilHelpers.GetNoExistingDirectory().FullName, "missing.txt"))).IsValid, Is.True);
                    Assert.That(checker.CheckValue(FileUtilHelpers.GetNoExistingFile()).IsValid, Is.True);
                });
            }
            finally
            {
                existing.Delete();
            }
        }

        [Test]
        public void CheckValue_AsPositive_OnNotAllowMissedDir()
        {
            checker.AllowMissedDirectory = false;

            FileInfo existing = FileUtilHelpers.GetNoExistingFile();
            try
            {
                existing.Create().Dispose();

                Assert.Multiple(() =>
                {
                    Assert.That(checker.CheckValue(existing).IsValid, Is.True);
                    Assert.That(checker.CheckValue(new FileInfo(Path.Combine(FileUtilHelpers.GetNoExistingDirectory().FullName, "missing.txt"))).IsValid, Is.False);
                    Assert.That(checker.CheckValue(FileUtilHelpers.GetNoExistingFile()).IsValid, Is.True);
                });
            }
            finally
            {
                existing.Delete();
            }
        }

        [Test]
        public void CheckValue_AsPositive_OnNotAllowOverwrite()
        {
            checker.AllowOverwrite = false;

            FileInfo existing = FileUtilHelpers.GetNoExistingFile();
            try
            {
                existing.Create().Dispose();
                existing.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(checker.CheckValue(existing).IsValid, Is.False);
                    Assert.That(checker.CheckValue(new FileInfo(Path.Combine(FileUtilHelpers.GetNoExistingDirectory().FullName, "missing.txt"))).IsValid, Is.True);
                    Assert.That(checker.CheckValue(FileUtilHelpers.GetNoExistingFile()).IsValid, Is.True);
                });
            }
            finally
            {
                existing.Delete();
            }
        }

        #endregion Methods
    }
}
