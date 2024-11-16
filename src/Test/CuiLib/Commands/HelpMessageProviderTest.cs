using CuiLib.Commands;
using CuiLib.Options;
using CuiLib.Parameters;
using NUnit.Framework;
using System.IO;
using Test.Helpers;

namespace Test.CuiLib.Commands
{
    public class HelpMessageProviderTest
    {
        private Command command;
        private HelpMessageProvider provider;
        private BufferedTextWriter writer;

        [SetUp]
        public void SetUp()
        {
            provider = new HelpMessageProvider();
            writer = new BufferedTextWriter();

            command = new Command("command")
            {
                Description = "description",
            };
            command.Options.Add(new FlagOption('h', "help")
            {
                Description = "Display help message",
            });
            command.Options.Add(new FlagOption('v', "version")
            {
                Description = "Display version",
            });
            command.Options.Add(new SingleValueOption<FileInfo>('i')
            {
                Description = "Input file",
                Required = true,
            });
            command.Options.Add(new SingleValueOption<FileInfo>("out")
            {
                Description = "Output file",
            });
            command.Options.Add(new OrGroupOption(new FlagOption("or1")
            {
                Description = "Or-1"
            }, new FlagOption("or2")
            {
                Description = "Or-2",
            }));
            command.Parameters.Add(new SingleValueParameter<int>("num", 0)
            {
                Description = "Number",
            });
            command.Parameters.Add(new MultipleValueParameter<string>("array", 1)
            {
                Description = "Array",
            });
        }

        [TearDown]
        public void TearDown()
        {
            writer.Dispose();
        }

        #region Ctors

        [Test]
        public void Ctor()
        {
            Assert.That(() => new HelpMessageProvider(), Throws.Nothing);
        }

        #endregion Ctors

        #region Methods

