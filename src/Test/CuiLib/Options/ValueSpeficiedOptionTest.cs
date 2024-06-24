using CuiLib.Options;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Options
{
    [TestFixture]
    public class ValueSpeficiedOptionTest : TestBase
    {
        private ValueSpeficiedOptionImpl option;

        [SetUp]
        public void SetUp()
        {
            option = new ValueSpeficiedOptionImpl('v', "value");
        }

        #region Ctors

        [Test]
        public void Ctor_WithShortName()
        {
            var option = new ValueSpeficiedOptionImpl('t');

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.EqualTo("t"));
                Assert.That(option.FullName, Is.Null);
            });
        }

        [Test]
        public void Ctor_WithNullFullName()
        {
            Assert.That(() => new ValueSpeficiedOptionImpl(fullName: null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithValidFullName()
        {
            var option = new ValueSpeficiedOptionImpl("test");

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.Null);
                Assert.That(option.FullName, Is.EqualTo("test"));
            });
        }

        [Test]
        public void Ctor_WithShortNameAndValidFullName()
        {
            var option = new ValueSpeficiedOptionImpl('t', "test");

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.EqualTo("t"));
                Assert.That(option.FullName, Is.EqualTo("test"));
            });
        }

        [Test]
        public void Ctor_WithShortNameAndNullFullName()
        {
            Assert.That(() => new ValueSpeficiedOptionImpl('t', null!), Throws.ArgumentNullException);
        }

        #endregion Ctors

        #region Properties

        [Test]
        public void Required_Get_OnDefault()
        {
            Assert.That(option.Required, Is.False);
        }

        #endregion Properties

        private sealed class ValueSpeficiedOptionImpl : ValueSpecifiedOption<string>
        {
            /// <inheritdoc/>
            internal override OptionType OptionType => throw new NotImplementedException();

            /// <inheritdoc/>
            public override string Value => throw new NotImplementedException();

            /// <see cref="ValueSpecifiedOption{T}.ValueSpecifiedOption(char)"/>
            public ValueSpeficiedOptionImpl(char shortName) : base(shortName)
            {
            }

            /// <see cref="ValueSpecifiedOption{T}.ValueSpecifiedOption(string)"/>
            public ValueSpeficiedOptionImpl(string fullName) : base(fullName)
            {
            }

            /// <see cref="ValueSpecifiedOption{T}.ValueSpecifiedOption(char, string)"/>
            public ValueSpeficiedOptionImpl(char shortName, string fullName) : base(shortName, fullName)
            {
            }
        }
    }
}
