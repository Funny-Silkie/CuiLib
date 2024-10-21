using CuiLib.Data;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Test.CuiLib.Data
{
    public class ValueRangeTest : TestBase
    {
        private ValueRange single;
        private ValueRange range;
        private ValueRange nullStart;
        private ValueRange nullEnd;

        [SetUp]
        public void SetUp()
        {
            single = new ValueRange(10);
            range = new ValueRange(1, 3);
            nullStart = new ValueRange(end: 10);
            nullEnd = new ValueRange(start: 10);
        }

        #region Ctors

        [Test]
        public void Ctor_WithSingleValue_WithNegative()
        {
            Assert.That(() => new ValueRange(-1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(100)]
        [TestCase(int.MaxValue)]
        public void Ctor_WithSingleValue_AsPositive(int value)
        {
            var range = new ValueRange(value);

            Assert.Multiple(() =>
            {
                Assert.That(range.Start, Is.EqualTo(value));
                Assert.That(range.End, Is.EqualTo(value));
            });
        }

        [Test]
        public void Ctor_WithTwoValues_WithNegative()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => new ValueRange(-1, 1), Throws.TypeOf<ArgumentOutOfRangeException>());
                Assert.That(() => new ValueRange(0, -1), Throws.TypeOf<ArgumentOutOfRangeException>());
            });
        }

        [Test]
        public void Ctor_WithTwoValues_AsInvalidStartAndEndRelation()
        {
            Assert.That(() => new ValueRange(3, 1), Throws.ArgumentException);
        }

        [TestCase(0, 0)]
        [TestCase(1, 3)]
        [TestCase(3, 10)]
        [TestCase(int.MaxValue, int.MaxValue)]
        public void Ctor_AsPositive(int start, int end)
        {
            var range = new ValueRange(start, end);

            Assert.Multiple(() =>
            {
                Assert.That(range.Start, Is.EqualTo(start));
                Assert.That(range.End, Is.EqualTo(end));
            });
        }

        [Test]
        public void Ctor_WithTwoNullableValues_WithTwoNulls()
        {
            Assert.That(() => new ValueRange(null, null), Throws.ArgumentException);
        }

        [Test]
        public void Ctor_WithTwoNullableValues_WithNegative()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => new ValueRange(start: -1), Throws.TypeOf<ArgumentOutOfRangeException>());
                Assert.That(() => new ValueRange(end: -1), Throws.TypeOf<ArgumentOutOfRangeException>());
            });
        }

        [Test]
        public void Ctor_WithTwoNullableValues_AsInvalidStartAndEndRelation()
        {
            Assert.That(() => new ValueRange((int?)3, 1), Throws.ArgumentException);
        }

        [TestCase(0, 0)]
        [TestCase(1, 3)]
        [TestCase(3, 10)]
        [TestCase(int.MaxValue, int.MaxValue)]
        public void Ctor_WithTwoNullableValues_WithNotNullValues(int start, int end)
        {
            var range = new ValueRange((int?)start, end);

            Assert.Multiple(() =>
            {
                Assert.That(range.Start, Is.EqualTo(start));
                Assert.That(range.End, Is.EqualTo(end));
            });
        }

        [TestCase(0)]
        [TestCase(3)]
        [TestCase(10)]
        [TestCase(int.MaxValue)]
        public void Ctor_WithTwoNullableValues_WithNullEnd(int start)
        {
            var range = new ValueRange(start: start);

            Assert.Multiple(() =>
            {
                Assert.That(range.Start, Is.EqualTo(start));
                Assert.That(range.End, Is.EqualTo(-1));
            });
        }

        [TestCase(0)]
        [TestCase(3)]
        [TestCase(10)]
        [TestCase(int.MaxValue)]
        public void Ctor_WithTwoNullableValues_WithNullStart(int end)
        {
            var range = new ValueRange(end: end);

            Assert.Multiple(() =>
            {
                Assert.That(range.Start, Is.EqualTo(-1));
                Assert.That(range.End, Is.EqualTo(end));
            });
        }

        #endregion Ctors

        #region Static Methods

        [Test]
        public void Between_WithNegative()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueRange.Between(-1, 1), Throws.TypeOf<ArgumentOutOfRangeException>());
                Assert.That(() => ValueRange.Between(1, -1), Throws.TypeOf<ArgumentOutOfRangeException>());
            });
        }

        [Test]
        public void Between_AsPositive_WithGreaterValue1ThanValue2()
        {
            ValueRange range = ValueRange.Between(3, 1);

            Assert.Multiple(() =>
            {
                Assert.That(range.Start, Is.EqualTo(1));
                Assert.That(range.End, Is.EqualTo(3));
            });
        }

        [Test]
        public void Between_AsPositive_WithGreaterValue2ThanValue1()
        {
            ValueRange range = ValueRange.Between(1, 3);

            Assert.Multiple(() =>
            {
                Assert.That(range.Start, Is.EqualTo(1));
                Assert.That(range.End, Is.EqualTo(3));
            });
        }

        [Test]
        public void Between_AsPositive_WithEqualValues()
        {
            ValueRange range = ValueRange.Between(1, 1);

            Assert.Multiple(() =>
            {
                Assert.That(range.Start, Is.EqualTo(1));
                Assert.That(range.End, Is.EqualTo(1));
            });
        }

        [Test]
        public void Parse_WithString_WithNullString()
        {
            Assert.That(() => ValueRange.Parse((string)null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Parse_WithString_AsInvalidFormat()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueRange.Parse(string.Empty), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("-"), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("Hoge"), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("A-B"), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("3-1"), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("1-3-5"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void Parse_WithString_AsOverflow()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueRange.Parse($"{int.MaxValue + 1L}"), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRange.Parse($"1-{int.MaxValue + 1L}"), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRange.Parse($"-{int.MaxValue + 1L}"), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRange.Parse($"{int.MaxValue + 1L}-"), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRange.Parse($"{int.MaxValue + 1L}-1"), Throws.TypeOf<OverflowException>());
            });
        }

        [Test]
        public void Parse_WithString_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ValueRange.Parse("10"), Is.EqualTo(single));
                Assert.That(ValueRange.Parse("1-3"), Is.EqualTo(range));
                Assert.That(ValueRange.Parse("-10"), Is.EqualTo(nullStart));
                Assert.That(ValueRange.Parse("10-"), Is.EqualTo(nullEnd));
            });
        }

        [Test]
        public void Parse_WithStringAndIFormatProvider_WithNullString()
        {
            Assert.That(() => ValueRange.Parse((string)null!, null), Throws.ArgumentNullException);
        }

        [Test]
        public void Parse_WithStringAndIFormatProvider_AsInvalidFormat()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueRange.Parse(string.Empty, null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("-", null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("Hoge", null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("A-B", null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("3-1", null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("1-3-5", null), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void Parse_WithStringAndIFormatProvider_AsOverflow()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueRange.Parse($"{int.MaxValue + 1L}", null), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRange.Parse($"1-{int.MaxValue + 1L}", null), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRange.Parse($"-{int.MaxValue + 1L}", null), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRange.Parse($"{int.MaxValue + 1L}-", null), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRange.Parse($"{int.MaxValue + 1L}-1", null), Throws.TypeOf<OverflowException>());
            });
        }

        [Test]
        public void Parse_WithStringAndIFormatProvider_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ValueRange.Parse("10", null), Is.EqualTo(single));
                Assert.That(ValueRange.Parse("1-3", null), Is.EqualTo(range));
                Assert.That(ValueRange.Parse("-10", null), Is.EqualTo(nullStart));
                Assert.That(ValueRange.Parse("10-", null), Is.EqualTo(nullEnd));
            });
        }

        [Test]
        public void Parse_WithReadOnlySpanNotExactValue_AsInvalidFormat()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueRange.Parse([]), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("-".AsSpan()), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("Hoge".AsSpan()), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("A-B".AsSpan()), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("3-1".AsSpan()), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("1-3-5".AsSpan()), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void Parse_WithReadOnlySpanNotExactValue_AsOverflow()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueRange.Parse($"{int.MaxValue + 1L}".AsSpan()), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRange.Parse($"1-{int.MaxValue + 1L}".AsSpan()), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRange.Parse($"-{int.MaxValue + 1L}".AsSpan()), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRange.Parse($"{int.MaxValue + 1L}-".AsSpan()), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRange.Parse($"{int.MaxValue + 1L}-1".AsSpan()), Throws.TypeOf<OverflowException>());
            });
        }

        [Test]
        public void Parse_WithReadOnlySpanNotExactValue_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ValueRange.Parse("10".AsSpan()), Is.EqualTo(single));
                Assert.That(ValueRange.Parse("1-3".AsSpan()), Is.EqualTo(range));
                Assert.That(ValueRange.Parse("-10".AsSpan()), Is.EqualTo(nullStart));
                Assert.That(ValueRange.Parse("10-".AsSpan()), Is.EqualTo(nullEnd));
            });
        }

        [Test]
        public void Parse_WithReadOnlySpanAndIFormatProvider_AsInvalidFormat()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueRange.Parse([], null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("-".AsSpan(), null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("Hoge".AsSpan(), null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("A-B".AsSpan(), null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("3-1".AsSpan(), null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRange.Parse("1-3-5".AsSpan(), null), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void Parse_WithReadOnlySpanAndIFormatProvider_AsOverflow()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueRange.Parse($"{int.MaxValue + 1L}".AsSpan(), null), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRange.Parse($"1-{int.MaxValue + 1L}".AsSpan(), null), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRange.Parse($"-{int.MaxValue + 1L}".AsSpan(), null), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRange.Parse($"{int.MaxValue + 1L}-".AsSpan(), null), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRange.Parse($"{int.MaxValue + 1L}-1".AsSpan(), null), Throws.TypeOf<OverflowException>());
            });
        }

        [Test]
        public void Parse_WithReadOnlySpanAndIFormatProvider_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ValueRange.Parse("10".AsSpan(), null), Is.EqualTo(single));
                Assert.That(ValueRange.Parse("1-3".AsSpan(), null), Is.EqualTo(range));
                Assert.That(ValueRange.Parse("-10".AsSpan(), null), Is.EqualTo(nullStart));
                Assert.That(ValueRange.Parse("10-".AsSpan(), null), Is.EqualTo(nullEnd));
            });
        }

        [Test]
        public void TryParse_WithString_WithNullString()
        {
            TryParse(null!);

            static void TryParse(string value)
            {
#pragma warning disable NUnit2045 // Use Assert.Multiple
                Assert.That(ValueRange.TryParse(value, out ValueRange result), Is.False);
                Assert.That(result, Is.EqualTo(default(ValueRange)));
#pragma warning restore NUnit2045 // Use Assert.Multiple
            }
        }

        [Test]
        public void TryParse_WithString_AsInvalidFormat()
        {
            Assert.Multiple(() =>
            {
                TryParse(string.Empty);
                TryParse("-");
                TryParse("Hoge");
                TryParse("A-B");
                TryParse("3-1");
                TryParse("1-3-5");
            });

            static void TryParse(string value)
            {
#pragma warning disable NUnit2045 // Use Assert.Multiple
                Assert.That(ValueRange.TryParse(value, out ValueRange result), Is.False);
                Assert.That(result, Is.EqualTo(default(ValueRange)));
#pragma warning restore NUnit2045 // Use Assert.Multiple
            }
        }

        [Test]
        public void TryParse_WithString_AsOverflow()
        {
            Assert.Multiple(() =>
            {
                TryParse($"{int.MaxValue + 1L}");
                TryParse($"1-{int.MaxValue + 1L}");
                TryParse($"-{int.MaxValue + 1L}");
                TryParse($"{int.MaxValue + 1L}-");
                TryParse($"{int.MaxValue + 1L}-1");
            });

            static void TryParse(string value)
            {
#pragma warning disable NUnit2045 // Use Assert.Multiple
                Assert.That(ValueRange.TryParse(value, out ValueRange result), Is.False);
                Assert.That(result, Is.EqualTo(default(ValueRange)));
#pragma warning restore NUnit2045 // Use Assert.Multiple
            }
        }

        [Test]
        public void TryParse_WithString_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(TryParse("10"), Is.EqualTo(single));
                Assert.That(TryParse("1-3"), Is.EqualTo(range));
                Assert.That(TryParse("-10"), Is.EqualTo(nullStart));
                Assert.That(TryParse("10-"), Is.EqualTo(nullEnd));
            });

            static ValueRange TryParse(string value)
            {
                Assert.That(ValueRange.TryParse(value, out ValueRange result), Is.True);
                return result;
            }
        }

        [Test]
        public void TryParse_WithStringAndIFormatProvider_WithNullString()
        {
            TryParse(null!);

            static void TryParse(string value)
            {
#pragma warning disable NUnit2045 // Use Assert.Multiple
                Assert.That(ValueRange.TryParse(value, null, out ValueRange result), Is.False);
                Assert.That(result, Is.EqualTo(default(ValueRange)));
#pragma warning restore NUnit2045 // Use Assert.Multiple
            }
        }

        [Test]
        public void TryParse_WithStringAndIFormatProvider_AsInvalidFormat()
        {
            Assert.Multiple(() =>
            {
                TryParse(string.Empty);
                TryParse("-");
                TryParse("Hoge");
                TryParse("A-B");
                TryParse("3-1");
                TryParse("1-3-5");
            });

            static void TryParse(string value)
            {
#pragma warning disable NUnit2045 // Use Assert.Multiple
                Assert.That(ValueRange.TryParse(value, null, out ValueRange result), Is.False);
                Assert.That(result, Is.EqualTo(default(ValueRange)));
#pragma warning restore NUnit2045 // Use Assert.Multiple
            }
        }

        [Test]
        public void TryParse_WithStringAndIFormatProvider_AsOverflow()
        {
            Assert.Multiple(() =>
            {
                TryParse($"{int.MaxValue + 1L}");
                TryParse($"1-{int.MaxValue + 1L}");
                TryParse($"-{int.MaxValue + 1L}");
                TryParse($"{int.MaxValue + 1L}-");
                TryParse($"{int.MaxValue + 1L}-1");
            });

            static void TryParse(string value)
            {
#pragma warning disable NUnit2045 // Use Assert.Multiple
                Assert.That(ValueRange.TryParse(value, null, out ValueRange result), Is.False);
                Assert.That(result, Is.EqualTo(default(ValueRange)));
#pragma warning restore NUnit2045 // Use Assert.Multiple
            }
        }

        [Test]
        public void TryParse_WithStringAndIFormatProvider_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(TryParse("10"), Is.EqualTo(single));
                Assert.That(TryParse("1-3"), Is.EqualTo(range));
                Assert.That(TryParse("-10"), Is.EqualTo(nullStart));
                Assert.That(TryParse("10-"), Is.EqualTo(nullEnd));
            });

            static ValueRange TryParse(string value)
            {
                Assert.That(ValueRange.TryParse(value, null, out ValueRange result), Is.True);
                return result;
            }
        }

        [Test]
        public void TryParse_WithReadOnlySpanNotExactValue_AsInvalidFormat()
        {
            Assert.Multiple(() =>
            {
                TryParse([]);
                TryParse("-".AsSpan());
                TryParse("Hoge".AsSpan());
                TryParse("A-B".AsSpan());
                TryParse("3-1".AsSpan());
                TryParse("1-3-5".AsSpan());
            });

            static void TryParse(ReadOnlySpan<char> value)
            {
#pragma warning disable NUnit2045 // Use Assert.Multiple
                Assert.That(ValueRange.TryParse(value, out ValueRange result), Is.False);
                Assert.That(result, Is.EqualTo(default(ValueRange)));
#pragma warning restore NUnit2045 // Use Assert.Multiple
            }
        }

        [Test]
        public void TryParse_WithReadOnlySpanNotExactValue_AsOverflow()
        {
            Assert.Multiple(() =>
            {
                TryParse($"{int.MaxValue + 1L}".AsSpan());
                TryParse($"1-{int.MaxValue + 1L}".AsSpan());
                TryParse($"-{int.MaxValue + 1L}".AsSpan());
                TryParse($"{int.MaxValue + 1L}-".AsSpan());
                TryParse($"{int.MaxValue + 1L}-1".AsSpan());
            });

            static void TryParse(ReadOnlySpan<char> value)
            {
#pragma warning disable NUnit2045 // Use Assert.Multiple
                Assert.That(ValueRange.TryParse(value, out ValueRange result), Is.False);
                Assert.That(result, Is.EqualTo(default(ValueRange)));
#pragma warning restore NUnit2045 // Use Assert.Multiple
            }
        }

        [Test]
        public void TryParse_WithReadOnlySpanNotExactValue_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(TryParse("10".AsSpan()), Is.EqualTo(single));
                Assert.That(TryParse("1-3".AsSpan()), Is.EqualTo(range));
                Assert.That(TryParse("-10".AsSpan()), Is.EqualTo(nullStart));
                Assert.That(TryParse("10-".AsSpan()), Is.EqualTo(nullEnd));
            });

            static ValueRange TryParse(ReadOnlySpan<char> value)
            {
                Assert.That(ValueRange.TryParse(value, out ValueRange result), Is.True);
                return result;
            }
        }

        [Test]
        public void TryParse_WithReadOnlySpanAndIFormatProvider_AsInvalidFormat()
        {
            Assert.Multiple(() =>
            {
                TryParse([]);
                TryParse("-".AsSpan());
                TryParse("Hoge".AsSpan());
                TryParse("A-B".AsSpan());
                TryParse("3-1".AsSpan());
                TryParse("1-3-5".AsSpan());
            });

            static void TryParse(ReadOnlySpan<char> value)
            {
#pragma warning disable NUnit2045 // Use Assert.Multiple
                Assert.That(ValueRange.TryParse(value, null, out ValueRange result), Is.False);
                Assert.That(result, Is.EqualTo(default(ValueRange)));
#pragma warning restore NUnit2045 // Use Assert.Multiple
            }
        }

        [Test]
        public void TryParse_WithReadOnlySpanAndIFormatProvider_AsOverflow()
        {
            Assert.Multiple(() =>
            {
                TryParse($"{int.MaxValue + 1L}".AsSpan());
                TryParse($"1-{int.MaxValue + 1L}".AsSpan());
                TryParse($"-{int.MaxValue + 1L}".AsSpan());
                TryParse($"{int.MaxValue + 1L}-".AsSpan());
                TryParse($"{int.MaxValue + 1L}-1".AsSpan());
            });

            static void TryParse(ReadOnlySpan<char> value)
            {
#pragma warning disable NUnit2045 // Use Assert.Multiple
                Assert.That(ValueRange.TryParse(value, null, out ValueRange result), Is.False);
                Assert.That(result, Is.EqualTo(default(ValueRange)));
#pragma warning restore NUnit2045 // Use Assert.Multiple
            }
        }

        [Test]
        public void TryParse_WithReadOnlySpanAndIFormatProvider_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(TryParse("10".AsSpan()), Is.EqualTo(single));
                Assert.That(TryParse("1-3".AsSpan()), Is.EqualTo(range));
                Assert.That(TryParse("-10".AsSpan()), Is.EqualTo(nullStart));
                Assert.That(TryParse("10-".AsSpan()), Is.EqualTo(nullEnd));
            });

            static ValueRange TryParse(ReadOnlySpan<char> value)
            {
                Assert.That(ValueRange.TryParse(value, null, out ValueRange result), Is.True);
                return result;
            }
        }

        #endregion Static Methods

        #region Instance Methods

