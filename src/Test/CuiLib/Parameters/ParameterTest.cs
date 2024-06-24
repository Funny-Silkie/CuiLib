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
            parameter = new ParameterImpl("param", 0);
        }

        #region Ctors

        [Test]
        public void Ctor_WithNullName()
        {
            Assert.That(() => new ParameterImpl(null!, 0), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithNegativeIndex()
        {
            Assert.That(() => new ParameterImpl("param", -1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Ctor_WithEmptyName()
        {
            Assert.That(() => new ParameterImpl(string.Empty, 0), Throws.ArgumentException);
        }

        [Test]
        public void Ctor_AsPositive()
        {
            var parameter = new ParameterImpl("param", 0);

            Assert.Multiple(() =>
            {
                Assert.That(parameter.Name, Is.EqualTo("param"));
                Assert.That(parameter.Index, Is.EqualTo(0));
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

            Assert.That(() => _ = parameter.RawValue, Throws.InvalidOperationException);
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
            Assert.That(() => Parameter.Create<int>(null!, 0), Throws.ArgumentNullException);
        }

        [Test]
        public void Create_WithEmptyName()
        {
            Assert.That(() => Parameter.Create<int>(string.Empty, 0), Throws.ArgumentException);
        }

        [Test]
        public void Create_WithNegativeIndex()
        {
            Assert.That(() => Parameter.Create<int>("param", -1), Throws.TypeOf<ArgumentOutOfRangeException>());
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
            Assert.That(() => Parameter.CreateAsArray<int>(null!, 0), Throws.ArgumentNullException);
        }

        [Test]
        public void CreateAsArray_WithEmptyName()
        {
            Assert.That(() => Parameter.CreateAsArray<int>(string.Empty, 0), Throws.ArgumentException);
        }

        [Test]
        public void CreateAsArray_WithNegativeIndex()
        {
            Assert.That(() => Parameter.CreateAsArray<int>("params", -1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void CreateAsArray_AsPositive()
        {
            MultipleValueParameter<int> parameter = Parameter.CreateAsArray<int>("params", 0);

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
            public override bool IsArray => throw new NotImplementedException();

            public ParameterImpl(string name, int index) : base(name, index)
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
            parameterString = new ParameterImpl<string>("param", 0);
            parameterInt = new ParameterImpl<int>("param", 0);
        }

        #region Ctors

        [Test]
        public void Ctor_WithNullName()
        {
            Assert.That(() => new ParameterImpl<string>(null!, 0), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithEmptyName()
        {
            Assert.That(() => new ParameterImpl<string>(string.Empty, 0), Throws.ArgumentException);
        }

        [Test]
        public void Ctor_AsPositive()
        {
            var parameter = new ParameterImpl<string>("param", 0);

            Assert.Multiple(() =>
            {
                Assert.That(parameter.Name, Is.EqualTo("param"));
                Assert.That(parameter.Index, Is.EqualTo(0));
            });
        }

        #endregion Ctors

        #region Properties

        [Test]
        public void DefaultValue_Get_OnDefault()
        {
            Assert.Multiple(() =>
            {
                Assert.That(parameterInt.DefaultValue, Is.Zero);
                Assert.That(parameterString.DefaultValue, Is.Null);
            });
        }

        #endregion Properties

        private sealed class ParameterImpl<T> : Parameter<T>
        {
            public override bool IsArray => throw new NotImplementedException();

            public override T Value => throw new NotImplementedException();

            public ParameterImpl(string name, int index) : base(name, index)
            {
            }
        }
    }
}
