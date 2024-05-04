using CuiLib.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Test.Helpers;

namespace Test.CuiLib.Options._ValueChecker
{
    [TestFixture]
    public class ValueCheckerTest
    {
        [Test]
        public void AlwaysSuccess_Check()
        {
            IValueChecker<int> checker = ValueChecker.AlwaysSuccess<int>();

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
        public void AlwaysSuccess_Equals()
        {
            Assert.That(ValueChecker.AlwaysSuccess<int>(), Is.EqualTo(ValueChecker.AlwaysSuccess<int>()));
        }

        [Test]
        public void AlwaysSucesss_GetHashCode()
        {
            Assert.That(ValueChecker.AlwaysSuccess<int>().GetHashCode(), Is.EqualTo(ValueChecker.AlwaysSuccess<int>().GetHashCode()));
        }

#pragma warning restore NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void FromDelegate_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => ValueChecker.FromDelegate<string>(null!));
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
            Assert.Throws<ArgumentNullException>(() => ValueChecker.And<int>(null!));
        }

        [Test]
        public void And_WithNullChecker()
        {
            Assert.Throws<ArgumentException>(() => ValueChecker.And(ValueChecker.AlwaysSuccess<int>(), ValueChecker.AlwaysSuccess<int>(), null!));
        }

        [Test]
        public void And_AsPositive()
        {
            IValueChecker<int> checker = ValueChecker.And(ValueChecker.LargerOrEqual(0), ValueChecker.Lower(10));

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
            Assert.Throws<ArgumentNullException>(() => ValueChecker.Or<int>(null!));
        }

        [Test]
        public void Or_WithNullChecker()
        {
            Assert.Throws<ArgumentException>(() => ValueChecker.Or(ValueChecker.AlwaysSuccess<int>(), ValueChecker.AlwaysSuccess<int>(), null!));
        }