#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

        [Test]
        public void Equals_WithValueRange()
        {
            Assert.Multiple(() =>
            {
                Assert.That(single.Equals(single), Is.True);
                Assert.That(single.Equals(10), Is.True);
                Assert.That(single.Equals(new ValueRange(10)), Is.True);
                Assert.That(single.Equals(new ValueRange(1)), Is.False);
                Assert.That(range.Equals(range), Is.True);
                Assert.That(range.Equals(new ValueRange(1, 3)), Is.True);
                Assert.That(range.Equals(new ValueRange(1, 2)), Is.False);
                Assert.That(nullStart.Equals(nullStart), Is.True);
                Assert.That(nullStart.Equals(new ValueRange(1, 10)), Is.False);
                Assert.That(nullStart.Equals(new ValueRange(1, 3)), Is.False);
                Assert.That(nullEnd.Equals(nullEnd), Is.True);
                Assert.That(nullEnd.Equals(new ValueRange(10, 15)), Is.False);
                Assert.That(nullEnd.Equals(new ValueRange(5, 15)), Is.False);
            });
        }

        [Test]
        public void Equals_WithObject()
        {
            Assert.Multiple(() =>
            {
                Assert.That(range.Equals(obj: range), Is.True);
                Assert.That(range.Equals(obj: new ValueRange(1, 3)), Is.True);
                Assert.That(range.Equals(obj: new ValueRange(3, 5)), Is.False);
                Assert.That(range.Equals(null), Is.False);
                Assert.That(single.Equals(obj: 10), Is.False);
                Assert.That(range.Equals("1-3"), Is.False);
            });
        }

