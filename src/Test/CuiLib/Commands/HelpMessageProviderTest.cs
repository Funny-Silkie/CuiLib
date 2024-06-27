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
            command.Options.Add(new SingleValueOption<FileInfo>('i', "in")
            {
                Description = "Input file",
                Required = true,
            });
            command.Options.Add(new SingleValueOption<FileInfo>("out")
            {
                Description = "Output file",
            });
            command.Parameters.Add(new SingleValueParameter<int>("num", 0)
            {
                Description = "Number",
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
                command [-h] [-v] -i file [--out file] <num>

                Options:
                  -h, --help     Display help message
                  -v, --version  Display version
                  -i, --in       Input file
                      --out      Output file

                Parameters:
                  num  Number

                """;

            Assert.That(writer.GetData(), Is.EqualTo(expected));
        }

        #endregion Methods
    }
}
