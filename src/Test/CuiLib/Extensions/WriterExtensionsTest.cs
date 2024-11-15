using CuiLib.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Test.Helpers;

namespace Test.CuiLib.Extensions
{
    [TestFixture]
    public class WriterExtensionsTest : TestBase
    {
        private BufferedTextWriter writer;

        private static IEnumerable<T> AsRawEnumerable<T>(IEnumerable<T> source)
        {
            foreach (T current in source) yield return current;
        }

        [SetUp]
        public void SetUp()
        {
            writer = new BufferedTextWriter();
        }

        [TearDown]
        public void TearDown()
        {
            writer.Dispose();
        }

        [Test]
        public void WriteJoinGenericWithChar_WithNullWriter()
        {
            Assert.That(() => WriterExtensions.WriteJoin<string>(null!, ',', Array.Empty<string>()), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteJoinGenericWithChar_WithNullValues()
        {
            Assert.That(() => writer.WriteJoin<string>(',', null!), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteJoinGenericWithChar_AsPositive_WithEmptyArray()
        {
            writer.WriteJoin<object>(',', Array.Empty<object>());

            Assert.That(writer.GetData(), Is.Empty);
        }

        [Test]
        public void WriteJoinGenericWithChar_AsPositive_WithArray()
        {
            writer.WriteJoin<object>(',', new object[] { 1, "hoge", 'c' });

            Assert.That(writer.GetData(), Is.EqualTo("1,hoge,c"));
        }

        [Test]
        public void WriteJoinGenericWithChar_AsPositive_WithEmptyList()
        {
            writer.WriteJoin(',', new List<object>());

            Assert.That(writer.GetData(), Is.Empty);
        }

        [Test]
        public void WriteJoinGenericWithChar_AsPositive_WithList()
        {
            writer.WriteJoin(',', new List<object> { 1, "hoge", 'c' });

            Assert.That(writer.GetData(), Is.EqualTo("1,hoge,c"));
        }

        [Test]
        public void WriteJoinGenericWithChar_AsPositive_WithEmptyIEnumerable()
        {
            writer.WriteJoin(',', AsRawEnumerable(Array.Empty<object>()));

            Assert.That(writer.GetData(), Is.Empty);
        }

        [Test]
        public void WriteJoinGenericWithChar_AsPositive_WithIEnumerable()
        {
            writer.WriteJoin(',', AsRawEnumerable(new object[] { 1, "hoge", 'c' }));

            Assert.That(writer.GetData(), Is.EqualTo("1,hoge,c"));
        }

        [Test]
        public void WriteJoinGenericWithString_WithNullWriter()
        {
            Assert.That(() => WriterExtensions.WriteJoin<string>(null!, ",", Array.Empty<string>()), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteJoinGenericWithString_WithNullValues()
        {
            Assert.That(() => writer.WriteJoin<string>("<>", null!), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteJoinGenericWithString_AsPositive_WithEmptyArray()
        {
            writer.WriteJoin<object>("<>", Array.Empty<object>());

            Assert.That(writer.GetData(), Is.Empty);
        }

        [Test]
        public void WriteJoinGenericWithString_AsPositive_WithArray()
        {
            writer.WriteJoin<object>("<>", new object[] { 1, "hoge", 'c' });

            Assert.That(writer.GetData(), Is.EqualTo("1<>hoge<>c"));
        }

        [Test]
        public void WriteJoinGenericWithString_AsPositive_WithEmptyList()
        {
            writer.WriteJoin("<>", new List<object>());

            Assert.That(writer.GetData(), Is.Empty);
        }

        [Test]
        public void WriteJoinGenericWithString_AsPositive_WithList()
        {
            writer.WriteJoin("<>", new List<object> { 1, "hoge", 'c' });

            Assert.That(writer.GetData(), Is.EqualTo("1<>hoge<>c"));
        }

        [Test]
        public void WriteJoinGenericWithString_AsPositive_WithEmptyIEnumerable()
        {
            writer.WriteJoin("<>", AsRawEnumerable(Array.Empty<object>()));

            Assert.That(writer.GetData(), Is.Empty);
        }

        [Test]
        public void WriteJoinGenericWithString_AsPositive_WithIEnumerable()
        {
            writer.WriteJoin<object>("<>", AsRawEnumerable(new object[] { 1, "hoge", 'c' }));

            Assert.That(writer.GetData(), Is.EqualTo("1<>hoge<>c"));
        }

        [Test]
        public void WriteJoinWithCharAndStringArray_WithNullWriter()
        {
            Assert.That(() => WriterExtensions.WriteJoin(null!, ',', Array.Empty<string>()), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteJoinWithCharAndStringArray_WithNullArray()
        {
            Assert.That(() => writer.WriteJoin(',', (string[])null!), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteJoinWithCharAndStringArray_WithEmpty()
        {
            writer.WriteJoin(',', Array.Empty<string>());

            Assert.That(writer.GetData(), Is.Empty);
        }

        [Test]
        public void WriteJoinWithCharAndStringArray_WithValues()
        {
            writer.WriteJoin(',', new[] { "hoge", "fuga", "piyo" });

            Assert.That(writer.GetData(), Is.EqualTo("hoge,fuga,piyo"));
        }

        [Test]
        public void WriteJoinWithCharAndStringReadOnlySpan_WithNullWriter()
        {
            Assert.That(() => WriterExtensions.WriteJoin(null!, ',', ReadOnlySpan<string?>.Empty), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteJoinWithCharAndStringReadOnlySpan_WithEmpty()
        {
            writer.WriteJoin(',', ReadOnlySpan<string?>.Empty);

            Assert.That(writer.GetData(), Is.Empty);
        }

        [Test]
        public void WriteJoinWithCharAndStringReadOnlySpan_WithValues()
        {
            writer.WriteJoin(',', "hoge", "fuga", "piyo");

            Assert.That(writer.GetData(), Is.EqualTo("hoge,fuga,piyo"));
        }

        [Test]
        public void WriteJoinWithStringAndStringArray_WithNullWriter()
        {
            Assert.That(() => WriterExtensions.WriteJoin(null!, "<>", Array.Empty<string>()), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteJoinWithStringAndStringArray_WithNullArray()
        {
            Assert.That(() => writer.WriteJoin("<>", (string[])null!), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteJoinWithStringAndStringArray_WithEmpty()
        {
            writer.WriteJoin("<>", Array.Empty<string>());

            Assert.That(writer.GetData(), Is.Empty);
        }

        [Test]
        public void WriteJoinWithStringAndStringArray_WithValues()
        {
            writer.WriteJoin("<>", new[] { "hoge", "fuga", "piyo" });

            Assert.That(writer.GetData(), Is.EqualTo("hoge<>fuga<>piyo"));
        }

        [Test]
        public void WriteJoinWithStringAndStringReadOnlySpan_WithNullWriter()
        {
            Assert.That(() => WriterExtensions.WriteJoin(null!, "<>", ReadOnlySpan<string?>.Empty), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteJoinWithStringAndStringReadOnlySpan_WithEmpty()
        {
            writer.WriteJoin("<>", ReadOnlySpan<string?>.Empty);

            Assert.That(writer.GetData(), Is.Empty);
        }

        [Test]
        public void WriteJoinWithStringAndStringReadOnlySpan_WithValues()
        {
            writer.WriteJoin("<>", "hoge", "fuga", "piyo");

            Assert.That(writer.GetData(), Is.EqualTo("hoge<>fuga<>piyo"));
        }

        [Test]
        public void WriteJoinWithCharAndObjectArray_WithNullWriter()
        {
            Assert.That(() => WriterExtensions.WriteJoin(null!, ',', Array.Empty<object>()), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteJoinWithCharAndObjectArray_WithNullArray()
        {
            Assert.That(() => writer.WriteJoin(',', (object[])null!), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteJoinWithCharAndObjectArray_WithEmpty()
        {
            writer.WriteJoin(',', Array.Empty<object>());

            Assert.That(writer.GetData(), Is.Empty);
        }

#pragma warning disable IDE0300 // コレクションの初期化を簡略化します

        [Test]
        public void WriteJoinWithCharAndObjectArray_WithValues()
        {
            writer.WriteJoin(',', new object?[] { 1, "hoge", null, 'c' });

            Assert.That(writer.GetData(), Is.EqualTo("1,hoge,,c"));
        }

#pragma warning restore IDE0300 // コレクションの初期化を簡略化します

        [Test]
        public void WriteJoinWithCharAndObjectReadOnlySpan_WithNullWriter()
        {
            Assert.That(() => WriterExtensions.WriteJoin(null!, ',', ReadOnlySpan<object?>.Empty), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteJoinWithCharAndObjectReadOnlySpan_WithEmpty()
        {
            writer.WriteJoin(',', ReadOnlySpan<object?>.Empty);

            Assert.That(writer.GetData(), Is.Empty);
        }

        [Test]
        public void WriteJoinWithCharAndObjectReadOnlySpan_WithValues()
        {
            writer.WriteJoin(',', 1, "hoge", null, 'c');

            Assert.That(writer.GetData(), Is.EqualTo("1,hoge,,c"));
        }

        [Test]
        public void WriteJoinWithStringAndObjectArray_WithNullWriter()
        {
            Assert.That(() => WriterExtensions.WriteJoin(null!, "<>", Array.Empty<object>()), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteJoinWithStringAndObjectArray_WithNullArray()
        {
            Assert.That(() => writer.WriteJoin("<>", (object[])null!), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteJoinWithStringAndObjectArray_WithEmpty()
        {
            writer.WriteJoin("<>", Array.Empty<object>());

            Assert.That(writer.GetData(), Is.Empty);
        }

#pragma warning disable IDE0300 // コレクションの初期化を簡略化します

        [Test]
        public void WriteJoinWithStringAndObjectArray_WithValues()
        {
            writer.WriteJoin("<>", new object?[] { 1, "hoge", null, 'c' });

            Assert.That(writer.GetData(), Is.EqualTo("1<>hoge<><>c"));
        }

#pragma warning restore IDE0300 // コレクションの初期化を簡略化します

        [Test]
        public void WriteJoinWithStringAndObjectReadOnlySpan_WithNullWriter()
        {
            Assert.That(() => WriterExtensions.WriteJoin(null!, "<>", ReadOnlySpan<object?>.Empty), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteJoinWithStringAndObjectReadOnlySpan_WithEmpty()
        {
            writer.WriteJoin("<>", ReadOnlySpan<object?>.Empty);

            Assert.That(writer.GetData(), Is.Empty);
        }

        [Test]
        public void WriteJoinWithStringAndObjectReadOnlySpan_WithValues()
        {
            writer.WriteJoin("<>", 1, "hoge", null, 'c');

            Assert.That(writer.GetData(), Is.EqualTo("1<>hoge<><>c"));
        }
    }
}
