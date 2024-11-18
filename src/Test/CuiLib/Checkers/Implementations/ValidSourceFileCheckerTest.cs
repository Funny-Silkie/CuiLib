using CuiLib.Checkers.Implementations;
using NUnit.Framework;
using System.IO;
using Test.Helpers;

namespace Test.CuiLib.Checkers.Implementations
{
    [TestFixture]
    public class ValidSourceFileCheckerTest : TestBase
    {
        private ValidSourceFileChecker checker;

        [SetUp]
        public void SetUp()
        {
            checker = new ValidSourceFileChecker();
        }

        #region Ctors

        [Test]
        public void Ctor()
        {
            Assert.That(() => new ValidSourceFileChecker(), Throws.Nothing);
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void CheckValue()
        {
            FileInfo existing = FileUtilHelpers.GetNoExistingFile();
            try
            {
                existing.Create().Dispose();
                existing.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(checker.CheckValue(existing).IsValid, Is.True);
                    Assert.That(checker.CheckValue(new FileInfo(Path.Combine(FileUtilHelpers.GetNoExistingDirectory().FullName, "missing.txt"))).IsValid, Is.False);
                    Assert.That(checker.CheckValue(FileUtilHelpers.GetNoExistingFile()).IsValid, Is.False);

                    Assert.That(() => checker.CheckValue(null!), Throws.ArgumentNullException);
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
