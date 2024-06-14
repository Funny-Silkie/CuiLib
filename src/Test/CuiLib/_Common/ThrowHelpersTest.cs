using CuiLib;
using CuiLib.Checkers;
using CuiLib.Options;
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
            Assert.Throws<ArgumentAnalysisException>(() => ThrowHelpers.ThrowAsOptionParseFailed(new ArgumentException("Error")));
        }

        [Test]
        public void ThrowAsEmptyOption()
        {
            Assert.Throws<ArgumentAnalysisException>(() => ThrowHelpers.ThrowAsEmptyOption(new SingleValueOption<int>("num")));
        }

        [Test]
        public void ThrowAsEmptyCollection()
        {
            Assert.Throws<InvalidOperationException>(ThrowHelpers.ThrowAsEmptyCollection);
        }

        [Test]
        public void ThrowIfNull_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => ThrowHelpers.ThrowIfNull<object>(null));
        }
        
        [Test]
        public void ThrowIfNull_AsPositive()
        {
            Assert.DoesNotThrow(() => ThrowHelpers.ThrowIfNull<object>(string.Empty));
            Assert.DoesNotThrow(() => ThrowHelpers.ThrowIfNull<object>(0));
            Assert.DoesNotThrow(() => ThrowHelpers.ThrowIfNull<object>(false));
        }

        [Test]
        public void ThrowIfNullOrEmpty_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => ThrowHelpers.ThrowIfNullOrEmpty(null));
        }

        [Test]
        public void ThrowIfNullOrEmpty_WithEmpty()
        {
            Assert.Throws<ArgumentException>(() => ThrowHelpers.ThrowIfNullOrEmpty(string.Empty));
        }

        [Test]
        public void ThrowIfNullOrEmpty_AsPositive()
        {
            Assert.DoesNotThrow(() => ThrowHelpers.ThrowIfNullOrEmpty("hoge"));
        }

        [Test]
        public void ThrowIfHasInvalidFileNameChar()
        {
            Assert.Multiple(() =>
            {
                foreach (char current in Path.GetInvalidFileNameChars()) Assert.Throws<ArgumentException>(() => ThrowHelpers.ThrowIfHasInvalidFileNameChar("hoge/fuga"));
            });
        }

        [Test]
        public void ThrowIfHasInvalidFileNameChar_AsPositive()
        {
            Assert.DoesNotThrow(() => ThrowHelpers.ThrowIfHasInvalidFileNameChar("hoge.txt"));
        }

        [Test]
        public void ThrowIfHasInvalidPathChar()
        {
            Assert.Multiple(() =>
            {
                foreach (char current in Path.GetInvalidPathChars()) Assert.Throws<ArgumentException>(() => ThrowHelpers.ThrowIfHasInvalidPathChar($"hoge{current}fuga"));
            });
        }

        [Test]
        public void ThrowIfHasInvalidPathChar_AsPositive()
        {
            Assert.DoesNotThrow(() => ThrowHelpers.ThrowIfHasInvalidPathChar("hoge/fuga.txt"));
        }

        [Test]
        public void ThrowIfNegativeWithInt32_WithNegative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ThrowHelpers.ThrowIfNegative(-1));
        }

        [Test]
        public void ThrowIfNegativeWithInt32_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => ThrowHelpers.ThrowIfNegative(0));
                Assert.DoesNotThrow(() => ThrowHelpers.ThrowIfNegative(1));
            });
        }

        [Test]
        public void ThrowIfNegativeWithInt64_WithNegative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ThrowHelpers.ThrowIfNegative(-1L));
        }

        [Test]
        public void ThrowIfNegativeWithInt64_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => ThrowHelpers.ThrowIfNegative(0L));
                Assert.DoesNotThrow(() => ThrowHelpers.ThrowIfNegative(1L));
            });
        }

        [Test]
        public void ThrowIfNegativeWithDouble_WithNegative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ThrowHelpers.ThrowIfNegative(-1d));
        }

        [Test]
        public void ThrowIfNegativeWithDouble_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => ThrowHelpers.ThrowIfNegative(0d));
                Assert.DoesNotThrow(() => ThrowHelpers.ThrowIfNegative(1d));
            });
        }

        [Test]
        public void ThrowIfNegativeWithDecimal_WithNegative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ThrowHelpers.ThrowIfNegative(-1m));
        }

        [Test]
        public void ThrowIfNegativeWithDecimal_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => ThrowHelpers.ThrowIfNegative(0m));
                Assert.DoesNotThrow(() => ThrowHelpers.ThrowIfNegative(1m));
            });
        }

        [Test]
        public void ThrowIfNotDefined_NotDefined()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => ThrowHelpers.ThrowIfNotDefined((TestEnum)(-1)));
                Assert.Throws<ArgumentOutOfRangeException>(() => ThrowHelpers.ThrowIfNotDefined((TestEnum)2));
            });
        }

        [Test]
        public void ThrowIfNotDefined_AsPositive()
        {
            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => ThrowHelpers.ThrowIfNotDefined(TestEnum.Zero));
                Assert.DoesNotThrow(() => ThrowHelpers.ThrowIfNotDefined(TestEnum.One));
            });
        }

        [Test]
        public void ThrowIfInvalidState_AsError()
        {
            Assert.Throws<ArgumentAnalysisException>(() => ThrowHelpers.ThrowIfInvalidState(ValueCheckState.AsError("Error")));
        }

        [Test]
        public void ThrowIfInvalidState_AsPositive()
        {
            Assert.DoesNotThrow(() => ThrowHelpers.ThrowIfInvalidState(ValueCheckState.Success));
        }
    }
}
