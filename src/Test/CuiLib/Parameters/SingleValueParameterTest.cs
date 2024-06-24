using CuiLib;
using CuiLib.Checkers;
using CuiLib.Converters;
using CuiLib.Parameters;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Parameters
{
    public class SingleValueParameterTest
    {
        private SingleValueParameter<int> parameter;

        [SetUp]
        public void SetUp()
        {
            parameter = new SingleValueParameter<int>("num", 0);
        }

        #region Ctors

        [Test]
        public void Ctor_WithNullName()
        {
            Assert.That(() => new SingleValueParameter<string>(null!, 0), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithEmptyName()
        {
            Assert.That(() => new SingleValueParameter<string>(string.Empty, 0), Throws.ArgumentException);
        }

        [Test]
        public void Ctor_WithNegativeIndex()
        {
            Assert.That(() => new SingleValueParameter<string>("param", -1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Ctor_AsPositive()
        {
            var param = new SingleValueParameter<string>("hoge", 1);

            Assert.Multiple(() =>
            {
                Assert.That(param.Name, Is.EqualTo("hoge"));
                Assert.That(param.Index, Is.EqualTo(1));
            });
        }

        #endregion Ctors

        #region Properties

        [Test]
        public void IsArray_Get()
        {
            Assert.That(parameter.IsArray, Is.False);
        }

        [Test]
        public void Converter_Get_OnDefault()
        {
            Assert.That(parameter.Converter.GetType(), Is.EqualTo(ValueConverter.GetDefault<int>().GetType()));
        }

        [Test]
        public void Converter_Get_OnAfterSetting()
        {
            IValueConverter<string, int> converter = ValueConverter.FromDelegate<string, int>(x => 1);
            parameter.Converter = converter;

            Assert.That(parameter.Converter, Is.EqualTo(converter));
        }

        [Test]
        public void Checker_Get_OnDefault()
        {
            Assert.That(parameter.Checker, Is.EqualTo(ValueChecker.AlwaysValid<int>()));
        }

        [Test]
        public void Checker_Set_WithNull()
        {
            Assert.That(() => parameter.Checker = null!, Throws.ArgumentNullException);
        }

        [Test]
        public void Value_Get_OnDefaultAndNotRequired()
        {
            parameter.DefaultValue = 100;

            Assert.That(parameter.Value, Is.EqualTo(100));
        }

        [Test]
        public void Value_Get_OnDefaultAndRequired()
        {
            parameter.DefaultValue = 100;
            parameter.Required = true;

            Assert.That(() => _ = parameter.Value, Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void Value_Get_AfterApplyValue_OnConversionError()
        {
            parameter.SetValue("oops!");

            Assert.That(() => _ = parameter.Value, Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void Value_Get_AfterApplyValue_OnCheckError()
        {
            parameter.Checker = ValueChecker.GreaterThan(0);
            parameter.SetValue("-100");

            Assert.That(() => _ = parameter.Value, Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void Value_Get_AfterApplyValue_AsPositive()
        {
            parameter.SetValue("100");

            Assert.That(parameter.Value, Is.EqualTo(100));
        }

        #endregion Properties
    }
}
