using CuiLib.Checkers.Implementations;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Test.CuiLib.Checkers.Implementations
{
    [TestFixture]
    public class GreaterThanValueCheckerTest : TestBase
    {
        private GreaterThanValueChecker<int> checker;

        [SetUp]
        public void SetUp()
        {
            checker = new GreaterThanValueChecker<int>(100, null);
        }

        #region Ctors

        [Test]
        public void Ctor_WithNullComparer()
        {
            var checker = new GreaterThanValueChecker<int>(3, null);

            Assert.Multiple(() =>
            {
                Assert.That(checker.Comparison, Is.EqualTo(3));
                Assert.That(checker.Comparer, Is.EqualTo(Comparer<int>.Default));
            });
        }

        [Test]
        public void Ctor_WithSpecifiedComparer()
        {
            Comparer<int> comparer = Comparer<int>.Create((x, y) => Math.Sign(x - y));
            var checker = new GreaterThanValueChecker<int>(3, comparer);

            Assert.Multiple(() =>
            {
                Assert.That(checker.Comparison, Is.EqualTo(3));
                Assert.That(checker.Comparer, Is.EqualTo(comparer));
            });
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void CheckValue()
        {
            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.True);
                Assert.That(checker.CheckValue(101).IsValid, Is.True);
                Assert.That(checker.CheckValue(100).IsValid, Is.False);
                Assert.That(checker.CheckValue(99).IsValid, Is.False);
                Assert.That(checker.CheckValue(0).IsValid, Is.False);
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.False);
            });
        }

        #endregion Methods
    }
}