#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

        [Test]
        public new void GetHashCode()
        {
#pragma warning disable NUnit2009 // The same value has been provided as both the actual and the expected argument

            Assert.Multiple(() =>
            {
                Assert.That(single.GetHashCode(), Is.EqualTo(single.GetHashCode()));
                Assert.That(single.GetHashCode(), Is.EqualTo(new ValueRange(10).GetHashCode()));
                Assert.That(range.GetHashCode(), Is.EqualTo(range.GetHashCode()));
                Assert.That(range.GetHashCode(), Is.EqualTo(new ValueRange(1, 3).GetHashCode()));
                Assert.That(nullStart.GetHashCode(), Is.EqualTo(nullStart.GetHashCode()));
                Assert.That(nullStart.GetHashCode(), Is.EqualTo(new ValueRange(end: 10).GetHashCode()));
                Assert.That(nullEnd.GetHashCode(), Is.EqualTo(nullEnd.GetHashCode()));
                Assert.That(nullEnd.GetHashCode(), Is.EqualTo(new ValueRange(start: 10).GetHashCode()));
            });

#pragma warning restore NUnit2009 // The same value has been provided as both the actual and the expected argument
        }

        [Test]
        public void Contains()
        {
            Assert.Multiple(() =>
            {
                Assert.That(single.Contains(10), Is.True);
                Assert.That(single.Contains(-1), Is.False);
                Assert.That(single.Contains(9), Is.False);
                Assert.That(single.Contains(11), Is.False);

                Assert.That(range.Contains(1), Is.True);
                Assert.That(range.Contains(2), Is.True);
                Assert.That(range.Contains(3), Is.True);
                Assert.That(range.Contains(-1), Is.False);
                Assert.That(range.Contains(0), Is.False);
                Assert.That(range.Contains(4), Is.False);

                Assert.That(nullStart.Contains(0), Is.True);
                Assert.That(nullStart.Contains(10), Is.True);
                Assert.That(nullStart.Contains(-1), Is.False);
                Assert.That(nullStart.Contains(11), Is.False);

                Assert.That(nullEnd.Contains(10), Is.True);
                Assert.That(nullEnd.Contains(int.MaxValue), Is.True);
                Assert.That(nullEnd.Contains(-1), Is.False);
                Assert.That(nullEnd.Contains(9), Is.False);
            });
        }

        [Test]
        public void GetEnumerator_AsNotExactRange()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => nullStart.GetEnumerator(), Throws.InvalidOperationException);
                Assert.That(() => nullEnd.GetEnumerator(), Throws.InvalidOperationException);
            });
        }

        [Test]
        public void GetEnumerator_AsPositive()
        {
            using IEnumerator<int> enumerator = range.GetEnumerator();

            Assert.Multiple(() =>
            {
                Assert.That(enumerator.MoveNext(), Is.True);
                Assert.That(enumerator.Current, Is.EqualTo(1));
            });
            Assert.Multiple(() =>
            {
                Assert.That(enumerator.MoveNext(), Is.True);
                Assert.That(enumerator.Current, Is.EqualTo(2));
            });
            Assert.Multiple(() =>
            {
                Assert.That(enumerator.MoveNext(), Is.True);
                Assert.That(enumerator.Current, Is.EqualTo(3));
                Assert.That(enumerator.MoveNext(), Is.False);
            });
        }

        [Test]
        public void Interface_IEnumerable_AsNotExactRange()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ((IEnumerable)nullStart).GetEnumerator(), Throws.InvalidOperationException);
                Assert.That(() => ((IEnumerable)nullEnd).GetEnumerator(), Throws.InvalidOperationException);
            });
        }

        [Test]
        public void Interface_IEnumerable_AsPositive()
        {
            IEnumerator enumerator = ((IEnumerable)range).GetEnumerator();

            Assert.Multiple(() =>
            {
                Assert.That(enumerator.MoveNext(), Is.True);
                Assert.That(enumerator.Current, Is.EqualTo(1));
            });
            Assert.Multiple(() =>
            {
                Assert.That(enumerator.MoveNext(), Is.True);
                Assert.That(enumerator.Current, Is.EqualTo(2));
            });
            Assert.Multiple(() =>
            {
                Assert.That(enumerator.MoveNext(), Is.True);
                Assert.That(enumerator.Current, Is.EqualTo(3));
                Assert.That(enumerator.MoveNext(), Is.False);
            });
        }

        [Test]
        public void ToString_WithoutArgs()
        {
            Assert.Multiple(() =>
            {
                Assert.That(((object)single).ToString(), Is.EqualTo("10"));
                Assert.That(((object)range).ToString(), Is.EqualTo("1-3"));
                Assert.That(((object)nullStart).ToString(), Is.EqualTo("-10"));
                Assert.That(((object)nullEnd).ToString(), Is.EqualTo("10-"));
            });
        }

        [Test]
        public void ToString_WithStringAndIFormatProvider()
        {
            Assert.Multiple(() =>
            {
                Assert.That(single.ToString("x", null), Is.EqualTo("a"));
                Assert.That(range.ToString("x", null), Is.EqualTo("1-3"));
                Assert.That(nullStart.ToString("X", null), Is.EqualTo("-A"));
                Assert.That(nullEnd.ToString("X", null), Is.EqualTo("A-"));
            });
        }

