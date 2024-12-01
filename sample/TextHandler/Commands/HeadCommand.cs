using CuiLib.Checkers;
using CuiLib.Commands;
using CuiLib.Options;
using CuiLib.Parameters;
using System.Text.RegularExpressions;

namespace TextHandler.Commands
{
    /// <summary>
    /// 先頭行の抽出を行うコマンドを表します。
    /// </summary>
    public class HeadCommand : Command
    {
        #region Options

        private readonly FlagOption optionHelp;
        private readonly SingleValueOption<int> optionLines;
        private readonly SingleValueOption<TextWriter> optionOut;

        #endregion Options

        #region Parameters

        private readonly SingleValueParameter<TextReader> parameterFile;

        #endregion Parameters

        /// <summary>
        /// <see cref="HeadCommand"/>の新しいインスタンスを初期化します。
        /// </summary>
        public HeadCommand() : base("head")
        {
            Description = "Extract head lines in text files";

            optionHelp = new FlagOption('h', "help")
            {
                Description = "Displays help message",
            };
            optionLines = new SingleValueOption<int>('n', "lines")
            {
                Description = "The number of lines to extract",
                Required = true,
                Checker = ValueChecker.GreaterThan(0),
            };
            optionOut = new SingleValueOption<TextWriter>('o', "out")
            {
                Description = "Destination file.\nDefault is standard out.",
                DefaultValue = Console.Out,
            };

            Options.Add(optionHelp);
            Options.Add(optionLines);
            Options.Add(optionOut);

            parameterFile = new SingleValueParameter<TextReader>("input", 0)
            {
                Description = "Text file to read",
            };

            Parameters.Add(parameterFile);
        }

        /// <inheritdoc/>
        protected override void OnExecution()
        {
            if (optionHelp.Value)
            {
                WriteHelp(Console.Out);
                return;
            }

            TextReader inputFile;
            if (!parameterFile.ValueAvailable)
            {
                if (!Console.IsInputRedirected)
                {
                    Console.Error.WriteError("Input files are not specified and nothing is redirected.");
                    return;
                }
                inputFile = Console.In;
            }
            else inputFile = parameterFile.Value;

            int lines = optionLines.Value;

            using TextWriter writer = optionOut.Value;

            try
            {
                string? line;
                while ((line = inputFile.ReadLine()) is not null)
                {
                    writer.WriteLine(line);

                    if (--lines == 0) break;
                }
            }
            finally
            {
                inputFile.Dispose();
            }
        }
    }
}
