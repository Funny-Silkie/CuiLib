using CuiLib;
using CuiLib.Checkers;
using CuiLib.Converters;
using CuiLib.Parameters;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Parameters
{
    public class NoGenericParameterTest : TestBase
    {
        private ParameterImpl parameter;

        [SetUp]
        public void SetUp()
        {
            parameter = new ParameterImpl("param", 0, false);
        }

        #region Ctors

        [Test]
        public void Ctor_WithNullName()
        {
            Assert.Throws<ArgumentNullException>(() => new ParameterImpl(null!, 0, false));
        }

        [Test]
        public void Ctor_WithNegativeIndex()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ParameterImpl("param", -1, false));
        }

        [Test]
        public void Ctor_WithEmptyName()
        {
            Assert.Throws<ArgumentException>(() => new ParameterImpl(string.Empty, 0, false));
        }

        [Test]
        public void Ctor_AsPositive()
        {
            var parameter = new ParameterImpl("param", 0, true);

            Assert.Multiple(() =>
            {
                Assert.That(parameter.Name, Is.EqualTo("param"));
                Assert.That(parameter.Index, Is.EqualTo(0));
                Assert.That(parameter.IsArray, Is.True);
            });
        }

        #endregion Ctors

        #region Properties

        [Test]
        public void Description_Get_OnDefault()
        {
            Assert.That(parameter.Description, Is.Null);
        }

        [Test]
        public void ValueAvailable_Get_OnDefault()
        {
            Assert.That(parameter.ValueAvailable, Is.False);
        }

        [Test]
        public void Required_Get_OnDefault()
        {
            Assert.That(parameter.Required, Is.False);
        }

        [Test]
        public void RawValue_Get_OnDefault()
        {
            Assert.That(parameter.RawValue, Is.Null);
        }

        [Test]
        public void RawValue_Get_OnEmptyValues()
        {
            parameter.SetValue([]);

            Assert.Throws<InvalidOperationException>(() => _ = parameter.RawValue);
        }

        [Test]
        public void RawValues_Get_OnDefault()
        {
            Assert.That(parameter.RawValues, Is.Null);
        }

        #endregion Properties

        #region Static Methods

#pragma warning disable CS0618 // 型またはメンバーが旧型式です

        [Test]
        public void Create_WithNullName()
        {
            Assert.Throws<ArgumentNullException>(() => Parameter.Create<int>(null!, 0));
        }

        [Test]
        public void Create_WithEmptyName()
        {
            Assert.Throws<ArgumentException>(() => Parameter.Create<int>(string.Empty, 0));
        }

        [Test]
        public void Create_WithNegativeIndex()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Parameter.Create<int>("param", -1));
        }

        [Test]
        public void Create_AsPositive()
        {
            SingleValueParameter<int> parameter = Parameter.Create<int>("param", 0);

            Assert.Multiple(() =>
            {
                Assert.That(parameter.Name, Is.EqualTo("param"));
                Assert.That(parameter.Index, Is.EqualTo(0));
                Assert.That(parameter.IsArray, Is.False);
            });
        }

        [Test]
        public void CreateAsArray_WithNullName()
        {
            Assert.Throws<ArgumentNullException>(() => Parameter.CreateAsArray<int>(null!, 0));
        }

        [Test]
        public void CreateAsArray_WithEmptyName()
        {
            Assert.Throws<ArgumentException>(() => Parameter.CreateAsArray<int>(string.Empty, 0));
        }

        [Test]
        public void CreateAsArray_WithNegativeIndex()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Parameter.CreateAsArray<int>("params", -1));
        }

        [Test]
        public void CreateAsArray_AsPositive()
        {
            Parameter<int> parameter = Parameter.CreateAsArray<int>("params", 0);

            Assert.Multiple(() =>
            {
                Assert.That(parameter.Name, Is.EqualTo("params"));
                Assert.That(parameter.Index, Is.EqualTo(0));
                Assert.That(parameter.IsArray, Is.True);
            });
        }

