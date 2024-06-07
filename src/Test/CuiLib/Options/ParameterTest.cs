using CuiLib.Options;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Options
{
    public class NoGenericParameterTest
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

        [Test]
        public void Create_WithNullName()
        {
            Assert.Throws<ArgumentNullException>(() => Parameter.Create<int>(null!, 0));
        }

        [Test]
        public void Create_WithNullEmpty()
        {
            Assert.Throws<ArgumentException>(() => Parameter.Create<int>(string.Empty, 0));
        }

        [Test]
        public void Create_AsPositive()
        {
            Parameter<int> parameter = Parameter.Create<int>("param", 0);

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
        public void CreateAsArray_WithNullEmpty()
        {
            Assert.Throws<ArgumentException>(() => Parameter.CreateAsArray<int>(string.Empty, 0));
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
}
