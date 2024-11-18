using CuiLib.Checkers.Implementations;
using NUnit.Framework;
using System.IO;
using Test.Helpers;

namespace Test.CuiLib.Checkers.Implementations
{
    [TestFixture]
    public class ValidSourceDirectoryCheckerTest : TestBase
    {
        private ValidSourceDirectoryChecker checker;

        [SetUp]
        public void SetUp()
        {
            checker = new ValidSourceDirectoryChecker();
        }

        #region Ctors

        [Test]
        public void Ctor()
        {
            Assert.That(() => new ValidSourceDirectoryChecker(), Throws.Nothing);
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void CheckValue()
        {
            DirectoryInfo existing = FileUtilHelpers.GetNoExistingDirectory();
            try
            {
                existing.Create();
                existing.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(checker.CheckValue(existing).IsValid, Is.True);
                    Assert.That(checker.CheckValue(new DirectoryInfo(Path.Combine(FileUtilHelpers.GetNoExistingDirectory().FullName, "missing.txt"))).IsValid, Is.False);
                    Assert.That(checker.CheckValue(FileUtilHelpers.GetNoExistingDirectory()).IsValid, Is.False);

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
