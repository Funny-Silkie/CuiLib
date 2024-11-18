using CuiLib.Checkers.Implementations;
using CuiLib.Options;
using NUnit.Framework;

namespace Test.CuiLib.Checkers.Implementations
{
    [TestFixture]
    public class DefinedEnumValueCheckerTest : TestBase
    {
        private DefinedEnumValueChecker<OptionType> checker;

        [SetUp]
        public void SetUp()
        {
            checker = new DefinedEnumValueChecker<OptionType>();
        }

        #region Ctor

        [Test]
        public void Ctor()
        {
            Assert.That(() => new DefinedEnumValueChecker<OptionType>(), Throws.Nothing);
        }

        #endregion Ctor

        #region Methods

        [Test]
        public void CheckValue()
        {
            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(OptionType.None).IsValid, Is.True);
                Assert.That(checker.CheckValue(OptionType.Flag).IsValid, Is.True);
                Assert.That(checker.CheckValue(OptionType.Valued).IsValid, Is.True);
                Assert.That(checker.CheckValue(OptionType.SingleValue).IsValid, Is.True);
                Assert.That(checker.CheckValue(OptionType.MultiValue).IsValid, Is.True);
                Assert.That(checker.CheckValue((OptionType)100).IsValid, Is.False);
                Assert.That(checker.CheckValue((OptionType)(-1)).IsValid, Is.False);
                Assert.That(checker.CheckValue((OptionType)(-2)).IsValid, Is.False);
                Assert.That(checker.CheckValue((OptionType)(-4)).IsValid, Is.False);
            });
        }

        [Test]
        public void Equals()
        {
            Assert.That(checker, Is.EqualTo(new DefinedEnumValueChecker<OptionType>()));
        }

        [Test]
        public new void GetHashCode()
        {
            Assert.That(checker.GetHashCode(), Is.EqualTo(new DefinedEnumValueChecker<OptionType>().GetHashCode()));
        }

        #endregion Methods
    }
}
