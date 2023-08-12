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
            var opValue1 = new SingleValueOption<int>('n', "number");
            var opValue2 = new SingleValueOption<string>('i', "in");

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
            var opValue1 = new SingleValueOption<int>('n', "number");
            var opValue2 = new SingleValueOption<string>('i', "in");

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
        public void NonDefinedParams()
        {
            string[] args = new[] { "--flag1", "-n", "1", "A", "B", "C" };

            var main = new MainCommand();
            main.Parameters.AllowAutomaticallyCreate = true;

            var opFlag1 = new FlagOption("flag1");
            var opFlag2 = new FlagOption("flag2");
            var opValue1 = new SingleValueOption<int>('n', "number");
            var opValue2 = new SingleValueOption<string>('i', "in");

            main.Options.Add(opFlag1);
            main.Options.Add(opFlag2);
            main.Options.Add(opValue1);
            main.Options.Add(opValue2);

            main.Invoke(args);
            Assert.Multiple(() =>
            {
                Assert.That(((Parameter<string>)main.Parameters[0]).Values![0], Is.EqualTo("A"));
                Assert.That(((Parameter<string>)main.Parameters[1]).Values![0], Is.EqualTo("B"));
                Assert.That(((Parameter<string>)main.Parameters[2]).Values![0], Is.EqualTo("C"));
                Assert.That(opFlag1.Value, Is.True);
                Assert.That(opFlag2.Value, Is.False);
                Assert.That(opValue1.Value, Is.EqualTo(1));
                Assert.That(opValue2.Value, Is.Null);
            });
        }

        [Test]
        public void MultiValueOption()
        {
            string[] args = new[] { "--flag1", "-n", "1", "-n", "2" };

            var main = new MainCommand();

            var opFlag1 = new FlagOption("flag1");
            var opFlag2 = new FlagOption("flag2");
            var opValue1 = new MultipleValueOption<int>('n', "number");
            var opValue2 = new SingleValueOption<string>('i', "in");

            main.Options.Add(opFlag1);
            main.Options.Add(opFlag2);
            main.Options.Add(opValue1);
            main.Options.Add(opValue2);

            main.Invoke(args);

            Assert.That(opFlag1.Value, Is.True);
            Assert.That(opFlag2.Value, Is.False);
            Assert.That(Enumerable.SequenceEqual(opValue1.Value, new[] { 1, 2 }), Is.True);
            Assert.That(opValue2.Value, Is.Null);
        }

        [Test]
        public void ShortOptions()
        {
            string[] args = new[] { "-1n", "1", "-i", "hoge" };

            var main = new MainCommand();

            var opFlag1 = new FlagOption('1', "flag1");
            var opFlag2 = new FlagOption('2', "flag2");
            var opValue1 = new SingleValueOption<int>('n', "number");
            var opValue2 = new SingleValueOption<string>('i', "in");

            main.Options.Add(opFlag1);
            main.Options.Add(opFlag2);
            main.Options.Add(opValue1);
            main.Options.Add(opValue2);

            main.Invoke(args);

            Assert.That(opFlag1.Value, Is.True);
            Assert.That(opFlag2.Value, Is.False);
            Assert.That(opValue1.Value, Is.EqualTo(1));
            Assert.That(opValue2.Value, Is.EqualTo("hoge"));
        }

        [Test]
        public void GroupOr()
        {
            string[] args = new[] { "-1", "1", "-3", "5" };

            var main = new MainCommand();

            var option1 = new SingleValueOption<int>('1')
            {
                Description = "数値1",
                Required = true,
            };
            var option2 = new SingleValueOption<string>('2')
            {
                Description = "文字列",
                Required = true,
            };
            var option3 = new SingleValueOption<int>('3')
            {
                Description = "数値2",
                Required = true,
            };
            var group = new OrGroupOption(option1, option2, option3);

            main.Options.Add(group);

            main.Invoke(args);

            Assert.That(group.ValueAvailable, Is.True);
            Assert.That(option1.ValueAvailable, Is.True);
            Assert.That(option1.Value, Is.EqualTo(1));
            Assert.That(option2.ValueAvailable, Is.False);
            Assert.That(option3.ValueAvailable, Is.True);
            Assert.That(option3.Value, Is.EqualTo(5));
        }

        [Test]
        public void GroupAnd()
        {
            string[] args = new[] { "-1", "1", "-2", "10", "-3", "5" };

            var main = new MainCommand();

            var option1 = new SingleValueOption<int>('1')
            {
                Description = "数値1",
                Required = true,
            };
            var option2 = new SingleValueOption<string>('2')
            {
                Description = "文字列",
                Required = true,
            };
            var option3 = new SingleValueOption<int>('3')
            {
                Description = "数値2",
                Required = true,
            };
            var group = new AndGroupOption(option1, option2, option3);

            main.Options.Add(group);

            main.Invoke(args);

            Assert.That(group.ValueAvailable, Is.True);
            Assert.That(option1.ValueAvailable, Is.True);
            Assert.That(option1.Value, Is.EqualTo(1));
            Assert.That(option2.ValueAvailable, Is.True);
            Assert.That(option2.Value, Is.EqualTo("10"));
            Assert.That(option3.ValueAvailable, Is.True);
            Assert.That(option3.Value, Is.EqualTo(5));
        }

        [Test]
        public void GroupXor()
        {
            string[] args = new[] { "-3", "5" };

            var main = new MainCommand();

            var option1 = new SingleValueOption<int>('1')
            {
                Description = "数値1",
                Required = true,
            };
            var option2 = new SingleValueOption<string>('2')
            {
                Description = "文字列",
                Required = true,
            };
            var option3 = new SingleValueOption<int>('3')
            {
                Description = "数値2",
                Required = true,
            };
            var group = new XorGroupOption(option1, option2, option3);

            main.Options.Add(group);

            main.Invoke(args);

            Assert.That(group.ValueAvailable, Is.True);
            Assert.That(option1.ValueAvailable, Is.False);
            Assert.That(option2.ValueAvailable, Is.False);
            Assert.That(option3.ValueAvailable, Is.True);
            Assert.That(option3.Value, Is.EqualTo(5));
        }

        [Test]
        public void Help()
        {
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
            var opValue1 = new SingleValueOption<int>('n', "number")
            {
                Description = "数値",
                Required = true,
            };
            var opValue2 = new SingleValueOption<string>('i', "in")
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
