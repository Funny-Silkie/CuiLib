using CuiLib.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Test.CuiLib.Extensions
{
    [TestFixture]
    public class StringExtensionsTest : TestBase
    {
        [Test]
        public void CompareTo()
        {
            Assert.Multiple(() =>
            {
                Assert.That('b'.CompareTo('b', StringComparison.Ordinal), Is.EqualTo(0));
                Assert.That('b'.CompareTo('c', StringComparison.Ordinal), Is.LessThan(0));
                Assert.That('b'.CompareTo('a', StringComparison.Ordinal), Is.GreaterThan(0));
                Assert.That('b'.CompareTo('b', StringComparison.OrdinalIgnoreCase), Is.EqualTo(0));
                Assert.That('b'.CompareTo('c', StringComparison.OrdinalIgnoreCase), Is.LessThan(0));
                Assert.That('b'.CompareTo('a', StringComparison.OrdinalIgnoreCase), Is.GreaterThan(0));

                Assert.That('B'.CompareTo('b', StringComparison.Ordinal), Is.LessThan(0));
                Assert.That('B'.CompareTo('c', StringComparison.Ordinal), Is.LessThan(0));
                Assert.That('B'.CompareTo('a', StringComparison.Ordinal), Is.LessThan(0));
                Assert.That('B'.CompareTo('b', StringComparison.OrdinalIgnoreCase), Is.EqualTo(0));
                Assert.That('B'.CompareTo('c', StringComparison.OrdinalIgnoreCase), Is.LessThan(0));
                Assert.That('B'.CompareTo('a', StringComparison.OrdinalIgnoreCase), Is.GreaterThan(0));
            });
        }

        [Test]
        public void Equals()
        {
            Assert.Multiple(() =>
            {
                Assert.That('b'.Equals('b', StringComparison.Ordinal), Is.True);
                Assert.That('b'.Equals('c', StringComparison.Ordinal), Is.False);
                Assert.That('b'.Equals('a', StringComparison.Ordinal), Is.False);
                Assert.That('b'.Equals('b', StringComparison.OrdinalIgnoreCase), Is.True);
                Assert.That('b'.Equals('c', StringComparison.OrdinalIgnoreCase), Is.False);
                Assert.That('b'.Equals('a', StringComparison.OrdinalIgnoreCase), Is.False);

                Assert.That('B'.Equals('b', StringComparison.Ordinal), Is.False);
                Assert.That('B'.Equals('c', StringComparison.Ordinal), Is.False);
                Assert.That('B'.Equals('a', StringComparison.Ordinal), Is.False);
                Assert.That('B'.Equals('b', StringComparison.OrdinalIgnoreCase), Is.True);
                Assert.That('B'.Equals('c', StringComparison.OrdinalIgnoreCase), Is.False);
                Assert.That('B'.Equals('a', StringComparison.OrdinalIgnoreCase), Is.False);
            });
        }

        [Test]
        public void ReplaceAll_WithStringAndIEnumerableAndChar_WithNullValue()
        {
            Assert.That(() => StringExtensions.ReplaceAll((null! as string)!, ['a', 'b', 'c'], 'R').ToString(), Throws.ArgumentNullException);
        }

        [Test]
        public void ReplaceAll_WithStringAndIEnumerableAndChar_WithNullFrom()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => "123".ReplaceAll(null!, '!'), Throws.ArgumentNullException);
                Assert.That(() => string.Empty.ReplaceAll(null!, '!'), Throws.ArgumentNullException);
            });
        }

        [Test]
        public void ReplaceAll_WithStringAndIEnumerableAndChar_AsPositiveWithEmptyValue()
        {
            Assert.That(string.Empty.ReplaceAll(['a', 'b', 'c'], 'R'), Is.Empty);
        }

        [Test]
        public void ReplaceAll_WithStringAndIEnumerableAndChar_AsPositiveWithEmptyFrom()
        {
            Assert.Multiple(() =>
            {
                Assert.That("ABC".ReplaceAll([], 'R'), Is.EqualTo("ABC"));
                Assert.That("ABC".ReplaceAll(Enumerable.Empty<char>(), 'R'), Is.EqualTo("ABC"));
            });
        }

        [Test]
        public void ReplaceAll_WithStringAndIEnumerableAndChar_AsPositiveWithNotEmpty()
        {
            Assert.That("abcdABCD".ReplaceAll(['a', 'b', 'c'], 'R'), Is.EqualTo("RRRdABCD"));
        }

        [Test]
        public void ReplaceAll_WithReadOnlySpanAndIEnumerableAndChar_WithNullFrom()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => "123".AsSpan().ReplaceAll(null!, '!'), Throws.ArgumentNullException);
                Assert.That(() => string.Empty.AsSpan().ReplaceAll(null!, '!'), Throws.ArgumentNullException);
            });
        }

        [Test]
        public void ReplaceAll_WithReadOnlySpanAndIEnumerableAndChar_AsPositiveWithEmptyValue()
        {
            Assert.That(ReadOnlySpan<char>.Empty.ReplaceAll(['a', 'b', 'c'], 'R').ToString(), Is.Empty);
        }

        [Test]
        public void ReplaceAll_WithReadOnlySpanAndIEnumerableAndChar_AsPositiveWithEmptyFrom()
        {
            Assert.Multiple(() =>
            {
                Assert.That("ABC".AsSpan().ReplaceAll([], 'R').ToString(), Is.EqualTo("ABC"));
                Assert.That("ABC".AsSpan().ReplaceAll(Enumerable.Empty<char>(), 'R').ToString(), Is.EqualTo("ABC"));
            });
        }

        [Test]
        public void ReplaceAll_WithReadOnlySpanAndIEnumerableAndChar_AsPositiveWithNotEmpty()
        {
            Assert.That("abcdABCD".AsSpan().ReplaceAll(['a', 'b', 'c'], 'R').ToString(), Is.EqualTo("RRRdABCD"));
        }

        [Test]
        public void ReplaceAll_WithStringAndIDictionary_WithNullValue()
        {
            Assert.That(() => StringExtensions.ReplaceAll((null! as string)!, new Dictionary<char, char>() { { 'a', 'R' } }), Throws.ArgumentNullException);
        }

        [Test]
        public void ReplaceAll_WithStringAndIDictionary_WithNullMap()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => "123".ReplaceAll(null!), Throws.ArgumentNullException);
                Assert.That(() => string.Empty.ReplaceAll(null!), Throws.ArgumentNullException);
            });
        }

        [Test]
        public void ReplaceAll_WithStringAndIDictionary_AsPositiveWithEmptyValue()
        {
            Assert.That(string.Empty.ReplaceAll(new Dictionary<char, char>() { { 'a', 'R' } }), Is.Empty);
        }

        [Test]
        public void ReplaceAll_WithStringAndIDictionary_AsPositiveWithEmptyFrom()
        {
            Assert.That("ABC".ReplaceAll(new Dictionary<char, char>()), Is.EqualTo("ABC"));
        }

        [Test]
        public void ReplaceAll_WithStringAndIDictionary_AsPositiveWithNotEmpty()
        {
            var replaceMap = new Dictionary<char, char>
            {
                { 'a', '1' },
                { 'b', '2' },
                { 'c', '3' },
            };
            Assert.That("abcdABCD".ReplaceAll(replaceMap), Is.EqualTo("123dABCD"));
        }

        [Test]
        public void ReplaceAll_WithReadOnlySpanAndIDictionary_WithNull()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => "123".AsSpan().ReplaceAll(null!), Throws.ArgumentNullException);
                Assert.That(() => string.Empty.AsSpan().ReplaceAll(null!), Throws.ArgumentNullException);
            });
        }

        [Test]
        public void ReplaceAll_WithReadOnlySpanAndIDictionary_AsPositiveWithEmptyValue()
        {
            Assert.That(ReadOnlySpan<char>.Empty.ReplaceAll(new Dictionary<char, char>() { { 'a', 'R' } }).ToString(), Is.Empty);
        }

        [Test]
        public void ReplaceAll_WithReadOnlySpanAndIDictionary_AsPositiveWithEmptyFrom()
        {
            Assert.That("ABC".AsSpan().ReplaceAll(new Dictionary<char, char>()).ToString(), Is.EqualTo("ABC"));
        }

        [Test]
        public void ReplaceAll_WithReadOnlySpanAndIDictionary_AsPositiveWithNotEmpty()
        {
            var replaceMap = new Dictionary<char, char>
            {
                { 'a', '1' },
                { 'b', '2' },
                { 'c', '3' },
            };
            Assert.That("abcdABCD".AsSpan().ReplaceAll(replaceMap).ToString(), Is.EqualTo("123dABCD"));
        }

        [Test]
        public void EscapedSplit_WithStringAndChar_WithNullValue()
        {
            Assert.That(() => StringExtensions.EscapedSplit((null! as string)!, ','), Throws.ArgumentNullException);
        }

        [Test]
        public void EscapedSplit_WithStringAndChar_WithEmptyValue()
        {
            Assert.That(string.Empty.EscapedSplit(','), Is.Empty);
        }

        [Test]
        public void EscapedSplit_WithStringAndChar_WithNoQuoted()
        {
            Assert.That("hoge,fuga,piyo".EscapedSplit(','), Is.EqualTo(new[] { "hoge", "fuga", "piyo" }));
        }

        [Test]
        public void EscapedSplit_WithStringAndChar_WithQuoted()
        {
            Assert.That("\"hoge1,hoge2,hoge3\",'fuga1,fuga2',p\"i'yo,\"dapo\"".EscapedSplit(','), Is.EqualTo(new[] { "hoge1,hoge2,hoge3", "fuga1,fuga2", "p\"i'yo", "dapo" }));
        }

        [Test]
        public void EscapedSplit_WithReadOnlySpanAndChar_WithEmptyValue()
        {
            Assert.That(ReadOnlySpan<char>.Empty.EscapedSplit(','), Is.Empty);
        }

        [Test]
        public void EscapedSplit_WithReadOnlySpanAndChar_WithNoQuoted()
        {
            Assert.That("hoge,fuga,piyo".AsSpan().EscapedSplit(','), Is.EqualTo(new[] { "hoge", "fuga", "piyo" }));
        }

        [Test]
        public void EscapedSplit_WithReadOnlySpanAndChar_WithQuoted()
        {
            Assert.That("\"hoge1,hoge2,hoge3\",'fuga1,fuga2',p\"i'yo,\"dapo\"".AsSpan().EscapedSplit(','), Is.EqualTo(new[] { "hoge1,hoge2,hoge3", "fuga1,fuga2", "p\"i'yo", "dapo" }));
        }

        [Test]
        public void EscapedSplit_WithStringAndString_WithNullValue()
        {
            Assert.That(() => StringExtensions.EscapedSplit((null! as string)!, ","), Throws.ArgumentNullException);
        }

        [Test]
        public void EscapedSplit_WithStringAndString_WithEmptyValue()
        {
            Assert.That(string.Empty.EscapedSplit(","), Is.Empty);
        }

        [Test]
        public void EscapedSplit_WithStringAndString_AsNullValue()
        {
            Assert.That(() => "hoge".EscapedSplit(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void EscapedSplit_WithStringAndString_AsEmptyValue()
        {
            Assert.That(() => "hoge".EscapedSplit(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void EscapedSplit_WithStringAndString_WithShortValue()
        {
            Assert.That("hoge".EscapedSplit("fugafuga"), Is.EqualTo(new[] { "hoge" }));
        }

        [Test]
        public void EscapedSplit_WithStringAndString_WithNoQuotedValue()
        {
            Assert.That("hoge<>fuga<>piyo".EscapedSplit("<>"), Is.EqualTo(new[] { "hoge", "fuga", "piyo" }));
        }

        [Test]
        public void EscapedSplit_WithStringAndString_WithQuotedValue()
        {
            Assert.That("\"hoge1<>hoge2<>hoge3\"<>'fuga1<>fuga2'<>p\"i'yo<>\"dapo\"".EscapedSplit("<>"), Is.EqualTo(new[] { "hoge1<>hoge2<>hoge3", "fuga1<>fuga2", "p\"i'yo", "dapo" }));
        }

        [Test]
        public void EscapedSplit_WithReadOnlySpanAndString_WithEmptyValue()
        {
            Assert.That(ReadOnlySpan<char>.Empty.EscapedSplit(","), Is.Empty);
        }

        [Test]
        public void EscapedSplit_WithReadOnlySpanAndString_AsNullValue()
        {
            Assert.That(() => "hoge".AsSpan().EscapedSplit(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void EscapedSplit_WithReadOnlySpanAndString_AsEmptyValue()
        {
            Assert.That(() => "hoge".AsSpan().EscapedSplit(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void EscapedSplit_WithReadOnlySpanAndString_WithShortValue()
        {
            Assert.That("hoge".AsSpan().EscapedSplit("fugafuga"), Is.EqualTo(new[] { "hoge" }));
        }

        [Test]
        public void EscapedSplit_WithReadOnlySpanAndString_WithNoQuotedValue()
        {
            Assert.That("hoge<>fuga<>piyo".AsSpan().EscapedSplit("<>"), Is.EqualTo(new[] { "hoge", "fuga", "piyo" }));
        }

        [Test]
        public void EscapedSplit_WithReadOnlySpanAndString_WithQuotedValue()
        {
            Assert.That("\"hoge1<>hoge2<>hoge3\"<>'fuga1<>fuga2'<>p\"i'yo<>\"dapo\"".AsSpan().EscapedSplit("<>"), Is.EqualTo(new[] { "hoge1<>hoge2<>hoge3", "fuga1<>fuga2", "p\"i'yo", "dapo" }));
        }
    }
}
