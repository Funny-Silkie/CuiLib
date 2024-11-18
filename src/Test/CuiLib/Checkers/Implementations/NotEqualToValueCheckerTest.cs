using CuiLib.Checkers.Implementations;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Test.CuiLib.Checkers.Implementations
{
    [TestFixture]
    public class NotEqualToValueCheckerTest : TestBase
    {
        private NotEqualToValueChecker<string> checker;

        [SetUp]
        public void SetUp()
        {
            checker = new NotEqualToValueChecker<string>(null, "value");
        }

        #region Ctors

        [Test]
        public void Ctor_WithNullComparer()
        {
            var checker = new NotEqualToValueChecker<string>(null, "value");

            Assert.Multiple(() =>
            {
                Assert.That(checker.Comparison, Is.EqualTo("value"));
                Assert.That(checker.Comparer, Is.EqualTo(EqualityComparer<string>.Default));
            });
        }

        [Test]
        public void Ctor_WithSpecifiedComparer()
        {
            var comparer = new DummyComparer<string>();
            var checker = new NotEqualToValueChecker<string>(comparer, "value");

            Assert.Multiple(() =>
            {
                Assert.That(checker.Comparison, Is.EqualTo("value"));
                Assert.That(checker.Comparer, Is.EqualTo(comparer));
            });
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void CheckValue()
        {
            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue("value").IsValid, Is.False);
                Assert.That(checker.CheckValue("Value").IsValid, Is.True);
                Assert.That(checker.CheckValue("other").IsValid, Is.True);
                Assert.That(checker.CheckValue(null!).IsValid, Is.True);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.True);
            });
        }

        #endregion Methods

        private sealed class DummyComparer<T> : IEqualityComparer<T>
        {
            /// <inheritdoc/>
            public bool Equals(T? x, T? y) => throw new NotImplementedException();

            /// <inheritdoc/>
            public int GetHashCode(T obj) => throw new NotImplementedException();
        }
    }
}