        [Test]
        public void Or_AsPositive()
        {
            IValueChecker<int> checker = ValueChecker.Or(ValueChecker.LowerOrEqual(0), ValueChecker.Larger(10));

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
        public void Larger_WithoutIComparer()
        {
            IValueChecker<int> checker = ValueChecker.Larger(100);

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
        public void Larger_WithIComparerAsNull()
        {
            IValueChecker<int> checker = ValueChecker.Larger(100, null);

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
        public void Larger_WithIComparerAsSpecified()
        {
            IValueChecker<int> checker = ValueChecker.Larger(100, Comparer<int>.Create((x, y) => Comparer<int>.Default.Compare(y, x)));

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
        public void LargerOrEqual_WithoutIComparer()
        {
            IValueChecker<int> checker = ValueChecker.LargerOrEqual(100);

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
        public void LargerOrEqual_WithIComparerAsNull()
        {
            IValueChecker<int> checker = ValueChecker.LargerOrEqual(100, null);

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
        public void LargerOrEqual_WithIComparerAsSpecified()
        {
            IValueChecker<int> checker = ValueChecker.LargerOrEqual(100, Comparer<int>.Create((x, y) => Comparer<int>.Default.Compare(y, x)));

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
        public void Lower_WithoutIComparer()
        {
            IValueChecker<int> checker = ValueChecker.Lower(100);

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
        public void Lower_WithIComparerAsNull()
        {
            IValueChecker<int> checker = ValueChecker.Lower(100, null);

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
        public void Lower_WithIComparerAsSpecified()
        {
            IValueChecker<int> checker = ValueChecker.Lower(100, Comparer<int>.Create((x, y) => Comparer<int>.Default.Compare(y, x)));

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
        public void LowerOrEqual_WithoutIComparer()
        {
            IValueChecker<int> checker = ValueChecker.LowerOrEqual(100);

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
        public void LowerOrEqual_WithIComparerAsNull()
        {
            IValueChecker<int> checker = ValueChecker.LowerOrEqual(100, null);

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
        public void LowerOrEqual_WithIComparerAsSpecified()
        {
            IValueChecker<int> checker = ValueChecker.LowerOrEqual(100, Comparer<int>.Create((x, y) => Comparer<int>.Default.Compare(y, x)));

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
        public void Contains_WithoutIEqualityComparer_WithNullCollection()
        {
            Assert.Throws<ArgumentNullException>(() => ValueChecker.Contains<int[], int>(null!));
        }

        [Test]
        public void Contains_WithoutIEqualityComparer_WithEmptyCollection()
        {
            Assert.Throws<ArgumentException>(() => ValueChecker.Contains<int[], int>(Array.Empty<int>()));
        }

        [Test]
        public void Contains_WithoutIEqualityComparer_AsPositive()
        {
            IValueChecker<int> checker = ValueChecker.Contains<int[], int>(new[] { 1, 2, 3 });

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
        public void Contains_WithIEqualityComparer_WithNullCollection()
        {
            Assert.Throws<ArgumentNullException>(() => ValueChecker.Contains<int[], int>(null!, EqualityComparer<int>.Default));
        }

        [Test]
        public void Contains_WithIEqualityComparer_WithEmptyCollection()
        {
            Assert.Throws<ArgumentException>(() => ValueChecker.Contains<int[], int>(Array.Empty<int>(), EqualityComparer<int>.Default));
        }

        [Test]
        public void Contains_WithIEqualityComparerAsNull_AsPositive()
        {
            IValueChecker<int> checker = ValueChecker.Contains<int[], int>(new[] { 1, 2, 3 }, null);

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
        public void Contains_WithIEqualityComparerAsSpecified_AsPositive()
        {
            IValueChecker<int> checker = ValueChecker.Contains(new[] { 1, 2, 3 }, EqualityComparer<int>.Default);

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
        public void NotEmpty_Check()
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
        public void NotEmpty_Equals()
        {
            Assert.That(ValueChecker.NotEmpty(), Is.EqualTo(ValueChecker.NotEmpty()));
        }

        [Test]
        public void NotEmpty_GetHashCode()
        {
            Assert.That(ValueChecker.NotEmpty().GetHashCode(), Is.EqualTo(ValueChecker.NotEmpty().GetHashCode()));
        }

#pragma warning restore NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void StartWith_WithCharAndWithoutStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.StartWith('s');

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
        public void StartWith_WithCharAndWithStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.StartWith('s', StringComparison.OrdinalIgnoreCase);

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
        public void StartWith_WithStringAsNullAndWithoutStringComparison()
        {
            Assert.Throws<ArgumentNullException>(() => ValueChecker.StartWith(null!));
        }

        [Test]
        public void StartWith_WithStringAsEmptyAndWithoutStringComparison()
        {
            Assert.Throws<ArgumentException>(() => ValueChecker.StartWith(string.Empty));
        }

        [Test]
        public void StartWith_WithStringAsPositiveAndWithoutStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.StartWith("st");

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
        public void StartWith_WithStringAsNullAndWithStringComparison()
        {
            Assert.Throws<ArgumentNullException>(() => ValueChecker.StartWith(null!, StringComparison.Ordinal));
        }

        [Test]
        public void StartWith_WithStringAsEmptyAndWithStringComparison()
        {
            Assert.Throws<ArgumentException>(() => ValueChecker.StartWith(string.Empty, StringComparison.Ordinal));
        }

        [Test]
        public void StartWith_WithStringAsPositiveAndWithStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.StartWith("st", StringComparison.OrdinalIgnoreCase);

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
        public void EndWith_WithCharAndWithoutStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.EndWith('d');

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
        public void EndWith_WithCharAndWithStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.EndWith('d', StringComparison.OrdinalIgnoreCase);

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
        public void EndWith_WithStringAsNullAndWithoutStringComparison()
        {
            Assert.Throws<ArgumentNullException>(() => ValueChecker.EndWith(null!));
        }

        [Test]
        public void EndWith_WithStringAsEmptyAndWithoutStringComparison()
        {
            Assert.Throws<ArgumentException>(() => ValueChecker.EndWith(string.Empty));
        }

        [Test]
        public void EndWith_WithStringAsPositiveAndWithoutStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.EndWith("nd");

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
        public void EndWith_WithStringAsNullAndWithStringComparison()
        {
            Assert.Throws<ArgumentNullException>(() => ValueChecker.EndWith(null!, StringComparison.Ordinal));
        }

        [Test]
        public void EndWith_WithStringAsEmptyAndWithStringComparison()
        {
            Assert.Throws<ArgumentException>(() => ValueChecker.EndWith(string.Empty, StringComparison.Ordinal));
        }

        [Test]
        public void EndWith_WithStringAsPositiveAndWithStringComparison()
        {
            IValueChecker<string> checker = ValueChecker.EndWith("nd", StringComparison.OrdinalIgnoreCase);

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
        public void Equals_WithoutIEqualityComparer()
        {
            IValueChecker<string> checker = ValueChecker.Equals("value");

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
        public void Equals_WithIEqualityComparerAsNull()
        {
            IValueChecker<string> checker = ValueChecker.Equals("value", comparer: null!);

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
        public void Equals_WithIEqualityComparerAsSpecified()
        {
            IValueChecker<string> checker = ValueChecker.Equals("value", StringComparer.OrdinalIgnoreCase);

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
        public void NotEquals_WithoutIEqualityComparer()
        {
            IValueChecker<string> checker = ValueChecker.NotEquals("value");

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
        public void NotEquals_WithIEqualityComparerAsNull()
        {
            IValueChecker<string> checker = ValueChecker.NotEquals("value", comparer: null!);

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
        public void NotEquals_WithIEqualityComparerAsSpecified()
        {
            IValueChecker<string> checker = ValueChecker.NotEquals("value", StringComparer.OrdinalIgnoreCase);

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
        public void FileExists_Check()
        {
            IValueChecker<string> checker = ValueChecker.FileExists();

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
        public void FileExists_Equals()
        {
            Assert.That(ValueChecker.FileExists(), Is.EqualTo(ValueChecker.FileExists()));
        }

        [Test]
        public void FileExists_GetHashCode()
        {
            Assert.That(ValueChecker.FileExists().GetHashCode(), Is.EqualTo(ValueChecker.FileExists().GetHashCode()));
        }

#pragma warning restore NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void DirectoryExists_Check()
        {
            IValueChecker<string> checker = ValueChecker.DirectoryExists();

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
        public void DirectoryExists_Equals()
        {
            Assert.That(ValueChecker.DirectoryExists(), Is.EqualTo(ValueChecker.DirectoryExists()));
        }

        [Test]
        public void DirectoryExists_GetHashCode()
        {
            Assert.That(ValueChecker.DirectoryExists().GetHashCode(), Is.EqualTo(ValueChecker.DirectoryExists().GetHashCode()));
        }

#pragma warning restore NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public void VerifySourceFile()
        {
            IValueChecker<FileInfo> checker = ValueChecker.VerifySourceFile();

            FileInfo existing = FileUtilHelpers.GetNoExistingFile();
            try
            {
                existing.Create().Dispose();

                Assert.Multiple(() =>
                {
                    Assert.That(checker.CheckValue(existing).IsValid, Is.True);
                    Assert.That(checker.CheckValue(new FileInfo(Path.Combine(FileUtilHelpers.GetNoExistingDirectory().FullName, "missing.txt"))).IsValid, Is.False);
                    Assert.That(checker.CheckValue(FileUtilHelpers.GetNoExistingFile()).IsValid, Is.False);

                    Assert.Throws<ArgumentNullException>(() => checker.CheckValue(null!));
                });
            }
            finally
            {
                existing.Delete();
            }
        }

        [Test]
        public void VerifyDestinationFile_AsAllowAll()
        {
            IValueChecker<FileInfo> checker = ValueChecker.VerifyDestinationFile(true, true);

            FileInfo existing = FileUtilHelpers.GetNoExistingFile();
            try
            {
                existing.Create().Dispose();

                Assert.Multiple(() =>
                {
                    Assert.That(checker.CheckValue(existing).IsValid, Is.True);
                    Assert.That(checker.CheckValue(new FileInfo(Path.Combine(FileUtilHelpers.GetNoExistingDirectory().FullName, "missing.txt"))).IsValid, Is.True);
                    Assert.That(checker.CheckValue(FileUtilHelpers.GetNoExistingFile()).IsValid, Is.True);

                    Assert.Throws<ArgumentNullException>(() => checker.CheckValue(null!));
                });
            }
            finally
            {
                existing.Delete();
            }
        }

        [Test]
        public void VerifyDestinationFile_AsNotAllowMissedDir()
        {
            IValueChecker<FileInfo> checker = ValueChecker.VerifyDestinationFile(false, true);

            FileInfo existing = FileUtilHelpers.GetNoExistingFile();
            try
            {
                existing.Create().Dispose();

                Assert.Multiple(() =>
                {
                    Assert.That(checker.CheckValue(existing).IsValid, Is.True);
                    Assert.That(checker.CheckValue(new FileInfo(Path.Combine(FileUtilHelpers.GetNoExistingDirectory().FullName, "missing.txt"))).IsValid, Is.False);
                    Assert.That(checker.CheckValue(FileUtilHelpers.GetNoExistingFile()).IsValid, Is.True);

                    Assert.Throws<ArgumentNullException>(() => checker.CheckValue(null!));
                });
            }
            finally
            {
                existing.Delete();
            }
        }

        [Test]
        public void VerifyDestinationFile_AsNotAllowOverwrite()
        {
            IValueChecker<FileInfo> checker = ValueChecker.VerifyDestinationFile(true, false);

            FileInfo existing = FileUtilHelpers.GetNoExistingFile();
            try
            {
                existing.Create().Dispose();

                Assert.Multiple(() =>
                {
                    Assert.That(checker.CheckValue(existing).IsValid, Is.False);
                    Assert.That(checker.CheckValue(new FileInfo(Path.Combine(FileUtilHelpers.GetNoExistingDirectory().FullName, "missing.txt"))).IsValid, Is.True);
                    Assert.That(checker.CheckValue(FileUtilHelpers.GetNoExistingFile()).IsValid, Is.True);

                    Assert.Throws<ArgumentNullException>(() => checker.CheckValue(null!));
                });
            }
            finally
            {
                existing.Delete();
            }
        }

        [Test]
        public void VerifySourceDirectory()
        {
            IValueChecker<DirectoryInfo> checker = ValueChecker.VerifySourceDirectory();

            DirectoryInfo existing = FileUtilHelpers.GetNoExistingDirectory();
            try
            {
                existing.Create();

                Assert.Multiple(() =>
                {
                    Assert.That(checker.CheckValue(existing).IsValid, Is.True);
                    Assert.That(checker.CheckValue(FileUtilHelpers.GetNoExistingDirectory()).IsValid, Is.False);

                    Assert.Throws<ArgumentNullException>(() => checker.CheckValue(null!));
                });
            }
            finally
            {
                existing.Delete();
            }
        }

        [Test]
        public void IsRegexMatch_WithoutRegexOptionsAndTimeSpan_AsNullPattern()
        {
            Assert.Throws<ArgumentNullException>(() => ValueChecker.IsRegexMatch(pattern: null!));
        }

#pragma warning disable RE0001 // 無効な RegEx パターン

        [Test]
        public void IsRegexMatch_WithoutRegexOptionsAndTimeSpan_AsInvalidPattern()
        {
            Assert.Catch<ArgumentException>(() => ValueChecker.IsRegexMatch(@"(\d"));
        }

#pragma warning restore RE0001 // 無効な RegEx パターン

        [Test]
        public void IsRegexMatch_WithoutRegexOptionsAndTimeSpan_AsPositive()
        {
            IValueChecker<string> checker = ValueChecker.IsRegexMatch(@"\d+");

            Assert.Multiple(() =>
            {
                Assert.That(() => checker.CheckValue("123").IsValid, Is.True);
                Assert.That(() => checker.CheckValue("-123-").IsValid, Is.True);
                Assert.That(() => checker.CheckValue("--").IsValid, Is.False);
                Assert.That(() => checker.CheckValue(string.Empty).IsValid, Is.False);

                Assert.Throws<ArgumentNullException>(() => checker.CheckValue(null!));
            });
        }

        [Test]
        public void IsRegexMatch_WithRegexOptions_AsNullPattern()
        {
            Assert.Throws<ArgumentNullException>(() => ValueChecker.IsRegexMatch(null!, RegexOptions.None));
        }

#pragma warning disable RE0001 // 無効な RegEx パターン

        [Test]
        public void IsRegexMatch_WithRegexOptions_AsInvalidPattern()
        {
            Assert.Catch<ArgumentException>(() => ValueChecker.IsRegexMatch(@"(\d", RegexOptions.Compiled));
        }

#pragma warning restore RE0001 // 無効な RegEx パターン

        [Test]
        public void IsRegexMatch_WithRegexOptions_AsInvalidRegexOptions()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ValueChecker.IsRegexMatch(@"\d+", (RegexOptions)2048));
        }

        [Test]
        public void IsRegexMatch_WithRegexOptions_AsPositive()
        {
            IValueChecker<string> checker = ValueChecker.IsRegexMatch(@"\d+", RegexOptions.None);

            Assert.Multiple(() =>
            {
                Assert.That(() => checker.CheckValue("123").IsValid, Is.True);
                Assert.That(() => checker.CheckValue("-123-").IsValid, Is.True);
                Assert.That(() => checker.CheckValue("--").IsValid, Is.False);
                Assert.That(() => checker.CheckValue(string.Empty).IsValid, Is.False);

                Assert.Throws<ArgumentNullException>(() => checker.CheckValue(null!));
            });
        }

        [Test]
        public void IsRegexMatch_WithRegexOptionsAndTimeSpan_AsNullPattern()
        {
            Assert.Throws<ArgumentNullException>(() => ValueChecker.IsRegexMatch(null!, RegexOptions.None, TimeSpan.MaxValue));
        }

#pragma warning disable RE0001 // 無効な RegEx パターン

        [Test]
        public void IsRegexMatch_WithRegexOptionsAndTimeSpan_AsInvalidPattern()
        {
            Assert.Catch<ArgumentException>(() => ValueChecker.IsRegexMatch(@"(\d", RegexOptions.Compiled, TimeSpan.FromSeconds(10)));
        }

#pragma warning restore RE0001 // 無効な RegEx パターン

        [Test]
        public void IsRegexMatch_WithRegexOptionsAndTimeSpan_AsInvalidRegexOptions()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ValueChecker.IsRegexMatch(@"\d+", (RegexOptions)2048, TimeSpan.FromSeconds(10)));
        }

        [Test]
        public void IsRegexMatch_WithRegexOptionsAndTimeSpan_AsInvalidTimeSpan()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ValueChecker.IsRegexMatch(@"\d+", RegexOptions.None, TimeSpan.FromSeconds(-1)));
        }

        [Test]
        public void IsRegexMatch_WithRegexOptionsAndTimeSpan_AsPositive()
        {
            IValueChecker<string> checker = ValueChecker.IsRegexMatch(@"\d+", RegexOptions.None, TimeSpan.FromSeconds(10));

            Assert.Multiple(() =>
            {
                Assert.That(() => checker.CheckValue("123").IsValid, Is.True);
                Assert.That(() => checker.CheckValue("-123-").IsValid, Is.True);
                Assert.That(() => checker.CheckValue("--").IsValid, Is.False);
                Assert.That(() => checker.CheckValue(string.Empty).IsValid, Is.False);

                Assert.Throws<ArgumentNullException>(() => checker.CheckValue(null!));
            });
        }

        [Test]
        public void IsRegexMatch_WithRegex_AsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ValueChecker.IsRegexMatch(regex: null!));
        }

        [Test]
        public void IsRegexMatch_WithRegex_AsPositive()
        {
            IValueChecker<string> checker = ValueChecker.IsRegexMatch(new Regex(@"\d+"));

            Assert.Multiple(() =>
            {
                Assert.That(() => checker.CheckValue("123").IsValid, Is.True);
                Assert.That(() => checker.CheckValue("-123-").IsValid, Is.True);
                Assert.That(() => checker.CheckValue("--").IsValid, Is.False);
                Assert.That(() => checker.CheckValue(string.Empty).IsValid, Is.False);

                Assert.Throws<ArgumentNullException>(() => checker.CheckValue(null!));
            });
        }
    }
}
