using CuiLib.Checkers;
using CuiLib.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Test.Helpers;

namespace Test.CuiLib.Checkers
{
    [TestFixture]
    public class ValueCheckerTest : TestBase
    {
        [Test]
        public void AlwaysValid_Check()
        {
            IValueChecker<int> checker = ValueChecker.AlwaysValid<int>();

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(1).IsValid, Is.True);
                Assert.That(checker.CheckValue(0).IsValid, Is.True);
                Assert.That(checker.CheckValue(-1).IsValid, Is.True);
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.True);
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.True);
            });
        }

#pragma warning disable NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void AlwaysValid_Equals()
        {
            Assert.That(ValueChecker.AlwaysValid<int>(), Is.EqualTo(ValueChecker.AlwaysValid<int>()));
        }

        [Test]
        public void AlwaysValid_GetHashCode()
        {
            Assert.That(ValueChecker.AlwaysValid<int>().GetHashCode(), Is.EqualTo(ValueChecker.AlwaysValid<int>().GetHashCode()));
        }

#pragma warning restore NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void FromDelegate_WithNull()
        {
            Assert.That(() => ValueChecker.FromDelegate<string>(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void FromDelegate_WithFunc()
        {
            IValueChecker<int> checker = ValueChecker.FromDelegate<int>(x =>
            {
                if (x < 0) return ValueCheckState.AsError("ERROR!");
                return ValueCheckState.Success;
            });

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(0).IsValid, Is.True);
                Assert.That(checker.CheckValue(1).IsValid, Is.True);
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.True);
                Assert.That(checker.CheckValue(-1).Error, Is.EqualTo("ERROR!"));
                Assert.That(checker.CheckValue(int.MinValue).Error, Is.EqualTo("ERROR!"));
            });
        }

        [Test]
        public void And_WithNull()
        {
            Assert.That(() => ValueChecker.And<int>(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void And_WithNullChecker()
        {
            Assert.That(() => ValueChecker.And(ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>(), null!), Throws.ArgumentException);
        }

        [Test]
        public void And_AsPositive()
        {
            IValueChecker<int> checker = ValueChecker.And(ValueChecker.GreaterThanOrEqualTo(0), ValueChecker.LessThan(10));

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.False);
                Assert.That(checker.CheckValue(-1).IsValid, Is.False);
                Assert.That(checker.CheckValue(0).IsValid, Is.True);
                Assert.That(checker.CheckValue(1).IsValid, Is.True);
                Assert.That(checker.CheckValue(5).IsValid, Is.True);
                Assert.That(checker.CheckValue(9).IsValid, Is.True);
                Assert.That(checker.CheckValue(10).IsValid, Is.False);
                Assert.That(checker.CheckValue(11).IsValid, Is.False);
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.False);
            });
        }

        [Test]
        public void Or_WithNull()
        {
            Assert.That(() => ValueChecker.Or<int>(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Or_WithNullChecker()
        {
            Assert.That(() => ValueChecker.Or(ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>(), null!), Throws.ArgumentException);
        }

        [Test]
        public void Or_AsPositive()
        {
            IValueChecker<int> checker = ValueChecker.Or(ValueChecker.LessThanOrEqualTo(0), ValueChecker.GreaterThan(10));

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.True);
                Assert.That(checker.CheckValue(-1).IsValid, Is.True);
                Assert.That(checker.CheckValue(0).IsValid, Is.True);
                Assert.That(checker.CheckValue(1).IsValid, Is.False);
                Assert.That(checker.CheckValue(5).IsValid, Is.False);
                Assert.That(checker.CheckValue(9).IsValid, Is.False);
                Assert.That(checker.CheckValue(10).IsValid, Is.False);
                Assert.That(checker.CheckValue(11).IsValid, Is.True);
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.True);
            });
        }

        [Test]
        public void GreaterThan_WithoutIComparer()
        {
            IValueChecker<int> checker = ValueChecker.GreaterThan(100);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.True);
                Assert.That(checker.CheckValue(101).IsValid, Is.True);
                Assert.That(checker.CheckValue(100).IsValid, Is.False);
                Assert.That(checker.CheckValue(99).IsValid, Is.False);
                Assert.That(checker.CheckValue(0).IsValid, Is.False);
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.False);
            });
        }

        [Test]
        public void GreaterThan_WithIComparerAsNull()
        {
            IValueChecker<int> checker = ValueChecker.GreaterThan(100, null);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.True);
                Assert.That(checker.CheckValue(101).IsValid, Is.True);
                Assert.That(checker.CheckValue(100).IsValid, Is.False);
                Assert.That(checker.CheckValue(99).IsValid, Is.False);
                Assert.That(checker.CheckValue(0).IsValid, Is.False);
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.False);
            });
        }

        [Test]
        public void GreaterThan_WithIComparerAsSpecified()
        {
            IValueChecker<int> checker = ValueChecker.GreaterThan(100, Comparer<int>.Create((x, y) => Comparer<int>.Default.Compare(y, x)));

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.False);
                Assert.That(checker.CheckValue(101).IsValid, Is.False);
                Assert.That(checker.CheckValue(100).IsValid, Is.False);
                Assert.That(checker.CheckValue(99).IsValid, Is.True);
                Assert.That(checker.CheckValue(0).IsValid, Is.True);
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.True);
            });
        }

        [Test]
        public void GreaterThanOrEqualTo_WithoutIComparer()
        {
            IValueChecker<int> checker = ValueChecker.GreaterThanOrEqualTo(100);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.True);
                Assert.That(checker.CheckValue(101).IsValid, Is.True);
                Assert.That(checker.CheckValue(100).IsValid, Is.True);
                Assert.That(checker.CheckValue(99).IsValid, Is.False);
                Assert.That(checker.CheckValue(0).IsValid, Is.False);
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.False);
            });
        }

        [Test]
        public void GreaterThanOrEqualTo_WithIComparerAsNull()
        {
            IValueChecker<int> checker = ValueChecker.GreaterThanOrEqualTo(100, null);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.True);
                Assert.That(checker.CheckValue(101).IsValid, Is.True);
                Assert.That(checker.CheckValue(100).IsValid, Is.True);
                Assert.That(checker.CheckValue(99).IsValid, Is.False);
                Assert.That(checker.CheckValue(0).IsValid, Is.False);
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.False);
            });
        }

        [Test]
        public void GreaterThanOrEqualTo_WithIComparerAsSpecified()
        {
            IValueChecker<int> checker = ValueChecker.GreaterThanOrEqualTo(100, Comparer<int>.Create((x, y) => Comparer<int>.Default.Compare(y, x)));

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.False);
                Assert.That(checker.CheckValue(101).IsValid, Is.False);
                Assert.That(checker.CheckValue(100).IsValid, Is.True);
                Assert.That(checker.CheckValue(99).IsValid, Is.True);
                Assert.That(checker.CheckValue(0).IsValid, Is.True);
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.True);
            });
        }

        [Test]
        public void LessThan_WithoutIComparer()
        {
            IValueChecker<int> checker = ValueChecker.LessThan(100);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.False);
                Assert.That(checker.CheckValue(101).IsValid, Is.False);
                Assert.That(checker.CheckValue(100).IsValid, Is.False);
                Assert.That(checker.CheckValue(99).IsValid, Is.True);
                Assert.That(checker.CheckValue(0).IsValid, Is.True);
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.True);
            });
        }

        [Test]
        public void LessThan_WithIComparerAsNull()
        {
            IValueChecker<int> checker = ValueChecker.LessThan(100, null);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.False);
                Assert.That(checker.CheckValue(101).IsValid, Is.False);
                Assert.That(checker.CheckValue(100).IsValid, Is.False);
                Assert.That(checker.CheckValue(99).IsValid, Is.True);
                Assert.That(checker.CheckValue(0).IsValid, Is.True);
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.True);
            });
        }

        [Test]
        public void LessThan_WithIComparerAsSpecified()
        {
            IValueChecker<int> checker = ValueChecker.LessThan(100, Comparer<int>.Create((x, y) => Comparer<int>.Default.Compare(y, x)));

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.True);
                Assert.That(checker.CheckValue(101).IsValid, Is.True);
                Assert.That(checker.CheckValue(100).IsValid, Is.False);
                Assert.That(checker.CheckValue(99).IsValid, Is.False);
                Assert.That(checker.CheckValue(0).IsValid, Is.False);
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.False);
            });
        }

        [Test]
        public void LessThanOrEqualTo_WithoutIComparer()
        {
            IValueChecker<int> checker = ValueChecker.LessThanOrEqualTo(100);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.False);
                Assert.That(checker.CheckValue(101).IsValid, Is.False);
                Assert.That(checker.CheckValue(100).IsValid, Is.True);
                Assert.That(checker.CheckValue(99).IsValid, Is.True);
                Assert.That(checker.CheckValue(0).IsValid, Is.True);
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.True);
            });
        }

        [Test]
        public void LessThanOrEqualTo_WithIComparerAsNull()
        {
            IValueChecker<int> checker = ValueChecker.LessThanOrEqualTo(100, null);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.False);
                Assert.That(checker.CheckValue(101).IsValid, Is.False);
                Assert.That(checker.CheckValue(100).IsValid, Is.True);
                Assert.That(checker.CheckValue(99).IsValid, Is.True);
                Assert.That(checker.CheckValue(0).IsValid, Is.True);
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.True);
            });
        }

        [Test]
        public void LessThanOrEqualTo_WithIComparerAsSpecified()
        {
            IValueChecker<int> checker = ValueChecker.LessThanOrEqualTo(100, Comparer<int>.Create((x, y) => Comparer<int>.Default.Compare(y, x)));

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(int.MaxValue).IsValid, Is.True);
                Assert.That(checker.CheckValue(101).IsValid, Is.True);
                Assert.That(checker.CheckValue(100).IsValid, Is.True);
                Assert.That(checker.CheckValue(99).IsValid, Is.False);
                Assert.That(checker.CheckValue(0).IsValid, Is.False);
                Assert.That(checker.CheckValue(int.MinValue).IsValid, Is.False);
            });
        }

        [Test]
        public void Defined()
        {
            IValueChecker<OptionType> checker = ValueChecker.Defined<OptionType>();

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(OptionType.None).IsValid, Is.True);
                Assert.That(checker.CheckValue(OptionType.Flag).IsValid, Is.True);
                Assert.That(checker.CheckValue(OptionType.Valued).IsValid, Is.True);
                Assert.That(checker.CheckValue(OptionType.SingleValue).IsValid, Is.True);
                Assert.That(checker.CheckValue(OptionType.MultiValue).IsValid, Is.True);
                Assert.That(checker.CheckValue((OptionType)100).IsValid, Is.False);
                Assert.That(checker.CheckValue((OptionType)(-1)).IsValid, Is.False);
                Assert.That(checker.CheckValue((OptionType)(-2)).IsValid, Is.False);
                Assert.That(checker.CheckValue((OptionType)(-4)).IsValid, Is.False);
            });
        }

        [Test]
        public void ContainedIn_WithoutIEqualityComparer_WithNullCollection()
        {
            Assert.That(() => ValueChecker.ContainedIn<int[], int>(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void ContainedIn_WithoutIEqualityComparer_WithEmptyCollection()
        {
            Assert.That(() => ValueChecker.ContainedIn<int[], int>([]), Throws.ArgumentException);
        }

        [Test]
        public void ContainedIn_WithoutIEqualityComparer_AsPositive()
        {
            IValueChecker<int> checker = ValueChecker.ContainedIn<int[], int>([1, 2, 3]);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(1).IsValid, Is.True);
                Assert.That(checker.CheckValue(2).IsValid, Is.True);
                Assert.That(checker.CheckValue(3).IsValid, Is.True);

                Assert.That(checker.CheckValue(0).IsValid, Is.False);
                Assert.That(checker.CheckValue(4).IsValid, Is.False);
            });
        }

        [Test]
        public void ContainedIn_WithIEqualityComparer_WithNullCollection()
        {
            Assert.That(() => ValueChecker.ContainedIn<int[], int>(null!, EqualityComparer<int>.Default), Throws.ArgumentNullException);
        }

        [Test]
        public void ContainedIn_WithIEqualityComparer_WithEmptyCollection()
        {
            Assert.That(() => ValueChecker.ContainedIn<int[], int>([], EqualityComparer<int>.Default), Throws.ArgumentException);
        }

        [Test]
        public void ContainedIn_WithIEqualityComparerAsNull_AsPositive()
        {
            IValueChecker<int> checker = ValueChecker.ContainedIn<int[], int>([1, 2, 3], null);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(1).IsValid, Is.True);
                Assert.That(checker.CheckValue(2).IsValid, Is.True);
                Assert.That(checker.CheckValue(3).IsValid, Is.True);

                Assert.That(checker.CheckValue(0).IsValid, Is.False);
                Assert.That(checker.CheckValue(4).IsValid, Is.False);
            });
        }

        [Test]
        public void ContainedIn_WithIEqualityComparerAsSpecified_AsPositive()
        {
            IValueChecker<int> checker = ValueChecker.ContainedIn(new[] { 1, 2, 3 }, EqualityComparer<int>.Default);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(1).IsValid, Is.True);
                Assert.That(checker.CheckValue(2).IsValid, Is.True);
                Assert.That(checker.CheckValue(3).IsValid, Is.True);

                Assert.That(checker.CheckValue(0).IsValid, Is.False);
                Assert.That(checker.CheckValue(4).IsValid, Is.False);
            });
        }

        [Test]
        public void Empty_As_String_Check()
        {
            IValueChecker<string?> checker = ValueChecker.Empty();

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(null).IsValid, Is.True);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.True);
                Assert.That(checker.CheckValue("not-empty").IsValid, Is.False);
                Assert.That(checker.CheckValue("  ").IsValid, Is.False);
            });
        }

