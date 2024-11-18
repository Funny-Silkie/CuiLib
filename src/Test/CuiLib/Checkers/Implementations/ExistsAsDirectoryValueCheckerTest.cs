using CuiLib.Checkers.Implementations;
using NUnit.Framework;
using System.IO;
using Test.Helpers;

namespace Test.CuiLib.Checkers.Implementations
{
    [TestFixture]
    public class ExistsAsDirectoryValueCheckerTest : TestBase
    {
        private ExistsAsDirectoryValueChecker checker;

        [SetUp]
        public void SetUp()
        {
            checker = new ExistsAsDirectoryValueChecker();
        }

        #region Ctors

        [Test]
        public void Ctor()
        {
            Assert.That(() => new ExistsAsDirectoryValueChecker(), Throws.Nothing);
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
                DirectoryInfo missing = FileUtilHelpers.GetNoExistingDirectory();

                Assert.Multiple(() =>
                {
                    Assert.That(checker.CheckValue(existing.FullName).IsValid, Is.True);
                    Assert.That(checker.CheckValue(missing.FullName).IsValid, Is.False);
                });
            }
            finally
            {
                existing.Delete();
            }
        }

        [Test]
        public void ExistsAsDirectory_Equals()
        {
            Assert.That(checker, Is.EqualTo(new ExistsAsDirectoryValueChecker()));
        }

        [Test]
        public void ExistsAsDirectory_GetHashCode()
        {
            Assert.That(checker.GetHashCode(), Is.EqualTo(new ExistsAsDirectoryValueChecker().GetHashCode()));
        }

        #endregion Methods
    }
}
