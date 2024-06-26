﻿using CuiLib.Extensions;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Extensions
{
    [TestFixture]
    public class SpanExtensionsTest : TestBase
    {
#pragma warning disable NUnit2045 // Use Assert.Multiple

        [Test]
        public void GetOrDefault_WithSpan_WithoutDefaultValue()
        {
            Span<object> span = [1, "hoge", true];

            Assert.That(span.GetOrDefault(-1), Is.EqualTo(null));
            Assert.That(span.GetOrDefault(0), Is.EqualTo(1));
            Assert.That(span.GetOrDefault(1), Is.EqualTo("hoge"));
            Assert.That(span.GetOrDefault(2), Is.EqualTo(true));
            Assert.That(span.GetOrDefault(3), Is.EqualTo(null));
        }

        [Test]
        public void GetOrDefault_WithReadOnlySpan_WithoutDefaultValue()
        {
            ReadOnlySpan<object> span = [1, "hoge", true];

            Assert.That(span.GetOrDefault(-1), Is.EqualTo(null));
            Assert.That(span.GetOrDefault(0), Is.EqualTo(1));
            Assert.That(span.GetOrDefault(1), Is.EqualTo("hoge"));
            Assert.That(span.GetOrDefault(2), Is.EqualTo(true));
            Assert.That(span.GetOrDefault(3), Is.EqualTo(null));
        }

        [Test]
        public void GetOrDefault_WithSpan_WithDefaultValue()
        {
            Span<object> span = [1, "hoge", true];
            var defaultValue = new object();

            Assert.That(span.GetOrDefault(-1, defaultValue), Is.EqualTo(defaultValue));
            Assert.That(span.GetOrDefault(0, defaultValue), Is.EqualTo(1));
            Assert.That(span.GetOrDefault(1, defaultValue), Is.EqualTo("hoge"));
            Assert.That(span.GetOrDefault(2, defaultValue), Is.EqualTo(true));
            Assert.That(span.GetOrDefault(3, defaultValue), Is.EqualTo(defaultValue));
        }

        [Test]
        public void GetOrDefault_WithReadOnlySpan_WithDefaultValue()
        {
            ReadOnlySpan<object> span = [1, "hoge", true];
            var defaultValue = new object();

            Assert.That(span.GetOrDefault(-1, defaultValue), Is.EqualTo(defaultValue));
            Assert.That(span.GetOrDefault(0, defaultValue), Is.EqualTo(1));
            Assert.That(span.GetOrDefault(1, defaultValue), Is.EqualTo("hoge"));
            Assert.That(span.GetOrDefault(2, defaultValue), Is.EqualTo(true));
            Assert.That(span.GetOrDefault(3, defaultValue), Is.EqualTo(defaultValue));
        }

        [Test]
        public void SliceOrDefault_WithSpan_WithStart()
        {
            Span<int> span = [1, 2, 3, 4, 5];

            Assert.That(span.SliceOrDefault(0).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(1).ToArray(), Is.EqualTo(new[] { 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(4).ToArray(), Is.EqualTo(new[] { 5 }));
            Assert.That(span.SliceOrDefault(5).ToArray(), Is.Empty);
            Assert.That(span.SliceOrDefault(-1).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
        }

        [Test]
        public void SliceOrDefault_WithReadOnlySpan_WithStart()
        {
            ReadOnlySpan<int> span = [1, 2, 3, 4, 5];

            Assert.That(span.SliceOrDefault(0).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(1).ToArray(), Is.EqualTo(new[] { 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(4).ToArray(), Is.EqualTo(new[] { 5 }));
            Assert.That(span.SliceOrDefault(5).ToArray(), Is.Empty);
            Assert.That(span.SliceOrDefault(-1).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
        }

        [Test]
        public void SliceOrDefault_WithSpan_WithStartAndDefaultValue()
        {
            Span<int> span = [1, 2, 3, 4, 5];
            Span<int> defaultValue = [int.MinValue, int.MaxValue];

            Assert.That(span.SliceOrDefault(0, defaultValue).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(1, defaultValue).ToArray(), Is.EqualTo(new[] { 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(4, defaultValue).ToArray(), Is.EqualTo(new[] { 5 }));
            Assert.That(span.SliceOrDefault(5, defaultValue).ToArray(), Is.EqualTo(defaultValue.ToArray()));
            Assert.That(span.SliceOrDefault(-1, defaultValue).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
        }

        [Test]
        public void SliceOrDefault_WithReadOnlySpan_WithStartAndDefaultValue()
        {
            ReadOnlySpan<int> span = [1, 2, 3, 4, 5];
            ReadOnlySpan<int> defaultValue = [int.MinValue, int.MaxValue];

            Assert.That(span.SliceOrDefault(0, defaultValue).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(1, defaultValue).ToArray(), Is.EqualTo(new[] { 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(4, defaultValue).ToArray(), Is.EqualTo(new[] { 5 }));
            Assert.That(span.SliceOrDefault(5, defaultValue).ToArray(), Is.EqualTo(defaultValue.ToArray()));
            Assert.That(span.SliceOrDefault(-1, defaultValue).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
        }

        [Test]
        public void SliceOrDefault_WithSpan_AsRange()
        {
            Span<int> span = [1, 2, 3, 4, 5];

            Assert.That(span.SliceOrDefault(0, 5).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(1, 3).ToArray(), Is.EqualTo(new[] { 2, 3, 4 }));
            Assert.That(span.SliceOrDefault(4, 1).ToArray(), Is.EqualTo(new[] { 5 }));
            Assert.That(span.SliceOrDefault(4, 1).ToArray(), Is.EqualTo(new[] { 5 }));
            Assert.That(span.SliceOrDefault(-1, 5).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(0, 6).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(5, 1).ToArray(), Is.Empty);
            Assert.That(span.SliceOrDefault(1, 0).ToArray(), Is.Empty);
            Assert.That(span.SliceOrDefault(6, 1).ToArray(), Is.Empty);
        }

        [Test]
        public void SliceOrDefault_WithReadOnlySpan_AsRange()
        {
            ReadOnlySpan<int> span = [1, 2, 3, 4, 5];

            Assert.That(span.SliceOrDefault(0, 5).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(1, 3).ToArray(), Is.EqualTo(new[] { 2, 3, 4 }));
            Assert.That(span.SliceOrDefault(4, 1).ToArray(), Is.EqualTo(new[] { 5 }));
            Assert.That(span.SliceOrDefault(4, 1).ToArray(), Is.EqualTo(new[] { 5 }));
            Assert.That(span.SliceOrDefault(-1, 5).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(0, 6).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(5, 1).ToArray(), Is.Empty);
            Assert.That(span.SliceOrDefault(1, 0).ToArray(), Is.Empty);
            Assert.That(span.SliceOrDefault(6, 1).ToArray(), Is.Empty);
        }

        [Test]
        public void SliceOrDefault_WithSpan_AsRange_WithDefaultValue()
        {
            Span<int> span = [1, 2, 3, 4, 5];
            Span<int> defaultValue = [int.MinValue, int.MaxValue];

            Assert.That(span.SliceOrDefault(0, 5, defaultValue).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(1, 3, defaultValue).ToArray(), Is.EqualTo(new[] { 2, 3, 4 }));
            Assert.That(span.SliceOrDefault(4, 1, defaultValue).ToArray(), Is.EqualTo(new[] { 5 }));
            Assert.That(span.SliceOrDefault(4, 1, defaultValue).ToArray(), Is.EqualTo(new[] { 5 }));
            Assert.That(span.SliceOrDefault(-1, 5).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(0, 6, defaultValue).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(5, 1, defaultValue).ToArray(), Is.EqualTo(defaultValue.ToArray()));
            Assert.That(span.SliceOrDefault(1, 0, defaultValue).ToArray(), Is.Empty);
            Assert.That(span.SliceOrDefault(6, 1, defaultValue).ToArray(), Is.EqualTo(defaultValue.ToArray()));
        }

        [Test]
        public void SliceOrDefault_WithReadOnlySpan_AsRange_WithDefaultValue()
        {
            ReadOnlySpan<int> span = [1, 2, 3, 4, 5];
            ReadOnlySpan<int> defaultValue = [int.MinValue, int.MaxValue];

            Assert.That(span.SliceOrDefault(0, 5, defaultValue).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(1, 3, defaultValue).ToArray(), Is.EqualTo(new[] { 2, 3, 4 }));
            Assert.That(span.SliceOrDefault(4, 1, defaultValue).ToArray(), Is.EqualTo(new[] { 5 }));
            Assert.That(span.SliceOrDefault(4, 1, defaultValue).ToArray(), Is.EqualTo(new[] { 5 }));
            Assert.That(span.SliceOrDefault(-1, 5).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(0, 6, defaultValue).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));
            Assert.That(span.SliceOrDefault(5, 1, defaultValue).ToArray(), Is.EqualTo(defaultValue.ToArray()));
            Assert.That(span.SliceOrDefault(1, 0, defaultValue).ToArray(), Is.Empty);
            Assert.That(span.SliceOrDefault(6, 1, defaultValue).ToArray(), Is.EqualTo(defaultValue.ToArray()));
        }

#pragma warning restore NUnit2045 // Use Assert.Multiple
    }
}
