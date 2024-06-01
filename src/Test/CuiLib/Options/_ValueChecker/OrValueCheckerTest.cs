using CuiLib.Options;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Options._ValueChecker
{
    [TestFixture]
    public class OrValueCheckerTest
    {
        #region Ctors

        [Test]
        public void Ctor_WithTwoCheckers_AsNull()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentNullException>(() => new OrValueChecker<int>(ValueChecker.AlwaysSuccess<int>(), null!));
                Assert.Throws<ArgumentNullException>(() => new OrValueChecker<int>(null!, ValueChecker.AlwaysSuccess<int>()));
            });
        }

        [Test]
        public void Ctor_WithTwoCheckers_AsPositive()
        {
            Assert.DoesNotThrow(() => new OrValueChecker<int>(ValueChecker.AlwaysSuccess<int>(), ValueChecker.AlwaysSuccess<int>()));
        }

        [Test]
        public void Ctor_WithTwoCheckers_AsPositive_WithNested()
        {
            IValueChecker<int> single = ValueChecker.AlwaysSuccess<int>();
            var nested = new OrValueChecker<int>(ValueChecker.AlwaysSuccess<int>(), ValueChecker.AlwaysSuccess<int>());

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => new OrValueChecker<int>(single, nested));
                Assert.DoesNotThrow(() => new OrValueChecker<int>(nested, single));
            });
        }

        [Test]
        public void Ctor_WithMultipleCheckers_AsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new OrValueChecker<int>(source: null!));
        }

        [Test]
        public void Ctor_WithMultipleCheckers_WithNull()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => new OrValueChecker<int>(new[] { null!, ValueChecker.AlwaysSuccess<int>() }));
                Assert.Throws<ArgumentException>(() => new OrValueChecker<int>(new[] { ValueChecker.AlwaysSuccess<int>(), null! }));
                Assert.Throws<ArgumentException>(() => new OrValueChecker<int>(null!, ValueChecker.AlwaysSuccess<int>(), ValueChecker.AlwaysSuccess<int>()));
            });
        }

        [Test]
        public void Ctor_WithMultipleCheckers_AsEmpty()
        {
            Assert.DoesNotThrow(() => new OrValueChecker<int>());
        }

        [Test]
        public void Ctor_WithMultipleCheckers_AsPositive_WithTwoCheckers()
        {
            Assert.DoesNotThrow(() => new OrValueChecker<int>(new[] { ValueChecker.AlwaysSuccess<int>(), ValueChecker.AlwaysSuccess<int>() }));
        }

        [Test]
        public void Ctor_WithMultipleCheckers_AsPositive()
        {
            Assert.DoesNotThrow(() => new OrValueChecker<int>(ValueChecker.AlwaysSuccess<int>(), ValueChecker.AlwaysSuccess<int>(), ValueChecker.AlwaysSuccess<int>()));
        }

        [Test]
        public void Ctor_WithMultipleCheckers_AsPositive_WithNested()
        {
            IValueChecker<int> single = ValueChecker.AlwaysSuccess<int>();
            IValueChecker<int> nested = ValueChecker.Or(ValueChecker.AlwaysSuccess<int>(), ValueChecker.AlwaysSuccess<int>());

            Assert.DoesNotThrow(() => new OrValueChecker<int>(single, single, nested));
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
            var checker = new OrValueChecker<int>(ValueChecker.LowerOrEqual(0), ValueChecker.Larger(10));

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
            IValueChecker<int> child2 = ValueChecker.Larger(10);
            var nested = new OrValueChecker<int>(child1, child2);
            var checker = new OrValueChecker<int>(ValueChecker.LowerOrEqual(0), nested);

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
