using CuiLib.Commands;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Test.CuiLib.Commands
{
    [TestFixture]
    public class CommandCollectionTest : TestBase
    {
        private CommandCollection hasNoParent;
        private CommandCollection hasParent;
        private Command parent;

        [SetUp]
        public void SetUp()
        {
            parent = new Command("parent");
            hasNoParent = [];
            hasParent = new CommandCollection(parent);
        }

        #region Ctors

        [Test]
        public void CtorWithoutArgs()
        {
            Assert.That(() => new CommandCollection(), Throws.Nothing);
        }

        [Test]
        public void CtorWithParent()
        {
            Assert.That(() => new CommandCollection(new Command("command")), Throws.Nothing);
        }

        #endregion Ctors

        #region Properties

#pragma warning disable NUnit2046 // Use CollectionConstraint for better assertion messages in case of failure

        [Test]
        public void Count_Get_OnDefault()
        {
            Assert.That(hasNoParent.Count, Is.EqualTo(0));
        }

#pragma warning restore NUnit2046 // Use CollectionConstraint for better assertion messages in case of failure

        [Test]
        public void Interface_ICollection_IsSynchronized()
        {
            Assert.That(((ICollection)hasNoParent).IsSynchronized, Is.False);
        }

        [Test]
        public void Interface_ICollection_SyncRoot()
        {
            Assert.That(((ICollection)hasNoParent).SyncRoot, Is.Not.Null);
        }

        [Test]
        public void Interface_ICollection_1_IsReadOnly()
        {
            Assert.That(((ICollection<Command>)hasNoParent).IsReadOnly, Is.False);
        }

        #endregion Properties

        #region Methods

        [Test]
        public void Add_WithNull()
        {
            Assert.That(() => hasNoParent.Add(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Add_WithNameDuplication()
        {
            hasNoParent.Add(new Command("child"));

            Assert.That(() => hasNoParent.Add(new Command("child")), Throws.ArgumentException);
        }

        [Test]
        public void Add_AsPositive_OnHasNoParent()
        {
            var child = new Command("child");
            hasNoParent.Add(child);

            Assert.Multiple(() =>
            {
                Assert.That(hasNoParent, Has.Count.EqualTo(1));
                Assert.That(child.Parent, Is.Null);
            });
        }

        [Test]
        public void Add_AsPositive_OnHasParent()
        {
            var child = new Command("child");
            hasParent.Add(child);

            Assert.Multiple(() =>
            {
                Assert.That(hasParent, Has.Count.EqualTo(1));
                Assert.That(child.Parent, Is.EqualTo(parent));
            });
        }

        [Test]
        public void Clear_OnHasNoParent()
        {
            var children = new Command[3];
            for (int i = 0; i < children.Length; i++)
            {
                var current = new Command($"child{i}");
                children[i] = current;
                hasNoParent.Add(current);
            }

            hasNoParent.Clear();

            Assert.Multiple(() =>
            {
                Assert.That(hasNoParent, Is.Empty);
                foreach (Command child in children) Assert.That(child.Parent, Is.Null);
            });
        }

        [Test]
        public void Clear_OnHasParent()
        {
            var children = new Command[3];
            for (int i = 0; i < children.Length; i++)
            {
                var current = new Command($"child{i}");
                children[i] = current;
                hasParent.Add(current);
            }

            hasParent.Clear();

            Assert.Multiple(() =>
            {
                Assert.That(hasParent, Is.Empty);
                foreach (Command child in children) Assert.That(child.Parent, Is.Null);
            });
        }

        [Test]
        public void Contains_WithCommandName_WithNull()
        {
            Assert.That(() => hasNoParent.Contains(commandName: null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Contains_WithCommandName_AsPositive()
        {
            hasNoParent.Add(new Command("child"));

            Assert.Multiple(() =>
            {
                Assert.That(hasNoParent.Contains(string.Empty), Is.False);
                Assert.That(hasNoParent.Contains("child"), Is.True);
                Assert.That(hasNoParent.Contains("Child"), Is.False);
                Assert.That(hasNoParent.Contains("other"), Is.False);
            });
        }

        [Test]
        public void Contains_WithCommand_WithNull()
        {
            Assert.That(() => hasNoParent.Contains(command: null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Contains_WithCommand_AsPositive()
        {
            var child = new Command("child");
            hasNoParent.Add(child);

            Assert.Multiple(() =>
            {
                Assert.That(hasNoParent, Does.Contain(child));
                Assert.That(hasNoParent, Does.Not.Contain(new Command("child")));
                Assert.That(hasNoParent, Does.Not.Contain(new Command("other")));
            });
        }

        [Test]
        public void CopyTo()
        {
            var children = new Command[3];
            for (int i = 0; i < children.Length; i++)
            {
                var current = new Command($"child{i}");
                children[i] = current;
                hasNoParent.Add(current);
            }

            var destination = new Command[4];
            hasNoParent.CopyTo(destination, 1);

            Assert.Multiple(() =>
            {
                Assert.That(destination[0], Is.Null);
                Assert.That(destination[1], Is.EqualTo(children[0]));
                Assert.That(destination[2], Is.EqualTo(children[1]));
                Assert.That(destination[3], Is.EqualTo(children[2]));
            });
        }

        [Test]
        public void Interface_ICollection_CopyTo()
        {
            var children = new Command[3];
            for (int i = 0; i < children.Length; i++)
            {
                var current = new Command($"child{i}");
                children[i] = current;
                hasNoParent.Add(current);
            }

            var destination = new Command[4];
            ((ICollection)hasNoParent).CopyTo(destination, 1);

            Assert.Multiple(() =>
            {
                Assert.That(destination[0], Is.Null);
                Assert.That(destination[1], Is.EqualTo(children[0]));
                Assert.That(destination[2], Is.EqualTo(children[1]));
                Assert.That(destination[3], Is.EqualTo(children[2]));
            });
        }

        [Test]
        public void GetEnumerator()
        {
            var children = new Command[3];
            for (int i = 0; i < children.Length; i++)
            {
                var current = new Command($"child{i}");
                children[i] = current;
                hasNoParent.Add(current);
            }
            using IEnumerator<Command> enumerator = hasNoParent.GetEnumerator();

            Assert.Multiple(() =>
            {
                Assert.That(enumerator.MoveNext(), Is.True);
                Assert.That(enumerator.Current, Is.EqualTo(children[0]));
            });
            Assert.Multiple(() =>
            {
                Assert.That(enumerator.MoveNext(), Is.True);
                Assert.That(enumerator.Current, Is.EqualTo(children[1]));
            });
            Assert.Multiple(() =>
            {
                Assert.That(enumerator.MoveNext(), Is.True);
                Assert.That(enumerator.Current, Is.EqualTo(children[2]));
                Assert.That(enumerator.MoveNext(), Is.False);
            });
        }

        [Test]
        public void Interface_IEnumearble_GetEnumerator()
        {
            var children = new Command[3];
            for (int i = 0; i < children.Length; i++)
            {
                var current = new Command($"child{i}");
                children[i] = current;
                hasNoParent.Add(current);
            }
            IEnumerator enumerator = ((IEnumerable)hasNoParent).GetEnumerator();

            Assert.Multiple(() =>
            {
                Assert.That(enumerator.MoveNext(), Is.True);
                Assert.That(enumerator.Current, Is.EqualTo(children[0]));
            });
            Assert.Multiple(() =>
            {
                Assert.That(enumerator.MoveNext(), Is.True);
                Assert.That(enumerator.Current, Is.EqualTo(children[1]));
            });
            Assert.Multiple(() =>
            {
                Assert.That(enumerator.MoveNext(), Is.True);
                Assert.That(enumerator.Current, Is.EqualTo(children[2]));
                Assert.That(enumerator.MoveNext(), Is.False);
            });
        }

        [Test]
        public void Remove_WithCommandName_WithNull()
        {
            Assert.That(() => hasNoParent.Remove(commandName: null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Remove_WithCommandName_OnHasNoParent_AsPositive()
        {
            var child1 = new Command("child1");
            var child2 = new Command("child2");
            hasNoParent.Add(child1);
            hasNoParent.Add(child2);

            Assert.Multiple(() =>
            {
                Assert.That(hasNoParent.Remove("other"), Is.False);
                Assert.That(hasNoParent.Remove("Child1"), Is.False);
                Assert.That(hasNoParent.Remove("child1"), Is.True);
                Assert.That(hasNoParent, Has.Count.EqualTo(1));
                Assert.That(child1.Parent, Is.Null);
            });
        }

        [Test]
        public void Remove_WithCommandName_OnHasParent_AsPositive()
        {
            var child1 = new Command("child1");
            var child2 = new Command("child2");
            hasParent.Add(child1);
            hasParent.Add(child2);

            Assert.Multiple(() =>
            {
                Assert.That(hasParent.Remove("other"), Is.False);
                Assert.That(hasParent.Remove("Child1"), Is.False);
                Assert.That(hasParent.Remove("child1"), Is.True);
                Assert.That(hasParent, Has.Count.EqualTo(1));
                Assert.That(child1.Parent, Is.Null);
                Assert.That(child2.Parent, Is.Not.Null);
            });
        }

        [Test]
        public void Remove_WithCommand_WithNull()
        {
            Assert.That(() => hasNoParent.Remove(command: null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Remove_WithCommand_OnHasNoParent_AsPositive()
        {
            var child1 = new Command("child1");
            var child2 = new Command("child2");
            hasNoParent.Add(child1);
            hasNoParent.Add(child2);

            Assert.Multiple(() =>
            {
                Assert.That(hasNoParent.Remove(new Command("other")), Is.False);
                Assert.That(hasNoParent.Remove(new Command("child1")), Is.False);
                Assert.That(hasNoParent.Remove(child1), Is.True);
                Assert.That(hasNoParent, Has.Count.EqualTo(1));
                Assert.That(child1.Parent, Is.Null);
            });
        }

        [Test]
        public void Remove_WithCommand_OnHasParent_AsPositive()
        {
            var child1 = new Command("child1");
            var child2 = new Command("child2");
            hasParent.Add(child1);
            hasParent.Add(child2);

            Assert.Multiple(() =>
            {
                Assert.That(hasParent.Remove(new Command("other")), Is.False);
                Assert.That(hasParent.Remove(new Command("child1")), Is.False);
                Assert.That(hasParent.Remove(child1), Is.True);
                Assert.That(hasParent, Has.Count.EqualTo(1));
                Assert.That(child1.Parent, Is.Null);
                Assert.That(child2.Parent, Is.Not.Null);
            });
        }

        [Test]
        public void TryGetCommand_WithNull()
        {
            Assert.That(() => hasNoParent.TryGetCommand(null!, out _), Throws.ArgumentNullException);
        }

        [Test]
        public void TryGetcommand_AsPositive()
        {
            var child = new Command("child");
            hasNoParent.Add(child);

            Assert.Multiple(() =>
            {
                Assert.That(hasNoParent.TryGetCommand("child", out Command? got), Is.True);
                Assert.That(got, Is.EqualTo(child));
            });
            Assert.Multiple(() =>
            {
                Assert.That(hasNoParent.TryGetCommand("Child", out Command? got), Is.False);
                Assert.That(got, Is.Null);
            });
            Assert.Multiple(() =>
            {
                Assert.That(hasNoParent.TryGetCommand("other", out Command? got), Is.False);
                Assert.That(got, Is.Null);
            });
        }

        #endregion Methods
    }
}
