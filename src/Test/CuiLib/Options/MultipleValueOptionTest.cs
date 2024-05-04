using CuiLib;
using CuiLib.Options;
using NUnit.Framework;
using System;
using System.Reflection;

namespace Test.CuiLib.Options
{
    [TestFixture]
    public class MultipleValueOptionTest
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
            Assert.Throws<ArgumentNullException>(() =>
            {
                var option = new MultipleValueOption<int>(fullName: null!);
            });
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
            Assert.Throws<ArgumentNullException>(() =>
            {
                var option = new MultipleValueOption<int>('m', null!);
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
            Type expectedType = Assembly.LoadFrom("CuiLib").GetType("CuiLib.Options.ValueChecker+AlwaysSuccessValueChecker`1")!.MakeGenericType(typeof(int));
            Assert.That(option.Checker, Is.InstanceOf(expectedType));
        }

        [Test]
        public void Checker_Set_WithNullValue()
        {
            Assert.Throws<ArgumentNullException>(() => option.Checker = null!);
        }

        [Test]
        public void Value_Get_OnDefaultAndNotRequired()
        {
            option.DefaultValue = new[] { 100 };

            Assert.That(option.Value, Is.EqualTo(new[] { 100 }));
        }

        [Test]
        public void Value_Get_OnDefaultAndRequired()
        {
            option.DefaultValue = new[] { 100 };
            option.Required = true;

            Assert.Throws<ArgumentAnalysisException>(() => _ = option.Value);
        }

        [Test]
        public void Value_Get_AfterApplyValue_OnConversionError()
        {
            option.ApplyValue("multi", "oops!");
            option.ApplyValue("multi", "100");

            Assert.Throws<ArgumentAnalysisException>(() => _ = option.Value);
        }

        [Test]
        public void Value_Get_AfterApplyValue_OnCheckError()
        {
            option.Checker = ValueChecker.Larger(0);
            option.ApplyValue("multi", "-100");
            option.ApplyValue("multi", "100");

            Assert.Throws<ArgumentAnalysisException>(() => _ = option.Value);
        }

        [Test]
        public void Value_Get_AfterApplyValue_AsPositive()
        {
            option.ApplyValue("multi", "100");
            option.ApplyValue("multi", "200");

            Assert.That(option.Value, Is.EqualTo(new[] { 100, 200 }));
        }

        [Test]
        public void DefaultValueString_OnDefault()
        {
            Assert.That(option.DefaultValueString, Is.EqualTo("[]"));
        }

        [Test]
        public void DefaultValueString_AsNullDefaultValue()
        {
            option.DefaultValue = null!;

            Assert.That(option.DefaultValueString, Is.EqualTo("[]"));
        }

        [Test]
        public void DefaultValueString_AsSpecifiedDefaultValue()
        {
            option.DefaultValue = new[] { 1, 2, 3 };

            Assert.That(option.DefaultValueString, Is.EqualTo("[1, 2, 3]"));
        }

        #endregion Properties
    }
}