#if NET6_0_OR_GREATER

        [Test]
        public void TryFormat_WithShortSpan()
        {
            Span<char> span = stackalloc char[2];

            Assert.That(range.TryFormat(span, out _, null, null), Is.False);
        }

        [Test]
        public void TryFormat_AsPositive()
        {
            CheckTryFormat(single, 3, "10\0", 2);
            CheckTryFormat(range, 3, "1-3", 3);
            CheckTryFormat(nullStart, 5, "-10\0\0", 3);
            CheckTryFormat(nullEnd, 3, "10-", 3);

            static void CheckTryFormat(ValueRange range, int length, string expectedValue, int expectedCharsWritten)
            {
                Span<char> span = stackalloc char[length];

#pragma warning disable NUnit2045 // Use Assert.Multiple
                Assert.That(range.TryFormat(span, out int actualCharsWritten, null, null), Is.True);
                Assert.That(span.ToString(), Is.EqualTo(expectedValue));
                Assert.That(actualCharsWritten, Is.EqualTo(expectedCharsWritten));
#pragma warning restore NUnit2045 // Use Assert.Multiple
            }
        }

#endif

        [Test]
        public void WithExactStart_WithNegative()
        {
            Assert.That(() => nullStart.WithExactStart(-1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void WithExactStart_WithValueGreaterThanEnd()
        {
            Assert.That(() => nullStart.WithExactStart(11), Throws.ArgumentException);
        }

        [Test]
        public void WithExactStart_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(nullStart.WithExactStart(0), Is.EqualTo(new ValueRange(0, 10)));
                Assert.That(nullStart.WithExactStart(1), Is.EqualTo(new ValueRange(1, 10)));
                Assert.That(nullStart.WithExactStart(10), Is.EqualTo(new ValueRange(10, 10)));
                Assert.That(range.WithExactStart(2), Is.EqualTo(range));
                Assert.That(single.WithExactStart(10), Is.EqualTo(single));
            });
        }

        [Test]
        public void WithExactEnd_WithNegative()
        {
            Assert.That(() => nullEnd.WithExactEnd(-1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void WithExactEnd_WithValueGreaterThanEnd()
        {
            Assert.That(() => nullEnd.WithExactEnd(9), Throws.ArgumentException);
        }

        [Test]
        public void WithExactEnd_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(nullEnd.WithExactEnd(10), Is.EqualTo(new ValueRange(10, 10)));
                Assert.That(nullEnd.WithExactEnd(11), Is.EqualTo(new ValueRange(10, 11)));
                Assert.That(nullEnd.WithExactEnd(int.MaxValue), Is.EqualTo(new ValueRange(10, int.MaxValue)));
                Assert.That(range.WithExactEnd(2), Is.EqualTo(range));
                Assert.That(single.WithExactEnd(10), Is.EqualTo(single));
            });
        }

        #endregion Instance Methods

        #region Operators

