using CuiLib.Checkers;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Checkers
{
    [TestFixture]
    public class IValueCheckerTest : TestBase
    {
        #region Methods

#if NET6_0_OR_GREATER

        [Test, Obsolete]
        public void And_WithArray_WithNull()
        {
            Assert.That(() => IValueChecker<int>.And(null!), Throws.ArgumentNullException);
        }

        [Test, Obsolete]
        public void And_WithArray_WithNullChecker()
        {
            Assert.That(() => IValueChecker<int>.And(new[] { ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>(), null! }), Throws.ArgumentException);
        }

        [Test]
        public void And_WithNullChecker()
        {
            Assert.That(() => IValueChecker<int>.And(ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>(), null!), Throws.ArgumentException);
        }

        [Test]
        public void And_AsPositive()
        {
            IValueChecker<int> checker = IValueChecker<int>.And(ValueChecker.GreaterThanOrEqualTo(0), ValueChecker.LessThan(10));

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

        [Test, Obsolete]
        public void Or_WithArray_WithNull()
        {
            Assert.That(() => IValueChecker<int>.Or(null!), Throws.ArgumentNullException);
        }

        [Test, Obsolete]
        public void Or_WithArray_WithNullChecker()
        {
            Assert.That(() => IValueChecker<int>.Or(new[] { ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>(), null! }), Throws.ArgumentException);
        }

        [Test]
        public void Or_WithNullChecker()
        {
            Assert.That(() => IValueChecker<int>.Or(ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>(), null!), Throws.ArgumentException);
        }

        [Test]
        public void Or_AsPositive()
        {
            IValueChecker<int> checker = IValueChecker<int>.Or(ValueChecker.LessThanOrEqualTo(0), ValueChecker.GreaterThan(10));

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

#endif

        #endregion Methods

        #region Operators

#if NET6_0_OR_GREATER

        [Test]
        public void Op_And_WithNull()
        {
            Assert.That(() => _ = ValueChecker.AlwaysValid<int>() & null!, Throws.ArgumentNullException);
            Assert.That(() => _ = null! & ValueChecker.AlwaysValid<int>(), Throws.ArgumentNullException);
        }

        [Test]
        public void Op_And_AsPositive()
        {
            IValueChecker<int> checker = ValueChecker.GreaterThanOrEqualTo(0) & ValueChecker.LessThan(10);

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
                Assert.That(() => _ = ValueChecker.AlwaysValid<int>() | null!, Throws.ArgumentNullException);
                Assert.That(() => _ = null! | ValueChecker.AlwaysValid<int>(), Throws.ArgumentNullException);
            });
        }

        [Test]
        public void Op_Or_AsPositive()
        {
            IValueChecker<int> checker = ValueChecker.LessThanOrEqualTo(0) | ValueChecker.GreaterThan(10);

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

#endif

        #endregion Operators
    }
}
