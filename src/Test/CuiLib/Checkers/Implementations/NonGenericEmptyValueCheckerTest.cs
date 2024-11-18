using CuiLib.Checkers.Implementations;
using NUnit.Framework;

namespace Test.CuiLib.Checkers.Implementations
{
    [TestFixture]
    public class NonGenericEmptyValueCheckerTest : TestBase
    {
        private EmptyValueChecker checker;

        [SetUp]
        public void SetUp()
        {
            checker = new EmptyValueChecker();
        }

        #region Ctors

        [Test]
        public void Ctor()
        {
            Assert.That(() => new EmptyValueChecker(), Throws.Nothing);
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void CheckValue()
        {
            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(null).IsValid, Is.True);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.True);
                Assert.That(checker.CheckValue("not-empty").IsValid, Is.False);
                Assert.That(checker.CheckValue("  ").IsValid, Is.False);
            });
        }

        [Test]
        public void Equals()
        {
            Assert.That(checker, Is.EqualTo(new EmptyValueChecker()));
        }

        [Test]
        public new void GetHashCode()
        {
            Assert.That(checker.GetHashCode(), Is.EqualTo(new EmptyValueChecker().GetHashCode()));
        }

        #endregion Methods
    }
}