#pragma warning disable NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void Empty_As_String_Equals()
        {
            Assert.That(ValueChecker.Empty(), Is.EqualTo(ValueChecker.Empty()));
        }

        [Test]
        public void Empty_As_String_GetHashCode()
        {
            Assert.That(ValueChecker.Empty().GetHashCode(), Is.EqualTo(ValueChecker.Empty().GetHashCode()));
        }

#pragma warning restore NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void Empty_As_Collection_Check()
        {
            IValueChecker<IEnumerable<char>?> checker = ValueChecker.Empty<char>();

#pragma warning disable IDE0028 // コレクションの初期化を簡略化します
            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(null).IsValid, Is.True);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.True);
                Assert.That(checker.CheckValue([]).IsValid, Is.True);
                Assert.That(checker.CheckValue(new List<char>()).IsValid, Is.True);
                Assert.That(checker.CheckValue(new HashSet<char>()).IsValid, Is.True);

                Assert.That(checker.CheckValue("hoge").IsValid, Is.False);
                Assert.That(checker.CheckValue(['1', '2', '3']).IsValid, Is.False);
                Assert.That(checker.CheckValue(new List<char>() { '1', '2', '3' }).IsValid, Is.False);
                Assert.That(checker.CheckValue(new HashSet<char>() { '1', '2', '3' }).IsValid, Is.False);
            });
