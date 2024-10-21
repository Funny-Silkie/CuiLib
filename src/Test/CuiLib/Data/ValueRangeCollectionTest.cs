using CuiLib.Data;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Test.CuiLib.Data
{
    public class ValueRangeCollectionTest : TestBase
    {
        private ValueRangeCollection empty;
        private ValueRangeCollection hasElements;

        [SetUp]
        public void SetUp()
        {
            empty = [];
            hasElements = new ValueRangeCollection([1, new ValueRange(3, 5), new ValueRange(10, 11)]);
        }

        #region Ctors

        [Test]
        public void Ctor_WithoutArgs()
        {
            Assert.That(() => new ValueRangeCollection(), Throws.Nothing);
        }

        [Test]
        public void Ctor_WithInt32_WithNegative()
        {
            Assert.That(() => new ValueRangeCollection(-1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [TestCase(0)]
        [TestCase(10)]
        [TestCase(300)]
        public void Ctor_WithInt32_AsPositive(int capacity)
        {
            var collection = new ValueRangeCollection(capacity);

            Assert.That(collection.Capacity, Is.EqualTo(capacity));
        }

        [Test]
        public void Ctor_WithIEnumerableOfValueRange_WithNull()
        {
            Assert.That(() => new ValueRangeCollection((IEnumerable<ValueRange>)null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithIEnumerableOfValueRange_AsPositive()
        {
            Assert.That(() => new ValueRangeCollection([new ValueRange(3, 5)]), Throws.Nothing);
        }

        [Test]
        public void Ctor_WithIEnumerableOfInt32_WithNull()
        {
            Assert.That(() => new ValueRangeCollection((IEnumerable<int>)null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithIEnumerableOfInt32_AsPositive()
        {
            Assert.That(new ValueRangeCollection(new[] { 1, 2, 3, 3, 2, 10, 3, 4, 5 }), Is.EqualTo(new[] { new ValueRange(1, 3), 3, 2, 10, new ValueRange(3, 5) }));
        }

        #endregion Ctors

        #region Properties

        [Test]
        public void Capacity_Set()
        {
            hasElements.Capacity = 30;

            Assert.That(hasElements.Capacity, Is.EqualTo(30));
        }

#pragma warning disable NUnit2046 // Use CollectionConstraint for better assertion messages in case of failure

        [Test]
        public void Count_Get()
        {
            Assert.Multiple(() =>
            {
                Assert.That(empty.Count, Is.Zero);
                Assert.That(hasElements.Count, Is.EqualTo(3));
            });
        }

#pragma warning restore NUnit2046 // Use CollectionConstraint for better assertion messages in case of failure

        [Test]
        public void Interface_IList_IsFixedSize()
        {
            Assert.That(((IList)hasElements).IsFixedSize, Is.False);
        }

        [Test]
        public void Interface_IList_IsReadOnly()
        {
            Assert.That(((IList)hasElements).IsReadOnly, Is.False);
        }

        [Test]
        public void Interface_ICollection_1_IsReadOnly_Get()
        {
            Assert.That(() => ((ICollection<ValueRange>)empty).IsReadOnly, Is.False);
        }

        [Test]
        public void Interface_ICollection_IsSynchronized_Get()
        {
            Assert.That(() => ((ICollection)empty).IsSynchronized, Is.False);
        }

        [Test]
        public void Interface_ICollection_SyncRoot_Get()
        {
            Assert.Multiple(() =>
            {
                Assert.That(((ICollection)empty).SyncRoot, Is.SameAs(empty));
                Assert.That(((ICollection)hasElements).SyncRoot, Is.SameAs(hasElements));
            });
        }

        #endregion Properties

        #region Indexers

        [Test]
        public void IndexerWithInt32_Get()
        {
            Assert.Multiple(() =>
            {
                Assert.That(hasElements[0], Is.EqualTo(new ValueRange(1)));
                Assert.That(hasElements[1], Is.EqualTo(new ValueRange(3, 5)));
                Assert.That(hasElements[2], Is.EqualTo(new ValueRange(10, 11)));
            });
        }

        [Test]
        public void IndexerWithInt32_Set()
        {
            hasElements[1] = new ValueRange(100, 105);

            Assert.That(hasElements[1], Is.EqualTo(new ValueRange(100, 105)));
        }

        [Test]
        public void Interface_IList_IndexerWithInt32_Get()
        {
            Assert.Multiple(() =>
            {
                Assert.That(((IList)hasElements)[0], Is.EqualTo(new ValueRange(1)));
                Assert.That(((IList)hasElements)[1], Is.EqualTo(new ValueRange(3, 5)));
                Assert.That(((IList)hasElements)[2], Is.EqualTo(new ValueRange(10, 11)));
            });
        }

        [Test]
        public void Interface_IList_IndexerWithInt32_Set()
        {
            ((IList)hasElements)[1] = new ValueRange(100, 105);

            Assert.That(((IList)hasElements)[1], Is.EqualTo(new ValueRange(100, 105)));
        }

        #endregion Indexers

        #region Static Methods

        [Test]
        public void Parse_WithString_WithNullString()
        {
            Assert.That(() => ValueRangeCollection.Parse((string)null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Parse_WithString_AsInvalidFormat()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueRangeCollection.Parse("-"), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("Hoge"), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("A-B"), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("3-1"), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("1-3-5"), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("1,3-5,"), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse(",1,3-5"), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void Parse_WithString_AsOverflow()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueRangeCollection.Parse($"{int.MaxValue + 1L}"), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRangeCollection.Parse($"1-{int.MaxValue + 1L}"), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRangeCollection.Parse($"-{int.MaxValue + 1L}"), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRangeCollection.Parse($"{int.MaxValue + 1L}-"), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRangeCollection.Parse($"{int.MaxValue + 1L}-1"), Throws.TypeOf<OverflowException>());
            });
        }

        [Test]
        public void Parse_WithString_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ValueRangeCollection.Parse(string.Empty), Is.EqualTo(empty));
                Assert.That(ValueRangeCollection.Parse("1,3-5,10-11"), Is.EqualTo(hasElements));
            });
        }

        [Test]
        public void Parse_WithStringAndIFormatProvider_WithNullString()
        {
            Assert.That(() => ValueRangeCollection.Parse((string)null!, null), Throws.ArgumentNullException);
        }

        [Test]
        public void Parse_WithStringAndIFormatProvider_AsInvalidFormat()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueRangeCollection.Parse("-", null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("Hoge", null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("A-B", null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("3-1", null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("1-3-5", null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("1,3-5,", null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse(",1,3-5", null), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void Parse_WithStringAndIFormatProvider_AsOverflow()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueRangeCollection.Parse($"{int.MaxValue + 1L}", null), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRangeCollection.Parse($"1-{int.MaxValue + 1L}", null), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRangeCollection.Parse($"-{int.MaxValue + 1L}", null), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRangeCollection.Parse($"{int.MaxValue + 1L}-", null), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRangeCollection.Parse($"{int.MaxValue + 1L}-1", null), Throws.TypeOf<OverflowException>());
            });
        }

        [Test]
        public void Parse_WithStringAndIFormatProvider_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ValueRangeCollection.Parse(string.Empty, null), Is.EqualTo(empty));
                Assert.That(ValueRangeCollection.Parse("1,3-5,10-11", null), Is.EqualTo(hasElements));
            });
        }

        [Test]
        public void Parse_WithReadOnlySpanNotExactValue_AsInvalidFormat()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueRangeCollection.Parse("-".AsSpan()), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("Hoge".AsSpan()), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("A-B".AsSpan()), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("3-1".AsSpan()), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("1-3-5".AsSpan()), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("1,3-5,".AsSpan()), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse(",1,3-5".AsSpan()), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void Parse_WithReadOnlySpanNotExactValue_AsOverflow()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueRangeCollection.Parse($"{int.MaxValue + 1L}".AsSpan()), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRangeCollection.Parse($"1-{int.MaxValue + 1L}".AsSpan()), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRangeCollection.Parse($"-{int.MaxValue + 1L}".AsSpan()), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRangeCollection.Parse($"{int.MaxValue + 1L}-".AsSpan()), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRangeCollection.Parse($"{int.MaxValue + 1L}-1".AsSpan()), Throws.TypeOf<OverflowException>());
            });
        }

        [Test]
        public void Parse_WithReadOnlySpanNotExactValue_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ValueRangeCollection.Parse([]), Is.EqualTo(empty));
                Assert.That(ValueRangeCollection.Parse("1,3-5,10-11".AsSpan()), Is.EqualTo(hasElements));
            });
        }

        [Test]
        public void Parse_WithReadOnlySpanAndIFormatProvider_AsInvalidFormat()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueRangeCollection.Parse("-".AsSpan(), null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("Hoge".AsSpan(), null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("A-B".AsSpan(), null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("3-1".AsSpan(), null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("1-3-5".AsSpan(), null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse("1,3-5,".AsSpan(), null), Throws.TypeOf<FormatException>());
                Assert.That(() => ValueRangeCollection.Parse(",1,3-5".AsSpan(), null), Throws.TypeOf<FormatException>());
            });
        }

        [Test]
        public void Parse_WithReadOnlySpanAndIFormatProvider_AsOverflow()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueRangeCollection.Parse($"{int.MaxValue + 1L}".AsSpan(), null), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRangeCollection.Parse($"1-{int.MaxValue + 1L}".AsSpan(), null), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRangeCollection.Parse($"-{int.MaxValue + 1L}".AsSpan(), null), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRangeCollection.Parse($"{int.MaxValue + 1L}-".AsSpan(), null), Throws.TypeOf<OverflowException>());
                Assert.That(() => ValueRangeCollection.Parse($"{int.MaxValue + 1L}-1".AsSpan(), null), Throws.TypeOf<OverflowException>());
            });
        }

        [Test]
        public void Parse_WithReadOnlySpanAndIFormatProvider_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ValueRangeCollection.Parse([], null), Is.EqualTo(empty));
                Assert.That(ValueRangeCollection.Parse("1,3-5,10-11".AsSpan(), null), Is.EqualTo(hasElements));
            });
        }

        [Test]
        public void TryParse_WithString_WithNullString()
        {
            TryParse(null!);

            static void TryParse(string value)
            {
#pragma warning disable NUnit2045 // Use Assert.Multiple
                Assert.That(ValueRangeCollection.TryParse(value, out ValueRangeCollection? result), Is.False);
                Assert.That(result, Is.Null);
#pragma warning restore NUnit2045 // Use Assert.Multiple
            }
        }

        [Test]
        public void TryParse_WithString_AsInvalidFormat()
        {
            Assert.Multiple(() =>
            {
                TryParse("-");
                TryParse("Hoge");
                TryParse("A-B");
                TryParse("3-1");
                TryParse("1-3-5");
                TryParse("1,3-5,");
                TryParse(",1,3-5");
            });

            static void TryParse(string value)
            {
#pragma warning disable NUnit2045 // Use Assert.Multiple
                Assert.That(ValueRangeCollection.TryParse(value, out ValueRangeCollection? result), Is.False);
                Assert.That(result, Is.Null);
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
                Assert.That(ValueRangeCollection.TryParse(value, out ValueRangeCollection? result), Is.False);
                Assert.That(result, Is.Null);
#pragma warning restore NUnit2045 // Use Assert.Multiple
            }
        }

        [Test]
        public void TryParse_WithString_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(TryParse(string.Empty), Is.EqualTo(empty));
                Assert.That(TryParse("1,3-5,10-11"), Is.EqualTo(hasElements));
            });

            static ValueRangeCollection TryParse(string value)
            {
                Assert.That(ValueRangeCollection.TryParse(value, out ValueRangeCollection? result), Is.True);
                return result!;
            }
        }

        [Test]
        public void TryParse_WithStringAndIFormatProvider_WithNullString()
        {
            TryParse(null!);

            static void TryParse(string value)
            {
#pragma warning disable NUnit2045 // Use Assert.Multiple
                Assert.That(ValueRangeCollection.TryParse(value, null, out ValueRangeCollection? result), Is.False);
                Assert.That(result, Is.Null);
#pragma warning restore NUnit2045 // Use Assert.Multiple
            }
        }

        [Test]
        public void TryParse_WithStringAndIFormatProvider_AsInvalidFormat()
        {
            Assert.Multiple(() =>
            {
                TryParse("-");
                TryParse("Hoge");
                TryParse("A-B");
                TryParse("3-1");
                TryParse("1-3-5");
                TryParse("1,3-5,");
                TryParse(",1,3-5");
            });

            static void TryParse(string value)
            {
#pragma warning disable NUnit2045 // Use Assert.Multiple
                Assert.That(ValueRangeCollection.TryParse(value, null, out ValueRangeCollection? result), Is.False);
                Assert.That(result, Is.Null);
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
                Assert.That(ValueRangeCollection.TryParse(value, null, out ValueRangeCollection? result), Is.False);
                Assert.That(result, Is.Null);
#pragma warning restore NUnit2045 // Use Assert.Multiple
            }
        }

        [Test]
        public void TryParse_WithStringAndIFormatProvider_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(TryParse(string.Empty), Is.EqualTo(empty));
                Assert.That(TryParse("1,3-5,10-11"), Is.EqualTo(hasElements));
            });

            static ValueRangeCollection TryParse(string value)
            {
                Assert.That(ValueRangeCollection.TryParse(value, null, out ValueRangeCollection? result), Is.True);
                return result!;
            }
        }

        [Test]
        public void TryParse_WithReadOnlySpanNotExactValue_AsInvalidFormat()
        {
            Assert.Multiple(() =>
            {
                TryParse("-".AsSpan());
                TryParse("Hoge".AsSpan());
                TryParse("A-B".AsSpan());
                TryParse("3-1".AsSpan());
                TryParse("1-3-5".AsSpan());
                TryParse("1,3-5,".AsSpan());
                TryParse(",1,3-5".AsSpan());
            });

            static void TryParse(ReadOnlySpan<char> value)
            {
#pragma warning disable NUnit2045 // Use Assert.Multiple
                Assert.That(ValueRangeCollection.TryParse(value, out ValueRangeCollection? result), Is.False);
                Assert.That(result, Is.Null);
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
                Assert.That(ValueRangeCollection.TryParse(value, out ValueRangeCollection? result), Is.False);
                Assert.That(result, Is.Null);
#pragma warning restore NUnit2045 // Use Assert.Multiple
            }
        }

        [Test]
        public void TryParse_WithReadOnlySpanNotExactValue_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(TryParse([]), Is.EqualTo(empty));
                Assert.That(TryParse("1,3-5,10-11".AsSpan()), Is.EqualTo(hasElements));
            });

            static ValueRangeCollection TryParse(ReadOnlySpan<char> value)
            {
                Assert.That(ValueRangeCollection.TryParse(value, out ValueRangeCollection? result), Is.True);
                return result!;
            }
        }

        [Test]
        public void TryParse_WithReadOnlySpanAndIFormatProvider_AsInvalidFormat()
        {
            Assert.Multiple(() =>
            {
                TryParse("-".AsSpan());
                TryParse("Hoge".AsSpan());
                TryParse("A-B".AsSpan());
                TryParse("3-1".AsSpan());
                TryParse("1-3-5".AsSpan());
                TryParse("1,3-5,".AsSpan());
                TryParse(",1,3-5".AsSpan());
            });

            static void TryParse(ReadOnlySpan<char> value)
            {
#pragma warning disable NUnit2045 // Use Assert.Multiple
                Assert.That(ValueRangeCollection.TryParse(value, null, out ValueRangeCollection? result), Is.False);
                Assert.That(result, Is.Null);
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
                Assert.That(ValueRangeCollection.TryParse(value, null, out ValueRangeCollection? result), Is.False);
                Assert.That(result, Is.Null);
#pragma warning restore NUnit2045 // Use Assert.Multiple
            }
        }

        [Test]
        public void TryParse_WithReadOnlySpanAndIFormatProvider_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(TryParse([]), Is.EqualTo(empty));
                Assert.That(TryParse("1,3-5,10-11".AsSpan()), Is.EqualTo(hasElements));
            });

            static ValueRangeCollection TryParse(ReadOnlySpan<char> value)
            {
                Assert.That(ValueRangeCollection.TryParse(value, null, out ValueRangeCollection? result), Is.True);
                return result!;
            }
        }

        #endregion Static Methods

        #region Instance Methods

        [Test]
        public void Add()
        {
            hasElements.Add(new ValueRange(30, 100));

            Assert.Multiple(() =>
            {
                Assert.That(hasElements, Has.Count.EqualTo(4));
                Assert.That(hasElements[^1], Is.EqualTo(new ValueRange(30, 100)));
            });
        }

        [Test]
        public void Interaface_IList_Add()
        {
            Assert.That(((IList)hasElements).Add(new ValueRange(30, 100)), Is.EqualTo(3));

            Assert.Multiple(() =>
            {
                Assert.That(hasElements, Has.Count.EqualTo(4));
                Assert.That(hasElements[^1], Is.EqualTo(new ValueRange(30, 100)));
            });
        }

        [Test]
        public void Clear()
        {
            hasElements.Clear();

            Assert.That(hasElements, Is.Empty);
        }

        [Test]
        public void Contains()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => hasElements.Contains(new ValueRange(0)), Is.False);
                Assert.That(() => hasElements.Contains(new ValueRange(1)), Is.True);
                Assert.That(() => hasElements.Contains(new ValueRange(2, 3)), Is.False);
                Assert.That(() => hasElements.Contains(new ValueRange(3, 5)), Is.True);
                Assert.That(() => hasElements.Contains(new ValueRange(10, 11)), Is.True);
                Assert.That(() => hasElements.Contains(new ValueRange(12)), Is.False);
            });
        }

        [Test]
        public void Interface_IList_Contains()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ((IList)hasElements).Contains(new ValueRange(0)), Is.False);
                Assert.That(() => ((IList)hasElements).Contains(new ValueRange(1)), Is.True);
                Assert.That(() => ((IList)hasElements).Contains(new ValueRange(2, 3)), Is.False);
                Assert.That(() => ((IList)hasElements).Contains(new ValueRange(3, 5)), Is.True);
                Assert.That(() => ((IList)hasElements).Contains(new ValueRange(10, 11)), Is.True);
                Assert.That(() => ((IList)hasElements).Contains(new ValueRange(12)), Is.False);
            });
        }

        [Test]
        public void CopyTo()
        {
            var array = new ValueRange[4];
            hasElements.CopyTo(array, 1);

            Assert.That(() => array, Is.EqualTo(new[] { default, new ValueRange(1), new ValueRange(3, 5), new ValueRange(10, 11) }));
        }

        [Test]
        public void Interface_ICollection_CopyTo()
        {
            var array = new ValueRange[4];
            ((ICollection)hasElements).CopyTo(array, 1);

            Assert.That(() => array, Is.EqualTo(new[] { default, new ValueRange(1), new ValueRange(3, 5), new ValueRange(10, 11) }));
        }

        [Test]
        public void GetEnumerator()
        {
            using IEnumerator<ValueRange> enumerator = hasElements.GetEnumerator();

#pragma warning disable NUnit2045 // Use Assert.Multiple

            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.EqualTo(new ValueRange(1)));

            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.EqualTo(new ValueRange(3, 5)));

            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.EqualTo(new ValueRange(10, 11)));

            Assert.That(enumerator.MoveNext(), Is.False);

