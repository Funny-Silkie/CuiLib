using CuiLib;
using CuiLib.Checkers;
using CuiLib.Converters;
using CuiLib.Options;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Options
{
    [TestFixture]
    public class SingleValueOptionTest : TestBase
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
            Assert.That(() => new SingleValueOption<int>(fullName: null!), Throws.ArgumentNullException);
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
            Assert.That(() => new SingleValueOption<int>('s', null!), Throws.ArgumentNullException);
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
        public void ValueTypeName_Get_OnDefault()
        {
            Assert.That(option.ValueTypeName, Is.EqualTo("int"));
        }

        [Test]
        public void Converter_Get_OnDefault()
        {
            Assert.That(option.Converter.GetType(), Is.EqualTo(ValueConverter.GetDefault<int>().GetType()));
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
            Assert.That(option.Checker, Is.EqualTo(ValueChecker.AlwaysValid<int>()));
        }

        [Test]
        public void Checker_Set_WithNullValue()
        {
            Assert.That(() => option.Checker = null!, Throws.ArgumentNullException);
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

            Assert.That(() => _ = option.Value, Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void Value_Get_AfterApplyValue_OnConversionError()
        {
            option.ApplyValue("single", "oops!");

            Assert.That(() => _ = option.Value, Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void Value_Get_AfterApplyValue_OnCheckError()
        {
            option.Checker = ValueChecker.GreaterThan(0);
            option.ApplyValue("single", "-100");

            Assert.That(() => _ = option.Value, Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void Value_Get_AfterApplyValue_AsPositive()
        {
            option.ApplyValue("single", "100");

            Assert.That(option.Value, Is.EqualTo(100));
        }

        [Test]
        public void Interface_IValuedOption_ValueCount_Get()
        {
            Assert.That(((IValuedOption)option).ValueCount, Is.EqualTo(1));
        }

        #endregion Properties
    }
}