#pragma warning restore IDE0028 // コレクションの初期化を簡略化します
        }

#pragma warning disable NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void Empty_As_Collection_Equals()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ValueChecker.Empty<char>(), Is.EqualTo(ValueChecker.Empty<char>()));
                Assert.That(ValueChecker.Empty<int>(), Is.EqualTo(ValueChecker.Empty<int>()));
                Assert.That(ValueChecker.Empty<char>(), Is.Not.EqualTo(ValueChecker.Empty<int>()));
            });
        }

        [Test]
        public void Empty_As_Collection_GetHashCode()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ValueChecker.Empty<char>().GetHashCode(), Is.EqualTo(ValueChecker.Empty<char>().GetHashCode()));
                Assert.That(ValueChecker.Empty<int>().GetHashCode(), Is.EqualTo(ValueChecker.Empty<int>().GetHashCode()));
            });
        }

#pragma warning restore NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void NotEmpty_As_String_Check()
        {
            IValueChecker<string?> checker = ValueChecker.NotEmpty();

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(null).IsValid, Is.False);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.False);
                Assert.That(checker.CheckValue("not-empty").IsValid, Is.True);
                Assert.That(checker.CheckValue("  ").IsValid, Is.True);
            });
        }

#pragma warning disable NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void NotEmpty_As_String_Equals()
        {
            Assert.That(ValueChecker.NotEmpty(), Is.EqualTo(ValueChecker.NotEmpty()));
        }

        [Test]
        public void NotEmpty_As_String_GetHashCode()
        {
            Assert.That(ValueChecker.NotEmpty().GetHashCode(), Is.EqualTo(ValueChecker.NotEmpty().GetHashCode()));
        }

