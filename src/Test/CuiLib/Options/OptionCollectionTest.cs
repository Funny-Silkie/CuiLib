using CuiLib.Options;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Test.CuiLib.Options
{
    [TestFixture]
    public partial class OptionCollectionTest
    {
        private OptionCollection collection;

        [SetUp]
        public void SetUp()
        {
            collection = [];
        }

        #region Properties

#pragma warning disable NUnit2046 // Use CollectionConstraint for better assertion messages in case of failure

        [Test]
        public void Count_OnDefault()
        {
            Assert.That(collection.Count, Is.Zero);
        }

#pragma warning restore NUnit2046 // Use CollectionConstraint for better assertion messages in case of failure

        [Test]
        public void IsSynchronized_Get()
        {
            Assert.That(((ICollection)collection).IsSynchronized, Is.False);
        }

        [Test]
        public void IsReadOnly_Get()
        {
            Assert.That(((ICollection<Option>)collection).IsReadOnly, Is.False);
        }

        #endregion Properties

        #region Methods

        [Test]
        public void Add_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => collection.Add(null!));
        }

        [Test]
        public void Add_OnNameCollision()
        {
            collection.Add(new FlagOption('s', "short"));

            Assert.Throws<ArgumentException>(() => collection.Add(new FlagOption('s', "collision")));
        }

        [Test]
        public void Add_AsPositive()
        {
            collection.Add(new FlagOption('o', "out"));

            Assert.That(collection, Has.Count.EqualTo(1));
        }

        [Test]
        public void Clear()
        {
            collection.Add(new FlagOption('f', "flag"));
            collection.Clear();

            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void Contains_WithString()
        {
            collection.Add(new FlagOption('f', "flag"));

            Assert.Multiple(() =>
            {
                Assert.That(collection.Contains("f"), Is.True);
                Assert.That(collection.Contains("flag"), Is.True);
                Assert.That(collection.Contains("-f"), Is.False);
                Assert.That(collection.Contains("--flag"), Is.False);
                Assert.That(collection.Contains(name: null), Is.False);
            });
        }

#pragma warning disable NUnit2014 // Use SomeItemsConstraint for better assertion messages in case of failure

        [Test]
        public void Contains_WithOption()
        {
            var option = new FlagOption('f', "flag");
            collection.Add(option);

            Assert.Multiple(() =>
            {
                Assert.That(collection.Contains(option), Is.True);
                Assert.That(collection.Contains(new FlagOption('f', "flag")), Is.False);
                Assert.That(collection.Contains(option: null!), Is.False);
            });
        }

#pragma warning restore NUnit2014 // Use SomeItemsConstraint for better assertion messages in case of failure

        [Test]
        public void GetEnumerator()
        {
            var option1 = new FlagOption('h', "help");
            var option2 = new FlagOption('v', "version");
            collection.Add(option1);
            collection.Add(option2);

            var list = new List<Option>();
            foreach (Option current in collection) list.Add(current);

            Assert.That(list, Is.EquivalentTo(new[] { option1, option2 }));
        }

        [Test]
        public void GetEnumerator_AsNonGeneric()
        {
            var option1 = new FlagOption('h', "help");
            var option2 = new FlagOption('v', "version");
            collection.Add(option1);
            collection.Add(option2);

            var list = new List<Option>();
            foreach (object? current in (IEnumerable)collection) list.Add((Option)current!);

            Assert.That(list, Is.EquivalentTo(new[] { option1, option2 }));
        }

        [Test]
        public void Remove()
        {
            var option = new FlagOption('h', "help");
            collection.Add(option);

            Assert.That(collection.Remove(new FlagOption("other")), Is.False);
            bool removed = collection.Remove(option);
            Assert.Multiple(() =>
            {
                Assert.That(removed, Is.True);
                Assert.That(collection, Is.Empty);
                Assert.That(collection.Remove(option), Is.False);
            });
        }

        [Test]
        public void TryGetValueWithChar()
        {
            var option = new FlagOption('f', "flag");
            collection.Add(option);

            Assert.Multiple(() =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(collection.TryGetValue('f', out Option? actual), Is.True);
                    Assert.That(actual, Is.EqualTo(option));
                });
                Assert.Multiple(() =>
                {
                    Assert.That(collection.TryGetValue('?', out Option? actual), Is.False);
                    Assert.That(actual, Is.Null);
                });
            });
        }

        [Test]
        public void TryGetValueWithString()
        {
            var option = new FlagOption('f', "flag");
            collection.Add(option);

            Assert.Multiple(() =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(collection.TryGetValue("flag", out Option? actual), Is.True);
                    Assert.That(actual, Is.EqualTo(option));
                });
                Assert.Multiple(() =>
                {
                    Assert.That(collection.TryGetValue(name: null, out Option? actual), Is.False);
                    Assert.That(actual, Is.Null);
                });
                Assert.Multiple(() =>
                {
                    Assert.That(collection.TryGetValue(string.Empty, out Option? actual), Is.False);
                    Assert.That(actual, Is.Null);
                });
                Assert.Multiple(() =>
                {
                    Assert.That(collection.TryGetValue("unknown", out Option? actual), Is.False);
                    Assert.That(actual, Is.Null);
                });
            });
        }

        #endregion Methods
    }
}
