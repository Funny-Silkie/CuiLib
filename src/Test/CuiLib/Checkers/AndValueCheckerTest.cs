using CuiLib.Checkers;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Checkers
{
    [TestFixture]
    public class AndValueCheckerTest : TestBase
    {
        #region Ctors

        [Test]
        public void Ctor_WithTwoCheckers_AsNull()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => new AndValueChecker<int>(ValueChecker.AlwaysValid<int>(), null!), Throws.ArgumentNullException);
                Assert.That(() => new AndValueChecker<int>(null!, ValueChecker.AlwaysValid<int>()), Throws.ArgumentNullException);
            });
        }

        [Test]
        public void Ctor_WithTwoCheckers_AsPositive()
        {
            Assert.That(() => new AndValueChecker<int>(ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>()), Throws.Nothing);
        }

        [Test]
        public void Ctor_WithTwoCheckers_AsPositive_WithNested()
        {
            IValueChecker<int> single = ValueChecker.AlwaysValid<int>();
            var nested = new AndValueChecker<int>(ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>());

            Assert.Multiple(() =>
            {
                Assert.That(() => new AndValueChecker<int>(single, nested), Throws.Nothing);
                Assert.That(() => new AndValueChecker<int>(nested, single), Throws.Nothing);
            });
        }

#pragma warning disable IDE0300 // コレクションの初期化を簡略化します
#pragma warning disable IDE0301 // コレクションの初期化を簡略化します

        [Test, Obsolete]
        public void Ctor_WithMultipleCheckersAsArray_AsNull()
        {
            Assert.That(() => new AndValueChecker<int>(source: null!), Throws.ArgumentNullException);
        }

        [Test, Obsolete]
        public void Ctor_WithMultipleCheckersAsArray_WithNull()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => new AndValueChecker<int>(new[] { null!, ValueChecker.AlwaysValid<int>() }), Throws.ArgumentException);
                Assert.That(() => new AndValueChecker<int>(new[] { ValueChecker.AlwaysValid<int>(), null! }), Throws.ArgumentException);
                Assert.That(() => new AndValueChecker<int>(new[] { null!, ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>() }), Throws.ArgumentException);
            });
        }

        [Test, Obsolete]
        public void Ctor_WithMultipleCheckersAsArray_AsEmpty()
        {
            Assert.That(() => new AndValueChecker<int>(Array.Empty<IValueChecker<int>>()), Throws.Nothing);
        }

        [Test, Obsolete]
        public void Ctor_WithMultipleCheckersAsArray_AsPositive_WithTwoCheckers()
        {
            Assert.That(() => new AndValueChecker<int>(new[] { ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>() }), Throws.Nothing);
        }

        [Test, Obsolete]
        public void Ctor_WithMultipleCheckersAsArray_AsPositive()
        {
            Assert.That(() => new AndValueChecker<int>(new[] { ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>() }), Throws.Nothing);
        }

#pragma warning restore IDE0301 // コレクションの初期化を簡略化します
#pragma warning restore IDE0300 // コレクションの初期化を簡略化します

        [Test]
        public void Ctor_WithMultipleCheckers_WithNull()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => new AndValueChecker<int>([null!, ValueChecker.AlwaysValid<int>()]), Throws.ArgumentException);
                Assert.That(() => new AndValueChecker<int>([ValueChecker.AlwaysValid<int>(), null!]), Throws.ArgumentException);
                Assert.That(() => new AndValueChecker<int>(null!, ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>()), Throws.ArgumentException);
            });
        }

        [Test]
        public void Ctor_WithMultipleCheckers_AsEmpty()
        {
            Assert.That(() => new AndValueChecker<int>(), Throws.Nothing);
        }

        [Test]
        public void Ctor_WithMultipleCheckers_AsPositive_WithTwoCheckers()
        {
            Assert.That(() => new AndValueChecker<int>([ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>()]), Throws.Nothing);
        }

        [Test]
        public void Ctor_WithMultipleCheckers_AsPositive()
        {
            Assert.That(() => new AndValueChecker<int>(ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>()), Throws.Nothing);
        }

        [Test]
        public void Ctor_WithMultipleCheckers_AsPositive_WithNested()
        {
            IValueChecker<int> single = ValueChecker.AlwaysValid<int>();
            IValueChecker<int> nested = ValueChecker.And(ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>());

            Assert.That(() => new AndValueChecker<int>(single, single, nested), Throws.Nothing);
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void CheckValue_AsEmpty()
        {
            var checker = new AndValueChecker<int>();

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
            var checker = new AndValueChecker<int>(ValueChecker.GreaterThanOrEqualTo(0), ValueChecker.LessThan(10));

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
        public void CheckValue_AsNested()
        {
            IValueChecker<int> child1 = ValueChecker.FromDelegate<int>(x => x % 2 == 0 ? ValueCheckState.Success : ValueCheckState.AsError("ERROR!"));
            IValueChecker<int> child2 = ValueChecker.LessThan(10);
            var nested = new AndValueChecker<int>(child1, child2);
            var checker = new AndValueChecker<int>(ValueChecker.GreaterThanOrEqualTo(0), nested);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.False);
                Assert.That(checker.CheckValue(-1).IsValid, Is.False);
                Assert.That(checker.CheckValue(0).IsValid, Is.True);
                Assert.That(checker.CheckValue(1).IsValid, Is.False);
                Assert.That(checker.CheckValue(2).IsValid, Is.True);
                Assert.That(checker.CheckValue(8).IsValid, Is.True);
                Assert.That(checker.CheckValue(9).IsValid, Is.False);
                Assert.That(checker.CheckValue(10).IsValid, Is.False);
                Assert.That(checker.CheckValue(11).IsValid, Is.False);
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.False);
            });
        }

        #endregion Methods
    }
}
