using CuiLib.Converters;
using CuiLib.Converters.Implementations;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Converters
{
    [TestFixture]
    public partial class ValueConverterTest : TestBase
    {
        [Test]
        public void Combine_WithNullIValueConverter()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueConverter.GetDefault<int>().Combine<string, int, long>(second: null!), Throws.ArgumentNullException);
                Assert.That(() => ValueConverter.Combine<object, string, int>(null!, ValueConverter.GetDefault<int>()), Throws.ArgumentNullException);
            });
        }

        [Test]
        public void Combine_WithIValueConverter_AsPositive()
        {
            IValueConverter<string, int> converter = ValueConverter.GetDefault<int>().Combine(ValueConverter.FromDelegate<int, int>(x => checked(x * 2)));

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("1"), Is.EqualTo(2));
                Assert.That(converter.Convert("0"), Is.EqualTo(0));
                Assert.That(converter.Convert("-100"), Is.EqualTo(-200));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert(int.MaxValue.ToString()), Throws.TypeOf<OverflowException>());
            });
        }

        [Test]
        public void Combine_WithNullConverter()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => ValueConverter.GetDefault<int>().Combine<string, int, long>(secondConverter: null!), Throws.ArgumentNullException);
                Assert.That(() => ValueConverter.Combine<object, string, int>(null!, int.Parse), Throws.ArgumentNullException);
            });
        }

        [Test]
        public void Combine_WithConverter_AsPositive()
        {
            IValueConverter<string, int> converter = ValueConverter.GetDefault<int>().Combine(x => checked(x * 2));

            Assert.Multiple(() =>
            {
                Assert.That(converter.Convert("1"), Is.EqualTo(2));
                Assert.That(converter.Convert("0"), Is.EqualTo(0));
                Assert.That(converter.Convert("-100"), Is.EqualTo(-200));

                Assert.That(() => converter.Convert(null!), Throws.ArgumentNullException);
                Assert.That(() => converter.Convert(int.MaxValue.ToString()), Throws.TypeOf<OverflowException>());
            });
        }

        [Test]
        public void FromDelegate_WithNull()
        {
            Assert.That(() => ValueConverter.FromDelegate<string, int>(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void FromDelegate_AsPositive()
        {
            IValueConverter<string, int> converter = ValueConverter.FromDelegate<string, int>(int.Parse);

            Assert.Multiple(() =>
            {
                Assert.That(converter, Is.TypeOf<DelegateValueConverter<string, int>>());
                Assert.That(((DelegateValueConverter<string, int>)converter).Converter, Is.EqualTo((Converter<string, int>)int.Parse));
            });
        }
    }
}
