using CuiLib;
using CuiLib.Checkers;
using CuiLib.Converters;
using CuiLib.Options;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Options
{
    [TestFixture]
    public class MultipleValueOptionTest : TestBase
    {
        private MultipleValueOption<int> option;

        [SetUp]
        public void SetUp()
        {
            option = new MultipleValueOption<int>('m', "multi");
        }

        #region Ctors

        [Test]
        public void Ctor_WithShortName()
        {
            var option = new MultipleValueOption<int>('m');

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.EqualTo("m"));
                Assert.That(option.FullName, Is.Null);
                Assert.That(option.DefaultValue, Is.Empty);
            });
        }

        [Test]
        public void Ctor_WithValidFullName()
        {
            var option = new MultipleValueOption<int>("multi");

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.Null);
                Assert.That(option.FullName, Is.EqualTo("multi"));
                Assert.That(option.DefaultValue, Is.Empty);
            });
        }

        [Test]
        public void Ctor_WithNullFullName()
        {
            Assert.That(() => new MultipleValueOption<int>(fullName: null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithShortNameAndNullFullName()
        {
            var option = new MultipleValueOption<int>('m', "multi");

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.EqualTo("m"));
                Assert.That(option.FullName, Is.EqualTo("multi"));
                Assert.That(option.DefaultValue, Is.Empty);
            });
        }

        [Test]
        public void Ctor_WithShortNameAndValidFullName()
        {
            Assert.That(() => new MultipleValueOption<int>('m', null!), Throws.ArgumentNullException);
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
            Assert.That(option.CanMultiValue, Is.True);
        }

        [Test]
        public void IsGroup_Get()
        {
            Assert.That(option.IsGroup, Is.False);
        }

        [Test]
        public void OptionType_Get()
        {
            Assert.That(option.OptionType, Is.EqualTo(OptionType.Valued | OptionType.MultiValue));
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
            option.DefaultValue = [100];

            Assert.That(option.Value, Is.EqualTo(new[] { 100 }));
        }

        [Test]
        public void Value_Get_OnDefaultAndRequired()
        {
            option.DefaultValue = [100];
            option.Required = true;

            Assert.That(() => _ = option.Value, Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void Value_Get_AfterApplyValue_OnConversionError()
        {
            option.ApplyValue("multi", "oops!");
            option.ApplyValue("multi", "100");

            Assert.That(() => _ = option.Value, Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void Value_Get_AfterApplyValue_OnCheckError()
        {
            option.Checker = ValueChecker.GreaterThan(0);
            option.ApplyValue("multi", "-100");
            option.ApplyValue("multi", "100");

            Assert.That(() => _ = option.Value, Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void Value_Get_AfterApplyValue_AsPositive()
        {
            option.ApplyValue("multi", "100");
            option.ApplyValue("multi", "200");

            Assert.That(option.Value, Is.EqualTo(new[] { 100, 200 }));
        }

        [Test]
        public void ValueCount_Get_OnDefault()
        {
            Assert.That(option.ValueCount, Is.EqualTo(1));
        }

        [Test]
        public void ValueCount_Set_WithNegative()
        {
            Assert.That(() => option.ValueCount = -1, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [TestCase(0)]
        [TestCase(2)]
        [TestCase(100)]
        public void ValueCount_Set_AsPositive(int value)
        {
            option.ValueCount = value;
            Assert.That(option.ValueCount, Is.EqualTo(value));
        }

        #endregion Properties
    }
}