#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
#pragma warning disable CS1718 // 同じ変数と比較されました

        [Test]
        public void Op_Equality()
        {
            Assert.Multiple(() =>
            {
                Assert.That(single == single, Is.True);
                Assert.That(single == 10, Is.True);
                Assert.That(single == new ValueRange(10), Is.True);
                Assert.That(single == new ValueRange(1), Is.False);
                Assert.That(range == range, Is.True);
                Assert.That(range == new ValueRange(1, 3), Is.True);
                Assert.That(range == new ValueRange(1, 2), Is.False);
                Assert.That(nullStart == nullStart, Is.True);
                Assert.That(nullStart == new ValueRange(1, 10), Is.False);
                Assert.That(nullStart == new ValueRange(1, 3), Is.False);
                Assert.That(nullEnd == nullEnd, Is.True);
                Assert.That(nullEnd == new ValueRange(10, 15), Is.False);
                Assert.That(nullEnd == new ValueRange(5, 15), Is.False);
            });
        }

        [Test]
        public void Op_Inequality()
        {
            Assert.Multiple(() =>
            {
                Assert.That(single != single, Is.False);
                Assert.That(single != 10, Is.False);
                Assert.That(single != new ValueRange(10), Is.False);
                Assert.That(single != new ValueRange(1), Is.True);
                Assert.That(range != range, Is.False);
                Assert.That(range != new ValueRange(1, 3), Is.False);
                Assert.That(range != new ValueRange(1, 2), Is.True);
                Assert.That(nullStart != nullStart, Is.False);
                Assert.That(nullStart != new ValueRange(1, 10), Is.True);
                Assert.That(nullStart != new ValueRange(1, 3), Is.True);
                Assert.That(nullEnd != nullEnd, Is.False);
                Assert.That(nullEnd != new ValueRange(10, 15), Is.True);
                Assert.That(nullEnd != new ValueRange(5, 15), Is.True);
            });
        }

#pragma warning restore CS1718 // 同じ変数と比較されました
#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(30)]
        [TestCase(int.MaxValue)]
        public void Op_Implicit_FromInt32(int value)
        {
            ValueRange range = value;

            Assert.That(range, Is.EqualTo(new ValueRange(value)));
        }

        #endregion Operators
    }
}
