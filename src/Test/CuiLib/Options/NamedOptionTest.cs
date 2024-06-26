﻿using CuiLib.Options;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Options
{
    [TestFixture]
    public class NamedOptionTest : TestBase
    {
        private NamedOptionImpl option;

        [SetUp]
        public void SetUp()
        {
            option = new NamedOptionImpl('n', "name");
        }

        #region Ctors

        [Test]
        public void Ctor_WithShortName()
        {
            var option = new NamedOptionImpl('t');

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.EqualTo("t"));
                Assert.That(option.FullName, Is.Null);
            });
        }

        [Test]
        public void Ctor_WithNullFullName()
        {
            Assert.That(() => new NamedOptionImpl(fullName: null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithValidFullName()
        {
            var option = new NamedOptionImpl("test");

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.Null);
                Assert.That(option.FullName, Is.EqualTo("test"));
            });
        }

        [Test]
        public void Ctor_WithShortNameAndValidFullName()
        {
            var option = new NamedOptionImpl('t', "test");

            Assert.Multiple(() =>
            {
                Assert.That(option.ShortName, Is.EqualTo("t"));
                Assert.That(option.FullName, Is.EqualTo("test"));
            });
        }

        [Test]
        public void Ctor_WithShortNameAndNullFullName()
        {
            Assert.That(() => new NamedOptionImpl('t', null!), Throws.ArgumentNullException);
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void MatchName_WithChar()
        {
            Assert.Multiple(() =>
            {
                Assert.That(option.MatchName('n'), Is.True);
                Assert.That(option.MatchName('m'), Is.False);
                Assert.That(option.MatchName('o'), Is.False);
                Assert.That(option.MatchName('N'), Is.False);
                Assert.That(option.MatchName('-'), Is.False);
            });
        }

        [Test]
        public void MatchName_WithChar_OnNullSingleName()
        {
            var option = new NamedOptionImpl("name");

            Assert.Multiple(() =>
            {
                Assert.That(option.MatchName('n'), Is.False);
                Assert.That(option.MatchName('m'), Is.False);
                Assert.That(option.MatchName('o'), Is.False);
                Assert.That(option.MatchName('N'), Is.False);
                Assert.That(option.MatchName('-'), Is.False);
            });
        }

        [Test]
        public void MatchName_WithString_AsNull()
        {
            Assert.That(() => option.MatchName(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void MatchName_WithString_AsEmpty()
        {
            Assert.That(() => option.MatchName(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void MatchName_WithString_OnNullFullName()
        {
            var option = new NamedOptionImpl('n');

            Assert.Multiple(() =>
            {
                Assert.That(option.MatchName("name"), Is.False);
                Assert.That(option.MatchName("123"), Is.False);
                Assert.That(option.MatchName("--name"), Is.False);
                Assert.That(option.MatchName("--"), Is.False);
            });
        }

        [Test]
        public void MatchName_WithString_AsValid()
        {
            Assert.Multiple(() =>
            {
                Assert.That(option.MatchName("name"), Is.True);
                Assert.That(option.MatchName("123"), Is.False);
                Assert.That(option.MatchName("--name"), Is.False);
                Assert.That(option.MatchName("--"), Is.False);
            });
        }

        [Test]
        public void GetActualOption_AsSingle()
        {
            Assert.Multiple(() =>
            {
                Assert.That(option.GetActualOption("n", true), Is.EqualTo(option));

                Assert.That(() => option.GetActualOption("N", true), Throws.ArgumentException);
                Assert.That(() => option.GetActualOption("-n", true), Throws.ArgumentException);
                Assert.That(() => option.GetActualOption("name", true), Throws.ArgumentException);
            });
        }

        [Test]
        public void GetActualOption_AsNotSingle()
        {
            Assert.Multiple(() =>
            {
                Assert.That(option.GetActualOption("name", false), Is.EqualTo(option));

                Assert.That(() => option.GetActualOption("123", false), Throws.ArgumentException);
                Assert.That(() => option.GetActualOption("--name", false), Throws.ArgumentException);
                Assert.That(() => option.GetActualOption("n", false), Throws.ArgumentException);
            });
        }

        [Test]
        public void GetAllNames_WithoutHyphen()
        {
            Assert.That(option.GetAllNames(false), Is.EquivalentTo(new[] { "n", "name" }));
        }

        [Test]
        public void GetAllNames_WithHyphen()
        {
            Assert.That(option.GetAllNames(true), Is.EquivalentTo(new[] { "-n", "--name" }));
        }

        [Test]
        public void GetAllNames_WithoutHyphen_OnNullShortName()
        {
            var option = new NamedOptionImpl("name");

            Assert.That(option.GetAllNames(false), Is.EquivalentTo(new[] { "name" }));
        }

        [Test]
        public void GetAllNames_WithHyphen_OnNullShortName()
        {
            var option = new NamedOptionImpl("name");

            Assert.That(option.GetAllNames(true), Is.EquivalentTo(new[] { "--name" }));
        }

        [Test]
        public void GetAllNames_WithoutHyphen_OnNullFullName()
        {
            var option = new NamedOptionImpl('n');

            Assert.That(option.GetAllNames(false), Is.EquivalentTo(new[] { "n" }));
        }

        [Test]
        public void GetAllNames_WithHyphen_OnNullFullName()
        {
            var option = new NamedOptionImpl('n');

            Assert.That(option.GetAllNames(true), Is.EquivalentTo(new[] { "-n" }));
        }

        #endregion Methods

        private sealed class NamedOptionImpl : NamedOption
        {
            /// <inheritdoc/>
            public override bool ValueAvailable => throw new NotImplementedException();

            /// <inheritdoc/>
            public override bool Required { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            /// <inheritdoc/>
            internal override OptionType OptionType => throw new NotImplementedException();

            /// <see cref="NamedOption(char)"/>
            public NamedOptionImpl(char shortName) : base(shortName)
            {
            }

            /// <see cref="NamedOption(string)"/>
            public NamedOptionImpl(string fullName) : base(fullName)
            {
            }

            /// <see cref="NamedOption(char, string)"/>
            public NamedOptionImpl(char shortName, string fullName) : base(shortName, fullName)
            {
            }

            /// <inheritdoc/>
            internal override void ApplyValue(string name, string rawValue) => throw new NotImplementedException();

            /// <inheritdoc/>
            internal override void ClearValue() => throw new NotImplementedException();
        }
    }
}
