using CuiLib.Options;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Options._ValueChecker
{
    [TestFixture]
    public class AndValueCheckerTest
    {
        #region Ctors

        [Test]
        public void Ctor_WithTwoCheckers_AsNull()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentNullException>(() => new AndValueChecker<int>(ValueChecker.AlwaysSuccess<int>(), null!));
                Assert.Throws<ArgumentNullException>(() => new AndValueChecker<int>(null!, ValueChecker.AlwaysSuccess<int>()));
            });
        }

        [Test]
        public void Ctor_WithTwoCheckers_AsPositive()
        {
            Assert.DoesNotThrow(() => new AndValueChecker<int>(ValueChecker.AlwaysSuccess<int>(), ValueChecker.AlwaysSuccess<int>()));
        }

        [Test]
        public void Ctor_WithTwoCheckers_AsPositive_WithNested()
        {
            IValueChecker<int> single = ValueChecker.AlwaysSuccess<int>();
            var nested = new AndValueChecker<int>(ValueChecker.AlwaysSuccess<int>(), ValueChecker.AlwaysSuccess<int>());

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => new AndValueChecker<int>(single, nested));
                Assert.DoesNotThrow(() => new AndValueChecker<int>(nested, single));
            });
        }

        [Test]
        public void Ctor_WithMultipleCheckers_AsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new AndValueChecker<int>(source: null!));
        }

        [Test]
        public void Ctor_WithMultipleCheckers_WithNull()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => new AndValueChecker<int>(new[] { null!, ValueChecker.AlwaysSuccess<int>() }));
                Assert.Throws<ArgumentException>(() => new AndValueChecker<int>(new[] { ValueChecker.AlwaysSuccess<int>(), null! }));
                Assert.Throws<ArgumentException>(() => new AndValueChecker<int>(null!, ValueChecker.AlwaysSuccess<int>(), ValueChecker.AlwaysSuccess<int>()));
            });
        }

        [Test]
        public void Ctor_WithMultipleCheckers_AsEmpty()
        {
            Assert.DoesNotThrow(() => new AndValueChecker<int>());
        }

        [Test]
        public void Ctor_WithMultipleCheckers_AsPositive_WithTwoCheckers()
        {
            Assert.DoesNotThrow(() => new AndValueChecker<int>(new[] { ValueChecker.AlwaysSuccess<int>(), ValueChecker.AlwaysSuccess<int>() }));
        }

        [Test]
        public void Ctor_WithMultipleCheckers_AsPositive()
        {
            Assert.DoesNotThrow(() => new AndValueChecker<int>(ValueChecker.AlwaysSuccess<int>(), ValueChecker.AlwaysSuccess<int>(), ValueChecker.AlwaysSuccess<int>()));
        }

        [Test]
        public void Ctor_WithMultipleCheckers_AsPositive_WithNested()
        {
            IValueChecker<int> single = ValueChecker.AlwaysSuccess<int>();
            IValueChecker<int> nested = ValueChecker.And(ValueChecker.AlwaysSuccess<int>(), ValueChecker.AlwaysSuccess<int>());

            Assert.DoesNotThrow(() => new AndValueChecker<int>(single, single, nested));
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
            var checker = new AndValueChecker<int>(ValueChecker.LargerOrEqual(0), ValueChecker.Lower(10));

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
            IValueChecker<int> child2 = ValueChecker.Lower(10);
            var nested = new AndValueChecker<int>(child1, child2);
            var checker = new AndValueChecker<int>(ValueChecker.LargerOrEqual(0), nested);

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