#pragma warning restore NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void NotEmpty_As_Collection_Check()
        {
            IValueChecker<IEnumerable<char>?> checker = ValueChecker.NotEmpty<char>();

#pragma warning disable IDE0028 // コレクションの初期化を簡略化します
            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue(null).IsValid, Is.False);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.False);
                Assert.That(checker.CheckValue([]).IsValid, Is.False);
                Assert.That(checker.CheckValue(new List<char>()).IsValid, Is.False);
                Assert.That(checker.CheckValue(new HashSet<char>()).IsValid, Is.False);

                Assert.That(checker.CheckValue("hoge").IsValid, Is.True);
                Assert.That(checker.CheckValue(['1', '2', '3']).IsValid, Is.True);
                Assert.That(checker.CheckValue(new List<char>() { '1', '2', '3' }).IsValid, Is.True);
                Assert.That(checker.CheckValue(new HashSet<char>() { '1', '2', '3' }).IsValid, Is.True);
            });
#pragma warning restore IDE0028 // コレクションの初期化を簡略化します
        }

#pragma warning disable NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void NotEmpty_As_Collection_Equals()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ValueChecker.NotEmpty<char>(), Is.EqualTo(ValueChecker.NotEmpty<char>()));
                Assert.That(ValueChecker.NotEmpty<int>(), Is.EqualTo(ValueChecker.NotEmpty<int>()));
                Assert.That(ValueChecker.NotEmpty<char>(), Is.Not.EqualTo(ValueChecker.NotEmpty<int>()));
            });
        }

        [Test]
        public void NotEmpty_As_Collection_GetHashCode()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ValueChecker.NotEmpty<char>().GetHashCode(), Is.EqualTo(ValueChecker.NotEmpty<char>().GetHashCode()));
                Assert.That(ValueChecker.NotEmpty<int>().GetHashCode(), Is.EqualTo(ValueChecker.NotEmpty<int>().GetHashCode()));
            });
        }

