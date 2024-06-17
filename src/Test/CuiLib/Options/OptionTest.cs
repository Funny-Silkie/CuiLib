using CuiLib.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Test.CuiLib.Options
{
    [TestFixture]
    public class OptionTest : TestBase
    {
        private OptionImpl option;

        [SetUp]
        public void SetUp()
        {
            option = new OptionImpl();
        }

        #region Properties

        [Test]
        public void Description_Get_OnDefault()
        {
            Assert.That(option.Description, Is.Null);
        }

        [Test]
        public void ValueTypeName_Get_OnDefault()
        {
            Assert.That(option.ValueTypeName, Is.Null);
        }

        [Test]
        public void IsValued_Get_OnDefaultEnumValue()
        {
            Assert.That(option.IsValued, Is.False);
        }

        [Test]
        public void IsValued_Get_OnSpecifiedEnumValue()
        {
            option.SetOptionType(OptionType.Valued);

            Assert.That(option.IsValued, Is.True);
        }

        [Test]
        public void CanMultiValue_Get_OnDefaultEnumValue()
        {
            Assert.That(option.CanMultiValue, Is.False);
        }

        [Test]
        public void CanMultiValue_Get_OnSpecifiedEnumValue()
        {
            option.SetOptionType(OptionType.MultiValue);

            Assert.That(option.CanMultiValue, Is.True);
        }

        [Test]
        public void IsGroup_Get_OnDefaultEnumValue()
        {
            Assert.That(option.IsGroup, Is.False);
        }

        [Test]
        public void IsGroup_Get_OnSpecifiedEnumValue()
        {
            option.SetOptionType(OptionType.Group);

            Assert.That(option.IsGroup, Is.True);
        }

        #endregion Properties

        private sealed class OptionImpl : Option
        {
            /// <inheritdoc/>
            public override bool ValueAvailable => throw new NotImplementedException();

            /// <inheritdoc/>
            public override bool Required { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            /// <inheritdoc/>
            internal override OptionType OptionType => _optionType;

            private OptionType _optionType;

            public void SetOptionType(OptionType value) => _optionType = value;

            /// <inheritdoc/>
            public override bool MatchName(char name) => throw new NotImplementedException();

            /// <inheritdoc/>
            public override bool MatchName(string name) => throw new NotImplementedException();

            /// <inheritdoc/>
            internal override void ApplyValue(string name, string rawValue) => throw new NotImplementedException();

            /// <inheritdoc/>
            internal override void ClearValue() => throw new NotImplementedException();

            /// <inheritdoc/>
            internal override Option GetActualOption(string name, bool isSingle) => throw new NotImplementedException();

            /// <inheritdoc/>
            internal override IEnumerable<string> GetAllNames(bool includeHyphen) => throw new NotImplementedException();
        }
    }
}
