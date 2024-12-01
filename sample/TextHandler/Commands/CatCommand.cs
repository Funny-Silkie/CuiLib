using CuiLib.Commands;
using CuiLib.Options;
using CuiLib.Parameters;

namespace TextHandler.Commands
{
    /// <summary>
    /// ファイルを連結するコマンドを表します。
    /// </summary>
    public class CatCommand : Command
    {
        #region Options

        private readonly FlagOption optionHelp;
        private readonly SingleValueOption<TextWriter> optionOut;

        #endregion Options

        #region Parameters

        private readonly MultipleValueParameter<TextReader> parameterFiles;

        #endregion Parameters

        /// <summary>
        /// <see cref="CatCommand"/>の新しいインスタンスを初期化します。
        /// </summary>
        public CatCommand() : base("cat")
        {
            Description = "Concatenate text files";

            optionHelp = new FlagOption('h', "help")
            {
                Description = "Displays help message",
            };
            optionOut = new SingleValueOption<TextWriter>('o', "out")
            {
                Description = "Destination file.\nDefault is standard out.",
                DefaultValue = Console.Out,
            };

            Options.Add(optionHelp);
            Options.Add(optionOut);

            parameterFiles = new MultipleValueParameter<TextReader>("input", 0)
            {
                Description = "Text files to read",
            };

            Parameters.Add(parameterFiles);
        }

        /// <inheritdoc/>
        protected override void OnExecution()
        {
            if (optionHelp.Value)
            {
                WriteHelp(Console.Out);
                return;
            }

            TextReader[] inputFiles;
            if (!parameterFiles.ValueAvailable)
            {
                if (!Console.IsInputRedirected)
                {
                    Console.Error.WriteError("Input files are not specified and nothing is redirected.");
                    return;
                }
                inputFiles = [Console.In];
            }
            else inputFiles = parameterFiles.Value;

            using TextWriter output = optionOut.Value;

            Span<char> buffer = stackalloc char[4096];
            foreach (TextReader currentInput in inputFiles)
            {
                try
                {
                    while (true)
                    {
                        int charsToRead = currentInput.Read(buffer);
                        if (charsToRead == 0) break;

                        output.Write(buffer);
                    }
                }
                finally
                {
                    currentInput.Dispose();
                }
            }
        }
    }
}