#pragma warning restore NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void StartsWith_WithCharAndWithoutStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.StartsWith('s');

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue("start").IsValid, Is.True);
                Assert.That(checker.CheckValue("s").IsValid, Is.True);
                Assert.That(checker.CheckValue("Start").IsValid, Is.False);
                Assert.That(checker.CheckValue("S").IsValid, Is.False);
                Assert.That(checker.CheckValue(null!).IsValid, Is.False);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.False);
            });
        }

        [Test]
        public void StartsWith_WithCharAndWithStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.StartsWith('s', StringComparison.OrdinalIgnoreCase);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue("start").IsValid, Is.True);
                Assert.That(checker.CheckValue("s").IsValid, Is.True);
                Assert.That(checker.CheckValue("Start").IsValid, Is.True);
                Assert.That(checker.CheckValue("S").IsValid, Is.True);
                Assert.That(checker.CheckValue(null!).IsValid, Is.False);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.False);
            });
        }

        [Test]
        public void StartsWith_WithStringAsNullAndWithoutStringComparison()
        {
            Assert.That(() => ValueChecker.StartsWith(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void StartsWith_WithStringAsEmptyAndWithoutStringComparison()
        {
            Assert.That(() => ValueChecker.StartsWith(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void StartsWith_WithStringAsPositiveAndWithoutStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.StartsWith("st");

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue("start").IsValid, Is.True);
                Assert.That(checker.CheckValue("st").IsValid, Is.True);
                Assert.That(checker.CheckValue("Start").IsValid, Is.False);
                Assert.That(checker.CheckValue("St").IsValid, Is.False);
                Assert.That(checker.CheckValue(null!).IsValid, Is.False);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.False);
            });
        }

        [Test]
        public void StartsWith_WithStringAsNullAndWithStringComparison()
        {
            Assert.That(() => ValueChecker.StartsWith(null!, StringComparison.Ordinal), Throws.ArgumentNullException);
        }

        [Test]
        public void StartsWith_WithStringAsEmptyAndWithStringComparison()
        {
            Assert.That(() => ValueChecker.StartsWith(string.Empty, StringComparison.Ordinal), Throws.ArgumentException);
        }

        [Test]
        public void StartsWith_WithStringAsPositiveAndWithStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.StartsWith("st", StringComparison.OrdinalIgnoreCase);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue("start").IsValid, Is.True);
                Assert.That(checker.CheckValue("st").IsValid, Is.True);
                Assert.That(checker.CheckValue("Start").IsValid, Is.True);
                Assert.That(checker.CheckValue("St").IsValid, Is.True);
                Assert.That(checker.CheckValue(null!).IsValid, Is.False);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.False);
            });
        }

        [Test]
        public void EndsWith_WithCharAndWithoutStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.EndsWith('d');

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue("end").IsValid, Is.True);
                Assert.That(checker.CheckValue("d").IsValid, Is.True);
                Assert.That(checker.CheckValue("END").IsValid, Is.False);
                Assert.That(checker.CheckValue("D").IsValid, Is.False);
                Assert.That(checker.CheckValue(null!).IsValid, Is.False);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.False);
            });
        }

        [Test]
        public void EndsWith_WithCharAndWithStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.EndsWith('d', StringComparison.OrdinalIgnoreCase);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue("end").IsValid, Is.True);
                Assert.That(checker.CheckValue("d").IsValid, Is.True);
                Assert.That(checker.CheckValue("END").IsValid, Is.True);
                Assert.That(checker.CheckValue("D").IsValid, Is.True);
                Assert.That(checker.CheckValue(null!).IsValid, Is.False);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.False);
            });
        }

        [Test]
        public void EndsWith_WithStringAsNullAndWithoutStringComparison()
        {
            Assert.That(() => ValueChecker.EndsWith(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void EndsWith_WithStringAsEmptyAndWithoutStringComparison()
        {
            Assert.That(() => ValueChecker.EndsWith(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void EndsWith_WithStringAsPositiveAndWithoutStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.EndsWith("nd");

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue("end").IsValid, Is.True);
                Assert.That(checker.CheckValue("nd").IsValid, Is.True);
                Assert.That(checker.CheckValue("END").IsValid, Is.False);
                Assert.That(checker.CheckValue("ND").IsValid, Is.False);
                Assert.That(checker.CheckValue(null!).IsValid, Is.False);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.False);
            });
        }

        [Test]
        public void EndsWith_WithStringAsNullAndWithStringComparison()
        {
            Assert.That(() => ValueChecker.EndsWith(null!, StringComparison.Ordinal), Throws.ArgumentNullException);
        }

        [Test]
        public void EndsWith_WithStringAsEmptyAndWithStringComparison()
        {
            Assert.That(() => ValueChecker.EndsWith(string.Empty, StringComparison.Ordinal), Throws.ArgumentException);
        }

        [Test]
        public void EndsWith_WithStringAsPositiveAndWithStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.EndsWith("nd", StringComparison.OrdinalIgnoreCase);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue("end").IsValid, Is.True);
                Assert.That(checker.CheckValue("nd").IsValid, Is.True);
                Assert.That(checker.CheckValue("END").IsValid, Is.True);
                Assert.That(checker.CheckValue("ND").IsValid, Is.True);
                Assert.That(checker.CheckValue(null!).IsValid, Is.False);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.False);
            });
        }

        [Test]
        public void EqualTo_WithoutIEqualityComparer()
        {
            IValueChecker<string> checker = ValueChecker.EqualTo("value");

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue("value").IsValid, Is.True);
                Assert.That(checker.CheckValue("Value").IsValid, Is.False);
                Assert.That(checker.CheckValue("other").IsValid, Is.False);
                Assert.That(checker.CheckValue(null!).IsValid, Is.False);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.False);
            });
        }

        [Test]
        public void EqualTo_WithIEqualityComparerAsNull()
        {
            IValueChecker<string> checker = ValueChecker.EqualTo("value", comparer: null!);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue("value").IsValid, Is.True);
                Assert.That(checker.CheckValue("Value").IsValid, Is.False);
                Assert.That(checker.CheckValue("other").IsValid, Is.False);
                Assert.That(checker.CheckValue(null!).IsValid, Is.False);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.False);
            });
        }

        [Test]
        public void EqualTo_WithIEqualityComparerAsSpecified()
        {
            IValueChecker<string> checker = ValueChecker.EqualTo("value", StringComparer.OrdinalIgnoreCase);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue("value").IsValid, Is.True);
                Assert.That(checker.CheckValue("Value").IsValid, Is.True);
                Assert.That(checker.CheckValue("other").IsValid, Is.False);
                Assert.That(checker.CheckValue(null!).IsValid, Is.False);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.False);
            });
        }

        [Test]
        public void NotEqualTo_WithoutIEqualityComparer()
        {
            IValueChecker<string> checker = ValueChecker.NotEqualTo("value");

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue("value").IsValid, Is.False);
                Assert.That(checker.CheckValue("Value").IsValid, Is.True);
                Assert.That(checker.CheckValue("other").IsValid, Is.True);
                Assert.That(checker.CheckValue(null!).IsValid, Is.True);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.True);
            });
        }

        [Test]
        public void NotEqualTo_WithIEqualityComparerAsNull()
        {
            IValueChecker<string> checker = ValueChecker.NotEqualTo("value", comparer: null!);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue("value").IsValid, Is.False);
                Assert.That(checker.CheckValue("Value").IsValid, Is.True);
                Assert.That(checker.CheckValue("other").IsValid, Is.True);
                Assert.That(checker.CheckValue(null!).IsValid, Is.True);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.True);
            });
        }

        [Test]
        public void NotEqualTo_WithIEqualityComparerAsSpecified()
        {
            IValueChecker<string> checker = ValueChecker.NotEqualTo("value", StringComparer.OrdinalIgnoreCase);

            Assert.Multiple(() =>
            {
                Assert.That(checker.CheckValue("value").IsValid, Is.False);
                Assert.That(checker.CheckValue("Value").IsValid, Is.False);
                Assert.That(checker.CheckValue("other").IsValid, Is.True);
                Assert.That(checker.CheckValue(null!).IsValid, Is.True);
                Assert.That(checker.CheckValue(string.Empty).IsValid, Is.True);
            });
        }

        [Test]
        public void ExistsAsFile_Check()
        {
            IValueChecker<string> checker = ValueChecker.ExistsAsFile();

            FileInfo existing = FileUtilHelpers.GetNoExistingFile();
            try
            {
                existing.Create().Dispose();
                FileInfo missing = FileUtilHelpers.GetNoExistingFile();

                Assert.Multiple(() =>
                {
                    Assert.That(checker.CheckValue(existing.FullName).IsValid, Is.True);
                    Assert.That(checker.CheckValue(missing.FullName).IsValid, Is.False);
                });
            }
            finally
            {
                existing.Delete();
            }
        }

