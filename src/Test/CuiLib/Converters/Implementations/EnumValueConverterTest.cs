using CuiLib.Converters.Implementations;
using CuiLib.Options;
using NUnit.Framework;

namespace Test.CuiLib.Converters.Implementations
{
    [TestFixture]
    public class EnumValueConverterTest : TestBase
    {
        private EnumValueConverter<OptionType> caseSensitiveConverter;
        private EnumValueConverter<OptionType> caseInsensitiveConverter;

        [SetUp]
        public void SetUp()
        {
            caseSensitiveConverter = new EnumValueConverter<OptionType>(false);
            caseInsensitiveConverter = new EnumValueConverter<OptionType>(true);
        }

        #region Ctors

        [TestCase(true)]
        [TestCase(false)]
        public void Ctor(bool ignoreCase)
        {
            var converter = new EnumValueConverter<OptionType>(ignoreCase);

            Assert.That(converter.IgnoreCase, Is.EqualTo(ignoreCase));
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void Convert_OnCaseSensitive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(caseSensitiveConverter.Convert("None"), Is.EqualTo(OptionType.None));
                Assert.That(caseSensitiveConverter.Convert("Flag"), Is.EqualTo(OptionType.Flag));

                Assert.That(() => caseSensitiveConverter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => caseSensitiveConverter.Convert("none"), Throws.ArgumentException);
                Assert.That(() => caseSensitiveConverter.Convert("fLAG"), Throws.ArgumentException);
                Assert.That(() => caseSensitiveConverter.Convert("!!"), Throws.ArgumentException);
            });
        }

        [Test]
        public void Convert_OnCaseInsensitive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(caseInsensitiveConverter.Convert("None"), Is.EqualTo(OptionType.None));
                Assert.That(caseInsensitiveConverter.Convert("none"), Is.EqualTo(OptionType.None));
                Assert.That(caseInsensitiveConverter.Convert("Flag"), Is.EqualTo(OptionType.Flag));
                Assert.That(caseInsensitiveConverter.Convert("fLAG"), Is.EqualTo(OptionType.Flag));

                Assert.That(() => caseInsensitiveConverter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => caseInsensitiveConverter.Convert("!!"), Throws.ArgumentException);
            });
        }

        #endregion Methods
    }
}
