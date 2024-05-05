using CuiLib.Options;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Options._ValueChecker
{
    [TestFixture]
    public class IValueCheckerTest
    {
        #region Methods

        [Test]
        public void And_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => IValueChecker<int>.And(null!));
        }

        [Test]
        public void And_WithNullChecker()
        {
            Assert.Throws<ArgumentException>(() => IValueChecker<int>.And(ValueChecker.AlwaysSuccess<int>(), ValueChecker.AlwaysSuccess<int>(), null!));
        }

        [Test]
        public void And_AsPositive()
        {
            IValueChecker<int> checker = IValueChecker<int>.And(ValueChecker.LargerOrEqual(0), ValueChecker.Lower(10));

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.False);
                Assert.That(checker.CheckValue(-1).IsValid, Is.False);
                Assert.That(checker.CheckValue(0).IsValid, Is.True);
                Assert.That(checker.CheckValue(1).IsValid, Is.True);
                Assert.That(checker.CheckValue(5).IsValid, Is.True);
                Assert.That(checker.CheckValue(9).IsValid, Is.True);
                Assert.That(checker.CheckValue(10).IsValid, Is.False);
                Assert.That(checker.CheckValue(11).IsValid, Is.False);
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.False);
            });
        }

        [Test]
        public void Or_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => IValueChecker<int>.Or(null!));
        }

        [Test]
        public void Or_WithNullChecker()
        {
            Assert.Throws<ArgumentException>(() => IValueChecker<int>.Or(ValueChecker.AlwaysSuccess<int>(), ValueChecker.AlwaysSuccess<int>(), null!));
        }

        [Test]
        public void Or_AsPositive()
        {
            IValueChecker<int> checker = IValueChecker<int>.Or(ValueChecker.LowerOrEqual(0), ValueChecker.Larger(10));

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.True);
                Assert.That(checker.CheckValue(-1).IsValid, Is.True);
                Assert.That(checker.CheckValue(0).IsValid, Is.True);
                Assert.That(checker.CheckValue(1).IsValid, Is.False);
                Assert.That(checker.CheckValue(5).IsValid, Is.False);
                Assert.That(checker.CheckValue(9).IsValid, Is.False);
                Assert.That(checker.CheckValue(10).IsValid, Is.False);
                Assert.That(checker.CheckValue(11).IsValid, Is.True);
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.True);
            });
        }

        #endregion Methods

        #region Operators

        [Test]
        public void Op_And_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => _ = ValueChecker.AlwaysSuccess<int>() & null!);
            Assert.Throws<ArgumentNullException>(() => _ = null! & ValueChecker.AlwaysSuccess<int>());
        }

        [Test]
        public void Op_And_AsPositive()
        {
            IValueChecker<int> checker = ValueChecker.LargerOrEqual(0) & ValueChecker.Lower(10);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.False);
                Assert.That(checker.CheckValue(-1).IsValid, Is.False);
                Assert.That(checker.CheckValue(0).IsValid, Is.True);
                Assert.That(checker.CheckValue(1).IsValid, Is.True);
                Assert.That(checker.CheckValue(5).IsValid, Is.True);
                Assert.That(checker.CheckValue(9).IsValid, Is.True);
                Assert.That(checker.CheckValue(10).IsValid, Is.False);
                Assert.That(checker.CheckValue(11).IsValid, Is.False);
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.False);
            });
        }

        [Test]
        public void Op_Or_WithNull()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentNullException>(() => _ = ValueChecker.AlwaysSuccess<int>() | null!);
                Assert.Throws<ArgumentNullException>(() => _ = null! | ValueChecker.AlwaysSuccess<int>());
            });
        }

        [Test]
        public void Op_Or_AsPositive()
        {
            IValueChecker<int> checker = ValueChecker.LowerOrEqual(0) | ValueChecker.Larger(10);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.True);
                Assert.That(checker.CheckValue(-1).IsValid, Is.True);
                Assert.That(checker.CheckValue(0).IsValid, Is.True);
                Assert.That(checker.CheckValue(1).IsValid, Is.False);
                Assert.That(checker.CheckValue(5).IsValid, Is.False);
                Assert.That(checker.CheckValue(9).IsValid, Is.False);
                Assert.That(checker.CheckValue(10).IsValid, Is.False);
                Assert.That(checker.CheckValue(11).IsValid, Is.True);
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.True);
            });
        }

        #endregion Operators
    }
}