#pragma warning disable NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void ExistsAsFile_Equals()
        {
            Assert.That(ValueChecker.ExistsAsFile(), Is.EqualTo(ValueChecker.ExistsAsFile()));
        }

        [Test]
        public void ExistsAsFile_GetHashCode()
        {
            Assert.That(ValueChecker.ExistsAsFile().GetHashCode(), Is.EqualTo(ValueChecker.ExistsAsFile().GetHashCode()));
        }

#pragma warning restore NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void ExistsAsDirectory_Check()
        {
            IValueChecker<string> checker = ValueChecker.ExistsAsDirectory();

            DirectoryInfo existing = FileUtilHelpers.GetNoExistingDirectory();
            try
            {
                existing.Create();
                DirectoryInfo missing = FileUtilHelpers.GetNoExistingDirectory();

                Assert.Multiple(() =>
                {
                    Assert.That(checker.CheckValue(existing.FullName).IsValid, Is.True);
                    Assert.That(checker.CheckValue(missing.FullName).IsValid, Is.False);
                });
            }
            finally
            {
                existing.Delete();
            }
        }

#pragma warning disable NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void ExistsAsDirectory_Equals()
        {
            Assert.That(ValueChecker.ExistsAsDirectory(), Is.EqualTo(ValueChecker.ExistsAsDirectory()));
        }

        [Test]
        public void ExistsAsDirectory_GetHashCode()
        {
            Assert.That(ValueChecker.ExistsAsDirectory().GetHashCode(), Is.EqualTo(ValueChecker.ExistsAsDirectory().GetHashCode()));
        }

