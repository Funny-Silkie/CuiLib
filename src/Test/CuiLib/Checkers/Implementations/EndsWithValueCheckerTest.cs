using CuiLib.Checkers.Implementations;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Checkers.Implementations
{
    [TestFixture]
    public class EndsWithValueCheckerTest : TestBase
    {
        private EndsWithValueChecker caseSensitiveChecker;
        private EndsWithValueChecker caseInsensitiveChecker;

        [SetUp]
        public void SetUp()
        {
            caseSensitiveChecker = new EndsWithValueChecker("nd", StringComparison.Ordinal);
            caseInsensitiveChecker = new EndsWithValueChecker("nd", StringComparison.OrdinalIgnoreCase);
        }

        #region Ctors

        [Test]
        public void Ctor_WithChar_WithInvalidStringComparison()
        {
            Assert.That(() => new EndsWithValueChecker('d', (StringComparison)(-1)), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Ctor_WithChar_AsPositive()
        {
            var checker = new EndsWithValueChecker('d', StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(checker.Comparison, Is.EqualTo("d"));
                Assert.That(checker.StringComparison, Is.EqualTo(StringComparison.Ordinal));
            });
        }

        [Test]
        public void Ctor_WithString_WithNull()
        {
            Assert.That(() => new EndsWithValueChecker(null!, StringComparison.CurrentCulture), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithString_WithEmpty()
        {
            Assert.That(() => new EndsWithValueChecker(string.Empty, StringComparison.CurrentCulture), Throws.ArgumentException);
        }

        [Test]
        public void Ctor_WithString_WithInvalidStringComparison()
        {
            Assert.That(() => new EndsWithValueChecker("nd", (StringComparison)(-1)), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Ctor_WithString_AsPositive()
        {
            var checker = new EndsWithValueChecker("nd", StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(checker.Comparison, Is.EqualTo("nd"));
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
                Assert.That(caseSensitiveChecker.CheckValue("end").IsValid, Is.True);
                Assert.That(caseSensitiveChecker.CheckValue("nd").IsValid, Is.True);
                Assert.That(caseSensitiveChecker.CheckValue("END").IsValid, Is.False);
                Assert.That(caseSensitiveChecker.CheckValue("ND").IsValid, Is.False);
                Assert.That(caseSensitiveChecker.CheckValue(null!).IsValid, Is.False);
                Assert.That(caseSensitiveChecker.CheckValue(string.Empty).IsValid, Is.False);
            });
        }

        [Test]
        public void CheckValue_OnCaseInsensitive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(caseInsensitiveChecker.CheckValue("end").IsValid, Is.True);
                Assert.That(caseInsensitiveChecker.CheckValue("nd").IsValid, Is.True);
                Assert.That(caseInsensitiveChecker.CheckValue("END").IsValid, Is.True);
                Assert.That(caseInsensitiveChecker.CheckValue("ND").IsValid, Is.True);
                Assert.That(caseInsensitiveChecker.CheckValue(null!).IsValid, Is.False);
                Assert.That(caseInsensitiveChecker.CheckValue(string.Empty).IsValid, Is.False);
            });
        }

        #endregion Methods
    }
}
