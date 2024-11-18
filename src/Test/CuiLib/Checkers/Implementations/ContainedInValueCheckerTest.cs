using CuiLib.Checkers;
using CuiLib.Checkers.Implementations;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Test.CuiLib.Checkers.Implementations
{
    [TestFixture]
    public class ContainedInValueCheckerTest : TestBase
    {
        private ContainedInValueChecker<int[], int> checker;

        [SetUp]
        public void SetUp()
        {
            checker = new ContainedInValueChecker<int[], int>([1, 2, 3], EqualityComparer<int>.Default);
        }

        #region Ctor

        [Test]
        public void Ctor_WithNullCollection()
        {
            Assert.That(() => ValueChecker.ContainedIn<int[], int>(null!, EqualityComparer<int>.Default), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithEmptyCollection()
        {
            Assert.That(() => ValueChecker.ContainedIn<int[], int>([], EqualityComparer<int>.Default), Throws.ArgumentException);
        }

        [Test]
        public void Ctor_AsPositive_WithNullComparer()
        {
            var checker = new ContainedInValueChecker<int[], int>([1, 2, 3], null);

            Assert.Multiple(() =>
            {
                Assert.That(checker.Source, Is.EqualTo(new[] { 1, 2, 3 }));
                Assert.That(checker.Comparer, Is.EqualTo(EqualityComparer<int>.Default));
            });
        }

        [Test]
        public void Ctor_AsPositive_WithSpecifiedComparer()
        {
            var comparer = new DummyComparer<int>();
            var checker = new ContainedInValueChecker<int[], int>([1, 2, 3], comparer);

            Assert.Multiple(() =>
            {
                Assert.That(checker.Source, Is.EqualTo(new[] { 1, 2, 3 }));
                Assert.That(checker.Comparer, Is.EqualTo(comparer));
            });
        }

        #endregion Ctor

        #region Methods

        [Test]
        public void CheckValue()
        {
            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(1).IsValid, Is.True);
                Assert.That(checker.CheckValue(2).IsValid, Is.True);
                Assert.That(checker.CheckValue(3).IsValid, Is.True);

                Assert.That(checker.CheckValue(0).IsValid, Is.False);
                Assert.That(checker.CheckValue(4).IsValid, Is.False);
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
