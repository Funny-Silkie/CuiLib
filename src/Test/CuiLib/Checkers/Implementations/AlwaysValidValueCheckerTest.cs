using CuiLib.Checkers;
using CuiLib.Checkers.Implementations;
using NUnit.Framework;

namespace Test.CuiLib.Checkers.Implementations
{
    [TestFixture]
    public class AlwaysValidValueCheckerTest : TestBase
    {
        private AlwaysValidValueChecker<int> checker;

        [SetUp]
        public void SetUp()
        {
            checker = new AlwaysValidValueChecker<int>();
        }

        #region Ctors

        [Test]
        public void Ctor()
        {
            Assert.That(() => new AlwaysValidValueChecker<int>(), Throws.Nothing);
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void CheckValue()
        {
            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(1).IsValid, Is.True);
                Assert.That(checker.CheckValue(0).IsValid, Is.True);
                Assert.That(checker.CheckValue(-1).IsValid, Is.True);
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.True);
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.True);
            });
        }

        [Test]
        public void Equals()
        {
            Assert.That(checker, Is.EqualTo(new AlwaysValidValueChecker<int>()));
        }

        [Test]
        public new void GetHashCode()
        {
            Assert.That(checker.GetHashCode(), Is.EqualTo(new AlwaysValidValueChecker<int>().GetHashCode()));
        }

        #endregion Methods
    }
}
