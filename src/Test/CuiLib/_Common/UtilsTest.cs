using CuiLib;
using NUnit.Framework;

namespace Test.CuiLib
{
    [TestFixture]
    public class UtilsTest : TestBase
    {
        [Test]
        public void ReplaceSpecialCharacters_WithNullOrEmpty()
        {
            Assert.Multiple(() =>
            {
                Assert.That(Utils.ReplaceSpecialCharacters(null), Is.Empty);
                Assert.That(Utils.ReplaceSpecialCharacters(string.Empty), Is.Empty);
            });
        }

        [Test]
        public void ReplaceSpecialCharacters_WithString()
        {
            Assert.That(Utils.ReplaceSpecialCharacters("hoge\\\a\b\f\r\n\t\vfuga"), Is.EqualTo(@"hoge\\\a\b\f\r\n\t\vfuga"));
        }
    }
}
