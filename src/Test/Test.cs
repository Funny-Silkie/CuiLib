using CuiLib.Commands;
using CuiLib.Log;
using CuiLib.Options;
using NUnit.Framework;
using System.Linq;

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
            Parameter<string> param = main.Parameters.CreateAndAppendArray<string>("Param");
            main.Invoke(args);

            Assert.That(Enumerable.SequenceEqual(args, param.Values!), Is.True);
        }

        [Test]
        public void NoOptionHasChild()
        {
            string[] args = new[] { "main", "A", "B", "C" };

            var parent = new Command("parent");
            var main = new MainCommand();
            parent.Children.Add(main);
            Parameter<string> param = main.Parameters.CreateAndAppendArray<string>("Param");
            parent.Invoke(args);

            Assert.That(Enumerable.SequenceEqual(args[1..], param.Values!), Is.True);
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

            Parameter<string> param = main.Parameters.CreateAndAppendArray<string>("Param");

            main.Invoke(args);

            Assert.That(Enumerable.SequenceEqual(param.Values!, new[] { "A", "B", "C" }), Is.True);
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

            Parameter<string> param = main.Parameters.CreateAndAppendArray<string>("Param");

            parent.Children.Add(main);

            parent.Invoke(args);

            Assert.That(Enumerable.SequenceEqual(param.Values!, new[] { "A", "B", "C" }), Is.True);
            Assert.That(opFlag1.Value, Is.True);
            Assert.That(opFlag2.Value, Is.False);
            Assert.That(opValue1.Value, Is.EqualTo(1));
            Assert.That(opValue2.Value, Is.Null);
        }

        [Test]
        public void Help()
        {
            string[] args = new[] { "main", "--flag1", "-n", "1", "A", "B", "C" };

            var parent = new Command("parent")
            {
                Description = "Parent Command",
            };
            var main = new MainCommand()
            {
                Description = "Main Command",
            };

            var opFlag1 = new FlagOption("flag1")
            {
                Description = "フラグ1",
            };
            var opFlag2 = new FlagOption("flag2")
            {
                Description = "フラグ2",
            };
            var opValue1 = new ValuedOption<int>('n', "number")
            {
                Description = "数値",
                Required = true,
            };
            var opValue2 = new ValuedOption<string>('i', "in")
            {
                Description = "文字",
            };

            main.Options.Add(opFlag1);
            main.Options.Add(opFlag2);
            main.Options.Add(opValue1);
            main.Options.Add(opValue2);

            parent.Children.Add(main);

            Parameter<string> param = main.Parameters.CreateAndAppendArray<string>("Param");
            param.Description = "Parameters\nThis is array";

            using var logger1 = new Logger("Log_parent.txt");
            parent.WriteHelp(logger1);
            using var logger2 = new Logger("Log_main.txt");
            main.WriteHelp(logger2);
        }

        private sealed class MainCommand : Command
        {
            public MainCommand() : base("main")
            {
            }

            protected override void OnExecution()
            {
            }
        }
    }
}
