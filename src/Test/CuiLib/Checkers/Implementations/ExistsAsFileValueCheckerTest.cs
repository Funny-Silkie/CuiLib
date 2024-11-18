using CuiLib.Checkers.Implementations;
using NUnit.Framework;
using System.IO;
using Test.Helpers;

namespace Test.CuiLib.Checkers.Implementations
{
    [TestFixture]
    public class ExistsAsFileValueCheckerTest : TestBase
    {
        private ExistsAsFileValueChecker checker;

        [SetUp]
        public void SetUp()
        {
            checker = new ExistsAsFileValueChecker();
        }

        #region Ctors

        [Test]
        public void Ctor()
        {
            Assert.That(() => new ExistsAsFileValueChecker(), Throws.Nothing);
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
                FileInfo missing = FileUtilHelpers.GetNoExistingFile();

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
        public void ExistsAsFile_Equals()
        {
            Assert.That(checker, Is.EqualTo(new ExistsAsFileValueChecker()));
        }

        [Test]
        public void ExistsAsFile_GetHashCode()
        {
            Assert.That(checker.GetHashCode(), Is.EqualTo(new ExistsAsFileValueChecker().GetHashCode()));
        }

        #endregion Methods
    }
}
