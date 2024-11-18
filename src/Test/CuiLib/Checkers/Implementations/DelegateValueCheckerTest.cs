using CuiLib.Checkers;
using CuiLib.Checkers.Implementations;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Checkers.Implementations
{
    [TestFixture]
    public class DelegateValueCheckerTest : TestBase
    {
        private DelegateValueChecker<int> checker;

        [SetUp]
        public void SetUp()
        {
            checker = new DelegateValueChecker<int>(x => x < 0 ? ValueCheckState.AsError("ERROR!") : ValueCheckState.Success);
        }

        #region Ctors

        [Test]
        public void Ctor_WithNull()
        {
            Assert.That(() => new DelegateValueChecker<int>(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_AsPositive()
        {
            Func<int, ValueCheckState> condition = x => x < 0 ? ValueCheckState.AsError("ERROR!") : ValueCheckState.Success;
            var checker = new DelegateValueChecker<int>(condition);

            Assert.That(() => checker.Condition, Is.EqualTo(condition));
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void CheckValue()
        {
            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(0).IsValid, Is.True);
                Assert.That(checker.CheckValue(1).IsValid, Is.True);
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.True);
                Assert.That(checker.CheckValue(-1).Error, Is.EqualTo("ERROR!"));
                Assert.That(checker.CheckValue(int.MinValue).Error, Is.EqualTo("ERROR!"));
            });
        }

        #endregion Methods
    }
}
