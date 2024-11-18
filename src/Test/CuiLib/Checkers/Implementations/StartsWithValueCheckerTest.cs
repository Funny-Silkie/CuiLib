using CuiLib.Checkers.Implementations;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Checkers.Implementations
{
    [TestFixture]
    public class StartsWithValueCheckerTest : TestBase
    {
        private StartsWithValueChecker caseSensitiveChecker;
        private StartsWithValueChecker caseInsensitiveChecker;

        [SetUp]
        public void SetUp()
        {
            caseSensitiveChecker = new StartsWithValueChecker("st", StringComparison.Ordinal);
            caseInsensitiveChecker = new StartsWithValueChecker("st", StringComparison.OrdinalIgnoreCase);
        }

        #region Ctors

        [Test]
        public void Ctor_WithChar_WithInvalidStringComparison()
        {
            Assert.That(() => new StartsWithValueChecker('s', (StringComparison)(-1)), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Ctor_WithChar_AsPositive()
        {
            var checker = new StartsWithValueChecker('s', StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(checker.Comparison, Is.EqualTo("s"));
                Assert.That(checker.StringComparison, Is.EqualTo(StringComparison.Ordinal));
            });
        }

        [Test]
        public void Ctor_WithString_WithNull()
        {
            Assert.That(() => new StartsWithValueChecker(null!, StringComparison.CurrentCulture), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithString_WithEmpty()
        {
            Assert.That(() => new StartsWithValueChecker(string.Empty, StringComparison.CurrentCulture), Throws.ArgumentException);
        }

        [Test]
        public void Ctor_WithString_WithInvalidStringComparison()
        {
            Assert.That(() => new StartsWithValueChecker("st", (StringComparison)(-1)), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Ctor_WithString_AsPositive()
        {
            var checker = new StartsWithValueChecker("st", StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(checker.Comparison, Is.EqualTo("st"));
                Assert.That(checker.StringComparison, Is.EqualTo(StringComparison.Ordinal));
            });
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void CheckValue_OnCaseSensitive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(caseSensitiveChecker.CheckValue("start").IsValid, Is.True);
                Assert.That(caseSensitiveChecker.CheckValue("st").IsValid, Is.True);
                Assert.That(caseSensitiveChecker.CheckValue("Start").IsValid, Is.False);
                Assert.That(caseSensitiveChecker.CheckValue("St").IsValid, Is.False);
                Assert.That(caseSensitiveChecker.CheckValue(null!).IsValid, Is.False);
                Assert.That(caseSensitiveChecker.CheckValue(string.Empty).IsValid, Is.False);
            });
        }

        [Test]
        public void CheckValue_OnCaseInsensitive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(caseInsensitiveChecker.CheckValue("start").IsValid, Is.True);
                Assert.That(caseInsensitiveChecker.CheckValue("st").IsValid, Is.True);
                Assert.That(caseInsensitiveChecker.CheckValue("Start").IsValid, Is.True);
                Assert.That(caseInsensitiveChecker.CheckValue("St").IsValid, Is.True);
                Assert.That(caseInsensitiveChecker.CheckValue(null!).IsValid, Is.False);
                Assert.That(caseInsensitiveChecker.CheckValue(string.Empty).IsValid, Is.False);
            });
        }

        #endregion Methods
    }
}
