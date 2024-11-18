using CuiLib.Checkers.Implementations;
using NUnit.Framework;
using System.Collections.Generic;

namespace Test.CuiLib.Checkers.Implementations
{
    [TestFixture]
    public class GenericNotEmptyValueCheckerTest : TestBase
    {
        private NotEmptyValueChecker<char> checker;

        [SetUp]
        public void SetUp()
        {
            checker = new NotEmptyValueChecker<char>();
        }

        #region Ctors

        [Test]
        public void Ctor()
        {
            Assert.That(() => new NotEmptyValueChecker(), Throws.Nothing);
        }

        #endregion Ctors

        #region Methods

#pragma warning disable IDE0028 // コレクションの初期化を簡略化します

        [Test]
        public void CheckValue()
        {
            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(null).IsValid, Is.False);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.False);
                Assert.That(checker.CheckValue([]).IsValid, Is.False);
                Assert.That(checker.CheckValue(new List<char>()).IsValid, Is.False);
                Assert.That(checker.CheckValue(new HashSet<char>()).IsValid, Is.False);

                Assert.That(checker.CheckValue("hoge").IsValid, Is.True);
                Assert.That(checker.CheckValue(['1', '2', '3']).IsValid, Is.True);
                Assert.That(checker.CheckValue(new List<char>() { '1', '2', '3' }).IsValid, Is.True);
                Assert.That(checker.CheckValue(new HashSet<char>() { '1', '2', '3' }).IsValid, Is.True);
            });
        }

#pragma warning restore IDE0028 // コレクションの初期化を簡略化します

        [Test]
        public void Equals()
        {
            Assert.That(checker, Is.EqualTo(new NotEmptyValueChecker<char>()));
        }

        [Test]
        public new void GetHashCode()
        {
            Assert.That(checker.GetHashCode(), Is.EqualTo(new NotEmptyValueChecker<char>().GetHashCode()));
        }

        #endregion Methods
    }
}
