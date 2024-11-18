using CuiLib.Checkers;
using CuiLib.Checkers.Implementations;
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

            Assert.That(checker, Is.TypeOf<AlwaysValidValueChecker<int>>());
        }

        [Test]
        public void FromDelegate_WithNull()
        {
            Assert.That(() => ValueChecker.FromDelegate<string>(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void FromDelegate_AsPositive()
        {
            Func<int, ValueCheckState> condition = x =>
            {
                if (x < 0) return ValueCheckState.AsError("ERROR!");
                return ValueCheckState.Success;
            };
            IValueChecker<int> checker = ValueChecker.FromDelegate(condition);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<DelegateValueChecker<int>>());
                Assert.That(((DelegateValueChecker<int>)checker).Condition, Is.EqualTo(condition));
            });
        }

        [Test, Obsolete]
        public void And_WithArray_WithNull()
        {
            Assert.That(() => ValueChecker.And<int>(null!), Throws.ArgumentNullException);
        }

        [Test, Obsolete]
        public void And_WithArray_WithNullChecker()
        {
            Assert.That(() => ValueChecker.And(new[] { ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>(), null! }), Throws.ArgumentException);
        }

        [Test, Obsolete]
        public void And_WithArray_AsPositive()
        {
            IValueChecker<int> checker = ValueChecker.And(new[] { ValueChecker.GreaterThanOrEqualTo(0), ValueChecker.LessThan(10) });

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

        [Test, Obsolete]
        public void Or_WithArray_WithNull()
        {
            Assert.That(() => ValueChecker.Or<int>(null!), Throws.ArgumentNullException);
        }

        [Test, Obsolete]
        public void Or_WithArray_WithNullChecker()
        {
            Assert.That(() => ValueChecker.Or(new[] { ValueChecker.AlwaysValid<int>(), ValueChecker.AlwaysValid<int>(), null! }), Throws.ArgumentException);
        }

        [Test, Obsolete]
        public void Or_WithArray_AsPositive()
        {
            IValueChecker<int> checker = ValueChecker.Or(new[] { ValueChecker.LessThanOrEqualTo(0), ValueChecker.GreaterThan(10) });

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
                Assert.That(checker, Is.TypeOf<GreaterThanValueChecker<int>>());
                Assert.That(((GreaterThanValueChecker<int>)checker).Comparison, Is.EqualTo(100));
                Assert.That(((GreaterThanValueChecker<int>)checker).Comparer, Is.EqualTo(Comparer<int>.Default));
            });
        }

        [Test]
        public void GreaterThan_WithIComparerAsNull()
        {
            IValueChecker<int> checker = ValueChecker.GreaterThan(100, null);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<GreaterThanValueChecker<int>>());
                Assert.That(((GreaterThanValueChecker<int>)checker).Comparison, Is.EqualTo(100));
                Assert.That(((GreaterThanValueChecker<int>)checker).Comparer, Is.EqualTo(Comparer<int>.Default));
            });
        }

        [Test]
        public void GreaterThan_WithIComparerAsSpecified()
        {
            Comparer<int> comparer = Comparer<int>.Create((x, y) => Comparer<int>.Default.Compare(y, x));
            IValueChecker<int> checker = ValueChecker.GreaterThan(100, comparer);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<GreaterThanValueChecker<int>>());
                Assert.That(((GreaterThanValueChecker<int>)checker).Comparison, Is.EqualTo(100));
                Assert.That(((GreaterThanValueChecker<int>)checker).Comparer, Is.EqualTo(comparer));
            });
        }

        [Test]
        public void GreaterThanOrEqualTo_WithoutIComparer()
        {
            IValueChecker<int> checker = ValueChecker.GreaterThanOrEqualTo(100);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<GreaterThanOrEqualToValueChecker<int>>());
                Assert.That(((GreaterThanOrEqualToValueChecker<int>)checker).Comparison, Is.EqualTo(100));
                Assert.That(((GreaterThanOrEqualToValueChecker<int>)checker).Comparer, Is.EqualTo(Comparer<int>.Default));
            });
        }

        [Test]
        public void GreaterThanOrEqualTo_WithIComparerAsNull()
        {
            IValueChecker<int> checker = ValueChecker.GreaterThanOrEqualTo(100, null);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<GreaterThanOrEqualToValueChecker<int>>());
                Assert.That(((GreaterThanOrEqualToValueChecker<int>)checker).Comparison, Is.EqualTo(100));
                Assert.That(((GreaterThanOrEqualToValueChecker<int>)checker).Comparer, Is.EqualTo(Comparer<int>.Default));
            });
        }

        [Test]
        public void GreaterThanOrEqualTo_WithIComparerAsSpecified()
        {
            Comparer<int> comparer = Comparer<int>.Create((x, y) => Comparer<int>.Default.Compare(y, x));
            IValueChecker<int> checker = ValueChecker.GreaterThanOrEqualTo(100, comparer);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<GreaterThanOrEqualToValueChecker<int>>());
                Assert.That(((GreaterThanOrEqualToValueChecker<int>)checker).Comparison, Is.EqualTo(100));
                Assert.That(((GreaterThanOrEqualToValueChecker<int>)checker).Comparer, Is.EqualTo(comparer));
            });
        }

        [Test]
        public void LessThan_WithoutIComparer()
        {
            IValueChecker<int> checker = ValueChecker.LessThan(100);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<LessThanValueChecker<int>>());
                Assert.That(((LessThanValueChecker<int>)checker).Comparison, Is.EqualTo(100));
                Assert.That(((LessThanValueChecker<int>)checker).Comparer, Is.EqualTo(Comparer<int>.Default));
            });
        }

        [Test]
        public void LessThan_WithIComparerAsNull()
        {
            IValueChecker<int> checker = ValueChecker.LessThan(100, null);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<LessThanValueChecker<int>>());
                Assert.That(((LessThanValueChecker<int>)checker).Comparison, Is.EqualTo(100));
                Assert.That(((LessThanValueChecker<int>)checker).Comparer, Is.EqualTo(Comparer<int>.Default));
            });
        }

        [Test]
        public void LessThan_WithIComparerAsSpecified()
        {
            Comparer<int> comparer = Comparer<int>.Create((x, y) => Comparer<int>.Default.Compare(y, x));
            IValueChecker<int> checker = ValueChecker.LessThan(100, comparer);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<LessThanValueChecker<int>>());
                Assert.That(((LessThanValueChecker<int>)checker).Comparison, Is.EqualTo(100));
                Assert.That(((LessThanValueChecker<int>)checker).Comparer, Is.EqualTo(comparer));
            });
        }

        [Test]
        public void LessThanOrEqualTo_WithoutIComparer()
        {
            IValueChecker<int> checker = ValueChecker.LessThanOrEqualTo(100);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<LessThanOrEqualToValueChecker<int>>());
                Assert.That(((LessThanOrEqualToValueChecker<int>)checker).Comparison, Is.EqualTo(100));
                Assert.That(((LessThanOrEqualToValueChecker<int>)checker).Comparer, Is.EqualTo(Comparer<int>.Default));
            });
        }

        [Test]
        public void LessThanOrEqualTo_WithIComparerAsNull()
        {
            IValueChecker<int> checker = ValueChecker.LessThanOrEqualTo(100, null);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<LessThanOrEqualToValueChecker<int>>());
                Assert.That(((LessThanOrEqualToValueChecker<int>)checker).Comparison, Is.EqualTo(100));
                Assert.That(((LessThanOrEqualToValueChecker<int>)checker).Comparer, Is.EqualTo(Comparer<int>.Default));
            });
        }

        [Test]
        public void LessThanOrEqualTo_WithIComparerAsSpecified()
        {
            Comparer<int> comparer = Comparer<int>.Create((x, y) => Comparer<int>.Default.Compare(y, x));
            IValueChecker<int> checker = ValueChecker.LessThanOrEqualTo(100, comparer);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<LessThanOrEqualToValueChecker<int>>());
                Assert.That(((LessThanOrEqualToValueChecker<int>)checker).Comparison, Is.EqualTo(100));
                Assert.That(((LessThanOrEqualToValueChecker<int>)checker).Comparer, Is.EqualTo(comparer));
            });
        }

        [Test]
        public void Defined()
        {
            IValueChecker<OptionType> checker = ValueChecker.Defined<OptionType>();

            Assert.That(checker, Is.TypeOf<DefinedEnumValueChecker<OptionType>>());
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
                Assert.That(checker, Is.TypeOf<ContainedInValueChecker<int[], int>>());
                Assert.That(((ContainedInValueChecker<int[], int>)checker).Source, Is.EqualTo(new[] { 1, 2, 3 }));
                Assert.That(((ContainedInValueChecker<int[], int>)checker).Comparer, Is.EqualTo(EqualityComparer<int>.Default));
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
                Assert.That(checker, Is.TypeOf<ContainedInValueChecker<int[], int>>());
                Assert.That(((ContainedInValueChecker<int[], int>)checker).Source, Is.EqualTo(new[] { 1, 2, 3 }));
                Assert.That(((ContainedInValueChecker<int[], int>)checker).Comparer, Is.EqualTo(EqualityComparer<int>.Default));
            });
        }

        [Test]
        public void ContainedIn_WithIEqualityComparerAsSpecified_AsPositive()
        {
            EqualityComparer<int> comparer = EqualityComparer<int>.Default;
            IValueChecker<int> checker = ValueChecker.ContainedIn(new[] { 1, 2, 3 }, comparer);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<ContainedInValueChecker<int[], int>>());
                Assert.That(((ContainedInValueChecker<int[], int>)checker).Source, Is.EqualTo(new[] { 1, 2, 3 }));
                Assert.That(((ContainedInValueChecker<int[], int>)checker).Comparer, Is.SameAs(comparer));
            });
        }

        [Test]
        public void Empty_NonGeneric()
        {
            IValueChecker<string?> checker = ValueChecker.Empty();

            Assert.That(checker, Is.TypeOf<EmptyValueChecker>());
        }

        [Test]
        public void Empty_Generic()
        {
            IValueChecker<IEnumerable<char>?> checker = ValueChecker.Empty<char>();

            Assert.That(checker, Is.TypeOf<EmptyValueChecker<char>>());
        }

        [Test]
        public void NotEmpty_NonGeneric()
        {
            IValueChecker<string?> checker = ValueChecker.NotEmpty();

            Assert.That(checker, Is.TypeOf<NotEmptyValueChecker>());
        }

        [Test]
        public void NotEmpty_As_Collection_Check()
        {
            IValueChecker<IEnumerable<char>?> checker = ValueChecker.NotEmpty<char>();

            Assert.That(checker, Is.TypeOf<NotEmptyValueChecker<char>>());
        }

        [Test]
        public void StartsWith_WithCharAndWithoutStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.StartsWith('s');

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<StartsWithValueChecker>());
                Assert.That(((StartsWithValueChecker)checker).Comparison, Is.EqualTo("s"));
                Assert.That(((StartsWithValueChecker)checker).StringComparison, Is.EqualTo(StringComparison.CurrentCulture));
            });
        }

        [Test]
        public void StartsWith_WithCharAndWithStringComparisonAsInvalid()
        {
            Assert.That(() => ValueChecker.StartsWith('s', (StringComparison)(-1)), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void StartsWith_WithCharAndWithStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.StartsWith('s', StringComparison.OrdinalIgnoreCase);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<StartsWithValueChecker>());
                Assert.That(((StartsWithValueChecker)checker).Comparison, Is.EqualTo("s"));
                Assert.That(((StartsWithValueChecker)checker).StringComparison, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
                Assert.That(checker, Is.TypeOf<StartsWithValueChecker>());
                Assert.That(((StartsWithValueChecker)checker).Comparison, Is.EqualTo("st"));
                Assert.That(((StartsWithValueChecker)checker).StringComparison, Is.EqualTo(StringComparison.CurrentCulture));
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
        public void StartsWith_WithStringAndWithStringComparisonAsInvalid()
        {
            Assert.That(() => ValueChecker.StartsWith("st", (StringComparison)(-1)), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void StartsWith_WithStringAsPositiveAndWithStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.StartsWith("st", StringComparison.OrdinalIgnoreCase);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<StartsWithValueChecker>());
                Assert.That(((StartsWithValueChecker)checker).Comparison, Is.EqualTo("st"));
                Assert.That(((StartsWithValueChecker)checker).StringComparison, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
            });
        }

        [Test]
        public void EndsWith_WithCharAndWithoutStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.EndsWith('d');

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<EndsWithValueChecker>());
                Assert.That(((EndsWithValueChecker)checker).Comparison, Is.EqualTo("d"));
                Assert.That(((EndsWithValueChecker)checker).StringComparison, Is.EqualTo(StringComparison.CurrentCulture));
            });
        }

        [Test]
        public void EndsWith_WithCharAndWithStringComparisonAsInvalid()
        {
            Assert.That(() => ValueChecker.EndsWith('s', (StringComparison)(-1)), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void EndsWith_WithCharAndWithStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.EndsWith('d', StringComparison.OrdinalIgnoreCase);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<EndsWithValueChecker>());
                Assert.That(((EndsWithValueChecker)checker).Comparison, Is.EqualTo("d"));
                Assert.That(((EndsWithValueChecker)checker).StringComparison, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
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
                Assert.That(checker, Is.TypeOf<EndsWithValueChecker>());
                Assert.That(((EndsWithValueChecker)checker).Comparison, Is.EqualTo("nd"));
                Assert.That(((EndsWithValueChecker)checker).StringComparison, Is.EqualTo(StringComparison.CurrentCulture));
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
        public void EndsWith_WithStringAndWithStringComparisonAsInvalid()
        {
            Assert.That(() => ValueChecker.EndsWith("st", (StringComparison)(-1)), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void EndsWith_WithStringAsPositiveAndWithStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.EndsWith("nd", StringComparison.OrdinalIgnoreCase);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<EndsWithValueChecker>());
                Assert.That(((EndsWithValueChecker)checker).Comparison, Is.EqualTo("nd"));
                Assert.That(((EndsWithValueChecker)checker).StringComparison, Is.EqualTo(StringComparison.OrdinalIgnoreCase));
            });
        }

        [Test]
        public void EqualTo_WithoutIEqualityComparer()
        {
            IValueChecker<string> checker = ValueChecker.EqualTo("value");

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<EqualToValueChecker<string>>());
                Assert.That(((EqualToValueChecker<string>)checker).Comparison, Is.EqualTo("value"));
                Assert.That(((EqualToValueChecker<string>)checker).Comparer, Is.EqualTo(EqualityComparer<string>.Default));
            });
        }

        [Test]
        public void EqualTo_WithIEqualityComparerAsNull()
        {
            IValueChecker<string> checker = ValueChecker.EqualTo("value", comparer: null!);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<EqualToValueChecker<string>>());
                Assert.That(((EqualToValueChecker<string>)checker).Comparison, Is.EqualTo("value"));
                Assert.That(((EqualToValueChecker<string>)checker).Comparer, Is.EqualTo(EqualityComparer<string>.Default));
            });
        }

        [Test]
        public void EqualTo_WithIEqualityComparerAsSpecified()
        {
            IValueChecker<string> checker = ValueChecker.EqualTo("value", StringComparer.OrdinalIgnoreCase);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<EqualToValueChecker<string>>());
                Assert.That(((EqualToValueChecker<string>)checker).Comparison, Is.EqualTo("value"));
                Assert.That(((EqualToValueChecker<string>)checker).Comparer, Is.EqualTo(StringComparer.OrdinalIgnoreCase));
            });
        }

        [Test]
        public void NotEqualTo_WithoutIEqualityComparer()
        {
            IValueChecker<string> checker = ValueChecker.NotEqualTo("value");

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<NotEqualToValueChecker<string>>());
                Assert.That(((NotEqualToValueChecker<string>)checker).Comparison, Is.EqualTo("value"));
                Assert.That(((NotEqualToValueChecker<string>)checker).Comparer, Is.EqualTo(EqualityComparer<string>.Default));
            });
        }

        [Test]
        public void NotEqualTo_WithIEqualityComparerAsNull()
        {
            IValueChecker<string> checker = ValueChecker.NotEqualTo("value", comparer: null!);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<NotEqualToValueChecker<string>>());
                Assert.That(((NotEqualToValueChecker<string>)checker).Comparison, Is.EqualTo("value"));
                Assert.That(((NotEqualToValueChecker<string>)checker).Comparer, Is.EqualTo(EqualityComparer<string>.Default));
            });
        }

        [Test]
        public void NotEqualTo_WithIEqualityComparerAsSpecified()
        {
            IValueChecker<string> checker = ValueChecker.NotEqualTo("value", StringComparer.OrdinalIgnoreCase);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<NotEqualToValueChecker<string>>());
                Assert.That(((NotEqualToValueChecker<string>)checker).Comparison, Is.EqualTo("value"));
                Assert.That(((NotEqualToValueChecker<string>)checker).Comparer, Is.EqualTo(StringComparer.OrdinalIgnoreCase));
            });
        }

        [Test]
        public void ExistsAsFile()
        {
            IValueChecker<string> checker = ValueChecker.ExistsAsFile();

            Assert.That(checker, Is.TypeOf<ExistsAsFileValueChecker>());
        }

        [Test]
        public void ExistsAsDirectory()
        {
            IValueChecker<string> checker = ValueChecker.ExistsAsDirectory();

            Assert.That(checker, Is.TypeOf<ExistsAsDirectoryValueChecker>());
        }

        [Test]
        public void ValidSourceFile()
        {
            IValueChecker<FileInfo> checker = ValueChecker.ValidSourceFile();

            Assert.That(checker, Is.TypeOf<ValidSourceFileChecker>());
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void ValidDestinationFile(bool allowMissedDirectory, bool allowOverwrite)
        {
            IValueChecker<FileInfo> checker = ValueChecker.ValidDestinationFile(allowMissedDirectory, allowOverwrite);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<ValidDestinationFileChecker>());
                Assert.That(((ValidDestinationFileChecker)checker).AllowMissedDirectory, Is.EqualTo(allowMissedDirectory));
                Assert.That(((ValidDestinationFileChecker)checker).AllowOverwrite, Is.EqualTo(allowOverwrite));
            });
        }

        [Test]
        public void ValidSourceDirectory()
        {
            IValueChecker<DirectoryInfo> checker = ValueChecker.ValidSourceDirectory();

            Assert.That(checker, Is.TypeOf<ValidSourceDirectoryChecker>());
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
                Assert.That(checker, Is.TypeOf<MatchesValueChecker>());
                Assert.That(((MatchesValueChecker)checker).Regex.ToString(), Is.EqualTo(@"\d+"));
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
                Assert.That(checker, Is.TypeOf<MatchesValueChecker>());
                Assert.That(((MatchesValueChecker)checker).Regex.ToString(), Is.EqualTo(@"\d+"));
                Assert.That(((MatchesValueChecker)checker).Regex.Options, Is.EqualTo(RegexOptions.None));
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
                Assert.That(checker, Is.TypeOf<MatchesValueChecker>());
                Assert.That(((MatchesValueChecker)checker).Regex.ToString(), Is.EqualTo(@"\d+"));
                Assert.That(((MatchesValueChecker)checker).Regex.Options, Is.EqualTo(RegexOptions.None));
                Assert.That(((MatchesValueChecker)checker).Regex.MatchTimeout, Is.EqualTo(TimeSpan.FromSeconds(10)));
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
            var regex = new Regex(@"\d+");
#pragma warning restore SYSLIB1045 // 'GeneratedRegexAttribute' に変換します。

            IValueChecker<string> checker = ValueChecker.Matches(regex);

            Assert.Multiple(() =>
            {
                Assert.That(checker, Is.TypeOf<MatchesValueChecker>());
                Assert.That(((MatchesValueChecker)checker).Regex, Is.EqualTo(regex));
            });
        }
    }
}
