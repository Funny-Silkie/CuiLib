using CuiLib;
using CuiLib.Options;
using NUnit.Framework;

namespace Test.CuiLib.Options
{
    [TestFixture]
    public class XorGroupOptionTest : TestBase
    {
        private XorGroupOption option;
        private SingleValueOption<string> child1;
        private SingleValueOption<int> child2;

        [SetUp]
        public void SetUp()
        {
            child1 = new SingleValueOption<string>('t', "text");
            child2 = new SingleValueOption<int>('n', "num");
            option = new XorGroupOption(child1, child2);
        }

        #region Ctors

        [Test]
        public void Ctor_WithNull()
        {
            Assert.That(() => new XorGroupOption(children: null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithEmpty()
        {
            Assert.That(() => new XorGroupOption([]), Throws.ArgumentException);
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

            Assert.That(option.ValueAvailable, Is.True);
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

            Assert.That(option.Required, Is.False);
        }

        [Test]
        public void Required_Get_OnBothChildrenAreRequired()
        {
            child1.Required = true;
            child2.Required = true;

            Assert.That(option.Required, Is.True);
        }

        #endregion Properties

        #region Methods

        [Test]
        public void ApplyValue_ToChild1()
        {
            option.ApplyValue("num", "100");

            Assert.Multiple(() =>
            {
                Assert.That(child1.ValueAvailable, Is.False);
                Assert.That(child2.ValueAvailable, Is.True);
                Assert.That(child2.Value, Is.EqualTo(100));
            });
        }

        [Test]
        public void ApplyValue_ToChild2()
        {
            option.ApplyValue("text", "value");

            Assert.Multiple(() =>
            {
                Assert.That(child1.ValueAvailable, Is.True);
                Assert.That(child1.Value, Is.EqualTo("value"));
                Assert.That(child2.ValueAvailable, Is.False);
            });
        }

        [Test]
        public void ApplyValue_InMultpileTimes_ToOrNested()
        {
            var nestedChild1 = new SingleValueOption<int>("nc1");
            var nestedChild2 = new SingleValueOption<int>("nc2");
            var nestedChild3 = new SingleValueOption<int>("nc3");
            var nested = new OrGroupOption(nestedChild1, nestedChild2);
            var option = new XorGroupOption(child1, child2, nested);
            option.ApplyValue("nc1", "100");
            option.ApplyValue("nc2", "200");

            Assert.Multiple(() =>
            {
                Assert.That(option.ValueAvailable, Is.True);
                Assert.That(child1.ValueAvailable, Is.False);
                Assert.That(child2.ValueAvailable, Is.False);
                Assert.That(nested.ValueAvailable, Is.True);
                Assert.That(nestedChild1.ValueAvailable, Is.True);
                Assert.That(nestedChild1.Value, Is.EqualTo(100));
                Assert.That(nestedChild2.ValueAvailable, Is.True);
                Assert.That(nestedChild2.Value, Is.EqualTo(200));
                Assert.That(nestedChild3.ValueAvailable, Is.False);
            });
        }

        [Test]
        public void ApplyValue_InMultpileTimes_ToAndNested()
        {
            var nestedChild1 = new SingleValueOption<int>("nc1");
            var nestedChild2 = new SingleValueOption<int>("nc2");
            var nested = new AndGroupOption(nestedChild1, nestedChild2);
            var option = new XorGroupOption(child1, child2, nested);
            option.ApplyValue("nc1", "100");
            option.ApplyValue("nc2", "200");

            Assert.Multiple(() =>
            {
                Assert.That(option.ValueAvailable, Is.True);
                Assert.That(child1.ValueAvailable, Is.False);
                Assert.That(child2.ValueAvailable, Is.False);
                Assert.That(nested.ValueAvailable, Is.True);
                Assert.That(nestedChild1.ValueAvailable, Is.True);
                Assert.That(nestedChild1.Value, Is.EqualTo(100));
                Assert.That(nestedChild2.ValueAvailable, Is.True);
                Assert.That(nestedChild2.Value, Is.EqualTo(200));
            });
        }

        [Test]
        public void ApplyValue_ToBothChildren()
        {
            option.ApplyValue("num", "100");

            Assert.That(() => option.ApplyValue("text", "oops!"), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void ApplyValue_ToMissedChild()
        {
            Assert.That(() => option.ApplyValue("unknown", "1"), Throws.TypeOf<ArgumentAnalysisException>());
        }

        #endregion Methods
    }
}
