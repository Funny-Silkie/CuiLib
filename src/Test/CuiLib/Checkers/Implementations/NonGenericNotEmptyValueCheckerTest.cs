using CuiLib.Checkers.Implementations;
using NUnit.Framework;

namespace Test.CuiLib.Checkers.Implementations
{
    [TestFixture]
    public class NonGenericNotEmptyValueCheckerTest : TestBase
    {
        private NotEmptyValueChecker checker;

        [SetUp]
        public void SetUp()
        {
            checker = new NotEmptyValueChecker();
        }

        #region Ctors

        [Test]
        public void Ctor()
        {
            Assert.That(() => new NotEmptyValueChecker(), Throws.Nothing);
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void CheckValue()
        {
            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(null).IsValid, Is.False);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.False);
                Assert.That(checker.CheckValue("not-empty").IsValid, Is.True);
                Assert.That(checker.CheckValue("  ").IsValid, Is.True);
            });
        }

        [Test]
        public void Equals()
        {
            Assert.That(checker, Is.EqualTo(new NotEmptyValueChecker()));
        }

        [Test]
        public new void GetHashCode()
        {
            Assert.That(checker.GetHashCode(), Is.EqualTo(new NotEmptyValueChecker().GetHashCode()));
        }

        #endregion Methods
    }
}
