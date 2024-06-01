using CuiLib.Extensions;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Extensions
{
    [TestFixture]
    public class CollectionExtensionsTest
    {
        [Test]
        public void GetOrDefault_WithoutDefaultValue_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => CollectionExtensions.GetOrDefault<int>(null!, 0));
        }

        [Test]
        public void GetOrDefault_WithoutDefaultValue_AsPositive()
        {
            var array = new object[] { 1, "hoge", true };

            Assert.Multiple(() =>
            {
                Assert.That(array.GetOrDefault(-1), Is.EqualTo(null));
                Assert.That(array.GetOrDefault(0), Is.EqualTo(1));
                Assert.That(array.GetOrDefault(1), Is.EqualTo("hoge"));
                Assert.That(array.GetOrDefault(2), Is.EqualTo(true));
                Assert.That(array.GetOrDefault(3), Is.EqualTo(null));
            });
        }

        [Test]
        public void GetOrDefault_WithDefaultValue_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => CollectionExtensions.GetOrDefault(null!, 0, -1));
        }

        [Test]
        public void GetOrDefault_WithDefaultValue_AsPositive()
        {
            var array = new object[] { 1, "hoge", true };
            var defaultValue = new object();

            Assert.Multiple(() =>
            {
                Assert.That(array.GetOrDefault(-1, defaultValue), Is.EqualTo(defaultValue));
                Assert.That(array.GetOrDefault(0, defaultValue), Is.EqualTo(1));
                Assert.That(array.GetOrDefault(1, defaultValue), Is.EqualTo("hoge"));
                Assert.That(array.GetOrDefault(2, defaultValue), Is.EqualTo(true));
                Assert.That(array.GetOrDefault(3, defaultValue), Is.EqualTo(defaultValue));
            });
        }
    }
}
