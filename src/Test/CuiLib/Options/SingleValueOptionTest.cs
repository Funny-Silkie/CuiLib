using CuiLib;
using CuiLib.Checkers;
using CuiLib.Options;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Options
{
    [TestFixture]
    public class SingleValueOptionTest
    {
        private SingleValueOption<int> option;

        [SetUp]
        public void SetUp()
        {
            option = new SingleValueOption<int>('s', "single");
        }

        #region Ctors

        [Test]
        public void Ctor_WithShortName()
        {
            var option = new SingleValueOption<int>('s');

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.EqualTo("s"));
                Assert.That(option.FullName, Is.Null);
            });
        }

        [Test]
        public void Ctor_WithValidFullName()
        {
            var option = new SingleValueOption<int>("single");

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.Null);
                Assert.That(option.FullName, Is.EqualTo("single"));
            });
        }

        [Test]
        public void Ctor_WithNullFullName()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var option = new SingleValueOption<int>(fullName: null!);
            });
        }

        [Test]
        public void Ctor_WithShortNameAndNullFullName()
        {
            var option = new SingleValueOption<int>('s', "single");

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.EqualTo("s"));
                Assert.That(option.FullName, Is.EqualTo("single"));
            });
        }

        [Test]
        public void Ctor_WithShortNameAndValidFullName()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var option = new SingleValueOption<int>('s', null!);
            });
        }

        #endregion Ctors

        #region Properties

        [Test]
        public void IsValued_Get()
        {
            Assert.That(option.IsValued, Is.True);
        }

        [Test]
        public void CanMultiValue_Get()
        {
            Assert.That(option.CanMultiValue, Is.False);
        }

        [Test]
        public void IsGroup_Get()
        {
            Assert.That(option.IsGroup, Is.False);
        }

        [Test]
        public void OptionType_Get()
        {
            Assert.That(option.OptionType, Is.EqualTo(OptionType.Valued | OptionType.SingleValue));
        }

        [Test]
        public void ValueTypeName_Get()
        {
            Assert.That(option.ValueTypeName, Is.EqualTo("int"));
        }

        [Test]
        public void Converter_Get_OnDefault()
        {
            Assert.That(option.Converter, Is.EqualTo(ValueConverter.GetDefault<int>()));
        }

        [Test]
        public void Converter_Get_OnSpecifiedValue()
        {
            IValueConverter<string, int> converter = ValueConverter.FromDelegate<string, int>(int.Parse);
            option.Converter = converter;

            Assert.That(option.Converter, Is.EqualTo(converter));
        }

        [Test]
        public void Checker_Get_OnDefault()
        {
            Assert.That(option.Checker, Is.EqualTo(ValueChecker.AlwaysSuccess<int>()));
        }

        [Test]
        public void Checker_Set_WithNullValue()
        {
            Assert.Throws<ArgumentNullException>(() => option.Checker = null!);
        }

        [Test]
        public void Value_Get_OnDefaultAndNotRequired()
        {
            option.DefaultValue = 100;

            Assert.That(option.Value, Is.EqualTo(100));
        }

        [Test]
        public void Value_Get_OnDefaultAndRequired()
        {
            option.DefaultValue = 100;
            option.Required = true;

            Assert.Throws<ArgumentAnalysisException>(() => _ = option.Value);
        }

        [Test]
        public void Value_Get_AfterApplyValue_OnConversionError()
        {
            option.ApplyValue("single", "oops!");

            Assert.Throws<ArgumentAnalysisException>(() => _ = option.Value);
        }

        [Test]
        public void Value_Get_AfterApplyValue_OnCheckError()
        {
            option.Checker = ValueChecker.Larger(0);
            option.ApplyValue("single", "-100");

            Assert.Throws<ArgumentAnalysisException>(() => _ = option.Value);
        }

        [Test]
        public void Value_Get_AfterApplyValue_AsPositive()
        {
            option.ApplyValue("single", "100");

            Assert.That(option.Value, Is.EqualTo(100));
        }

        #endregion Properties
    }
}