#pragma warning restore NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void ValidSourceFile()
        {
            IValueChecker<FileInfo> checker = ValueChecker.ValidSourceFile();

            FileInfo existing = FileUtilHelpers.GetNoExistingFile();
            try
            {
                existing.Create().Dispose();
                existing.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(checker.CheckValue(existing).IsValid, Is.True);
                    Assert.That(checker.CheckValue(new FileInfo(Path.Combine(FileUtilHelpers.GetNoExistingDirectory().FullName, "missing.txt"))).IsValid, Is.False);
                    Assert.That(checker.CheckValue(FileUtilHelpers.GetNoExistingFile()).IsValid, Is.False);

                    Assert.That(() => checker.CheckValue(null!), Throws.ArgumentNullException);
                });
            }
            finally
            {
                existing.Delete();
            }
        }

        [Test]
        public void ValidDestinationFile_AsAllowAll()
        {
            IValueChecker<FileInfo> checker = ValueChecker.ValidDestinationFile(true, true);

            FileInfo existing = FileUtilHelpers.GetNoExistingFile();
            try
            {
                existing.Create().Dispose();

                Assert.Multiple(() =>
                {
                    Assert.That(checker.CheckValue(existing).IsValid, Is.True);
                    Assert.That(checker.CheckValue(new FileInfo(Path.Combine(FileUtilHelpers.GetNoExistingDirectory().FullName, "missing.txt"))).IsValid, Is.True);
                    Assert.That(checker.CheckValue(FileUtilHelpers.GetNoExistingFile()).IsValid, Is.True);

                    Assert.That(() => checker.CheckValue(null!), Throws.ArgumentNullException);
                });
            }
            finally
            {
                existing.Delete();
            }
        }

        [Test]
        public void ValidDestinationFile_AsNotAllowMissedDir()
        {
            IValueChecker<FileInfo> checker = ValueChecker.ValidDestinationFile(false, true);

            FileInfo existing = FileUtilHelpers.GetNoExistingFile();
            try
            {
                existing.Create().Dispose();

                Assert.Multiple(() =>
                {
                    Assert.That(checker.CheckValue(existing).IsValid, Is.True);
                    Assert.That(checker.CheckValue(new FileInfo(Path.Combine(FileUtilHelpers.GetNoExistingDirectory().FullName, "missing.txt"))).IsValid, Is.False);
                    Assert.That(checker.CheckValue(FileUtilHelpers.GetNoExistingFile()).IsValid, Is.True);

                    Assert.That(() => checker.CheckValue(null!), Throws.ArgumentNullException);
                });
            }
            finally
            {
                existing.Delete();
            }
        }

        [Test]
        public void ValidDestinationFile_AsNotAllowOverwrite()
        {
            IValueChecker<FileInfo> checker = ValueChecker.ValidDestinationFile(true, false);

            FileInfo existing = FileUtilHelpers.GetNoExistingFile();
            try
            {
                existing.Create().Dispose();
                existing.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(checker.CheckValue(existing).IsValid, Is.False);
                    Assert.That(checker.CheckValue(new FileInfo(Path.Combine(FileUtilHelpers.GetNoExistingDirectory().FullName, "missing.txt"))).IsValid, Is.True);
                    Assert.That(checker.CheckValue(FileUtilHelpers.GetNoExistingFile()).IsValid, Is.True);

                    Assert.That(() => checker.CheckValue(null!), Throws.ArgumentNullException);
                });
            }
            finally
            {
                existing.Delete();
            }
        }

        [Test]
        public void ValidSourceDirectory()
        {
            IValueChecker<DirectoryInfo> checker = ValueChecker.ValidSourceDirectory();

            DirectoryInfo existing = FileUtilHelpers.GetNoExistingDirectory();
            try
            {
                existing.Create();
                existing.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(checker.CheckValue(existing).IsValid, Is.True);
                    Assert.That(checker.CheckValue(FileUtilHelpers.GetNoExistingDirectory()).IsValid, Is.False);

                    Assert.That(() => checker.CheckValue(null!), Throws.ArgumentNullException);
                });
            }
            finally
            {
                existing.Delete();
            }
        }

        [Test]
        public void MatchesWithoutRegexOptionsAndTimeSpan_AsNullPattern()
        {
            Assert.That(() => ValueChecker.Matches(pattern: null!), Throws.ArgumentNullException);
        }

#pragma warning disable RE0001 // 無効な RegEx パターン

        [Test]
        public void MatchesWithoutRegexOptionsAndTimeSpan_AsInvalidPattern()
        {
            Assert.That(() => ValueChecker.Matches(@"(\d"), Throws.InstanceOf<ArgumentException>());
        }

