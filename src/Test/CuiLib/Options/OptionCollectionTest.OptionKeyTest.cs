using CuiLib.Options;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Test.CuiLib.Options
{
    public partial class OptionCollectionTest
    {
        [TestFixture]
        public class OptionKeyTest
        {
            private OptionCollection.OptionKey keys;

            [SetUp]
            public void SetUp()
            {
                keys = new OptionCollection.OptionKey(["s", "full"]);
            }

            #region Ctors

            [Test]
            public void Ctor_WithEmpty()
            {
                Assert.Throws<ArgumentException>(() => new OptionCollection.OptionKey([]));
            }

            [Test]
            public void Ctor_AsPositive()
            {
                Assert.DoesNotThrow(() => new OptionCollection.OptionKey(["s", "full"]));
            }

            #endregion Ctors

            #region Methods

            [Test]
            public void ContainsName()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(keys.ContainName("s"), Is.True);
                    Assert.That(keys.ContainName("full"), Is.True);
                    Assert.That(keys.ContainName("-s"), Is.False);
                    Assert.That(keys.ContainName("--full"), Is.False);
                    Assert.That(keys.ContainName(string.Empty), Is.False);
                });
            }

            [Test]
            public void HasSameNames()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(keys.HasSameNames(["s", "full"]), Is.True);
                    Assert.That(keys.HasSameNames(["s"]), Is.False);
                    Assert.That(keys.HasSameNames(["full"]), Is.False);
                    Assert.That(keys.HasSameNames(["s", "full", "hoge"]), Is.False);
                });
            }

#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

            [Test]
            public void Equals_WithObject()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(keys.Equals(obj: keys), Is.True);
                    Assert.That(keys.Equals(obj: new OptionCollection.OptionKey(["s", "full"])), Is.True);
                    Assert.That(keys.Equals(obj: new OptionCollection.OptionKey(["other"])), Is.False);
                    Assert.That(keys.Equals(obj: 1), Is.False);
                    Assert.That(keys.Equals(obj: "other"), Is.False);
                    Assert.That(keys.Equals(obj: new[] { "s", "full" }), Is.False);
                    Assert.That(keys.Equals(obj: null), Is.False);
                });
            }

            [Test]
            public void Equals_WithOptionKeys()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(keys.Equals(other: keys), Is.True);
                    Assert.That(keys.Equals(other: new OptionCollection.OptionKey(["s", "full"])), Is.True);
                    Assert.That(keys.Equals(other: new OptionCollection.OptionKey(["other"])), Is.False);
                    Assert.That(keys.Equals(other: null), Is.False);
                });
            }

#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

            [Test]
            public new void GetHashCode()
            {
                Assert.Multiple(() =>
                {
#pragma warning disable NUnit2009 // The same value has been provided as both the actual and the expected argument
                    Assert.That(keys.GetHashCode(), Is.EqualTo(keys.GetHashCode()));
#pragma warning restore NUnit2009 // The same value has been provided as both the actual and the expected argument

                    Assert.That(keys.GetHashCode(), Is.EqualTo(new OptionCollection.OptionKey(["s", "full"]).GetHashCode()));
                });
            }

            [Test]
            public void GetEnumerator()
            {
                var list = new List<string>();
                foreach (string current in keys) list.Add(current);

                Assert.That(list, Is.EquivalentTo(new[] { "s", "full" }));
            }

            [Test]
            public void GetEnumerator_AsNonGeneric()
            {
                var list = new List<string>();
                foreach (string current in (IEnumerable)keys) list.Add(current);

                Assert.That(list, Is.EquivalentTo(new[] { "s", "full" }));
            }

            #endregion Methods
        }
    }
}
