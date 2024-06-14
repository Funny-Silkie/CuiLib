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
        public void ReplaceAllWithIEnumerableAndChar_WithNullFrom()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentNullException>(() => "123".AsSpan().ReplaceAll(null!, '!'));
                Assert.Throws<ArgumentNullException>(() => string.Empty.AsSpan().ReplaceAll(null!, '!'));
            });
        }

        [Test]
        public void ReplaceAllWithIEnumerableAndChar_AsPositiveWithEmptyValue()
        {
            Assert.That(ReadOnlySpan<char>.Empty.ReplaceAll(['a', 'b', 'c'], 'R').ToString(), Is.Empty);
        }

        [Test]
        public void ReplaceAllWithIEnumerableAndChar_AsPositiveWithEmptyFrom()
        {
            Assert.Multiple(() =>
            {
                Assert.That("ABC".AsSpan().ReplaceAll([], 'R').ToString(), Is.EqualTo("ABC"));
                Assert.That("ABC".AsSpan().ReplaceAll(Enumerable.Empty<char>(), 'R').ToString(), Is.EqualTo("ABC"));
            });
        }

        [Test]
        public void ReplaceAllWithIEnumerableAndChar_AsPositiveWithNotEmpty()
        {
            Assert.That("abcdABCD".AsSpan().ReplaceAll(['a', 'b', 'c'], 'R').ToString(), Is.EqualTo("RRRdABCD"));
        }

        [Test]
        public void ReplaceAllWithIDictionary_WithNull()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentNullException>(() => "123".AsSpan().ReplaceAll(null!));
                Assert.Throws<ArgumentNullException>(() => string.Empty.AsSpan().ReplaceAll(null!));
            });
        }

        [Test]
        public void ReplaceAllWithIDictionary_AsPositiveWithEmptyValue()
        {
            Assert.That(ReadOnlySpan<char>.Empty.ReplaceAll(new Dictionary<char, char>() { { 'a', 'R' } }).ToString(), Is.Empty);
        }

        [Test]
        public void ReplaceAllWithIDictionary_AsPositiveWithEmptyFrom()
        {
            Assert.Multiple(() =>
            {
                Assert.That("ABC".AsSpan().ReplaceAll(new Dictionary<char, char>()).ToString(), Is.EqualTo("ABC"));
                Assert.That("ABC".AsSpan().ReplaceAll(new Dictionary<char, char>()).ToString(), Is.EqualTo("ABC"));
            });
        }

        [Test]
        public void ReplaceAllWithIDictionary_AsPositiveWithNotEmpty()
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
        public void EscapedSplitWithChar_WithEmpty()
        {
            Assert.That(ReadOnlySpan<char>.Empty.EscapedSplit(','), Is.Empty);
        }

        [Test]
        public void EscapedSplitWithChar_WithNoQuoted()
        {
            Assert.That("hoge,fuga,piyo".AsSpan().EscapedSplit(','), Is.EqualTo(new[] { "hoge", "fuga", "piyo" }));
        }

        [Test]
        public void EscapedSplitWithChar_WithQuoted()
        {
            Assert.That("\"hoge1,hoge2,hoge3\",'fuga1,fuga2',p\"i'yo,\"dapo\"".AsSpan().EscapedSplit(','), Is.EqualTo(new[] { "hoge1,hoge2,hoge3", "fuga1,fuga2", "p\"i'yo", "dapo" }));
        }

        [Test]
        public void EscapedSplitWithString_WithEmptyValue()
        {
            Assert.That(ReadOnlySpan<char>.Empty.EscapedSplit(","), Is.Empty);
        }

        [Test]
        public void EscapedSplitWithString_AsNullValue()
        {
            Assert.Throws<ArgumentNullException>(() => "hoge".AsSpan().EscapedSplit(null!));
        }

        [Test]
        public void EscapedSplitWithString_AsEmptyValue()
        {
            Assert.Throws<ArgumentException>(() => "hoge".AsSpan().EscapedSplit(string.Empty));
        }

        [Test]
        public void EscapedSplitWithString_WithShortValue()
        {
            Assert.That("hoge".AsSpan().EscapedSplit("fugafuga"), Is.EqualTo(new[] { "hoge" }));
        }

        [Test]
        public void EscapedSplitWithString_WithNoQuotedValue()
        {
            Assert.That("hoge<>fuga<>piyo".AsSpan().EscapedSplit("<>"), Is.EqualTo(new[] { "hoge", "fuga", "piyo" }));
        }

        [Test]
        public void EscapedSplitWithString_WithQuotedValue()
        {
            Assert.That("\"hoge1<>hoge2<>hoge3\"<>'fuga1<>fuga2'<>p\"i'yo<>\"dapo\"".AsSpan().EscapedSplit("<>"), Is.EqualTo(new[] { "hoge1<>hoge2<>hoge3", "fuga1<>fuga2", "p\"i'yo", "dapo" }));
        }
    }
}