#pragma warning restore CS0618 // 型またはメンバーが旧型式です

        #endregion Static Methods

        #region Instance Methods

        [Test]
        public void ClearValue()
        {
            parameter.SetValue("value");
            parameter.ClearValue();

            Assert.Multiple(() =>
            {
                Assert.That(parameter.ValueAvailable, Is.False);
                Assert.That(parameter.RawValue, Is.Null);
                Assert.That(parameter.RawValues, Is.Null);
            });
        }

        [Test]
        public void SetValue_WithString()
        {
            parameter.SetValue("value");

            Assert.Multiple(() =>
            {
                Assert.That(parameter.ValueAvailable, Is.True);
                Assert.That(parameter.RawValue, Is.EqualTo("value"));
                Assert.That(parameter.RawValues, Is.EqualTo(new[] { "value" }));
            });
        }

        [Test]
        public void SetValue_WithStringSpan()
        {
            parameter.SetValue(["val1", "val2", "val3"]);

            Assert.Multiple(() =>
            {
                Assert.That(parameter.ValueAvailable, Is.True);
                Assert.That(parameter.RawValue, Is.EqualTo("val1"));
                Assert.That(parameter.RawValues, Is.EqualTo(new[] { "val1", "val2", "val3" }));
            });
        }

        #endregion Instance Methods

        private sealed class ParameterImpl : Parameter
        {
            public ParameterImpl(string name, int index, bool isArray) : base(name, index, isArray)
            {
            }
        }
    }

    public class GenericParameterTest : TestBase
    {
        private ParameterImpl<string> parameterString;
        private ParameterImpl<int> parameterInt;

        [SetUp]
        public void SetUp()
        {
            parameterString = new ParameterImpl<string>("param", 0, false);
            parameterInt = new ParameterImpl<int>("param", 0, false)
            {
                Checker = ValueChecker.GreaterThanOrEqualTo(0)
            };
        }

        #region Ctors

        [Test]
        public void Ctor_WithNullName()
        {
            Assert.Throws<ArgumentNullException>(() => new ParameterImpl<string>(null!, 0, false));
        }

        [Test]
        public void Ctor_WithEmptyName()
        {
            Assert.Throws<ArgumentException>(() => new ParameterImpl<string>(string.Empty, 0, false));
        }

        [Test]
        public void Ctor_AsPositive()
        {
            var parameter = new ParameterImpl<string>("param", 0, true);

            Assert.Multiple(() =>
            {
                Assert.That(parameter.Name, Is.EqualTo("param"));
                Assert.That(parameter.Index, Is.EqualTo(0));
                Assert.That(parameter.IsArray, Is.True);
            });
        }

        #endregion Ctors

        #region Properties

        [Test]
        public void Converter_Get_OnDefault()
        {
            Assert.That(parameterString.Converter, Is.EqualTo(ValueConverter.GetDefault<string>()));
        }

        [Test]
        public void Converter_Get_OnAfterSetting()
        {
            IValueConverter<string, string> converter = ValueConverter.FromDelegate<string, string>(x => x);
            parameterString.Converter = converter;

            Assert.That(parameterString.Converter, Is.EqualTo(converter));
        }

        [Test]
        public void Checker_Get_OnDefault()
        {
            Assert.That(parameterString.Checker, Is.EqualTo(ValueChecker.AlwaysValid<string>()));
        }

        [Test]
        public void Checker_Set_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => parameterString.Checker = null!);
        }

        [Test]
        public void DefaultValue_Get_OnDefault()
        {
            Assert.Multiple(() =>
            {
                Assert.That(parameterInt.DefaultValue, Is.Zero);
                Assert.That(parameterString.DefaultValue, Is.Null);
            });
        }

        [Test]
        public void Value_Get_OnDefault()
        {
            Assert.Multiple(() =>
            {
                Assert.That(parameterString.Value, Is.Null);
                Assert.That(parameterInt.Value, Is.Zero);
            });
        }

        [Test]
        public void Value_Get_OnEmpty()
        {
            parameterString.SetValue([]);

            Assert.Throws<InvalidOperationException>(() => _ = parameterString.Value);
        }

        [Test]
        public void Value_Get_AfterApplyValue_OnConversionError()
        {
            parameterInt.SetValue(["!!!"]);

            Assert.Throws<ArgumentAnalysisException>(() => _ = parameterInt.Value);
        }

        [Test]
        public void Value_Get_AfterApplyValue_OnCheckError()
        {
            parameterInt.SetValue(["-1"]);

            Assert.Throws<ArgumentAnalysisException>(() => _ = parameterInt.Value);
        }

        [Test]
        public void Value_Get_AfterApplyValue_AsPositive_WithSingleValue()
        {
            parameterString.SetValue(["value"]);

            Assert.That(parameterString.Value, Is.EqualTo("value"));
        }

        [Test]
        public void Value_Get_AfterApplyValue_AsPositive_WithMultipleValue()
        {
            parameterString.SetValue(["val1", "val2"]);

            Assert.That(parameterString.Value, Is.EqualTo("val1"));
        }

        [Test]
        public void Values_Get_OnDefault()
        {
            Assert.That(parameterString.Values, Is.Null);
        }

        [Test]
        public void Values_Get_OnEmpty()
        {
            parameterString.SetValue([]);

            Assert.That(parameterString.Values, Is.Empty);
        }

        [Test]
        public void Values_Get_AfterApplyValue_OnConversionError()
        {
            parameterInt.SetValue(["123", "!!!"]);

            Assert.Throws<ArgumentAnalysisException>(() => _ = parameterInt.Values);
        }

        [Test]
        public void Values_Get_AfterApplyValue_OnCheckError()
        {
            parameterInt.SetValue(["123", "-1"]);

            Assert.Throws<ArgumentAnalysisException>(() => _ = _ = parameterInt.Values);
        }

        [Test]
        public void Values_Get_AfterApplyValue_AsPositive()
        {
            parameterString.SetValue(["val1", "val2"]);

            Assert.That(parameterString.Values, Is.EqualTo(new[] { "val1", "val2" }));
        }

        #endregion Properties

        private sealed class ParameterImpl<T> : Parameter<T>
        {
            public override bool IsArray { get; }

            public ParameterImpl(string name, int index, bool isArray) : base(name, index, isArray)
            {
                IsArray = isArray;
            }
        }
    }
}
