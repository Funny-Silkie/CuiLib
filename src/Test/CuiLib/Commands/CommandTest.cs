using CuiLib;
using CuiLib.Commands;
using CuiLib.Options;
using CuiLib.Output;
using CuiLib.Parameters;
using NUnit.Framework;
using System;
using System.IO;
using Test.Helpers;

namespace Test.CuiLib.Commands
{
    [TestFixture]
    public class CommandTest : TestBase
    {
        private MemorizeCommand command;

        [SetUp]
        public void SetUp()
        {
            command = new MemorizeCommand("cmd");
        }

        #region Ctors

        [Test]
        public void Ctor_WithNull()
        {
            Assert.That(() => new Command(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_AsPositive()
        {
            Assert.That(() => new Command("cmd"), Throws.Nothing);
        }

        #endregion Ctors

        #region Properties

        [Test]
        public void Name_Get()
        {
            Assert.That(command.Name, Is.EqualTo("cmd"));
        }

        [Test]
        public void Description_Get_OnDefault()
        {
            Assert.That(command.Description, Is.Null);
        }

        [Test]
        public void Parent_Get_OnDefault()
        {
            Assert.That(command.Parent, Is.Null);
        }

        [Test]
        public void Children_Get_OnDefault()
        {
            Assert.That(command.Children, Is.Empty);
        }

        [Test]
        public void Options_Get_OnDefault()
        {
            Assert.That(command.Options, Is.Empty);
        }

        [Test]
        public void Parameters_Get_OnDefault()
        {
            Assert.That(command.Parameters, Is.Empty);
        }

        #endregion Properties

        #region Static Methods

        [Test]
        public void InvokeFromCollection_WithNullArgs()
        {
            Assert.That(() => Command.InvokeFromCollection(null!, []), Throws.ArgumentNullException);
        }

        [Test]
        public void InvokeFromCollection_WithNullCommands()
        {
            Assert.That(() => Command.InvokeFromCollection([], null!), Throws.ArgumentNullException);
        }

        [Test]
        public void InvokeFromCollection_WithMissingCommand()
        {
            var collection = new CommandCollection() { command };

            Assert.That(() => Command.InvokeFromCollection(["other"], collection), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void InvokeFromCollection_AsPositive()
        {
            var collection = new CommandCollection() { command };
            Command.InvokeFromCollection(["cmd"], collection);

            Assert.That(command.Invoked, Is.True);
        }

        [Test]
        public void InvokeFromCollectionAsync_WithNullArgs()
        {
            Assert.That(() => Command.InvokeFromCollectionAsync(null!, []).GetAwaiter().GetResult(), Throws.ArgumentNullException);
        }

        [Test]
        public void InvokeFromCollectionAsync_WithNullCommands()
        {
            Assert.That(() => Command.InvokeFromCollectionAsync([], null!).GetAwaiter().GetResult(), Throws.ArgumentNullException);
        }

        [Test]
        public void InvokeFromCollectionAsync_WithMissingCommand()
        {
            var collection = new CommandCollection() { command };

            Assert.That(() => Command.InvokeFromCollectionAsync(["other"], collection).GetAwaiter().GetResult(), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void InvokeFromCollectionAsync_AsPositive()
        {
            var collection = new CommandCollection() { command };
            Command.InvokeFromCollectionAsync(["cmd"], collection).GetAwaiter().GetResult();

            Assert.That(command.Invoked, Is.True);
        }

        #endregion Static Methods

        #region Instance Methods

        [Test]
        public void Invoke_WithNull()
        {
            Assert.That(() => command.Invoke(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Invoke_WithNullContainingValue()
        {
            Assert.That(() => command.Invoke([null!]), Throws.ArgumentException);
        }

        [Test]
        public void Invoke_WithUnknownOption()
        {
            Assert.That(() => command.Invoke(["-e"]), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void Invoke_WithOptionValueMissing()
        {
            var option = new SingleValueOption<string>('v');
            command.Options.Add(option);

            Assert.That(() => command.Invoke(["-v"]), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void Invoke_OnHasChild_WithChildCommand()
        {
            var child = new MemorizeCommand("child");
            command.Children.Add(child);
            var childOption = new FlagOption('f');
            child.Options.Add(childOption);
            SingleValueParameter<int> childParam = child.Parameters.CreateAndAdd<int>("num");

            command.Invoke(["child", "-f", "100"]);

            Assert.Multiple(() =>
            {
                Assert.That(command.Invoked, Is.False);
                Assert.That(child.Invoked, Is.True);

                Assert.That(childOption.ValueAvailable, Is.True);
                Assert.That(childParam.ValueAvailable, Is.True);
                Assert.That(childParam.Value, Is.EqualTo(100));
            });
        }

        [Test]
        public void Invoke_OnHasChild_WithParentOptionAndChildCommand()
        {
            var child = new MemorizeCommand("child");
            command.Children.Add(child);
            var option = new FlagOption('f');
            command.Options.Add(option);

            command.Invoke(["-f", "child"]);

            Assert.Multiple(() =>
            {
                Assert.That(command.Invoked, Is.False);
                Assert.That(child.Invoked, Is.True);

                Assert.That(option.ValueAvailable, Is.True);
            });
        }

        [Test]
        public void Invoke_OnHasChild_WithOption()
        {
            var child = new MemorizeCommand("child");
            command.Children.Add(child);
            var option = new FlagOption('f');
            command.Options.Add(option);

            command.Invoke(["-f"]);

            Assert.Multiple(() =>
            {
                Assert.That(command.Invoked, Is.True);
                Assert.That(child.Invoked, Is.False);

                Assert.That(option.ValueAvailable, Is.True);
            });
        }

        [Test]
        public void Invoke_OnHasNoChild_WithOption()
        {
            var option = new SingleValueOption<int>('v');
            command.Options.Add(option);

            command.Invoke(["-v", "30"]);

            Assert.Multiple(() =>
            {
                Assert.That(command.Invoked, Is.True);

                Assert.That(option.ValueAvailable, Is.True);
                Assert.That(option.Value, Is.EqualTo(30));
            });
        }

        [Test]
        public void Invoke_OnHasNoChild_WithOptionAndParameter()
        {
            var option1 = new FlagOption('f');
            command.Options.Add(option1);
            var option2 = new FlagOption('v');
            command.Options.Add(option2);
            SingleValueParameter<int> param1 = command.Parameters.CreateAndAdd<int>("num");
            MultipleValueParameter<string> param2 = command.Parameters.CreateAndAddAsArray<string>("texts");

            command.Invoke(["-f", "13", "value1", "-v", "value2"]);

            Assert.Multiple(() =>
            {
                Assert.That(command.Invoked, Is.True);

                Assert.That(option1.ValueAvailable, Is.True);
                Assert.That(option2.ValueAvailable, Is.False);

                Assert.That(param1.ValueAvailable, Is.True);
                Assert.That(param1.Value, Is.EqualTo(13));
                Assert.That(param2.ValueAvailable, Is.True);
                Assert.That(param2.Value, Is.EqualTo(new[] { "value1", "-v", "value2" }));
            });
        }

        [Test]
        public void Invoke_OnHasNoChild_WithParameter()
        {
            SingleValueParameter<int> param1 = command.Parameters.CreateAndAdd<int>("num");
            MultipleValueParameter<string> param2 = command.Parameters.CreateAndAddAsArray<string>("texts");

            command.Invoke(["13", "value1", "-v", "value2"]);

            Assert.Multiple(() =>
            {
                Assert.That(command.Invoked, Is.True);

                Assert.That(param1.ValueAvailable, Is.True);
                Assert.That(param1.Value, Is.EqualTo(13));
                Assert.That(param2.ValueAvailable, Is.True);
                Assert.That(param2.Value, Is.EqualTo(new[] { "value1", "-v", "value2" }));
            });
        }

        [Test]
        public void Invoke_CombinedOptions_AsMissingValue()
        {
            var flag1 = new FlagOption('a');
            var flag2 = new FlagOption('b');
            var valued = new SingleValueOption<int>('c');

            command.Options.Add(flag1);
            command.Options.Add(flag2);
            command.Options.Add(valued);

            Assert.That(() => command.Invoke(["-acb", "10"]), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void Invoke_CombinedOptions_AsPositive()
        {
            var flag1 = new FlagOption('a');
            var flag2 = new FlagOption('b');
            var valued = new SingleValueOption<int>('c');

            command.Options.Add(flag1);
            command.Options.Add(flag2);
            command.Options.Add(valued);

            command.Invoke(["-abc", "10"]);

            Assert.Multiple(() =>
            {
                Assert.That(command.Invoked, Is.True);
                Assert.That(flag1.ValueAvailable, Is.True);
                Assert.That(flag1.Value, Is.True);
                Assert.That(flag2.ValueAvailable, Is.True);
                Assert.That(flag2.Value, Is.True);
                Assert.That(valued.ValueAvailable, Is.True);
                Assert.That(valued.Value, Is.EqualTo(10));
            });
        }

        [Test]
        public void InvokeAsync_WithNull()
        {
            Assert.That(() => command.InvokeAsync(null!).GetAwaiter().GetResult(), Throws.ArgumentNullException);
        }

        [Test]
        public void InvokeAsync_WithNullContainingValue()
        {
            Assert.That(() => command.InvokeAsync([null!]).GetAwaiter().GetResult(), Throws.ArgumentException);
        }

        [Test]
        public void InvokeAsync_WithUnknownOption()
        {
            Assert.That(() => command.InvokeAsync(["-e"]).GetAwaiter().GetResult(), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void InvokeAsync_WithOptionValueMissing()
        {
            var option = new SingleValueOption<string>('v');
            command.Options.Add(option);

            Assert.That(() => command.InvokeAsync(["-v"]).GetAwaiter().GetResult(), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void InvokeAsync_OnHasChild_WithChildCommand()
        {
            var child = new MemorizeCommand("child");
            command.Children.Add(child);
            var childOption = new FlagOption('f');
            child.Options.Add(childOption);
            SingleValueParameter<int> childParam = child.Parameters.CreateAndAdd<int>("num");

            command.InvokeAsync(["child", "-f", "100"]).GetAwaiter().GetResult();

            Assert.Multiple(() =>
            {
                Assert.That(command.Invoked, Is.False);
                Assert.That(child.Invoked, Is.True);

                Assert.That(childOption.ValueAvailable, Is.True);
                Assert.That(childParam.ValueAvailable, Is.True);
                Assert.That(childParam.Value, Is.EqualTo(100));
            });
        }

        [Test]
        public void InvokeAsync_OnHasChild_WithParentOptionAndChildCommand()
        {
            var child = new MemorizeCommand("child");
            command.Children.Add(child);
            var option = new FlagOption('f');
            command.Options.Add(option);

            command.InvokeAsync(["-f", "child"]).GetAwaiter().GetResult();

            Assert.Multiple(() =>
            {
                Assert.That(command.Invoked, Is.False);
                Assert.That(child.Invoked, Is.True);

                Assert.That(option.ValueAvailable, Is.True);
            });
        }

        [Test]
        public void InvokeAsync_OnHasChild_WithOption()
        {
            var child = new MemorizeCommand("child");
            command.Children.Add(child);
            var option = new FlagOption('f');
            command.Options.Add(option);

            command.InvokeAsync(["-f"]).GetAwaiter().GetResult();

            Assert.Multiple(() =>
            {
                Assert.That(command.Invoked, Is.True);
                Assert.That(child.Invoked, Is.False);

                Assert.That(option.ValueAvailable, Is.True);
            });
        }

        [Test]
        public void InvokeAsync_OnHasNoChild_WithOption()
        {
            var option = new SingleValueOption<int>('v');
            command.Options.Add(option);

            command.InvokeAsync(["-v", "30"]).GetAwaiter().GetResult();

            Assert.Multiple(() =>
            {
                Assert.That(command.Invoked, Is.True);

                Assert.That(option.ValueAvailable, Is.True);
                Assert.That(option.Value, Is.EqualTo(30));
            });
        }

        [Test]
        public void InvokeAsync_OnHasNoChild_WithOptionAndParameter()
        {
            var option1 = new FlagOption('f');
            command.Options.Add(option1);
            var option2 = new FlagOption('v');
            command.Options.Add(option2);
            SingleValueParameter<int> param1 = command.Parameters.CreateAndAdd<int>("num");
            MultipleValueParameter<string> param2 = command.Parameters.CreateAndAddAsArray<string>("texts");

            command.InvokeAsync(["-f", "13", "value1", "-v", "value2"]).GetAwaiter().GetResult();

            Assert.Multiple(() =>
            {
                Assert.That(command.Invoked, Is.True);

                Assert.That(option1.ValueAvailable, Is.True);
                Assert.That(option2.ValueAvailable, Is.False);

                Assert.That(param1.ValueAvailable, Is.True);
                Assert.That(param1.Value, Is.EqualTo(13));
                Assert.That(param2.ValueAvailable, Is.True);
                Assert.That(param2.Value, Is.EqualTo(new[] { "value1", "-v", "value2" }));
            });
        }

        [Test]
        public void InvokeAsync_OnHasNoChild_WithParameter()
        {
            SingleValueParameter<int> param1 = command.Parameters.CreateAndAdd<int>("num");
            MultipleValueParameter<string> param2 = command.Parameters.CreateAndAddAsArray<string>("texts");

            command.InvokeAsync(["13", "value1", "-v", "value2"]).GetAwaiter().GetResult();

            Assert.Multiple(() =>
            {
                Assert.That(command.Invoked, Is.True);

                Assert.That(param1.ValueAvailable, Is.True);
                Assert.That(param1.Value, Is.EqualTo(13));
                Assert.That(param2.ValueAvailable, Is.True);
                Assert.That(param2.Value, Is.EqualTo(new[] { "value1", "-v", "value2" }));
            });
        }

        [Test]
        public void InvokeAsync_CombinedOptions_AsMissingValue()
        {
            var flag1 = new FlagOption('a');
            var flag2 = new FlagOption('b');
            var valued = new SingleValueOption<int>('c');

            command.Options.Add(flag1);
            command.Options.Add(flag2);
            command.Options.Add(valued);

            Assert.That(() => command.InvokeAsync(["-acb", "10"]).GetAwaiter().GetResult(), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void InvokeAsync_CombinedOptions_AsPositive()
        {
            var flag1 = new FlagOption('a');
            var flag2 = new FlagOption('b');
            var valued = new SingleValueOption<int>('c');

            command.Options.Add(flag1);
            command.Options.Add(flag2);
            command.Options.Add(valued);

            command.InvokeAsync(["-abc", "10"]).Wait();

            Assert.Multiple(() =>
            {
                Assert.That(command.Invoked, Is.True);
                Assert.That(flag1.ValueAvailable, Is.True);
                Assert.That(flag1.Value, Is.True);
                Assert.That(flag2.ValueAvailable, Is.True);
                Assert.That(flag2.Value, Is.True);
                Assert.That(valued.ValueAvailable, Is.True);
                Assert.That(valued.Value, Is.EqualTo(10));
            });
        }

        [Test, Obsolete]
        public void WriteHelp_WithWriter_WithNull()
        {
            Assert.That(() => command.WriteHelp(null!), Throws.ArgumentNullException);
        }

        [Test, Obsolete]
        public void WriteHelp_WithWriter_AsPositive()
        {
            command.Description = "Sample command";

            command.Options.Add(new FlagOption('f', "flag")
            {
                Description = "flag option",
            });
            command.Options.Add(new SingleValueOption<int>('n', "num")
            {
                Description = "number option\n(required)",
                Required = true,
            });
            {
                SingleValueParameter<int> param = command.Parameters.CreateAndAdd<int>("num");
                param.Description = "number parameter";
            }
            {
                MultipleValueParameter<string> param = command.Parameters.CreateAndAddAsArray<string>("texts");
                param.Description = "variable length parameter";
            }

            using var writer = new BufferedTextWriter();
            command.WriteHelp(writer);

            string expected = """
                              cmd

                              Description:
                              Sample command

                              Usage:
                              cmd [-f] -n int <num> <texts ..>

                              Options:
                                -f, --flag  flag option
                                -n, --num   number option
                                            (required)

                              Parameters:
                                  num  number parameter
                                texts  variable length parameter

                              """;
            Assert.That(writer.GetData(), Is.EqualTo(expected));
        }

        [Test]
        public void WriteHelp_WithNullWriter()
        {
            Assert.That(() => command.WriteHelp(null!, null), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteHelp_AsPositive_WithNullProvider()
        {
            command.Description = "Sample command";

            command.Options.Add(new FlagOption('f', "flag")
            {
                Description = "flag option",
            });
            command.Options.Add(new SingleValueOption<int>('n', "num")
            {
                Description = "number option\n(required)",
                Required = true,
            });
            {
                SingleValueParameter<int> param = command.Parameters.CreateAndAdd<int>("num");
                param.Description = "number parameter";
            }
            {
                MultipleValueParameter<string> param = command.Parameters.CreateAndAddAsArray<string>("texts");
                param.Description = "variable length parameter";
            }

            using var writer = new BufferedTextWriter();
            command.WriteHelp(writer, null);

            string expected = """
                              cmd

                              Description:
                              Sample command

                              Usage:
                              cmd [-f] -n int <num> <texts ..>

                              Options:
                                -f, --flag  flag option
                                -n, --num   number option
                                            (required)

                              Parameters:
                                  num  number parameter
                                texts  variable length parameter

                              """;
            Assert.That(writer.GetData(), Is.EqualTo(expected));
        }

        [Test]
        public void WriteHelp_AsPositive_WithProvider()
        {
            command.Description = "Sample command";

            using var writer = new BufferedTextWriter();
            command.WriteHelp(writer, new DummyHelpMessageProvider());

            string expected = "Help";
            Assert.That(writer.GetData(), Is.EqualTo(expected));
        }

        #endregion Instance Methods

        private sealed class MemorizeCommand : Command
        {
            public bool Invoked { get; private set; }

            public MemorizeCommand(string name) : base(name)
            {
            }

            /// <inheritdoc/>
            protected override void OnExecution()
            {
                Invoked = true;
            }
        }

        private sealed class DummyHelpMessageProvider : IHelpMessageProvider
        {
            /// <inheritdoc/>
            public void WriteHelp(TextWriter writer, Command command) => writer.Write("Help");
        }
    }
}
