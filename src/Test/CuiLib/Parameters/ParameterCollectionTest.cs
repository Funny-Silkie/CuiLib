﻿using CuiLib.Parameters;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Test.CuiLib.Parameters
{
    public class ParameterCollectionTest : TestBase
    {
        private ParameterCollection collection;

        [SetUp]
        public void SetUp()
        {
            collection = [];
        }

        #region Ctors

        [Test]
        public void Ctor()
        {
            Assert.That(() => new ParameterCollection(), Throws.Nothing);
        }

        #endregion Ctors

        #region Properties

        [Test]
        public void AllowAutomaticallyCreate_Get_OnDefault()
        {
            Assert.That(collection.AllowAutomaticallyCreate, Is.False);
        }

#pragma warning disable NUnit2046 // Use CollectionConstraint for better assertion messages in case of failure

        [Test]
        public void Count_Get_OnDefault()
        {
            Assert.That(collection.Count, Is.EqualTo(0));
        }

#pragma warning restore NUnit2046 // Use CollectionConstraint for better assertion messages in case of failure

        [Test]
        public void HasArray_Get_OnDefault()
        {
            Assert.That(collection.HasArray, Is.False);
        }

        [Test]
        public void Interface_ICollection_IsSynchronized_Get()
        {
            Assert.That(((ICollection)collection).IsSynchronized, Is.False);
        }

        [Test]
        public void Interface_ICollection_SyncRoot_Get()
        {
            Assert.That(((ICollection)collection).SyncRoot, Is.Not.Null);
        }

        [Test]
        public void Interface_ICollection_1_IsReadOnly_Get()
        {
            Assert.That(((ICollection<Parameter>)collection).IsReadOnly, Is.False);
        }

        #endregion Properties

        #region Indexer

        [Test]
        public void Indexer_Get_WithNegative()
        {
            Assert.That(() => _ = collection[-1], Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Indexer_Get_WithNotExistingIndex()
        {
            collection.CreateAndAdd<string>("value");

            Assert.That(() => _ = collection[1], Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Indexer_Get_AsPositive_OnHasNoArray()
        {
            SingleValueParameter<string> param1 = collection.CreateAndAdd<string>("value1");
            SingleValueParameter<string> param2 = collection.CreateAndAdd<string>("value2");

            Assert.Multiple(() =>
            {
                Assert.That(collection[0], Is.EqualTo(param1));
                Assert.That(collection[1], Is.EqualTo(param2));
            });
        }

        [Test]
        public void Indexer_Get_AsPositive_OnHasArray()
        {
            SingleValueParameter<string> param1 = collection.CreateAndAdd<string>("value1");
            SingleValueParameter<string> param2 = collection.CreateAndAdd<string>("value2");
            MultipleValueParameter<string> paramArray = collection.CreateAndAddAsArray<string>("array");

            Assert.Multiple(() =>
            {
                Assert.That(collection[0], Is.EqualTo(param1));
                Assert.That(collection[1], Is.EqualTo(param2));
                Assert.That(collection[2], Is.EqualTo(paramArray));
                Assert.That(collection[3], Is.EqualTo(paramArray));
                Assert.That(collection[int.MaxValue], Is.EqualTo(paramArray));
            });
        }

        #endregion Indexer

        #region Methods

        [Test]
        public void CreateAndAdd_WithNullName()
        {
            Assert.That(() => collection.CreateAndAdd<string>(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void CreateAndAdd_WithEmptyName()
        {
            Assert.That(() => collection.CreateAndAdd<string>(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void CreateAndAdd_AsPositive()
        {
            SingleValueParameter<string> param1 = collection.CreateAndAdd<string>("value1");
            SingleValueParameter<string> param2 = collection.CreateAndAdd<string>("value2");
            SingleValueParameter<string> param3 = collection.CreateAndAdd<string>("value3");

            Assert.Multiple(() =>
            {
                Assert.That(param1.Name, Is.EqualTo("value1"));
                Assert.That(param1.Index, Is.EqualTo(0));
                Assert.That(param1.IsArray, Is.False);
                Assert.That(param2.Index, Is.EqualTo(1));
                Assert.That(param3.Index, Is.EqualTo(2));
                Assert.That(collection.HasArray, Is.False);
            });
        }

        [Test]
        public void CreateAndAddAsArray_WithNullName()
        {
            Assert.That(() => collection.CreateAndAddAsArray<string>(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void CreateAndAddAsArray_WithEmptyName()
        {
            Assert.That(() => collection.CreateAndAddAsArray<string>(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void CreateAndAddAsArray_AsPositive()
        {
            MultipleValueParameter<string> param = collection.CreateAndAddAsArray<string>("value");

            Assert.Multiple(() =>
            {
                Assert.That(param.Name, Is.EqualTo("value"));
                Assert.That(param.Index, Is.EqualTo(0));
                Assert.That(param.IsArray, Is.True);
                Assert.That(collection.HasArray, Is.True);
            });
        }

        [Test]
        public void SetValues_OnHasNoArray_WithNotFullValues()
        {
            SingleValueParameter<string> param1 = collection.CreateAndAdd<string>("text");
            SingleValueParameter<int> param2 = collection.CreateAndAdd<int>("num");
            collection.SetValues(["hoge"]);

            Assert.Multiple(() =>
            {
                Assert.That(param1.Value, Is.EqualTo("hoge"));
                Assert.That(param2.ValueAvailable, Is.False);
            });
        }

        [Test]
        public void SetValues_OnHasNoArray_WithFullValues()
        {
            SingleValueParameter<string> param1 = collection.CreateAndAdd<string>("text");
            SingleValueParameter<int> param2 = collection.CreateAndAdd<int>("num");
            collection.SetValues(["hoge", "10"]);

            Assert.Multiple(() =>
            {
                Assert.That(param1.Value, Is.EqualTo("hoge"));
                Assert.That(param2.Value, Is.EqualTo(10));
            });
        }

        [Test]
        public void SetValues_OnHasNoArrayAndNoAutoCreate_WithRedundantValues()
        {
            SingleValueParameter<string> param1 = collection.CreateAndAdd<string>("text");
            SingleValueParameter<int> param2 = collection.CreateAndAdd<int>("num");

            Assert.That(() => collection.SetValues(["hoge", "10", "over"]), Throws.InvalidOperationException);
        }

        [Test]
        public void SetValues_OnHasNoArrayAndAutoCreate_WithRedundantValues()
        {
            collection.AllowAutomaticallyCreate = true;
            SingleValueParameter<string> param1 = collection.CreateAndAdd<string>("text");
            SingleValueParameter<int> param2 = collection.CreateAndAdd<int>("num");
            collection.SetValues(["hoge", "10", "over1", "over2"]);

            Assert.Multiple(() =>
            {
                Assert.That(collection, Has.Count.EqualTo(4));
                Assert.That(param1.Value, Is.EqualTo("hoge"));
                Assert.That(param2.Value, Is.EqualTo(10));
                Assert.That(collection[2], Is.InstanceOf<Parameter<string>>());
                Assert.That(((Parameter<string>)collection[2]).Value, Is.EqualTo("over1"));
                Assert.That(collection[3], Is.InstanceOf<Parameter<string>>());
                Assert.That(((Parameter<string>)collection[3]).Value, Is.EqualTo("over2"));
            });
        }

        [Test]
        public void SetValues_OnHasArray_WithNotFullValues()
        {
            SingleValueParameter<string> paramSingle = collection.CreateAndAdd<string>("text");
            MultipleValueParameter<string> paramArray = collection.CreateAndAddAsArray<string>("array");
            collection.SetValues(["hoge"]);

            Assert.Multiple(() =>
            {
                Assert.That(paramSingle.Value, Is.EqualTo("hoge"));
                Assert.That(paramArray.ValueAvailable, Is.False);
            });
        }

        [Test]
        public void SetValues_OnHasArray_WithFullValues()
        {
            SingleValueParameter<string> paramSingle = collection.CreateAndAdd<string>("text");
            MultipleValueParameter<string> paramArray = collection.CreateAndAddAsArray<string>("array");
            collection.SetValues(["hoge", "val1", "val2", "val3"]);

            Assert.Multiple(() =>
            {
                Assert.That(paramSingle.Value, Is.EqualTo("hoge"));
                Assert.That(paramArray.Value, Is.EqualTo(new[] { "val1", "val2", "val3" }));
            });
        }

        #region Collection Operations

        [Test]
        public void Add_WithNull()
        {
            Assert.That(() => collection.Add(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Add_WithArray_OnHasArray()
        {
            collection.CreateAndAddAsArray<string>("array");

            Assert.That(() => collection.Add(new MultipleValueParameter<string>("other", 0)), Throws.InvalidOperationException);
        }

        [Test]
        public void Add_WithConflictSingle_OnHasArray()
        {
            collection.Add(new MultipleValueParameter<string>("array", 1));

            Assert.That(() => collection.Add(new SingleValueParameter<string>("other", 2)), Throws.ArgumentException);
        }

        [Test]
        public void Add_AsPositive_WithSingle()
        {
            collection.Add(new SingleValueParameter<int>("num", 0));

            Assert.Multiple(() =>
            {
                Assert.That(collection, Has.Count.EqualTo(1));
                Assert.That(collection.HasArray, Is.False);
            });
        }

        [Test]
        public void Add_AsPositive_WithArray_OnHasArray()
        {
            collection.Add(new MultipleValueParameter<string>("array", 1));
            collection.Add(new SingleValueParameter<string>("value", 0));

            Assert.Multiple(() =>
            {
                Assert.That(collection, Has.Count.EqualTo(2));
                Assert.That(collection.HasArray, Is.True);
            });
        }

        [Test]
        public void Add_AsPositive_WithArray_OnHasNoArray()
        {
            collection.Add(new MultipleValueParameter<int>("num", 0));

            Assert.Multiple(() =>
            {
                Assert.That(collection, Has.Count.EqualTo(1));
                Assert.That(collection.HasArray, Is.True);
            });
        }

        [Test]
        public void Contains_WithNull()
        {
            Assert.That(() => collection.Contains(null!), Throws.ArgumentNullException);
        }

#pragma warning disable NUnit2014 // Use SomeItemsConstraint for better assertion messages in case of failure

        [Test]
        public void Contains_AsPositive()
        {
            SingleValueParameter<string> paramSingle = collection.CreateAndAdd<string>("value");
            MultipleValueParameter<string> paramArray = collection.CreateAndAddAsArray<string>("array");

            Assert.Multiple(() =>
            {
                Assert.That(collection.Contains(paramSingle), Is.True);
                Assert.That(collection.Contains(paramArray), Is.True);
                Assert.That(collection.Contains(new SingleValueParameter<string>("value", 0)), Is.False);
                Assert.That(collection.Contains(new MultipleValueParameter<string>("array", 1)), Is.False);
            });
        }

#pragma warning restore NUnit2014 // Use SomeItemsConstraint for better assertion messages in case of failure

        [Test]
        public void ContainsAt()
        {
            collection.CreateAndAdd<string>("value");
            collection.CreateAndAddAsArray<string>("array");

            Assert.Multiple(() =>
            {
                Assert.That(collection.ContainsAt(-1), Is.False);
                Assert.That(collection.ContainsAt(0), Is.True);
                Assert.That(collection.ContainsAt(1), Is.True);
                Assert.That(collection.ContainsAt(2), Is.True);
                Assert.That(collection.ContainsAt(int.MaxValue), Is.True);
            });
        }

        [Test]
        public void Clear()
        {
            collection.CreateAndAdd<string>("value");
            collection.CreateAndAddAsArray<string>("array");

            collection.Clear();

            Assert.Multiple(() =>
            {
                Assert.That(collection, Is.Empty);
                Assert.That(collection.HasArray, Is.False);
            });
        }

        [Test]
        public void CopyTo_WithNullArray()
        {
            Assert.That(() => collection.CopyTo(null!, 0), Throws.ArgumentNullException);
        }

        [Test]
        public void CopyTo_WithNegativeArrayIndex()
        {
            Assert.That(() => collection.CopyTo([], -1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void CopyTo_WithShortArray()
        {
            collection.CreateAndAdd<string>("value");
            collection.CreateAndAddAsArray<string>("array");

            Assert.That(() => collection.CopyTo(new Parameter[2], 1), Throws.ArgumentException);
        }

        [Test]
        public void CopyTo_AsPositive()
        {
            SingleValueParameter<string> paramSingle = collection.CreateAndAdd<string>("value");
            MultipleValueParameter<string> paramArray = collection.CreateAndAddAsArray<string>("array");

            var array = new Parameter[3];
            collection.CopyTo(array, 1);

            Assert.Multiple(() =>
            {
                Assert.That(array[0], Is.Null);
                Assert.That(array[1], Is.EqualTo(paramSingle));
                Assert.That(array[2], Is.EqualTo(paramArray));
            });
        }

        [Test]
        public void Interface_ICollection_CopyTo()
        {
            SingleValueParameter<string> paramSingle = collection.CreateAndAdd<string>("value");
            MultipleValueParameter<string> paramArray = collection.CreateAndAddAsArray<string>("array");

            var array = new Parameter[3];
            ((ICollection)collection).CopyTo(array, 1);

            Assert.Multiple(() =>
            {
                Assert.That(array[0], Is.Null);
                Assert.That(array[1], Is.EqualTo(paramSingle));
                Assert.That(array[2], Is.EqualTo(paramArray));
            });
        }

        [Test]
        public void GetEnumerator()
        {
            SingleValueParameter<string> paramSingle = collection.CreateAndAdd<string>("value");
            MultipleValueParameter<string> paramArray = collection.CreateAndAddAsArray<string>("array");

            using IEnumerator<Parameter> enumerator = collection.GetEnumerator();

            Assert.Multiple(() =>
            {
                Assert.That(enumerator.MoveNext(), Is.True);
                Assert.That(enumerator.Current, Is.EqualTo(paramSingle));

                Assert.That(enumerator.MoveNext(), Is.True);
                Assert.That(enumerator.Current, Is.EqualTo(paramArray));

                Assert.That(enumerator.MoveNext(), Is.False);
            });
        }

        [Test]
        public void Interface_IEnumerable_GetEnumerator()
        {
            SingleValueParameter<string> paramSingle = collection.CreateAndAdd<string>("value");
            MultipleValueParameter<string> paramArray = collection.CreateAndAddAsArray<string>("array");

            IEnumerator enumerator = ((IEnumerable)collection).GetEnumerator();

            Assert.Multiple(() =>
            {
                Assert.That(enumerator.MoveNext(), Is.True);
                Assert.That(enumerator.Current, Is.EqualTo(paramSingle));

                Assert.That(enumerator.MoveNext(), Is.True);
                Assert.That(enumerator.Current, Is.EqualTo(paramArray));

                Assert.That(enumerator.MoveNext(), Is.False);
            });
        }

        [Test]
        public void Remove_WithNull()
        {
            Assert.That(() => collection.Remove(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Remove_AsPositive()
        {
            SingleValueParameter<string> paramSingle = collection.CreateAndAdd<string>("value");
            MultipleValueParameter<string> paramArray = collection.CreateAndAddAsArray<string>("array");

            Assert.Multiple(() =>
            {
                Assert.That(collection.Remove(new SingleValueParameter<string>("value", 0)), Is.False);
                Assert.That(collection.Remove(new MultipleValueParameter<string>("array", 1)), Is.False);
            });

            Assert.Multiple(() =>
            {
                Assert.That(collection.Remove(paramSingle), Is.True);
                Assert.That(collection, Has.Count.EqualTo(1));
                Assert.That(collection.HasArray, Is.True);
                Assert.That(collection.Remove(paramSingle), Is.False);
            });

            Assert.Multiple(() =>
            {
                Assert.That(collection.Remove(paramArray), Is.True);
                Assert.That(collection, Is.Empty);
                Assert.That(collection.HasArray, Is.False);
                Assert.That(collection.Remove(paramArray), Is.False);
            });
        }

        [Test]
        public void RemoveAt_WithNegative()
        {
            Assert.That(() => collection.RemoveAt(-1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void RemoveAt_AsPositive()
        {
            collection.CreateAndAdd<string>("value");
            collection.CreateAndAddAsArray<string>("array");

            Assert.That(collection.RemoveAt(2), Is.False);

            Assert.Multiple(() =>
            {
                Assert.That(collection.RemoveAt(0), Is.True);
                Assert.That(collection, Has.Count.EqualTo(1));
                Assert.That(collection.HasArray, Is.True);
                Assert.That(collection.RemoveAt(0), Is.False);
            });

            Assert.Multiple(() =>
            {
                Assert.That(collection.RemoveAt(1), Is.True);
                Assert.That(collection, Is.Empty);
                Assert.That(collection.HasArray, Is.False);
                Assert.That(collection.RemoveAt(1), Is.False);
            });
        }

        [Test]
        public void TryGetValue()
        {
            SingleValueParameter<string> paramSingle = collection.CreateAndAdd<string>("value");
            MultipleValueParameter<string> paramArray = collection.CreateAndAddAsArray<string>("array");

            Assert.Multiple(() =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(collection.TryGetValue(-1, out Parameter? parameter), Is.False);
                    Assert.That(parameter, Is.Null);
                });
                Assert.Multiple(() =>
                {
                    Assert.That(collection.TryGetValue(0, out Parameter? parameter), Is.True);
                    Assert.That(parameter, Is.EqualTo(paramSingle));
                });
                Assert.Multiple(() =>
                {
                    Assert.That(collection.TryGetValue(1, out Parameter? parameter), Is.True);
                    Assert.That(parameter, Is.EqualTo(paramArray));
                });
                Assert.Multiple(() =>
                {
                    Assert.That(collection.TryGetValue(2, out Parameter? parameter), Is.True);
                    Assert.That(parameter, Is.EqualTo(paramArray));
                });
                Assert.Multiple(() =>
                {
                    Assert.That(collection.TryGetValue(int.MaxValue, out Parameter? parameter), Is.True);
                    Assert.That(parameter, Is.EqualTo(paramArray));
                });
            });
        }

        #endregion Collection Operations

        #endregion Methods
    }
}
