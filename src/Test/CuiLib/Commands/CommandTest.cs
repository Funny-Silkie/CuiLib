using CuiLib;
using CuiLib.Commands;
using CuiLib.Options;
using NUnit.Framework;
using System;
using Test.Helpers;

namespace Test.CuiLib.Commands
{
    [TestFixture]
    public class CommandTest
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
            Assert.Throws<ArgumentNullException>(() => new Command(null!));
        }

        [Test]
        public void Ctor_AsPositive()
        {
            Assert.DoesNotThrow(() => new Command("cmd"));
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
            Assert.Throws<ArgumentNullException>(() => Command.InvokeFromCollection(null!, []));
        }

        [Test]
        public void InvokeFromCollection_WithNullCommands()
        {
            Assert.Throws<ArgumentNullException>(() => Command.InvokeFromCollection([], null!));
        }

        [Test]
        public void InvokeFromCollection_WithMissingCommand()
        {
            var collection = new CommandCollection() { command };

            Assert.Throws<ArgumentAnalysisException>(() => Command.InvokeFromCollection(["other"], collection));
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
            Assert.Throws<ArgumentNullException>(() => Command.InvokeFromCollectionAsync(null!, []).GetAwaiter().GetResult());
        }

        [Test]
        public void InvokeFromCollectionAsync_WithNullCommands()
        {
            Assert.Throws<ArgumentNullException>(() => Command.InvokeFromCollectionAsync([], null!).GetAwaiter().GetResult());
        }

        [Test]
        public void InvokeFromCollectionAsync_WithMissingCommand()
        {
            var collection = new CommandCollection() { command };

            Assert.Throws<ArgumentAnalysisException>(() => Command.InvokeFromCollectionAsync(["other"], collection).GetAwaiter().GetResult());
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
            Assert.Throws<ArgumentNullException>(() => command.Invoke(null!));
        }

        [Test]
        public void Invoke_WithUnknownOption()
        {
            Assert.Throws<ArgumentAnalysisException>(() => command.Invoke(["-e"]));
        }

        [Test]
        public void Invoke_WithOptionValueMissing()
        {
            var option = new SingleValueOption<string>('v');
            command.Options.Add(option);

            Assert.Throws<ArgumentAnalysisException>(() => command.Invoke(["-v"]));
        }

        [Test]
        public void Invoke_OnHasChild_WithChildCommand()
        {
            var child = new MemorizeCommand("child");
            command.Children.Add(child);
            var childOption = new FlagOption('f');
            child.Options.Add(childOption);
            Parameter<int> childParam = child.Parameters.CreateAndAdd<int>("num");

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
            Parameter<int> param1 = command.Parameters.CreateAndAdd<int>("num");
            Parameter<string> param2 = command.Parameters.CreateAndAddAsArray<string>("texts");

            command.Invoke(["-f", "13", "value1", "-v", "value2"]);

            Assert.Multiple(() =>
            {
                Assert.That(command.Invoked, Is.True);

                Assert.That(option1.ValueAvailable, Is.True);
                Assert.That(option2.ValueAvailable, Is.False);

                Assert.That(param1.ValueAvailable, Is.True);
                Assert.That(param1.Value, Is.EqualTo(13));
                Assert.That(param2.ValueAvailable, Is.True);
                Assert.That(param2.Values, Is.EqualTo(new[] { "value1", "-v", "value2" }));
            });
        }

        [Test]
        public void Invoke_OnHasNoChild_WithParameter()
        {
            Parameter<int> param1 = command.Parameters.CreateAndAdd<int>("num");
            Parameter<string> param2 = command.Parameters.CreateAndAddAsArray<string>("texts");

            command.Invoke(["13", "value1", "-v", "value2"]);

            Assert.Multiple(() =>
            {
                Assert.That(command.Invoked, Is.True);

                Assert.That(param1.ValueAvailable, Is.True);
                Assert.That(param1.Value, Is.EqualTo(13));
                Assert.That(param2.ValueAvailable, Is.True);
                Assert.That(param2.Values, Is.EqualTo(new[] { "value1", "-v", "value2" }));
            });
        }

        [Test]
        public void InvokeAsync_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => command.InvokeAsync(null!).GetAwaiter().GetResult());
        }

        [Test]
        public void InvokeAsync_WithUnknownOption()
        {
            Assert.Throws<ArgumentAnalysisException>(() => command.InvokeAsync(["-e"]).GetAwaiter().GetResult());
        }

        [Test]
        public void InvokeAsync_WithOptionValueMissing()
        {
            var option = new SingleValueOption<string>('v');
            command.Options.Add(option);

            Assert.Throws<ArgumentAnalysisException>(() => command.InvokeAsync(["-v"]).GetAwaiter().GetResult());
        }

        [Test]
        public void InvokeAsync_OnHasChild_WithChildCommand()
        {
            var child = new MemorizeCommand("child");
            command.Children.Add(child);
            var childOption = new FlagOption('f');
            child.Options.Add(childOption);
            Parameter<int> childParam = child.Parameters.CreateAndAdd<int>("num");

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
            Parameter<int> param1 = command.Parameters.CreateAndAdd<int>("num");
            Parameter<string> param2 = command.Parameters.CreateAndAddAsArray<string>("texts");

            command.InvokeAsync(["-f", "13", "value1", "-v", "value2"]).GetAwaiter().GetResult();

            Assert.Multiple(() =>
            {
                Assert.That(command.Invoked, Is.True);

                Assert.That(option1.ValueAvailable, Is.True);
                Assert.That(option2.ValueAvailable, Is.False);

                Assert.That(param1.ValueAvailable, Is.True);
                Assert.That(param1.Value, Is.EqualTo(13));
                Assert.That(param2.ValueAvailable, Is.True);
                Assert.That(param2.Values, Is.EqualTo(new[] { "value1", "-v", "value2" }));
            });
        }

        [Test]
        public void InvokeAsync_OnHasNoChild_WithParameter()
        {
            Parameter<int> param1 = command.Parameters.CreateAndAdd<int>("num");
            Parameter<string> param2 = command.Parameters.CreateAndAddAsArray<string>("texts");

            command.InvokeAsync(["13", "value1", "-v", "value2"]).GetAwaiter().GetResult();

            Assert.Multiple(() =>
            {
                Assert.That(command.Invoked, Is.True);

                Assert.That(param1.ValueAvailable, Is.True);
                Assert.That(param1.Value, Is.EqualTo(13));
                Assert.That(param2.ValueAvailable, Is.True);
                Assert.That(param2.Values, Is.EqualTo(new[] { "value1", "-v", "value2" }));
            });
        }

        [Test]
        public void WriteHelp_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => command.WriteHelp(null!));
        }

        [Test]
        public void WriteHelp_AsPositive()
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
                Parameter<int> param = command.Parameters.CreateAndAdd<int>("num");
                param.Description = "number parameter";
            }
            {
                Parameter<string> param = command.Parameters.CreateAndAddAsArray<string>("texts");
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
    }
}
