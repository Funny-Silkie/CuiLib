using CuiLib.Checkers.Implementations;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace Test.CuiLib.Checkers.Implementations
{
    [TestFixture]
    public partial class MatchesValueCheckerTest : TestBase
    {
        // lang=regex
        private const string SampleRegex = @"\d+";

        private MatchesValueChecker checker;

        [SetUp]
        public void SetUp()
        {
            checker = new MatchesValueChecker(GetSampleRegex());
        }

        #region Ctors

        [Test]
        public void Ctor_WithNull()
        {
            Assert.That(() => new MatchesValueChecker(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_AsPositive()
        {
            Regex regex = GetSampleRegex();
            var checker = new MatchesValueChecker(regex);

            Assert.That(checker.Regex, Is.EqualTo(regex));
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void CheckValue_WithNull()
        {
            Assert.That(() => checker.CheckValue(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void CheckValue_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => checker.CheckValue("123").IsValid, Is.True);
                Assert.That(() => checker.CheckValue("-123-").IsValid, Is.True);
                Assert.That(() => checker.CheckValue("--").IsValid, Is.False);
                Assert.That(() => checker.CheckValue(string.Empty).IsValid, Is.False);
            });
        }

        #endregion Methods

#if NET7_0_OR_GREATER

        [GeneratedRegex(SampleRegex)]
        private static partial Regex GetSampleRegex();

#else
        private static Regex GetSampleRegex() => new Regex(SampleRegex);

#endif
    }
}
