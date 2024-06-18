using CuiLib.Commands;
using CuiLib.Parsing;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Parsing
{
    public class ArgumentParserTest
    {
        private ArgumentParser parser;

        [SetUp]
        public void SetUp()
        {
            parser = new ArgumentParser(["child", "--num", "100", "-f", "value1", "value2"]);
        }

        #region Ctors

        [Test]
        public void Ctor_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ArgumentParser(null!));
        }

        [Test]
        public void Ctor_AsPositive()
        {
            Assert.DoesNotThrow(() => new ArgumentParser(["hoge"]));
        }

        #endregion Ctors

        #region Properties

        [Test]
        public void EndOfArguments_Get_OnEmptyArgs()
        {
            var parser = new ArgumentParser([]);

            Assert.That(parser.EndOfArguments, Is.True);
        }

        [Test]
        public void EndOfArguments_Get_OnNotEnd()
        {
            Assert.That(parser.EndOfArguments, Is.False);
        }

        [Test]
        public void EndOfArguments_Get_OnEnd()
        {
            var parser = new ArgumentParser(["hoge"]);
            parser.SkipArguments(6);

            Assert.That(parser.EndOfArguments, Is.True);
        }

        [Test]
        public void Index_Get_OnDefault()
        {
            Assert.That(parser.Index, Is.EqualTo(0));
        }

        #endregion Properties

        #region Methods

        [Test]
        public void SkipArguments_WithNegative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => parser.SkipArguments(-1));
        }

        [Test]
        public void SkipArguments_AsPositive_WithZero()
        {
            parser.SkipArguments(0);

            Assert.That(parser.Index, Is.EqualTo(0));
        }

        [Test]
        public void SkipArguments_AsPositive_WithNonOverflow()
        {
            parser.SkipArguments(2);

            Assert.That(parser.Index, Is.EqualTo(2));
        }

        [Test]
        public void SkipArguments_AsPositive_WithOverflow()
        {
            parser.SkipArguments(int.MaxValue);

            Assert.That(parser.Index, Is.EqualTo(6));
        }

        [Test]
        public void SkipArguments_AsPositive_OnEndOfArguments()
        {
            parser.SkipArguments(int.MaxValue);
            parser.SkipArguments(0);

            Assert.That(parser.Index, Is.EqualTo(6));
        }

        [Test]
        public void GetTargetCommand_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => parser.GetTargetCommand(null!));
        }

        [Test]
        public void GetTargetCommand_AsPositive_WithEmpty()
        {
            Assert.Multiple(() =>
            {
                Assert.That(parser.GetTargetCommand([]), Is.Null);
                Assert.That(parser.Index, Is.EqualTo(0));
            });
        }

        [Test]
        public void GetTargetCommand_AsPositive_WithMissingCommand()
        {
            var child = new Command("1-2");
            child.Children.Add(new Command("2-1"));
            child.Children.Add(new Command("2-2"));
            var target = new Command("2-3");
            child.Children.Add(target);
            var commandCollection = new CommandCollection()
            {
                new Command("1-1"),
                child,
                new Command("1-3"),
            };

            Assert.Multiple(() =>
            {
                Assert.Multiple(() =>
                {
                    var parser = new ArgumentParser(["1-4"]);
                    Assert.That(parser.GetTargetCommand(commandCollection), Is.Null);
                    Assert.That(parser.Index, Is.EqualTo(0));
                });
                Assert.Multiple(() =>
                {
                    var parser = new ArgumentParser(["2-3", "1-2"]);
                    Assert.That(parser.GetTargetCommand(commandCollection), Is.Null);
                    Assert.That(parser.Index, Is.EqualTo(0));
                });
                Assert.Multiple(() =>
                {
                    var parser = new ArgumentParser(["1-2", "2-0"]);
                    Assert.That(parser.GetTargetCommand(commandCollection), Is.Null);
                    Assert.That(parser.Index, Is.EqualTo(0));
                });
            });
        }

        [Test]
        public void GetTargetCommand_AsPositive_WithExistingCommand()
        {
            var parser = new ArgumentParser(["hoge"]);
            var target = new Command("hoge");

            Assert.Multiple(() =>
            {
                Assert.That(parser.GetTargetCommand([target, new Command("fuga")]), Is.EqualTo(target));
                Assert.That(parser.Index, Is.EqualTo(1));
            });
        }

        [Test]
        public void GetTargetCommand_AsPositive_WithNestedCommand()
        {
            var parser = new ArgumentParser(["1-2", "2-3"]);
            var child = new Command("1-2");
            child.Children.Add(new Command("2-1"));
            child.Children.Add(new Command("2-2"));
            var target = new Command("2-3");
            child.Children.Add(target);

            Assert.Multiple(() =>
            {
                Assert.That(parser.GetTargetCommand([new Command("1-1"), child, new Command("1-3")]), Is.EqualTo(target));
                Assert.That(parser.Index, Is.EqualTo(2));
            });
        }

        #endregion Methods
    }
}
