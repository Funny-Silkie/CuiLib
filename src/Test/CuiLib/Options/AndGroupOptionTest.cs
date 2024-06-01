using CuiLib.Options;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Options
{
    [TestFixture]
    public class AndGroupOptionTest
    {
        private AndGroupOption option;
        private SingleValueOption<string> child1;
        private SingleValueOption<int> child2;

        [SetUp]
        public void SetUp()
        {
            child1 = new SingleValueOption<string>('t', "text");
            child2 = new SingleValueOption<int>('n', "num");
            option = new AndGroupOption(child1, child2);
        }

        #region Ctors

        [Test]
        public void Ctor_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => new AndGroupOption(children: null!));
        }

        [Test]
        public void Ctor_WithEmpty()
        {
            Assert.Throws<ArgumentException>(() => new AndGroupOption([]));
        }

        #endregion Ctors

        #region Properties

        [Test]
        public void ValueAvailable_Get_OnDefault()
        {
            Assert.That(option.ValueAvailable, Is.False);
        }

        [Test]
        public void ValueAvailable_Get_OnOneChildIsAvailable()
        {
            child1.ApplyValue(string.Empty, string.Empty);

            Assert.That(option.ValueAvailable, Is.False);
        }

        [Test]
        public void ValueAvailable_Get_OnBothChildrenAreAvailable()
        {
            child1.ApplyValue("flag", string.Empty);
            child2.ApplyValue("num", "100");

            Assert.That(option.ValueAvailable, Is.True);
        }

        [Test]
        public void Required_Get_OnDefault()
        {
            Assert.That(option.Required, Is.False);
        }

        [Test]
        public void Required_Get_OnOneChildIsRequired()
        {
            child1.Required = true;

            Assert.That(option.Required, Is.True);
        }

        [Test]
        public void Required_Get_OnBothChildrenAreRequired()
        {
            child1.Required = true;
            child2.Required = true;

            Assert.That(option.Required, Is.True);
        }

        #endregion Properties
    }
}
