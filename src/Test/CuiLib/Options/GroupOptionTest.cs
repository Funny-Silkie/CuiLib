﻿using CuiLib;
using CuiLib.Options;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Test.CuiLib.Options
{
    [TestFixture]
    public class GroupOptionTest : TestBase
    {
        private GroupOptionImpl option;
        private FlagOption child1;
        private SingleValueOption<int> child2;
        private GroupOptionImpl child3;
        private SingleValueOption<string> gChild;

        [SetUp]
        public void SetUp()
        {
            option = new GroupOptionImpl();
            child1 = new FlagOption('f', "flag");
            child2 = new SingleValueOption<int>('n', "num");
            gChild = new SingleValueOption<string>('t', "text");

            child3 = new GroupOptionImpl();

            child3.AddChildOption(gChild);

            option.AddChildOption(child1);
            option.AddChildOption(child2);
            option.AddChildOption(child3);
        }

        #region Properties

        [Test]
        public void IsValued_Get()
        {
            Assert.That(option.IsValued, Is.False);
        }

        [Test]
        public void CanMultiValue_Get()
        {
            Assert.That(option.CanMultiValue, Is.False);
        }

        [Test]
        public void IsGroup_Get()
        {
            Assert.That(option.IsGroup, Is.True);
        }

        [Test]
        public void OptionType_Get()
        {
            Assert.That(option.OptionType, Is.EqualTo(OptionType.Group));
        }

        [Test]
        public void Required_Set()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => option.Required = true, Throws.TypeOf<NotSupportedException>());
                Assert.That(() => option.Required = false, Throws.TypeOf<NotSupportedException>());
            });
        }

        #endregion Properties

        #region Methods

        [Test]
        public void ApplyValue_ToChild1_ByShortName()
        {
            option.ApplyValue("f", string.Empty);

            Assert.Multiple(() =>
            {
                Assert.That(child1.ValueAvailable, Is.True);
                Assert.That(child2.ValueAvailable, Is.False);
            });
        }

        [Test]
        public void ApplyValue_ToChild1_ByFullName()
        {
            option.ApplyValue("flag", string.Empty);

            Assert.Multiple(() =>
            {
                Assert.That(child1.ValueAvailable, Is.True);
                Assert.That(child2.ValueAvailable, Is.False);
            });
        }

        [Test]
        public void ApplyValue_ToChild2_ByShortName()
        {
            option.ApplyValue("n", "1");

            Assert.Multiple(() =>
            {
                Assert.That(child1.ValueAvailable, Is.False);
                Assert.That(child2.ValueAvailable, Is.True);
            });
        }

        [Test]
        public void ApplyValue_ToChild2_ByFullName()
        {
            option.ApplyValue("num", "1");

            Assert.Multiple(() =>
            {
                Assert.That(child1.ValueAvailable, Is.False);
                Assert.That(child2.ValueAvailable, Is.True);
            });
        }

        [Test]
        public void ApplyValue_ToMissedChild()
        {
            Assert.That(() => option.ApplyValue("unknown", "1"), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void ApplyValue_ToBothChildren()
        {
            option.ApplyValue("f", string.Empty);
            option.ApplyValue("n", "1");

            Assert.Multiple(() =>
            {
                Assert.That(child1.ValueAvailable, Is.True);
                Assert.That(child2.ValueAvailable, Is.True);
            });
        }

        [Test]
        public void ClearValue()
        {
            option.ApplyValue("f", string.Empty);
            option.ApplyValue("n", "1");
            option.ClearValue();

            Assert.Multiple(() =>
            {
                Assert.That(child1.ValueAvailable, Is.False);
                Assert.That(child2.ValueAvailable, Is.False);
            });
        }

        [Test]
        public void GetAllNames_WithoutHyphen()
        {
            Assert.That(option.GetAllNames(false), Is.EquivalentTo(new[] { "f", "flag", "n", "num", "t", "text" }));
        }

        [Test]
        public void GetAllNames_WithHyphen()
        {
            Assert.That(option.GetAllNames(true), Is.EquivalentTo(new[] { "-f", "--flag", "-n", "--num", "-t", "--text" }));
        }

        [Test]
        public void GetActualOption_ForExistingChild_AsSingle()
        {
            Assert.Multiple(() =>
            {
                Assert.That(option.GetActualOption("f", true), Is.EqualTo(child1));
                Assert.That(option.GetActualOption("n", true), Is.EqualTo(child2));
            });
        }

        [Test]
        public void GetActualOption_ForExistingChild_AsNotSingle()
        {
            Assert.Multiple(() =>
            {
                Assert.That(option.GetActualOption("flag", true), Is.EqualTo(child1));
                Assert.That(option.GetActualOption("num", true), Is.EqualTo(child2));
            });
        }

        [Test]
        public void GetActualOption_ForNestedChild_AsSingle()
        {
            Assert.That(option.GetActualOption("t", true), Is.EqualTo(gChild));
        }

        [Test]
        public void GetActualOption_ForNestedChild_AsNotSingle()
        {
            Assert.That(option.GetActualOption("text", true), Is.EqualTo(gChild));
        }

        [Test]
        public void GetActualOption_ForMissingChild()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => option.GetActualOption("oops!", false), Throws.TypeOf<ArgumentException>());
                Assert.That(() => option.GetActualOption("!", true), Throws.TypeOf<ArgumentException>());
            });
        }

        [Test]
        public void MatchName_WithChar()
        {
            Assert.Multiple(() =>
            {
                Assert.That(option.MatchName('f'), Is.True);
                Assert.That(option.MatchName('n'), Is.True);
                Assert.That(option.MatchName('F'), Is.False);
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
        public void MatchName_WithString_AsValid()
        {
            Assert.Multiple(() =>
            {
                Assert.That(option.MatchName("flag"), Is.True);
                Assert.That(option.MatchName("num"), Is.True);
                Assert.That(option.MatchName("123"), Is.False);
                Assert.That(option.MatchName("--flag"), Is.False);
                Assert.That(option.MatchName("--name"), Is.False);
                Assert.That(option.MatchName("--"), Is.False);
            });
        }

        [Test]
        public void GetEnumerator()
        {
            var list = new List<Option>();
            foreach (Option current in option) list.Add(current);

            Assert.That(list, Is.EqualTo(new Option[] { child1, child2, child3 }));
        }

        [Test]
        public void GetEnumerator_AsNonGeneric()
        {
            var list = new List<Option>();
            foreach (object? current in (IEnumerable)option) list.Add((Option)current!);

            Assert.That(list, Is.EqualTo(new Option[] { child1, child2, child3 }));
        }

        #endregion Methods

        private sealed class GroupOptionImpl : GroupOption
        {
            /// <inheritdoc/>
            public override bool ValueAvailable => throw new NotImplementedException();

            /// <inheritdoc/>
            public override bool Required
            {
                get => throw new NotImplementedException();
#pragma warning disable CS8770 // メソッドには、実装された、またはオーバーライドされたメンバーと一致する '[DoesNotReturn]' 注釈がありません。
                set => base.Required = value;
#pragma warning restore CS8770 // メソッドには、実装された、またはオーバーライドされたメンバーと一致する '[DoesNotReturn]' 注釈がありません。
            }

            public int ChildrenCount => Children.Count;

            public void AddChildOption(Option option) => Children.Add(option);
        }
    }
}