#pragma warning restore NUnit2045 // Use Assert.Multiple
        }

        [Test]
        public void Interface_IEnumerable_GetEnumerator()
        {
            IEnumerator enumerator = ((IEnumerable)hasElements).GetEnumerator();

#pragma warning disable NUnit2045 // Use Assert.Multiple

            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.EqualTo(new ValueRange(1)));

            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.EqualTo(new ValueRange(3, 5)));

            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.EqualTo(new ValueRange(10, 11)));

            Assert.That(enumerator.MoveNext(), Is.False);

#pragma warning restore NUnit2045 // Use Assert.Multiple
        }

        [Test]
        public void IndexOf()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => hasElements.IndexOf(new ValueRange(0)), Is.EqualTo(-1));
                Assert.That(() => hasElements.IndexOf(new ValueRange(1)), Is.EqualTo(0));
                Assert.That(() => hasElements.IndexOf(new ValueRange(2, 3)), Is.EqualTo(-1));
                Assert.That(() => hasElements.IndexOf(new ValueRange(3, 5)), Is.EqualTo(1));
                Assert.That(() => hasElements.IndexOf(new ValueRange(10, 11)), Is.EqualTo(2));
                Assert.That(() => hasElements.IndexOf(new ValueRange(12)), Is.EqualTo(-1));
            });
        }

        [Test]
        public void Interface_IList_IndexOf()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ((IList)hasElements).IndexOf(new ValueRange(0)), Is.EqualTo(-1));
                Assert.That(() => ((IList)hasElements).IndexOf(new ValueRange(1)), Is.EqualTo(0));
                Assert.That(() => ((IList)hasElements).IndexOf(new ValueRange(2, 3)), Is.EqualTo(-1));
                Assert.That(() => ((IList)hasElements).IndexOf(new ValueRange(3, 5)), Is.EqualTo(1));
                Assert.That(() => ((IList)hasElements).IndexOf(new ValueRange(10, 11)), Is.EqualTo(2));
                Assert.That(() => ((IList)hasElements).IndexOf(new ValueRange(12)), Is.EqualTo(-1));
            });
        }

        [Test]
        public void Insert()
        {
            hasElements.Insert(1, new ValueRange(100));

            Assert.That(hasElements, Is.EqualTo(new[] { new ValueRange(1), new ValueRange(100), new ValueRange(3, 5), new ValueRange(10, 11) }));
        }

        [Test]
        public void Interface_IList_Insert()
        {
            ((IList)hasElements).Insert(1, new ValueRange(100));

            Assert.That(hasElements, Is.EqualTo(new[] { new ValueRange(1), new ValueRange(100), new ValueRange(3, 5), new ValueRange(10, 11) }));
        }

        [Test]
        public void Remove()
        {
            Assert.Multiple(() =>
            {
                Assert.That(empty.Remove(new ValueRange(5, 7)), Is.False);
                Assert.That(hasElements.Remove(new ValueRange(5, 7)), Is.False);
            });

            Assert.That(hasElements.Remove(new ValueRange(3, 5)), Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(hasElements, Has.Count.EqualTo(2));
                Assert.That(hasElements, Is.EqualTo(new[] { new ValueRange(1), new ValueRange(10, 11) }));
                Assert.That(hasElements.Remove(new ValueRange(3, 5)), Is.False);
            });
        }

        [Test]
        public void Interface_IList_Remove()
        {
            ((IList)hasElements).Remove(new ValueRange(3, 5));

            Assert.Multiple(() =>
            {
                Assert.That(hasElements, Has.Count.EqualTo(2));
                Assert.That(hasElements, Is.EqualTo(new[] { new ValueRange(1), new ValueRange(10, 11) }));
            });
        }

        [Test]
        public void RemoveAt()
        {
            hasElements.RemoveAt(1);

            Assert.That(hasElements, Is.EqualTo(new[] { new ValueRange(1), new ValueRange(10, 11) }));
        }

        [Test]
        public void ToStringWithoutArgs()
        {
            Assert.Multiple(() =>
            {
                Assert.That(((object)empty).ToString(), Is.EqualTo(string.Empty));
                Assert.That(((object)hasElements).ToString(), Is.EqualTo("1,3-5,10-11"));
            });
        }

        [Test]
        public void ToStringWithStringAndIFormatProvider()
        {
            Assert.Multiple(() =>
            {
                Assert.That(empty.ToString("x", null), Is.EqualTo(string.Empty));
                Assert.That(hasElements.ToString("x", null), Is.EqualTo("1,3-5,a-b"));
            });
        }

#if NET6_0_OR_GREATER

        [Test]
        public void TryFormat_WithShortSpan()
        {
            Span<char> span = stackalloc char[2];

            Assert.That(hasElements.TryFormat(span, out _, null, null), Is.False);
        }

        [Test]
        public void TryFormat_AsPositive()
        {
            CheckTryFormat(empty, 3, "\0\0\0", 0);
            CheckTryFormat(hasElements, 11, "1,3-5,10-11", 11);
            CheckTryFormat(hasElements, 12, "1,3-5,10-11\0", 11);

            static void CheckTryFormat(ValueRangeCollection collection, int length, string expectedValue, int expectedCharsWritten)
            {
                Span<char> span = stackalloc char[length];

#pragma warning disable NUnit2045 // Use Assert.Multiple
                Assert.That(collection.TryFormat(span, out int actualCharsWritten, null, null), Is.True);
                Assert.That(span.ToString(), Is.EqualTo(expectedValue));
                Assert.That(actualCharsWritten, Is.EqualTo(expectedCharsWritten));
#pragma warning restore NUnit2045 // Use Assert.Multiple
            }
        }

#endif

        #endregion Instance Methods
    }
}