        [Test]
        public void WriteHeader_WithNullWriter()
        {
            Assert.That(() => provider.WriteHeader(null!, command), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteHeader_WithNullCommand()
        {
            Assert.That(() => provider.WriteHeader(writer, null!), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteHeader_AsPositive_OnEmptyNameAndEmptyDescription()
        {
            var command = new Command(string.Empty);
            provider.WriteHeader(writer, command);

            Assert.That(writer.GetData(), Is.Empty);
        }

        [Test]
        public void WriteHeader_AsPositive_OnEmptyNameAndExistingDescription()
        {
            var command = new Command(string.Empty)
            {
                Description = "description",
            };
            provider.WriteHeader(writer, command);

            string expected = """
                Description:
                description

                """;

            Assert.That(writer.GetData(), Is.EqualTo(expected));
        }

        [Test]
        public void WriteHeader_AsPositive_OnExistingNameAndEmptyDescription()
        {
            command.Description = null;
            provider.WriteHeader(writer, command);

            string expected = """
                command

                """;

            Assert.That(writer.GetData(), Is.EqualTo(expected));
        }

        [Test]
        public void WriteHeader_AsPositive_OnExistingNameAndExistingDescription()
        {
            command.Description = "description";

            provider.WriteHeader(writer, command);

            string expected = """
                command

                Description:
                description

                """;

            Assert.That(writer.GetData(), Is.EqualTo(expected));
        }

        [Test]
        public void WriteUsage_WithNullWriter()
        {
            Assert.That(() => provider.WriteUsage(null!, command), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteUsage_WithNullCommand()
        {
            Assert.That(() => provider.WriteUsage(writer, null!), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteUsage_AsPositive_WithChildren()
        {
            command.Children.Add(new Command("child1")
            {
                Description = "Child command No. 1",
            });
            command.Children.Add(new Command("child2")
            {
                Description = "Child command No. 2",
            });
            command.Children.Add(new Command("child3")
            {
                Description = "Child command No. 3",
            });

            provider.WriteUsage(writer, command);

            string expected = """
                Usage:
                command [-h] [-v] -i file [--out file] [(--or1|--or2)] [Subcommand]

                """;

            Assert.That(writer.GetData(), Is.EqualTo(expected));
        }

        [Test]
        public void WriteUsage_AsPositive_WithParameters()
        {
            provider.WriteUsage(writer, command);

            string expected = """
                Usage:
                command [-h] [-v] -i file [--out file] [(--or1|--or2)] <num> <array ..>

                """;

            Assert.That(writer.GetData(), Is.EqualTo(expected));
        }

        [Test]
        public void WriteUsage_AsPositive_WithoutChildrenNorParameters()
        {
            command.Parameters.Clear();

            provider.WriteUsage(writer, command);

            string expected = """
                Usage:
                command [-h] [-v] -i file [--out file] [(--or1|--or2)]

                """;

            Assert.That(writer.GetData(), Is.EqualTo(expected));
        }

        [Test]
        public void WriteOptions_WithNullWriter()
        {
            Assert.That(() => provider.WriteOptions(null!, command), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteOptions_WithNullCommand()
        {
            Assert.That(() => provider.WriteOptions(writer, null!), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteOptions_AsPositive_WithEmptyOptions()
        {
            command.Options.Clear();
            provider.WriteOptions(writer, command);

            Assert.That(writer.GetData(), Is.Empty);
        }

        [Test]
        public void WriteOptions_AsPositive_WithOptions()
        {
            provider.WriteOptions(writer, command);

            string expected = """
                Options:
                  -h, --help     Display help message
                  -v, --version  Display version
                  -i             Input file
                      --out      Output file
                      --or1      Or-1
                      --or2      Or-2

                """;

            Assert.That(writer.GetData(), Is.EqualTo(expected));
        }

        [Test]
        public void WriteSubcommands_WithNullWriter()
        {
            Assert.That(() => provider.WriteSubcommands(null!, command), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteSubcommands_WithNullCommand()
        {
            Assert.That(() => provider.WriteSubcommands(writer, null!), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteSubcommands_AsPositive_WithEmptySubcommands()
        {
            command.Children.Clear();
            provider.WriteSubcommands(writer, command);

            Assert.That(writer.GetData(), Is.Empty);
        }

        [Test]
        public void WriteSubcommands_AsPositive_WithSubcommands()
        {
            command.Children.Add(new Command("child1")
            {
                Description = "Child command No. 1",
            });
            command.Children.Add(new Command("child2")
            {
                Description = "Child command No. 2",
            });
            command.Children.Add(new Command("child")
            {
                Description = "Child command",
            });
            provider.WriteSubcommands(writer, command);

            string expected = """
                Subcommands:
                  child1  Child command No. 1
                  child2  Child command No. 2
                   child  Child command

                """;

            Assert.That(writer.GetData(), Is.EqualTo(expected));
        }

        [Test]
        public void WriteParameters_WithNullWriter()
        {
            Assert.That(() => provider.WriteParameters(null!, command), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteParameters_WithNullCommand()
        {
            Assert.That(() => provider.WriteParameters(writer, null!), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteParameters_AsPositive_WithEmptyParameters()
        {
            command.Parameters.Clear();
            provider.WriteParameters(writer, command);

            Assert.That(writer.GetData(), Is.Empty);
        }

        [Test]
        public void WriteParameters_AsPositive_WithParameters()
        {
            provider.WriteParameters(writer, command);

            string expected = """
                Parameters:
                    num  Number
                  array  Array

                """;

            Assert.That(writer.GetData(), Is.EqualTo(expected));
        }

        [Test]
        public void WriteHelp_WithNullWriter()
        {
            Assert.That(() => provider.WriteHelp(null!, command), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteHelp_WithNullCommand()
        {
            Assert.That(() => provider.WriteHelp(writer, null!), Throws.ArgumentNullException);
        }

        [Test]
        public void WriteHelp_AsPositive()
        {
            provider.WriteHelp(writer, command);

            string expected = """
                command

                Description:
                description

                Usage:
                command [-h] [-v] -i file [--out file] [(--or1|--or2)] <num> <array ..>

                Options:
                  -h, --help     Display help message
                  -v, --version  Display version
                  -i             Input file
                      --out      Output file
                      --or1      Or-1
                      --or2      Or-2

                Parameters:
                    num  Number
                  array  Array

                """;

            Assert.That(writer.GetData(), Is.EqualTo(expected));
        }

        #endregion Methods
    }
}
