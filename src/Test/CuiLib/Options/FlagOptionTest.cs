﻿using CuiLib.Options;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Options
{
    [TestFixture]
    public class FlagOptionTest
    {
        private FlagOption option;

        [SetUp]
        public void Setup()
        {
            option = new FlagOption('f', "flag");
        }

        #region Ctors

        [Test]
        public void Ctor_WithShortName()
        {
            var option = new FlagOption('f');

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.EqualTo("f"));
                Assert.That(option.FullName, Is.Null);
            });
        }

        [Test]
        public void Ctor_WithValidFullName()
        {
            var option = new FlagOption("flag");

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.Null);
                Assert.That(option.FullName, Is.EqualTo("flag"));
            });
        }

        [Test]
        public void Ctor_WithNullFullName()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var option = new FlagOption(fullName: null!);
            });
        }

        [Test]
        public void Ctor_WithShortNameAndNullFullName()
        {
            var option = new FlagOption('f', "flag");

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.EqualTo("f"));
                Assert.That(option.FullName, Is.EqualTo("flag"));
            });
        }

        [Test]
        public void Ctor_WithShortNameAndValidFullName()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var option = new FlagOption('f', null!);
            });
        }

        #endregion Ctors

        #region Properties

        [Test]
        public void IsValued_Get()
        {
            Assert.That(option.IsValued, Is.False);
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
            Assert.That(option.OptionType, Is.EqualTo(OptionType.Flag | OptionType.SingleValue));
        }

        [Test]
        public void ValueTypeName_Get()
        {
            Assert.That(option.ValueTypeName, Is.Null);
        }

        [Test]
        public void Required_Get()
        {
            Assert.That(option.Required, Is.False);
        }

        [Test]
        public void Required_Set()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<NotSupportedException>(() => option.Required = true);
                Assert.Throws<NotSupportedException>(() => option.Required = false);
            });
        }

        [Test]
        public void Value_OnDefault()
        {
            Assert.That(option.Value, Is.False);
        }

        [Test]
        public void Value_OnDefaultWithDefaultValue()
        {
            option.DefaultValue = true;

            Assert.That(option.Value, Is.True);
        }

        [Test]
        public void Value_AfterApplyValue()
        {
            option.ApplyValue(string.Empty, string.Empty);

            Assert.That(option.Value, Is.True);
        }

        [Test]
        public void Value_AfterApplyValueWithDefaultValue()
        {
            option.DefaultValue = true;
            option.ApplyValue(string.Empty, string.Empty);

            Assert.That(option.Value, Is.True);
        }

        #endregion Properties
    }
}