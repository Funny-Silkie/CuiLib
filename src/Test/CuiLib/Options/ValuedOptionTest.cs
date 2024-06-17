using CuiLib.Options;
using NUnit.Framework;
using System;
using System.Collections.ObjectModel;

namespace Test.CuiLib.Options
{
    [TestFixture]
    public class ValuedOptionTest : TestBase
    {
        private ValuedOptionImpl option;

        [SetUp]
        public void SetUp()
        {
            option = new ValuedOptionImpl('v', "value");
        }

        #region Ctors

        [Test]
        public void Ctor_WithShortName()
        {
            var option = new ValuedOptionImpl('t');

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.EqualTo("t"));
                Assert.That(option.FullName, Is.Null);
            });
        }

        [Test]
        public void Ctor_WithNullFullName()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var option = new ValuedOptionImpl(fullName: null!);
            });
        }

        [Test]
        public void Ctor_WithValidFullName()
        {
            var option = new ValuedOptionImpl("test");

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.Null);
                Assert.That(option.FullName, Is.EqualTo("test"));
            });
        }

        [Test]
        public void Ctor_WithShortNameAndValidFullName()
        {
            var option = new ValuedOptionImpl('t', "test");

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.EqualTo("t"));
                Assert.That(option.FullName, Is.EqualTo("test"));
            });
        }

        [Test]
        public void Ctor_WithShortNameAndNullFullName()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var option = new ValuedOptionImpl('t', null!);
            });
        }

        #endregion Ctors

        #region Properties

        [Test]
        public void RawValues_Get_OnDefault()
        {
            Assert.That(option.RawValues, Is.Null);
        }

        [Test]
        public void ValueAvailable_Get_OnDefault()
        {
            Assert.That(option.ValueAvailable, Is.False);
        }

        [Test]
        public void DefaultValue_Get_OnDefault()
        {
            Assert.That(option.DefaultValue, Is.EqualTo(default(string)));
        }

        #endregion Properties

        #region Methods

        [Test]
        public void ApplyValue()
        {
            option.ApplyValue("test", "value1");
            option.ApplyValue("test", "value2");

            Assert.Multiple(() =>
            {
                Assert.That(option.ValueAvailable, Is.True);
                Assert.That(option.RawValues, Is.EqualTo(new[] { "value1", "value2" }));
            });
        }

        [Test]
        public void ClearValue()
        {
            option.ApplyValue("test", "value");
            option.ClearValue();

            Assert.Multiple(() =>
            {
                Assert.That(option.ValueAvailable, Is.False);
                Assert.That(option.RawValues, Is.Null);
            });
        }

        #endregion Methods

        private sealed class ValuedOptionImpl : ValuedOption<string>
        {
            /// <inheritdoc/>
            internal override OptionType OptionType => throw new NotImplementedException();

            /// <inheritdoc/>
            public override bool Required { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            /// <inheritdoc/>
            public override string Value => throw new NotImplementedException();

            /// <see cref="ValuedOption{T}.RawValues"/>
            public new ReadOnlyCollection<string>? RawValues => base.RawValues;

            /// <see cref="ValuedOption{T}.ValuedOption(char)"/>
            public ValuedOptionImpl(char shortName) : base(shortName)
            {
            }

            /// <see cref="ValuedOption{T}.ValuedOption(string)"/>
            public ValuedOptionImpl(string fullName) : base(fullName)
            {
            }

            /// <see cref="ValuedOption{T}.ValuedOption(char, string)"/>
            public ValuedOptionImpl(char shortName, string fullName) : base(shortName, fullName)
            {
            }
        }
    }
}