#pragma warning restore RE0001 // 無効な RegEx パターン

        [Test]
        public void MatchesWithoutRegexOptionsAndTimeSpan_AsPositive()
        {
            IValueChecker<string> checker = ValueChecker.Matches(@"\d+");

            Assert.Multiple(() =>
            {
                Assert.That(() => checker.CheckValue("123").IsValid, Is.True);
                Assert.That(() => checker.CheckValue("-123-").IsValid, Is.True);
                Assert.That(() => checker.CheckValue("--").IsValid, Is.False);
                Assert.That(() => checker.CheckValue(string.Empty).IsValid, Is.False);

                Assert.That(() => checker.CheckValue(null!), Throws.ArgumentNullException);
            });
        }

        [Test]
        public void MatchesWithRegexOptions_AsNullPattern()
        {
            Assert.That(() => ValueChecker.Matches(null!, RegexOptions.None), Throws.ArgumentNullException);
        }

#pragma warning disable RE0001 // 無効な RegEx パターン

        [Test]
        public void MatchesWithRegexOptions_AsInvalidPattern()
        {
            Assert.That(() => ValueChecker.Matches(@"(\d", RegexOptions.Compiled), Throws.InstanceOf<ArgumentException>());
        }

#pragma warning restore RE0001 // 無効な RegEx パターン

        [Test]
        public void MatchesWithRegexOptions_AsInvalidRegexOptions()
        {
            Assert.That(() => ValueChecker.Matches(@"\d+", (RegexOptions)2048), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void MatchesWithRegexOptions_AsPositive()
        {
            IValueChecker<string> checker = ValueChecker.Matches(@"\d+", RegexOptions.None);

            Assert.Multiple(() =>
            {
                Assert.That(() => checker.CheckValue("123").IsValid, Is.True);
                Assert.That(() => checker.CheckValue("-123-").IsValid, Is.True);
                Assert.That(() => checker.CheckValue("--").IsValid, Is.False);
                Assert.That(() => checker.CheckValue(string.Empty).IsValid, Is.False);

                Assert.That(() => checker.CheckValue(null!), Throws.ArgumentNullException);
            });
        }

        [Test]
        public void MatchesWithRegexOptionsAndTimeSpan_AsNullPattern()
        {
            Assert.That(() => ValueChecker.Matches(null!, RegexOptions.None, TimeSpan.MaxValue), Throws.ArgumentNullException);
        }

#pragma warning disable RE0001 // 無効な RegEx パターン

        [Test]
        public void MatchesWithRegexOptionsAndTimeSpan_AsInvalidPattern()
        {
            Assert.That(() => ValueChecker.Matches(@"(\d", RegexOptions.Compiled, TimeSpan.FromSeconds(10)), Throws.InstanceOf<ArgumentException>());
        }

#pragma warning restore RE0001 // 無効な RegEx パターン

        [Test]
        public void MatchesWithRegexOptionsAndTimeSpan_AsInvalidRegexOptions()
        {
            Assert.That(() => ValueChecker.Matches(@"\d+", (RegexOptions)2048, TimeSpan.FromSeconds(10)), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void MatchesWithRegexOptionsAndTimeSpan_AsInvalidTimeSpan()
        {
            Assert.That(() => ValueChecker.Matches(@"\d+", RegexOptions.None, TimeSpan.FromSeconds(-1)), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void MatchesWithRegexOptionsAndTimeSpan_AsPositive()
        {
            IValueChecker<string> checker = ValueChecker.Matches(@"\d+", RegexOptions.None, TimeSpan.FromSeconds(10));

            Assert.Multiple(() =>
            {
                Assert.That(() => checker.CheckValue("123").IsValid, Is.True);
                Assert.That(() => checker.CheckValue("-123-").IsValid, Is.True);
                Assert.That(() => checker.CheckValue("--").IsValid, Is.False);
                Assert.That(() => checker.CheckValue(string.Empty).IsValid, Is.False);

                Assert.That(() => checker.CheckValue(null!), Throws.ArgumentNullException);
            });
        }

        [Test]
        public void MatchesWithRegex_AsNull()
        {
            Assert.That(() => ValueChecker.Matches(regex: null!), Throws.ArgumentNullException);
        }

        [Test]
        public void MatchesWithRegex_AsPositive()
        {
#pragma warning disable SYSLIB1045 // 'GeneratedRegexAttribute' に変換します。
            IValueChecker<string> checker = ValueChecker.Matches(new Regex(@"\d+"));
#pragma warning restore SYSLIB1045 // 'GeneratedRegexAttribute' に変換します。

            Assert.Multiple(() =>
            {
                Assert.That(() => checker.CheckValue("123").IsValid, Is.True);
                Assert.That(() => checker.CheckValue("-123-").IsValid, Is.True);
                Assert.That(() => checker.CheckValue("--").IsValid, Is.False);
                Assert.That(() => checker.CheckValue(string.Empty).IsValid, Is.False);

                Assert.That(() => checker.CheckValue(null!), Throws.ArgumentNullException);
            });
        }
    }
}
