using CuiLib.Options;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Options
{
    [TestFixture]
    public class FlagOptionTest
    {
        private FlagOption option;

        [SetUp]
        public void Setup()
        {
            option = new FlagOption('f', "flag");
        }

        [Test]
        public void CtorWithShortName()
        {
            var option = new FlagOption('f');

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.EqualTo("f"));
                Assert.That(option.FullName, Is.Null);
            });
        }

        [Test]
        public void CtorWithFullName()
        {
            var option = new FlagOption("flag");

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.Null);
                Assert.That(option.FullName, Is.EqualTo("flag"));
            });
        }

        [Test]
        public void CtorWithShortNameAndFullName()
        {
            var option = new FlagOption('f', "flag");

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.EqualTo("f"));
                Assert.That(option.FullName, Is.EqualTo("flag"));
            });
        }

        [Test]
        public void OptionType_Get()
        {
            Assert.That(option.OptionType, Is.EqualTo(OptionType.Flag | OptionType.SingleValue));
        }

        [Test]
        public void ValueTypeName_Get()
        {
            Assert.That(option.ValueTypeName, Is.Null);
        }

        [Test]
        public void Required_Get()
        {
            Assert.That(option.Required, Is.False);
        }

        [Test]
        public void Required_Set()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<NotSupportedException>(() => option.Required = true);
                Assert.Throws<NotSupportedException>(() => option.Required = false);
            });
        }

        [Test]
        public void Value_OnDefault()
        {
            Assert.That(option.Value, Is.False);
        }

        [Test]
        public void Value_OnDefaultWithDefaultValue()
        {
            option.DefaultValue = true;

            Assert.That(option.Value, Is.True);
        }

        [Test]
        public void Value_AfterApplyValue()
        {
            option.ApplyValue(string.Empty, string.Empty);

            Assert.That(option.Value, Is.True);
        }

        [Test]
        public void Value_AfterApplyValueWithDefaultValue()
        {
            option.DefaultValue = true;
            option.ApplyValue(string.Empty, string.Empty);

            Assert.That(option.Value, Is.True);
        }
    }
}
