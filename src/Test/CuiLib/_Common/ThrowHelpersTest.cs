using CuiLib;
using CuiLib.Checkers;
using CuiLib.Options;
using CuiLib.Parameters;
using NUnit.Framework;
using System;
using System.IO;

namespace Test.CuiLib
{
    [TestFixture]
    public class ThrowHelpersTest : TestBase
    {
        private enum TestEnum
        {
            Zero = 0,
            One = 1,
        }

        [Test]
        public void ThrowAsOptionParseFailed()
        {
            Assert.That(() => ThrowHelpers.ThrowAsOptionParseFailed(new ArgumentException("Error")), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void ThrowAsEmptyOption_WithNull()
        {
            Assert.That(() => ThrowHelpers.ThrowAsEmptyOption(null), Throws.Nothing);
        }

        [Test]
        public void ThrowAsEmptyOption_AsThrown()
        {
            Assert.That(() => ThrowHelpers.ThrowAsEmptyOption(new SingleValueOption<int>("num")), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void ThrowAsEmptyParameter_WithNull()
        {
            Assert.That(() => ThrowHelpers.ThrowAsEmptyParameter(null), Throws.Nothing);
        }

        [Test]
        public void ThrowAsEmptyParameter_AsThrown()
        {
            Assert.That(() => ThrowHelpers.ThrowAsEmptyParameter(new SingleValueParameter<string>("param", 0)), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void ThrowAsEmptyCollection()
        {
            Assert.That(ThrowHelpers.ThrowAsEmptyCollection, Throws.InvalidOperationException);
        }

        [Test]
        public void ThrowIfNull_WithNull()
        {
            Assert.That(() => ThrowHelpers.ThrowIfNull<object>(null), Throws.ArgumentNullException);
        }

        [Test]
        public void ThrowIfNull_AsPositive()
        {
            Assert.That(() => ThrowHelpers.ThrowIfNull<object>(string.Empty), Throws.Nothing);
            Assert.That(() => ThrowHelpers.ThrowIfNull<object>(0), Throws.Nothing);
            Assert.That(() => ThrowHelpers.ThrowIfNull<object>(false), Throws.Nothing);
        }

        [Test]
        public void ThrowIfNullOrEmpty_WithNull()
        {
            Assert.That(() => ThrowHelpers.ThrowIfNullOrEmpty(null), Throws.ArgumentNullException);
        }

        [Test]
        public void ThrowIfNullOrEmpty_WithEmpty()
        {
            Assert.That(() => ThrowHelpers.ThrowIfNullOrEmpty(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void ThrowIfNullOrEmpty_AsPositive()
        {
            Assert.That(() => ThrowHelpers.ThrowIfNullOrEmpty("hoge"), Throws.Nothing);
        }

        [Test]
        public void ThrowIfHasInvalidFileNameChar()
        {
            Assert.Multiple(() =>
            {
                foreach (char current in Path.GetInvalidFileNameChars()) Assert.That(() => ThrowHelpers.ThrowIfHasInvalidFileNameChar("hoge/fuga"), Throws.ArgumentException);
            });
        }

        [Test]
        public void ThrowIfHasInvalidFileNameChar_AsPositive()
        {
            Assert.That(() => ThrowHelpers.ThrowIfHasInvalidFileNameChar("hoge.txt"), Throws.Nothing);
        }

        [Test]
        public void ThrowIfHasInvalidPathChar()
        {
            Assert.Multiple(() =>
            {
                foreach (char current in Path.GetInvalidPathChars()) Assert.That(() => ThrowHelpers.ThrowIfHasInvalidPathChar($"hoge{current}fuga"), Throws.ArgumentException);
            });
        }

        [Test]
        public void ThrowIfHasInvalidPathChar_AsPositive()
        {
            Assert.That(() => ThrowHelpers.ThrowIfHasInvalidPathChar("hoge/fuga.txt"), Throws.Nothing);
        }

        [Test]
        public void ThrowIfNegativeWithInt32_WithNegative()
        {
            Assert.That(() => ThrowHelpers.ThrowIfNegative(-1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void ThrowIfNegativeWithInt32_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ThrowHelpers.ThrowIfNegative(0), Throws.Nothing);
                Assert.That(() => ThrowHelpers.ThrowIfNegative(1), Throws.Nothing);
            });
        }

        [Test]
        public void ThrowIfNegativeWithInt64_WithNegative()
        {
            Assert.That(() => ThrowHelpers.ThrowIfNegative(-1L), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void ThrowIfNegativeWithInt64_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ThrowHelpers.ThrowIfNegative(0L), Throws.Nothing);
                Assert.That(() => ThrowHelpers.ThrowIfNegative(1L), Throws.Nothing);
            });
        }

        [Test]
        public void ThrowIfNegativeWithDouble_WithNegative()
        {
            Assert.That(() => ThrowHelpers.ThrowIfNegative(-1d), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void ThrowIfNegativeWithDouble_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ThrowHelpers.ThrowIfNegative(0d), Throws.Nothing);
                Assert.That(() => ThrowHelpers.ThrowIfNegative(1d), Throws.Nothing);
            });
        }

        [Test]
        public void ThrowIfNegativeWithDecimal_WithNegative()
        {
            Assert.That(() => ThrowHelpers.ThrowIfNegative(-1m), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void ThrowIfNegativeWithDecimal_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ThrowHelpers.ThrowIfNegative(0m), Throws.Nothing);
                Assert.That(() => ThrowHelpers.ThrowIfNegative(1m), Throws.Nothing);
            });
        }

        [Test]
        public void ThrowIfNotDefined_NotDefined()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ThrowHelpers.ThrowIfNotDefined((TestEnum)(-1)), Throws.TypeOf<ArgumentOutOfRangeException>());
                Assert.That(() => ThrowHelpers.ThrowIfNotDefined((TestEnum)2), Throws.TypeOf<ArgumentOutOfRangeException>());
            });
        }

        [Test]
        public void ThrowIfNotDefined_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ThrowHelpers.ThrowIfNotDefined(TestEnum.Zero), Throws.Nothing);
                Assert.That(() => ThrowHelpers.ThrowIfNotDefined(TestEnum.One), Throws.Nothing);
            });
        }

        [Test]
        public void ThrowIfInvalidState_AsError()
        {
            Assert.That(() => ThrowHelpers.ThrowIfInvalidState(ValueCheckState.AsError("Error")), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void ThrowIfInvalidState_AsPositive()
        {
            Assert.That(() => ThrowHelpers.ThrowIfInvalidState(ValueCheckState.Success), Throws.Nothing);
        }
    }
}
