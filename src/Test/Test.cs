using CuiLib.Commands;
using CuiLib.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [TestFixture]
    public class ArgumentAnalysisTest
    {
        [Test]
        public void NoOptionNoChild()
        {
            string[] args = new[] { "A", "B", "C" };

            var main = new MainCommand();
            main.Invoke(args);

            Assert.That(Enumerable.SequenceEqual(args, main.Arguments!), Is.True);
        }

        [Test]
        public void NoOptionHasChild()
        {
            string[] args = new[] { "main", "A", "B", "C" };

            var parent = new Command("parent");
            var main = new MainCommand();
            parent.Children.Add(main);
            parent.Invoke(args);

            Assert.That(Enumerable.SequenceEqual(args[1..], main.Arguments!), Is.True);
        }

        [Test]
        public void HasOptionNoChild()
        {
            string[] args = new[] { "--flag1", "-n", "1", "A", "B", "C" };

            var main = new MainCommand();

            var opFlag1 = new FlagOption("flag1");
            var opFlag2 = new FlagOption("flag2");
            var opValue1 = new ValuedOption<int>('n', "number");
            var opValue2 = new ValuedOption<string>('i', "in");

            main.Options.Add(opFlag1);
            main.Options.Add(opFlag2);
            main.Options.Add(opValue1);
            main.Options.Add(opValue2);

            main.Invoke(args);

            Assert.That(Enumerable.SequenceEqual(main.Arguments!, new[] { "A", "B", "C" }), Is.True);
            Assert.That(opFlag1.Value, Is.True);
            Assert.That(opFlag2.Value, Is.False);
            Assert.That(opValue1.Value, Is.EqualTo(1));
            Assert.That(opValue2.Value, Is.Null);
        }

        [Test]
        public void HasOptionHasChild()
        {
            string[] args = new[] { "main", "--flag1", "-n", "1", "A", "B", "C" };

            var parent = new Command("parent");
            var main = new MainCommand();

            var opFlag1 = new FlagOption("flag1");
            var opFlag2 = new FlagOption("flag2");
            var opValue1 = new ValuedOption<int>('n', "number");
            var opValue2 = new ValuedOption<string>('i', "in");

            main.Options.Add(opFlag1);
            main.Options.Add(opFlag2);
            main.Options.Add(opValue1);
            main.Options.Add(opValue2);

            parent.Children.Add(main);

            parent.Invoke(args);

            Assert.That(Enumerable.SequenceEqual(main.Arguments!, new[] { "A", "B", "C" }), Is.True);
            Assert.That(opFlag1.Value, Is.True);
            Assert.That(opFlag2.Value, Is.False);
            Assert.That(opValue1.Value, Is.EqualTo(1));
            Assert.That(opValue2.Value, Is.Null);
        }

        private sealed class MainCommand : Command
        {
            public string[]? Arguments { get; private set; }

            public MainCommand() : base("main")
            {
            }

            protected override void Execute(ReadOnlySpan<string> args)
            {
                Arguments = args.ToArray();
            }
        }
    }
}
