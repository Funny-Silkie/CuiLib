using CuiLib.Checkers;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Checkers
{
    [TestFixture]
    public class OrValueCheckerTest : TestBase
    {
        #region Ctors

        [Test]
        public void Ctor_WithTwoCheckers_AsNull()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => new OrValueChecker<int>(ValueChecker.AlwaysValid<int>(), null!), Throws.ArgumentNullException);
                Assert.That(() => new OrValueChecker<int>(null!, ValueChecker.AlwaysValid<int>()), Throws.ArgumentNullException);
            });
        }

        [Test]
        public void Ctor_WithTwoCheckers_AsPositive()
        {
            Assert.That(() => new OrValueChecker<int>(ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>()), Throws.Nothing);
        }

        [Test]
        public void Ctor_WithTwoCheckers_AsPositive_WithNested()
        {
            IValueChecker<int> single = ValueChecker.AlwaysValid<int>();
            var nested = new OrValueChecker<int>(ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>());

            Assert.Multiple(() =>
            {
                Assert.That(() => new OrValueChecker<int>(single, nested), Throws.Nothing);
                Assert.That(() => new OrValueChecker<int>(nested, single), Throws.Nothing);
            });
        }

        [Test]
        public void Ctor_WithMultipleCheckers_AsNull()
        {
            Assert.That(() => new OrValueChecker<int>(source: null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithMultipleCheckers_WithNull()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => new OrValueChecker<int>([null!, ValueChecker.AlwaysValid<int>()]), Throws.ArgumentException);
                Assert.That(() => new OrValueChecker<int>([ValueChecker.AlwaysValid<int>(), null!]), Throws.ArgumentException);
                Assert.That(() => new OrValueChecker<int>(null!, ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>()), Throws.ArgumentException);
            });
        }

        [Test]
        public void Ctor_WithMultipleCheckers_AsEmpty()
        {
            Assert.That(() => new OrValueChecker<int>(), Throws.Nothing);
        }

        [Test]
        public void Ctor_WithMultipleCheckers_AsPositive_WithTwoCheckers()
        {
            Assert.That(() => new OrValueChecker<int>([ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>()]), Throws.Nothing);
        }

        [Test]
        public void Ctor_WithMultipleCheckers_AsPositive()
        {
            Assert.That(() => new OrValueChecker<int>(ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>()), Throws.Nothing);
        }

        [Test]
        public void Ctor_WithMultipleCheckers_AsPositive_WithNested()
        {
            IValueChecker<int> single = ValueChecker.AlwaysValid<int>();
            IValueChecker<int> nested = ValueChecker.Or(ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>());

            Assert.That(() => new OrValueChecker<int>(single, single, nested), Throws.Nothing);
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void CheckValue_AsEmpty()
        {
            var checker = new OrValueChecker<int>();

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(0).IsValid, Is.True);
                Assert.That(checker.CheckValue(1).IsValid, Is.True);
                Assert.That(checker.CheckValue(-1).IsValid, Is.True);
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.True);
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.True);
            });
        }

        [Test]
        public void CheckValue_AsNoNested()
        {
            var checker = new OrValueChecker<int>(ValueChecker.LessThanOrEqualTo(0), ValueChecker.GreaterThan(10));

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

        [Test]
        public void CheckValue_AsNested()
        {
            IValueChecker<int> child1 = ValueChecker.FromDelegate<int>(x => x % 2 == 0 ? ValueCheckState.Success : ValueCheckState.AsError("ERROR!"));
            IValueChecker<int> child2 = ValueChecker.GreaterThan(10);
            var nested = new OrValueChecker<int>(child1, child2);
            var checker = new OrValueChecker<int>(ValueChecker.LessThanOrEqualTo(0), nested);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.True);
                Assert.That(checker.CheckValue(-1).IsValid, Is.True);
                Assert.That(checker.CheckValue(0).IsValid, Is.True);
                Assert.That(checker.CheckValue(1).IsValid, Is.False);
                Assert.That(checker.CheckValue(2).IsValid, Is.True);
                Assert.That(checker.CheckValue(4).IsValid, Is.True);
                Assert.That(checker.CheckValue(5).IsValid, Is.False);
                Assert.That(checker.CheckValue(6).IsValid, Is.True);
                Assert.That(checker.CheckValue(8).IsValid, Is.True);
                Assert.That(checker.CheckValue(9).IsValid, Is.False);
                Assert.That(checker.CheckValue(10).IsValid, Is.True);
                Assert.That(checker.CheckValue(11).IsValid, Is.True);
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.True);
            });
        }

        #endregion Methods
    }
}
